using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoSingleton<PlayerAudio>
{
    private AudioSource _audioSource;
    public AudioClip hurt;
    // Start is called before the first frame update
    void Start()
    {
        _audioSource=GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Hurt()
    {
        _audioSource.clip=hurt;
        _audioSource.Play();
    }
}
