using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public class AndroidAdapter : AdapterBase
{
    AndroidJavaClass jc;
    private string itemIds = "";

    public override void Init()
    {
        jc = new AndroidJavaClass("figergun.guoyue.AppActivity");
        //获取所有商品列表
        itemIds = DataManager.Instance.CfgGlobal.mDataMap["4"].value;
        jc.CallStatic("getAllItem", itemIds);
    }

    public override void ShowBannerAd(bool value)
    {

    }

    public override void ShowInsertAd()
    {
        jc.CallStatic("showInsertAd");
    }

    public override void BuyItem(string id, Callback callback = null)
    {
        AdapterCallBack.Instance.callBack = callback;
        jc.CallStatic("buyItem", id);
    }

    public override void ShowVideoAd(Callback callback = null)
    {
        AdapterCallBack.Instance.callBack = callback;
        jc.CallStatic("showVideoAd");
    }

    public override void ReportEvent(string action, string actionName, string result)
    {
        jc.CallStatic("reportEvent", action, actionName, "", result);
    }
}
