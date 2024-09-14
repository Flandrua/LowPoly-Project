using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LockerType
{
    K,
    N
}
public class Locker : MonoBehaviour
{
    public LockerType type;
    private Animator _animator;
    private AudioSource _as;
    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _as = GetComponent<AudioSource>();
        EventManager.AddListener(EventCommon.START_GAME, InitLock);
    }
    private void InitLock()
    {
        _animator.Play("Lock");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if(DataCenter.Instance.GameData.LockerTypes.Contains(type))
            {
                _animator.SetBool("unlock", true);
                
            }
        }
    }

    private void PlayerAudio()
    {
        _as.Play();
    }
}
