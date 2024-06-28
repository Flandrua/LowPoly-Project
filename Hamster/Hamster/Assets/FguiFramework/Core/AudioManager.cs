using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoSingleton<AudioManager>
{
    //音乐播放器
    public AudioSource musicPlayer;
    //音效播放器
    public AudioSource soundPlayer;

    public bool soundSwitch = true;
    public bool musicSwitch = true;


    public override void Init()
    {
        //soundSwitch = GameData.LoadBool("EffectsMuted", true);
        //musicSwitch = GameData.LoadBool("TuneMuted", true);
        DontDestroyOnLoad(this);
        var tempGameObj1 = new GameObject();
        tempGameObj1.transform.parent = transform;
        var tempGameObj2 = new GameObject();
        tempGameObj2.transform.parent = transform;
        musicPlayer = tempGameObj1.AddComponent<AudioSource>();
        soundPlayer = tempGameObj2.AddComponent<AudioSource>();
    }

    public void SetMusicSwitch(bool value)
    {
        musicSwitch = value;
        if (value)
        {
            ResumeMusic();
            if (!musicPlayer.isPlaying)
            {
                musicPlayer.Play();
            }
        }
        else
        {
            PauseMusic();
        }
        //GameData.SaveBool("TuneMuted", musicSwitch);
    }

    public void SetSoundSwitch(bool value)
    {
        soundSwitch = value;

        //GameData.SaveBool("EffectsMuted", soundSwitch);
    }
    /// <summary>
    /// 播放音乐
    /// </summary>
    /// <param name="name">资源名称</param>
    public void PlayMusic(string name)
    {
        if (!musicPlayer.isPlaying)
        {
            AudioClip clip = Resources.Load<AudioClip>(string.Format("Audio/{0}", name));
            musicPlayer.clip = clip;
            if (musicSwitch)
            {
                musicPlayer.Play();
            }
        }
    }

    /// <summary>
    /// 暂停音乐
    /// </summary>
    public void PauseMusic()
    {
        if (musicPlayer.isPlaying)
        {
            musicPlayer.Pause();
        }
    }

    /// <summary>
    /// 恢复暂停的音乐
    /// </summary>
    public void ResumeMusic()
    {
        if (!musicPlayer.isPlaying)
        {
            musicPlayer.UnPause();
        }
    }

    /// <summary>
    /// 停止播放音乐
    /// </summary>
    public void StopMusic()
    {
        musicPlayer.Stop();
    }

    /// <summary>
    /// 播放音效
    /// </summary>
    /// <param name="name">资源名称</param>
    public void PlaySound(string name)
    {
        if (soundSwitch)
        {
            AudioClip clip = Resources.Load<AudioClip>(string.Format("Audio/{0}", name));
            soundPlayer.clip = clip;
            soundPlayer.PlayOneShot(clip);
        }
    }

}
