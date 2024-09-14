using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMManager : MonoSingleton<BGMManager>
{
    public AudioClip bgm1;
    public AudioClip bgm2;
    private AudioSource _as;
    // Start is called before the first frame update
    void Start()
    {
        _as = GetComponent<AudioSource>();
        EventManager.AddListener(EventCommon.START_GAME, Bgm1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Bgm1()
    {
        _as.clip = bgm1;
        _as.Play();
    }
    public void Bgm2()
    {
        _as.clip = bgm2;
        _as.Play();
    }
}
