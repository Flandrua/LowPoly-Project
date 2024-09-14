using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LockerKey : MonoBehaviour
{
    public LockerType type;
    private Animator _animator;
    private AudioSource _as;
    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _as = GetComponent<AudioSource>();
        EventManager.AddListener(EventCommon.START_GAME, InitKey);
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void InitKey()
    {
        _animator.SetBool("get", false);
        this.GetComponent<Collider>().enabled = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            _animator.SetBool("get", true);
            DataCenter.Instance.GameData.LockerTypes.Add(type);
            this.GetComponent<Collider>().enabled = false;
            _as.Play();
        }
    }
}
