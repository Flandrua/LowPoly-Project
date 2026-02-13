using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// TTS语音管理器，单例模式
/// 用于播放文本转语音（TTS）音频
/// </summary>
public class TTSManager : MonoSingleton<TTSManager>
{
    [Header("音频设置")]
    [Tooltip("用于播放TTS音频的AudioSource组件")]
    private AudioSource audioSource;

    [Tooltip("是否在播放时暂停其他音频")]
    public bool pauseOtherAudio = false;

    [Header("音量设置")]
    [Range(0f, 1f)]
    [Tooltip("TTS音量")]
    public float volume = 1f;

    private AudioSource[] allAudioSources;

    // 音频播放队列数据结构
    private class QueuedAudio
    {
        public AudioClip clip;
        public System.Action onComplete;

        public QueuedAudio(AudioClip clip, System.Action onComplete)
        {
            this.clip = clip;
            this.onComplete = onComplete;
        }
    }

    // 音频播放队列
    private Queue<QueuedAudio> audioQueue = new Queue<QueuedAudio>();
    private bool isProcessingQueue = false; // 标记是否正在处理队列

    /// <summary>
    /// 初始化TTS管理器
    /// </summary>
    public override void Init()
    {
        base.Init();

        // 确保对象在场景切换时不被销毁
        DontDestroyOnLoad(gameObject);

        // 创建或获取AudioSource组件
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // 配置AudioSource
        audioSource.playOnAwake = false;
        audioSource.volume = volume;
        audioSource.spatialBlend = 0f; // 2D音效
    }

    /// <summary>
    /// 播放TTS语音
    /// </summary>
    /// <param name="audioClip">要播放的音频片段</param>
    /// <param name="onComplete">播放完成后的回调（可选）</param>
    public void PlayTTS(AudioClip audioClip, System.Action onComplete = null)
    {
        if (audioClip == null)
        {
            Debug.LogWarning("TTSManager: 音频片段为空，无法播放");
            return;
        }

        if (audioSource == null)
        {
            Debug.LogError("TTSManager: AudioSource组件未初始化");
            return;
        }

        // 如果正在播放，立即停止当前播放并替换为新音频
        if (audioSource.isPlaying)
        {
            StopCurrentPlayback();
        }

        // 暂停其他音频（如果需要）
        if (pauseOtherAudio)
        {
            PauseOtherAudio();
        }

        // 播放音频
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.Play();

        // 标记正在处理队列
        isProcessingQueue = true;

        // 播放完成后检查队列并调用回调
        StartCoroutine(WaitForAudioComplete(() =>
        {
            // 先调用用户提供的回调
            onComplete?.Invoke();
            
            // 然后检查队列并播放下一个音频
            ProcessQueue();
        }));
    }

    public void PlayTTSByClip(AudioClip audioClip)
    {
        PlayTTS(audioClip, null);
    }
    /// <summary>
    /// 播放TTS语音（通过资源路径）
    /// </summary>
    /// <param name="audioPath">Resources文件夹下的音频资源路径（不含扩展名）</param>
    /// <param name="onComplete">播放完成后的回调（可选）</param>
    public void PlayTTS(string audioPath, System.Action onComplete = null)
    {
        AudioClip clip = Resources.Load<AudioClip>(audioPath);
        if (clip == null)
        {
            Debug.LogError($"TTSManager: 无法加载音频资源: {audioPath}");
            return;
        }

        PlayTTS(clip, onComplete);
    }


    /// <summary>
    /// 停止当前播放的音频（内部方法，不影响队列）
    /// </summary>
    private void StopCurrentPlayback()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        // 停止所有等待协程
        StopAllCoroutines();

        // 恢复其他音频
        if (pauseOtherAudio)
        {
            ResumeOtherAudio();
        }
    }

    /// <summary>
    /// 停止当前播放的TTS并清空队列
    /// </summary>
    public void StopTTS()
    {
        StopCurrentPlayback();
        
        // 清空队列
        ClearQueue();
        isProcessingQueue = false;
    }

    /// <summary>
    /// 暂停当前播放的TTS
    /// </summary>
    public void PauseTTS()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Pause();
        }
    }

    /// <summary>
    /// 恢复播放TTS
    /// </summary>
    public void ResumeTTS()
    {
        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.UnPause();
        }
    }

    /// <summary>
    /// 检查是否正在播放TTS
    /// </summary>
    /// <returns>是否正在播放</returns>
    public bool IsPlaying()
    {
        return audioSource != null && audioSource.isPlaying;
    }

    /// <summary>
    /// 设置TTS音量
    /// </summary>
    /// <param name="newVolume">新音量值（0-1）</param>
    public void SetVolume(float newVolume)
    {
        volume = Mathf.Clamp01(newVolume);
        if (audioSource != null)
        {
            audioSource.volume = volume;
        }
    }

    /// <summary>
    /// 暂停场景中其他所有音频
    /// </summary>
    private void PauseOtherAudio()
    {
        allAudioSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource source in allAudioSources)
        {
            if (source != audioSource && source.isPlaying)
            {
                source.Pause();
            }
        }
    }

    /// <summary>
    /// 恢复场景中其他所有音频
    /// </summary>
    private void ResumeOtherAudio()
    {
        if (allAudioSources != null)
        {
            foreach (AudioSource source in allAudioSources)
            {
                if (source != audioSource)
                {
                    source.UnPause();
                }
            }
        }
    }

    /// <summary>
    /// 播放第一个音频，完成后立即播放第二个音频
    /// </summary>
    /// <param name="firstClip">第一个音频片段</param>
    /// <param name="nextClip">第二个音频片段（第一个播放完后播放）</param>
    /// <param name="onComplete">所有音频播放完成后的回调（可选）</param>
    public void PlayTTSChain(AudioClip firstClip, AudioClip nextClip, System.Action onComplete = null)
    {
        if (firstClip == null)
        {
            Debug.LogWarning("TTSManager: 第一个音频片段为空，无法播放");
            return;
        }

        if (nextClip == null)
        {
            Debug.LogWarning("TTSManager: 第二个音频片段为空，将只播放第一个音频");
            PlayTTS(firstClip, onComplete);
            return;
        }

        // 播放第一个音频，完成后播放第二个
        PlayTTS(firstClip, () =>
        {
            // 第一个音频播放完成后，立即播放第二个
            PlayTTS(nextClip, onComplete);
        });
    }

    /// <summary>
    /// 播放第一个音频，完成后立即播放第二个音频（通过资源路径）
    /// </summary>
    /// <param name="firstPath">第一个音频资源路径</param>
    /// <param name="nextPath">第二个音频资源路径（第一个播放完后播放）</param>
    /// <param name="onComplete">所有音频播放完成后的回调（可选）</param>
    public void PlayTTSChain(string firstPath, string nextPath, System.Action onComplete = null)
    {
        AudioClip firstClip = Resources.Load<AudioClip>(firstPath);
        if (firstClip == null)
        {
            Debug.LogError($"TTSManager: 无法加载第一个音频资源: {firstPath}");
            return;
        }

        AudioClip nextClip = Resources.Load<AudioClip>(nextPath);
        if (nextClip == null)
        {
            Debug.LogError($"TTSManager: 无法加载第二个音频资源: {nextPath}");
            // 如果第二个加载失败，至少播放第一个
            PlayTTS(firstClip, onComplete);
            return;
        }

        PlayTTSChain(firstClip, nextClip, onComplete);
    }

    /// <summary>
    /// 播放音频队列（按顺序播放多个音频）
    /// </summary>
    /// <param name="audioClips">音频片段数组</param>
    /// <param name="onComplete">所有音频播放完成后的回调（可选）</param>
    public void PlayTTSQueue(AudioClip[] audioClips, System.Action onComplete = null)
    {
        if (audioClips == null || audioClips.Length == 0)
        {
            Debug.LogWarning("TTSManager: 音频队列为空");
            onComplete?.Invoke();
            return;
        }

        StartCoroutine(PlayTTSQueueCoroutine(audioClips, 0, onComplete));
    }

    /// <summary>
    /// 播放音频队列协程
    /// </summary>
    private IEnumerator PlayTTSQueueCoroutine(AudioClip[] audioClips, int currentIndex, System.Action onComplete)
    {
        if (currentIndex >= audioClips.Length)
        {
            // 所有音频播放完成
            onComplete?.Invoke();
            yield break;
        }

        AudioClip currentClip = audioClips[currentIndex];
        if (currentClip == null)
        {
            Debug.LogWarning($"TTSManager: 音频队列中索引 {currentIndex} 的音频片段为空，跳过");
            // 跳过空音频，播放下一个
            StartCoroutine(PlayTTSQueueCoroutine(audioClips, currentIndex + 1, onComplete));
            yield break;
        }

        // 播放当前音频
        bool isPlaying = true;
        PlayTTS(currentClip, () => { isPlaying = false; });

        // 等待当前音频播放完成
        yield return new WaitWhile(() => isPlaying);

        // 播放下一个音频
        StartCoroutine(PlayTTSQueueCoroutine(audioClips, currentIndex + 1, onComplete));
    }

    /// <summary>
    /// 将音频添加到播放队列
    /// </summary>
    /// <param name="audioClip">要添加的音频片段</param>
    /// <param name="onComplete">该音频播放完成后的回调（可选）</param>
    public void EnqueueTTS(AudioClip audioClip, System.Action onComplete = null)
    {
        if (audioClip == null)
        {
            Debug.LogWarning("TTSManager: 音频片段为空，无法添加到队列");
            return;
        }

        // 将音频和回调添加到队列
        audioQueue.Enqueue(new QueuedAudio(audioClip, onComplete));

        // 如果当前没有播放音频，立即开始处理队列
        if (!isProcessingQueue && !IsPlaying())
        {
            ProcessQueue();
        }
    }

    /// <summary>
    /// 将音频添加到播放队列（通过资源路径）
    /// </summary>
    /// <param name="audioPath">Resources文件夹下的音频资源路径（不含扩展名）</param>
    /// <param name="onComplete">该音频播放完成后的回调（可选）</param>
    public void EnqueueTTS(string audioPath, System.Action onComplete = null)
    {
        AudioClip clip = Resources.Load<AudioClip>(audioPath);
        if (clip == null)
        {
            Debug.LogError($"TTSManager: 无法加载音频资源: {audioPath}");
            return;
        }

        EnqueueTTS(clip, onComplete);
    }

    /// <summary>
    /// 处理播放队列，播放下一个音频
    /// </summary>
    private void ProcessQueue()
    {
        // 如果队列为空，停止处理
        if (audioQueue.Count == 0)
        {
            isProcessingQueue = false;
            return;
        }

        // 如果正在播放，等待当前播放完成（ProcessQueue会在播放完成后被调用）
        if (IsPlaying())
        {
            return;
        }

        // 从队列中取出下一个音频并播放
        QueuedAudio queuedAudio = audioQueue.Dequeue();
        if (queuedAudio != null && queuedAudio.clip != null)
        {
            PlayTTS(queuedAudio.clip, queuedAudio.onComplete);
        }
        else
        {
            // 如果音频为空，继续处理队列
            ProcessQueue();
        }
    }

    /// <summary>
    /// 清空播放队列
    /// </summary>
    public void ClearQueue()
    {
        audioQueue.Clear();
    }

    /// <summary>
    /// 获取队列中剩余的音频数量
    /// </summary>
    /// <returns>队列中剩余的音频数量</returns>
    public int GetQueueCount()
    {
        return audioQueue.Count;
    }

    /// <summary>
    /// 检查队列是否为空
    /// </summary>
    /// <returns>队列是否为空</returns>
    public bool IsQueueEmpty()
    {
        return audioQueue.Count == 0;
    }

    /// <summary>
    /// 等待音频播放完成的协程
    /// </summary>
    private IEnumerator WaitForAudioComplete(System.Action onComplete)
    {
        yield return new WaitWhile(() => audioSource.isPlaying);

        // 恢复其他音频
        if (pauseOtherAudio)
        {
            ResumeOtherAudio();
        }

        // 调用回调
        onComplete?.Invoke();
    }

    private void OnDestroy()
    {
        StopTTS();
    }
}
