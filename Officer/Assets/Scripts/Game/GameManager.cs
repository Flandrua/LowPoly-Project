using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        EventManager.AddListener(EventCommon.CHANGE_TIME, ChangeTime);
    }
    private void OnDestroy()
    {
        EventManager.RemoveListener(EventCommon.CHANGE_TIME, ChangeTime);
    }

    private void ChangeTime()
    {

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
