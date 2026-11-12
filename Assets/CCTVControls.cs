using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

/// <summary>
/// Controls the CCTV recording: a silent video on the monitor, a contextual panel that
/// appears near the workstation, play/pause, a timeline slider and an in-world timestamp.
/// </summary>
public class CCTVControls : MonoBehaviour
{
    [Header("Playback")]
    public VideoPlayer videoPlayer;
    public Slider timeline;

    [Tooltip("Playback speed of the recording (1 = normal).")]
    [Range(0.25f, 4f)]
    [SerializeField] private float playbackSpeed = 1f;

    [Tooltip("Loop the recording instead of stopping at the end.")]
    [SerializeField] private bool loopRecording = false;

    [Header("Monitor (the recording plays here)")]
    [Tooltip("The curved screen of the CCTV computer.")]
    [SerializeField] private Renderer monitorScreen;

    [Tooltip("Texture the video is drawn into. The monitor's material displays it.")]
    [SerializeField] private RenderTexture monitorTexture;

    [Tooltip("Shader texture slots on the monitor material that should show the recording.")]
    [SerializeField] private string[] monitorTextureProperties = { "_BaseMap", "_MainTex", "_EmissionMap" };

    [Header("Panel preview (static image, no video)")]
    [SerializeField] private RawImage previewImage;
    [Tooltip("Still CCTV image shown in the panel instead of the live video.")]
    [SerializeField] private Texture previewStill;

    [Header("Panel")]
    [SerializeField] private GameObject panel;
    [SerializeField] private Button playButton;
    [SerializeField] private TMP_Text playLabel;
    [SerializeField] private Button closeButton;
    [SerializeField] private TMP_Text timestampText;
    [SerializeField] private TMP_Text previewTimestampText;

    [Tooltip("Panels that take priority. The CCTV panel stays hidden while any of them is open.")]
    [SerializeField] private GameObject[] hideWhileActive;

    [Header("Workstation proximity (metres, measured across the floor)")]
    [SerializeField] private Transform workstation;
    [SerializeField] private float showDistance = 1.8f;
    [SerializeField] private float hideDistance = 2.2f;

    [Tooltip("Play the recording from the start each time the player arrives, and stop it when they leave.")]
    [SerializeField] private bool autoPlayOnEnter = true;

    [Header("Recording clock (HH:MM:SS shown across the whole clip)")]
    [SerializeField] private string recordingStart = "16:56:00";
    [SerializeField] private string recordingEnd = "17:00:00";

    [Header("Labels")]
    [SerializeField] private string playText = "PLAY RECORDING";
    [SerializeField] private string pauseText = "PAUSE RECORDING";
    [SerializeField] private string replayText = "PLAY RECORDING";

    private Transform head;
    private bool inRange;
    private bool closedByPlayer;
    private bool reachedEnd;
    private bool seekPending;
    private bool pauseAfterSeek;
    private double seekTarget;
    private float seekStartedAt;

    [Header("Seeking")]
    [Tooltip("How close (seconds) a reported seek must land to the requested time to count as finished.")]
    [SerializeField] private double SeekTolerance = 0.5;
    [Tooltip("Give up waiting for a seek to be reported after this many seconds.")]
    [SerializeField] private float SeekTimeout = 4f;

    private bool hasPendingScrub;
    private float pendingScrub;
    private int startSeconds;
    private int endSeconds;

    private void Awake()
    {
        startSeconds = ParseClock(recordingStart);
        endSeconds = ParseClock(recordingEnd);

        // The recording is evidence footage and must always play without sound.
        videoPlayer.audioOutputMode = VideoAudioOutputMode.None;
        videoPlayer.playOnAwake = false;
        videoPlayer.isLooping = loopRecording;
        videoPlayer.playbackSpeed = playbackSpeed;

        SetUpMonitor();
        SetUpStaticPreview();

        if (panel != null)
            panel.SetActive(false);
    }

    private void OnValidate()
    {
        if (hideDistance < showDistance)
            hideDistance = showDistance;
    }

    /// <summary>Draws the video only onto the computer's monitor.</summary>
    private void SetUpMonitor()
    {
        if (monitorTexture == null)
            monitorTexture = videoPlayer.targetTexture;

        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        videoPlayer.targetTexture = monitorTexture;

        if (monitorScreen == null || monitorTexture == null)
            return;

        var block = new MaterialPropertyBlock();
        monitorScreen.GetPropertyBlock(block);
        Material material = monitorScreen.sharedMaterial;
        foreach (string property in monitorTextureProperties)
        {
            if (material != null && material.HasProperty(property))
                block.SetTexture(property, monitorTexture);
        }
        monitorScreen.SetPropertyBlock(block);
    }

    /// <summary>The panel shows a still image; the moving recording is only on the monitor.</summary>
    private void SetUpStaticPreview()
    {
        if (previewImage == null)
            return;

        if (previewStill != null)
            previewImage.texture = previewStill;
        else if (previewImage.texture == monitorTexture)
            previewImage.enabled = false;
    }

    private void Start()
    {
        timeline.minValue = 0f;
        timeline.maxValue = 1f;
        timeline.SetValueWithoutNotify(0f);

        timeline.onValueChanged.AddListener(OnTimelineChanged);
        playButton.onClick.AddListener(TogglePlayback);
        if (closeButton != null)
            closeButton.onClick.AddListener(ClosePanel);
        videoPlayer.loopPointReached += OnRecordingFinished;
        videoPlayer.seekCompleted += OnSeekCompleted;
        videoPlayer.prepareCompleted += OnPrepared;
        videoPlayer.frameReady += OnFrameReady;

        // Pausing a prepared player shows the first frame on the monitor without playing it.
        videoPlayer.Pause();
        RefreshLabels();
    }

    private void Update()
    {
        UpdatePanelVisibility();

        // Never let a seek that is not reported block the timeline for long.
        if (seekPending && Time.unscaledTime - seekStartedAt > SeekTimeout)
            FinishSeek();

        // While a seek is landing the player still reports its old time, so leave the slider alone.
        if (videoPlayer.isPlaying && !seekPending && videoPlayer.length > 0)
            timeline.SetValueWithoutNotify((float)(videoPlayer.time / videoPlayer.length));

        UpdateTimestamp(timeline.value);

        // Play() and Stop() take effect a few frames later, so keep the button label in step.
        RefreshLabels();
    }

    public void StartVideo()
    {
        if (reachedEnd)
        {
            PlayFromBeginning();
            return;
        }

        videoPlayer.Play();
        RefreshLabels();
    }

    /// <summary>Restarts the recording at 00:00 and plays it.</summary>
    public void PlayFromBeginning()
    {
        ResetPlaybackState();

        // Already loaded and waiting on the first frame: start immediately.
        // Otherwise Stop rewinds without a seek, which is slow and unreliable on some decoders.
        bool readyAtStart = videoPlayer.isPrepared && !videoPlayer.isPlaying && videoPlayer.frame <= 0;
        if (!readyAtStart)
            videoPlayer.Stop();

        videoPlayer.Play();
        RefreshLabels();
    }

    /// <summary>Stops the recording, rewinds it and shows its first frame on the monitor.</summary>
    public void StopRecording()
    {
        ResetPlaybackState();
        videoPlayer.Stop();
        // Pausing reloads the clip at its first frame, so the next visit can start straight away.
        videoPlayer.Pause();
        RefreshLabels();
    }

    private void ResetPlaybackState()
    {
        reachedEnd = false;
        seekPending = false;
        pauseAfterSeek = false;
        timeline.SetValueWithoutNotify(0f);
    }

    public void PauseVideo()
    {
        videoPlayer.Pause();
        RefreshLabels();
    }

    public void TogglePlayback()
    {
        if (pauseAfterSeek)
        {
            // Play was pressed while a paused scrub was landing: keep playing once it lands.
            pauseAfterSeek = false;
            RefreshLabels();
            return;
        }

        if (videoPlayer.isPlaying)
            PauseVideo();
        else
            StartVideo();
    }

    public void ClosePanel()
    {
        closedByPlayer = true;
        panel.SetActive(false);
    }

    private void OnTimelineChanged(float value)
    {
        // Only the player changes the slider with notifications; playback updates it silently.
        reachedEnd = false;
        if (!videoPlayer.isPrepared || videoPlayer.length <= 0)
        {
            // Not loaded yet: remember where the player dragged to and jump there once it is.
            hasPendingScrub = true;
            pendingScrub = value;
            videoPlayer.Prepare();
            return;
        }

        if (!videoPlayer.canSetTime)
            return;

        BeginSeek(value * videoPlayer.length);

        // A paused video does not redraw after a seek, so play until the seek lands, then pause.
        if (!videoPlayer.isPlaying)
        {
            pauseAfterSeek = true;
            // Stop on the first frame drawn at the new position, so the monitor matches the slider.
            videoPlayer.sendFrameReadyEvents = true;
            videoPlayer.Play();
        }
        RefreshLabels();
    }

    private void OnFrameReady(VideoPlayer source, long frameIndex)
    {
        if (!pauseAfterSeek || source.frameRate <= 0)
            return;

        double frameTime = frameIndex / source.frameRate;
        if (System.Math.Abs(frameTime - seekTarget) <= SeekTolerance)
            FinishSeek();
    }

    private void OnPrepared(VideoPlayer source)
    {
        if (!hasPendingScrub)
            return;

        hasPendingScrub = false;
        OnTimelineChanged(pendingScrub);
    }

    private void BeginSeek(double targetSeconds)
    {
        seekTarget = targetSeconds;
        seekStartedAt = Time.unscaledTime;
        seekPending = true;
        videoPlayer.time = targetSeconds;
    }

    private void OnSeekCompleted(VideoPlayer source)
    {
        // Ignore completions of earlier seeks (for example the initial one when playback starts).
        if (seekPending && System.Math.Abs(source.time - seekTarget) > SeekTolerance)
            return;

        FinishSeek();
    }

    private void FinishSeek()
    {
        seekPending = false;

        if (pauseAfterSeek)
        {
            pauseAfterSeek = false;
            videoPlayer.Pause();
            videoPlayer.sendFrameReadyEvents = false;

            // Keep the slider and timestamp on the frame the monitor is actually showing.
            if (videoPlayer.length > 0)
                timeline.SetValueWithoutNotify((float)(videoPlayer.time / videoPlayer.length));
        }
        RefreshLabels();
    }

    private void OnRecordingFinished(VideoPlayer source)
    {
        seekPending = false;
        pauseAfterSeek = false;
        reachedEnd = true;
        timeline.SetValueWithoutNotify(1f);
        RefreshLabels();
    }

    private void UpdatePanelVisibility()
    {
        if (head == null)
        {
            Camera mainCamera = Camera.main;
            if (mainCamera == null)
                return;
            head = mainCamera.transform;
        }

        Transform anchor = workstation != null ? workstation : transform;
        Vector3 offset = head.position - anchor.position;
        offset.y = 0f;
        float distance = offset.magnitude;

        // The gap between showDistance and hideDistance means hovering at the edge cannot retrigger:
        // playback starts once per visit and only re-arms after the player has fully left.
        if (!inRange && distance <= showDistance)
        {
            inRange = true;
            if (autoPlayOnEnter)
                PlayFromBeginning();
        }
        else if (inRange && distance >= hideDistance)
        {
            inRange = false;
            closedByPlayer = false;
            if (autoPlayOnEnter)
                StopRecording();
        }

        bool shouldShow = inRange && !closedByPlayer && !AnyPriorityPanelOpen();
        if (panel.activeSelf != shouldShow)
            panel.SetActive(shouldShow);
    }

    private bool AnyPriorityPanelOpen()
    {
        if (hideWhileActive == null)
            return false;

        foreach (GameObject other in hideWhileActive)
        {
            if (other != null && other.activeInHierarchy)
                return true;
        }
        return false;
    }

    private void UpdateTimestamp(float normalisedTime)
    {
        int seconds = Mathf.RoundToInt(Mathf.Lerp(startSeconds, endSeconds, Mathf.Clamp01(normalisedTime)));
        string clock = FormatClock(seconds);

        if (timestampText != null)
            timestampText.text = clock;
        if (previewTimestampText != null)
            previewTimestampText.text = clock;
    }

    private void RefreshLabels()
    {
        if (playLabel == null)
            return;

        if (videoPlayer.isPlaying && !pauseAfterSeek)
            playLabel.text = pauseText;
        else if (reachedEnd)
            playLabel.text = replayText;
        else
            playLabel.text = playText;
    }

    private static int ParseClock(string clock)
    {
        string[] parts = clock.Split(':');
        int hours = parts.Length > 0 ? int.Parse(parts[0]) : 0;
        int minutes = parts.Length > 1 ? int.Parse(parts[1]) : 0;
        int seconds = parts.Length > 2 ? int.Parse(parts[2]) : 0;
        return hours * 3600 + minutes * 60 + seconds;
    }

    private static string FormatClock(int totalSeconds)
    {
        return string.Format("{0:00}:{1:00}:{2:00}", totalSeconds / 3600, totalSeconds / 60 % 60, totalSeconds % 60);
    }

    private void OnDestroy()
    {
        if (timeline != null)
            timeline.onValueChanged.RemoveListener(OnTimelineChanged);
        if (playButton != null)
            playButton.onClick.RemoveListener(TogglePlayback);
        if (closeButton != null)
            closeButton.onClick.RemoveListener(ClosePanel);
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnRecordingFinished;
            videoPlayer.seekCompleted -= OnSeekCompleted;
            videoPlayer.prepareCompleted -= OnPrepared;
            videoPlayer.frameReady -= OnFrameReady;
        }
    }
}
