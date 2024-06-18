using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public class DefaultAdapter : AdapterBase
{

    public override void Init()
    {

    }

    public override void ShowBannerAd(bool value)
    {

    }

    public override void ShowInsertAd()
    {

    }

    public override void BuyItem(string id, Callback callback = null)
    {
        AdapterCallBack.Instance.callBack = callback;
        AdapterResult result = new AdapterResult();
        result.data = null;
        result.errCode = 0;
        string str = JsonMapper.ToJson(result);
        AdapterCallBack.Instance.CommonResult(str);
    }


    public override void ShowVideoAd(Callback callback = null)
    {
        AdapterCallBack.Instance.callBack = callback;
        AdapterResult result = new AdapterResult();
        result.data = null;
        result.errCode = 0;
        string str = JsonMapper.ToJson(result);
        AdapterCallBack.Instance.CommonResult(str);
    }

    public override void ReportEvent(string action, string actionName, string result)
    {

    }
}
