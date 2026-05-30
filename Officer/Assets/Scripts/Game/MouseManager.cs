using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoSingleton<MouseManager>
{
    public bool canSwitchTime = false;

    // Start is called before the first frame update
    void Start()
    {
        EventManager.AddListener(EventCommon.NEXT_STAGE, ResetToDefault);
    }
    private void OnDestroy()
    {
        EventManager.RemoveListener(EventCommon.NEXT_STAGE, ResetToDefault);
    }


    // Update is called once per frame
    void Update()
    {

    }
    public void ResetToDefault()
    {
        canSwitchTime = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Mouse hit no longer advances time. Stage changes are now automatic
        // after keyboard work or hamster interaction is completed.
    }
}


