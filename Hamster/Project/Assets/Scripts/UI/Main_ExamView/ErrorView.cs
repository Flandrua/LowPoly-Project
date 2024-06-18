using FguiFramework;
using FairyGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErrorView : UIBase
{
    public static string Name = "ErrorView";
    private GTextField txtTitle;
    private GList listAnswer;
    private GButton btnA;
    private GButton btnB;
    private GButton btnC;
    private GButton btnD;
    private GTextField txtCount;
    private GButton btnNext;
    private GButton btnBack;
    List<string> answers = new List<string>();
    ResultData resultData = null;
    private int curIndex = 0;

    public override void OnAddListener()
    {

    }

    public override void OnRemoveListener()
    {

    }

    public override void OnInitUI()
    {
        btnBack.onClick.Add(BackClickHandler);
        btnNext.onClick.Add(NextClickHandler);
        listAnswer.itemRenderer = ExamListItemRender;
    }

    private void ExamListItemRender(int index, GObject obj)
    {
        obj.asLabel.title = $"{GetAnswerFlag(index)}.{answers[index]}";

        obj.asLabel.color = DataCenter.Instance.HexToColor("#222222");
        int tempIndex = 0;
        var data = resultData.errorQuestionDatas[curIndex];
        var configData = DataManager.Instance.CfgQuestion.mDataMap[$"{data.id}"];
        switch (configData.rightAnswer)
        {
            case "A":
                tempIndex = 0;
                break;
            case "B":
                tempIndex = 1;
                break;
            case "C":
                tempIndex = 2;
                break;
            case "D":
                tempIndex = 3;
                break;
        }

        if (tempIndex == index)
        {
            obj.asLabel.color = DataCenter.Instance.HexToColor("#10AA65");
        } 
    }

    private string GetAnswerFlag(int index)
    {
        switch (index)
        {
            case 0:
                return "A";
            case 1:
                return "B";
            case 2:
                return "C";
            case 3:
                return "D";
        }
        return "";
    }

    private void BackClickHandler()
    {
        AudioManager.Instance.PlaySound("click");
        Close();
        UIManager2D.Instance.OpenPanel(MainView.Name);
    }

    private void NextClickHandler()
    {
        AudioManager.Instance.PlaySound("click");
        curIndex++;
        if(curIndex>= resultData.errorQuestionDatas.Count - 1)
        {
            btnNext.visible = false;
        }
        RefreshData();
    }

    private void RefreshData()
    {
        if (curIndex >= resultData.errorQuestionDatas.Count)
        { 
            return;
        }
        var data = resultData.errorQuestionDatas[curIndex];
        txtCount.text = $"{curIndex + 1}[color=#868686]/{resultData.errorQuestionDatas.Count}[/color]";
        var configData = DataManager.Instance.CfgQuestion.mDataMap[$"{data.id}"];
        txtTitle.text = $"{configData.question}";
        answers.Clear();
        answers.Add(configData.A);
        answers.Add(configData.B);
        answers.Add(configData.C);
        answers.Add(configData.D);
        btnA.GetController("ctr_state").selectedIndex = 0;
        btnB.GetController("ctr_state").selectedIndex = 0;
        btnC.GetController("ctr_state").selectedIndex = 0;
        btnD.GetController("ctr_state").selectedIndex = 0;

        switch (configData.rightAnswer)
        {
            case "A":
                btnA.GetController("ctr_state").selectedIndex = 1;
                break;
            case "B":
                btnB.GetController("ctr_state").selectedIndex = 1;
                break;
            case "C":
                btnC.GetController("ctr_state").selectedIndex = 1;
                break;
            case "D":
                btnD.GetController("ctr_state").selectedIndex = 1;
                break;
        } switch (configData.rightAnswer)
        {
            case "A":
                btnA.GetController("ctr_state").selectedIndex = 1;
                break;
            case "B":
                btnB.GetController("ctr_state").selectedIndex = 1;
                break;
            case "C":
                btnC.GetController("ctr_state").selectedIndex = 1;
                break;
            case "D":
                btnD.GetController("ctr_state").selectedIndex = 1;
                break;
        }
        switch (data.errorAnswer)
        {
            case 0:
                btnA.GetController("ctr_state").selectedIndex = 2;
                break;
            case 1:
                btnB.GetController("ctr_state").selectedIndex = 2;
                break;
            case 2:
                btnC.GetController("ctr_state").selectedIndex = 2;
                break;
            case 3:
                btnD.GetController("ctr_state").selectedIndex = 2;
                break;
        } 
        listAnswer.numItems = answers.Count;
    }

    public override void OnOpen(object ParamData)
    {
        resultData = (ResultData)ParamData;
        curIndex = 0;
        btnNext.visible = true;
        RefreshData();
    }
     

    public override void OnClose()
    {

    }

    public override void OnDestroy()
    {

    }
}
