using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SDKManager : Singleton<SDKManager>
{
    public AdapterBase adapter;
    public override void Init()
    {

#if UNITY_EDITOR
        adapter = new DefaultAdapter();
#elif UNITY_XBOX360
        
#elif UNITY_IPHONE
        
#elif UNITY_ANDROID
        adapter = new AndroidAdapter();
#elif UNITY_STANDALONE_OSX
       
#elif UNITY_STANDALONE_WIN

#endif
    }
}
