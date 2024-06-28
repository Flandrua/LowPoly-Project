using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public delegate void VideoEndCallBack(); 
public class GameVideoPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    private VideoEndCallBack endCb = null;
    public VideoPlayer videoPlayer = null;
    private void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();
    }

    void Start()
    {
        videoPlayer.loopPointReached += OnVideoEnd;
    }

    void OnDestroy()
    {
        videoPlayer.loopPointReached -= OnVideoEnd;
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        LogManager.Instance.Log("视频已播放完毕");
        endCb?.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public double GetTotalTime()
    {
        return videoPlayer.length;
    }

    public void SetCurTime(double time)
    {
        videoPlayer.time = time;
    }

    public double GetCurTime(double time)
    {
        return videoPlayer.time;
    }

    public void Play(VideoClip video, VideoEndCallBack videoEndCallBack = null)
    {
        videoPlayer.clip = video;
        endCb = videoEndCallBack; 
        videoPlayer.Play();
    }

    public void Pause()
    {
        videoPlayer.Pause();
    }

    public void Stop()
    {
        videoPlayer.Stop();
    }
}
