using FguiFramework;
using FairyGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainView : UIBase
{
    public static string Name = "MainView";

    private GProgressBar hpBar;
    public override void OnAddListener()
    {
        EventManager.AddListener<int[]>(EventCommon.UPDATE_HP, UpdateHpBar);
    }

    public override void OnRemoveListener()
    {
        EventManager.RemoveListener<int[]>(EventCommon.UPDATE_HP, UpdateHpBar);
    }
    private void UpdateHpBar(int[] hplist)
    {
        hpBar.value = (hplist[0] * 1.0f / hplist[1] * 1.0f)*100;
    }
    public override void OnInitUI()
    {
        
    }

    public override void OnOpen(object ParamData)
    {

    }
    public override void OnClose()
    {

    }

    public override void OnDestroy()
    {

    }
}
