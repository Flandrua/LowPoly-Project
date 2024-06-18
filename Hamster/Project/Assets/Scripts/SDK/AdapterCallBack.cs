using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public class AdapterCallBack : MonoSingleton<AdapterCallBack>
{
    public AdapterBase.Callback callBack;
    public override void Init()
    {
        DontDestroyOnLoad(this);
    }
   
    public void CommonResult(string res)
    {
        AdapterResult result = JsonMapper.ToObject<AdapterResult>(res);
        callBack?.Invoke(result);
    }
}
