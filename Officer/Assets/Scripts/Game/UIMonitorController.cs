using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIMonitorController : MonoSingleton<UIMonitorController>
{
    // Start is called before the first frame update
    private Animator _animator;
    public TextMeshProUGUI content = null;
    public TextMeshProUGUI nameTxt = null;

    void Start()
    {
        _animator = GetComponent<Animator>();
        if (content == null)
            content = transform.Find("Content").GetComponent<TextMeshProUGUI>();
        if (nameTxt == null)
            nameTxt = transform.Find("Name").GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void Show(bool flag)
    {
        //_animator.SetBool("Show", flag);
    }
}
