using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectController : MonoBehaviour
{
    // Start is called before the first frame update
    private Animator _animator;
    private bool canHover = true;
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void Show(bool flag)
    {
        if (canHover)
            _animator.SetBool("Show", flag);
        else
        {
            _animator.SetBool("Show", false);
        }
    }
    public void CanHover(bool flag)
    {
        canHover= flag; 
    }
}
