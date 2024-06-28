using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogManager : Singleton<LogManager>
{
    public void Log(object content)
    {
#if UNITY_EDITOR
        Debug.Log(content);
#endif
    }
}
