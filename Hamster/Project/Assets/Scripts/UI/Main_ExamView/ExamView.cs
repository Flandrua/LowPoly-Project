using FguiFramework;
using FairyGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Configs;
using System;

public class ErrorQuestionData
{
    public int id;
    public int errorAnswer = -1;
}

public class ResultData
{
    public List<ErrorQuestionData> errorQuestionDatas = new List<ErrorQuestionData>();
    public int score = 0;
}

public class ExamView : UIBase
{
    public static string Name = "ExamView";
    private GTextField txtScore;
    private GTextField txtTime;
    private GTextField txtCount;
    private GTextField txtCorrect;
    private GTextField txtError;
    private GButton btnBack;
    private GTextField txtTitle;
    private GList listAnswer;
    private GButton btnA;
    private GButton btnB;
    private GButton btnC;
    private GButton btnD;
    private Transition transCorrect;
    private Transition transError;
    private List<QuestionData> listQuestionData = new List<QuestionData>();
    private List<ErrorQuestionData> errorQuestionDatas = new List<ErrorQuestionData>();
    List<string> answers = new List<string>();
    private int curIndex = 0;
    private int curTime = 0;
    private int score = 0;
    private int curError = 0;
    private int curCorrect = 0;
    private bool canClickAnswer = true;

    public override void OnAddListener()
    {

    }

    public override void OnRemoveListener()
    {

    }

    public override void OnInitUI()
    {
        btnBack.onClick.Add(BackClickHandler);
        btnA.onClick.Add(AnswerClickHandler);
        btnB.onClick.Add(AnswerClickHandler);
        btnC.onClick.Add(AnswerClickHandler);
        btnD.onClick.Add(AnswerClickHandler);
        listAnswer.itemRenderer = ExamListItemRender;
    }

    private void ExamListItemRender(int index, GObject obj)
    {
        obj.asLabel.title = $"{GetAnswerFlag(index)}.{answers[index]}";
#if UNITY_EDITOR
        obj.asLabel.color = DataCenter.Instance.HexToColor("#222222");
        int tempIndex = 0;
        var data = listQuestionData[curIndex];
        switch (data.rightAnswer)
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
            obj.asLabel.color = Color.red;
        }
#endif
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

    private void AnswerClickHandler(EventContext context)
    {
        if (!canClickAnswer)
        {
            return;
        }
        AudioManager.Instance.PlaySound("click");
        GButton btn = (GButton)context.sender;
        switch (btn.name)
        {
            case "btnA":
                Answer(0);
                break;
            case "btnB":
                Answer(1);
                break;
            case "btnC":
                Answer(2);
                break;
            case "btnD":
                Answer(3);
                break;
        }
    }

    void Shuffle<T>(List<T> list)
    {
        System.Random random = new System.Random();

        int n = list.Count;
        for (int i = n - 1; i > 0; i--)
        {
            int j = random.Next(0, i + 1);
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }


    private void BackClickHandler()
    {
        AudioManager.Instance.PlaySound("click");
        Close();
        UIManager2D.Instance.OpenPanel(MainView.Name);
    }

    public override void OnOpen(object ParamData)
    {
        listQuestionData.Clear();
        errorQuestionDatas.Clear();
        curIndex = 0;
        score = 0;
        curTime = 0;
        curCorrect = 0;
        curError = 0;
        List<QuestionData> tempList = new List<QuestionData>();
        foreach (var data in DataManager.Instance.CfgQuestion.mDataMap.Values)
        {
            tempList.Add(data);
        }
        Shuffle(tempList);
        for (int i = 0; i < 10; i++)
        {
            listQuestionData.Add(tempList[i]);
        }
        RefreshData();
    }

    private void RefreshData()
    {
        if (curIndex >= listQuestionData.Count)
        {
            txtScore.text = $"{score}";
            ResultData resultData = new ResultData();
            resultData.errorQuestionDatas = errorQuestionDatas;
            resultData.score = score;
            if (UIManager2D.Instance.IsOpenPanel(ExamView.Name))
            {
                UIManager2D.Instance.OpenPanel(ResultView.Name, resultData); 
            }
            return;
        }
        canClickAnswer = true;
        var data = listQuestionData[curIndex];
        txtTitle.text = data.question;
        answers.Clear();
        answers.Add(data.A);
        answers.Add(data.B);
        answers.Add(data.C);
        answers.Add(data.D);
        listAnswer.numItems = answers.Count;
        txtScore.text = $"{score}";
        txtCount.text = $"{curIndex + 1}/{listQuestionData.Count}";
        txtCorrect.text = $"{curCorrect}";
        txtError.text = $"{curError}";
        curTime = 30;
        txtTime.text = $"{curTime}";
        TimeManager.Instance.AddTask(1f, true, TimeHandler, displayObject.gameObject);
    }

    private void TimeHandler()
    {
        curTime--;
        txtTime.text = $"{curTime}";
        if (curTime <= 0)
        {
            canClickAnswer = false;
            curTime = 0;
            TimeManager.Instance.RemoveTask(TimeHandler, displayObject.gameObject);
            ErrorQuestionData errorQuestionData = new ErrorQuestionData();
            var data = listQuestionData[curIndex];
            errorQuestionData.id = data.ID;
            errorQuestionData.errorAnswer = -1;
            errorQuestionDatas.Add(errorQuestionData);
            transError.Play(() =>
            {
                TimeManager.Instance.AddTask(1f, false, () =>
                {
                    transError.PlayReverse();
                    transError.Stop(true, true);
                    curIndex++;
                    curError++;
                    RefreshData();
                }, displayObject.gameObject);
            });
        }
    }

    private void Answer(int index)
    {
        canClickAnswer = false;
        TimeManager.Instance.RemoveTask(TimeHandler, displayObject.gameObject);
        var data = listQuestionData[curIndex];
        int tempIndex = 0;
        switch (data.rightAnswer)
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
            score += 10;
            transCorrect.Play(() =>
            {
                TimeManager.Instance.AddTask(1f, false, () =>
                {
                    transCorrect.PlayReverse();
                    transCorrect.Stop(true, true);
                    curIndex++;
                    curCorrect++;
                    RefreshData();
                }, displayObject.gameObject);

            });
        }
        else
        {
            ErrorQuestionData errorQuestionData = new ErrorQuestionData();
            errorQuestionData.id = data.ID;
            errorQuestionData.errorAnswer = index;
            errorQuestionDatas.Add(errorQuestionData);
            transError.Play(() =>
            {
                TimeManager.Instance.AddTask(1f, false, () =>
                {
                    transError.PlayReverse();
                    transError.Stop(true, true);
                    curIndex++;
                    curError++;
                    RefreshData();
                }, displayObject.gameObject);

            });
        }
    }


    public override void OnClose()
    {
        TimeManager.Instance.RemoveTask(TimeHandler, displayObject.gameObject);
    }

    public override void OnDestroy()
    {

    }
}
