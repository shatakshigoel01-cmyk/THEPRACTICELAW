using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class CCTVVideoScrubber : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public Slider timeline;

    void Start()
    {
        timeline.onValueChanged.AddListener(ScrubVideo);
    }

    void Update()
    {
        if (videoPlayer.isPrepared && !timeline.interactable)
            return;
    }

    void ScrubVideo(float value)
    {
        if (videoPlayer.isPrepared)
        {
            videoPlayer.time = value * videoPlayer.length;
        }
    }
}