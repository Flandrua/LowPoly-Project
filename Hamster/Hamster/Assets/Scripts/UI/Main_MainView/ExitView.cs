using FguiFramework;
using FairyGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitView : UIBase
{
    public static string Name = "ExitView";
    private Transition trans;
    private GButton btnYes;
    private GButton btnNo;

    public override void OnAddListener()
    {

    }

    public override void OnRemoveListener()
    {

    }

    public override void OnInitUI()
    {

    }

    public override void OnOpen(object ParamData)
    {
        trans.Play();
        btnYes.onClick.Add(YesClickHandler);
        btnNo.onClick.Add(NoClickHandler);
    }

    private void YesClickHandler()
    {
        AudioManager.Instance.PlaySound("click");
        Close();
        Application.Quit();
    }

    private void NoClickHandler()
    {
        AudioManager.Instance.PlaySound("click");
        trans.PlayReverse(() =>
        {
            Close();
        });
    }

    public override void OnClose()
    {

    }

    public override void OnDestroy()
    {

    }
}
