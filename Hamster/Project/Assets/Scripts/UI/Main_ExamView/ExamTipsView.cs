using FguiFramework;
using FairyGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExamTipsView : UIBase
{
    public static string Name = "ExamTipsView";
    private GButton btnYes = null;


    public override void OnAddListener()
    {
        
    }

    public override void OnRemoveListener()
    {
     
    }

    public override void OnInitUI()
    {
        btnYes.onClick.Add(YesClickHandler);
    }

    public override void OnOpen(object ParamData)
    {

    }
     
    private void YesClickHandler()
    {
        AudioManager.Instance.PlaySound("click");
        Close();
        UIManager2D.Instance.OpenPanel(ExamView.Name);
    }

    public override void OnClose()
    {

    }

    public override void OnDestroy()
    {

    }
}
