using FguiFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

public class LoadView : UIBase
{
    public static string Name = "LoadView";


    public override void OnAddListener()
    {
        EventManager.AddListener<int>(EventCommon.REFRESH_LOADING_PROGRESS, RefreshProgress);
    }

    public override void OnRemoveListener()
    {
        EventManager.RemoveListener<int>(EventCommon.REFRESH_LOADING_PROGRESS, RefreshProgress);
    }

    public override void OnInitUI()
    {

    }

    public override void OnOpen(object ParamData)
    {


    }

    private void RefreshProgress(int value)
    {


    }

    public override void OnClose()
    {

    }

    public override void OnDestroy()
    {

    }




}
