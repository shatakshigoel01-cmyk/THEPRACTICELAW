
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class CCTVControls : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public Slider timeline;

    private bool isDragging = false;

    void Start()
    {
        timeline.minValue = 0;
        timeline.maxValue = 1;
        timeline.value = 0;

        timeline.onValueChanged.AddListener(OnTimelineChanged);
    }

    void Update()
    {
        if (videoPlayer == null || timeline == null)
            return;

        if (videoPlayer.isPlaying && !isDragging &&
            videoPlayer.length > 0)
        {
            timeline.SetValueWithoutNotify(
                (float)(videoPlayer.time / videoPlayer.length)
            );
        }
    }

    public void StartVideo()
    {
        if (videoPlayer != null)
            videoPlayer.Play();
    }

    public void PauseVideo()
    {
        if (videoPlayer != null)
            videoPlayer.Pause();
    }

    public void BeginDrag()
    {
        isDragging = true;
    }

    public void EndDrag()
    {
        SeekVideo(timeline.value);
        isDragging = false;
    }

    void OnTimelineChanged(float value)
    {
        if (isDragging)
            SeekVideo(value);
    }

    void SeekVideo(float value)
    {
        if (videoPlayer != null && videoPlayer.canSetTime &&
            videoPlayer.length > 0)
        {
            videoPlayer.time = value * videoPlayer.length;
        }
    }

    void OnDestroy()
    {
        if (timeline != null)
            timeline.onValueChanged.RemoveListener(OnTimelineChanged);
    }
}
