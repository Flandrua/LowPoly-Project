using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    private int curTimeStage = 0;//0早上，1下午，2晚上
    public Material morning;
    public Material afternoon;
    public Material night;
    
    void Start()
    {
        EventManager.AddListener<string>(EventCommon.PREPARE_CHANGE_TIME, PrepareChangeTime);
        EventManager.AddListener(EventCommon.CHANGE_TIME,ChangeTime);
    }
    private void OnDestroy()
    {
        EventManager.RemoveListener<string>(EventCommon.PREPARE_CHANGE_TIME, PrepareChangeTime);
        EventManager.RemoveListener(EventCommon.CHANGE_TIME, ChangeTime);
    }

    private void PrepareChangeTime(string type)
    {
        if (type == "play")
        {
            DataCenter.Instance.GetWorkProgress(DataCenter.Instance.GetTotalWorkEfficiency());
        }
        else if (type == "work")
        {
            DataCenter.Instance.GetFavorability(DataCenter.Instance.GetTotalFavorabilityAbility());
        }

    }
    private void ChangeTime()
    {
        //需要重置小鼠的状态（各种bool，动画状态
        //需要重置玩家的状态
        //需要重置键盘的状态（可互动
        //重置所有物体的位置
        if (curTimeStage ==0)
        {
            RenderSettings.skybox = afternoon;
            curTimeStage++;
        }
        else if(curTimeStage == 1)
        {
            RenderSettings.skybox = night;
            curTimeStage++;
        }
        else if (curTimeStage == 2)//一天过去了
        {
            RenderSettings.skybox = morning;
            //小鼠回复血量
            DataCenter.Instance.GameData.HamsterData.hp = 10;
            curTimeStage = 0;
            if (SnackManager.Instance.isPlayer)            //玩家如果吃了零食，减去1点工作效率
            {
                DataCenter.Instance.GetWorkEfficiency(-1);
            }
            //玩家需要在床边醒来
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
