using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LockerKey : MonoBehaviour
{
    public LockerType type;
    private Animator _animator;
    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            _animator.SetBool("get", true);
            DataCenter.Instance.GameData.LockerTypes.Add(type);
        }
    }
}
