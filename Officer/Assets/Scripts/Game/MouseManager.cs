using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoSingleton<MouseManager>
{
    public bool canSwitchTime = false;

    // Start is called before the first frame update
    void Start()
    {
        EventManager.AddListener<string>(EventCommon.PREPARE_CHANGE_TIME, CanSwitchTime);
    }
    private void OnDestroy()
    {
        EventManager.RemoveListener<string>(EventCommon.PREPARE_CHANGE_TIME, CanSwitchTime);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
    private void CanSwitchTime(string str)
    {
        //得让鼠标有高亮或者提示
        canSwitchTime = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(canSwitchTime)
            {
                
                EventManager.DispatchEvent(EventCommon.CHANGE_TIME);
            }
        }
    }

}
