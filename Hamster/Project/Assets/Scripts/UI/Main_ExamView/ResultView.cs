using FguiFramework;
using FairyGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultView : UIBase
{
    public static string Name = "ResultView";
    private Transition trans;
    ResultData resultData = null;
    private GButton btnAgain;
    private GButton btnCheck;
    private GButton btnBack;
    private GTextField txtScore;
    private GTextField txtCorrect;
    private GTextField txtError;

    public override void OnAddListener()
    {
        
    }

    public override void OnRemoveListener()
    {
     
    }

    public override void OnInitUI()
    {
        btnAgain.onClick.Add(AgainClickHandler);
        btnCheck.onClick.Add(CheckClickHandler);
        btnBack.onClick.Add(BackClickHandler);
    }

    private void BackClickHandler()
    {
        AudioManager.Instance.PlaySound("click");
        trans.PlayReverse(() =>
        {
            Close();
            UIManager2D.Instance.ClosePanel(ExamView.Name);
            UIManager2D.Instance.OpenPanel(MainView.Name);
        }); 
    }

    public override void OnOpen(object ParamData)
    {
        resultData = (ResultData)ParamData;
        trans.Play();
        txtScore.text = $"{resultData.score}";
        txtCorrect.text = $"{10 - resultData.errorQuestionDatas.Count}";
        txtError.text = $"{resultData.errorQuestionDatas.Count}";
        if (resultData.errorQuestionDatas.Count > 0)
        {
            btnBack.visible = false;
            btnCheck.visible = true;
        }
        else
        {
            btnBack.visible = true;
            btnCheck.visible = false;
        }
    }

    private void AgainClickHandler()
    {
        AudioManager.Instance.PlaySound("click");
        trans.PlayReverse(() =>
        {
            UIManager2D.Instance.ClosePanel(ExamView.Name);
            Close();
            UIManager2D.Instance.OpenPanel(ExamView.Name);
        });
      
    }

    private void CheckClickHandler()
    {
        AudioManager.Instance.PlaySound("click");
        trans.PlayReverse(() =>
        {
            UIManager2D.Instance.ClosePanel(ExamView.Name);
            Close();
            UIManager2D.Instance.OpenPanel(ErrorView.Name, resultData);
        }); 
    }

    public override void OnClose()
    {

    }

    public override void OnDestroy()
    {

    }
}
