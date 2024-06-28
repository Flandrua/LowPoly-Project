using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IAdapterInterface
{
    /// <summary>
    /// 初始化
    /// </summary>
    void Init();

    /// <summary>
    /// 显示视频广告
    /// </summary>
    void ShowVideoAd(AdapterBase.Callback callback = null);

    /// <summary>
    /// 显示banner广告
    /// </summary>
    /// <param name="value">关闭</param>
    void ShowBannerAd(bool value);

    /// <summary>
    /// 显示插屏广告
    /// </summary>
    void ShowInsertAd();

    /// <summary>
    /// 购买商品
    /// </summary>
    /// <param name="id">商品ID</param>
    /// <param name="callback">回调</param>
    void BuyItem(string id, AdapterBase.Callback callback = null);

    /// <summary>
    /// 上报
    /// </summary>
    /// <param name="action">行为</param>
    /// <param name="actionName">行为名字</param>
    /// <param name="result">行为结果</param>
    void ReportEvent(string action, string actionName, string result);

}

public class AdapterResult
{
    public int errCode;
    public object data;
}

public abstract class AdapterBase:IAdapterInterface
{
    public AdapterBase()
    {
        Init();
    }

    public abstract void Init();

    public abstract void ShowBannerAd(bool value);

    public abstract void ShowInsertAd();

    public abstract void ShowVideoAd(Callback callback = null);

    public abstract void BuyItem(string id, AdapterBase.Callback callback = null);

    public abstract void ReportEvent(string action, string actionName, string result);

    public delegate void Callback(AdapterResult result);
}
