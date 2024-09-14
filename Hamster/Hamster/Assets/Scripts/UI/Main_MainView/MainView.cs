using FguiFramework;
using FairyGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainView : UIBase
{
    public static string Name = "MainView";

    private GProgressBar hpBar;
    private GGroup groupStart;
    private GGroup groupRestart;
    private GButton btnStart;
    private GButton btnRestart;
    public override void OnAddListener()
    {
        EventManager.AddListener<int[]>(EventCommon.UPDATE_HP, UpdateHpBar);
        EventManager.AddListener(EventCommon.GAME_OVER, GameOver);
    }

    public override void OnRemoveListener()
    {
        EventManager.RemoveListener<int[]>(EventCommon.UPDATE_HP, UpdateHpBar);
    }
    private void UpdateHpBar(int[] hplist)
    {
        hpBar.value = (hplist[0] * 1.0f / hplist[1] * 1.0f) * 100;
    }
    public override void OnInitUI()
    {

    }

    public override void OnOpen(object ParamData)
    {
        btnStart.onClick.Add(BtnStart);
        btnRestart.onClick.Add(BtnRestart);
    }
    private void BtnStart()
    {
        groupStart.visible = false;
        EventManager.DispatchEvent(EventCommon.START_GAME);
    }
    private void BtnRestart()
    {
        groupRestart.visible = false;
        EventManager.DispatchEvent(EventCommon.START_GAME);
    }
    private void GameOver()
    {
        groupRestart.visible = true;
    }
    public override void OnClose()
    {

    }

    public override void OnDestroy()
    {

    }
}
