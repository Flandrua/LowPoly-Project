using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class GameManager : MonoSingleton<GameManager>
{
    private int curTimeStage = 0;//0早上，1下午，2晚上
    private Animator _animator;
    public Material morning;
    public LightingDataAsset morningDataAsset;
    public Material afternoon;
    public LightingDataAsset afternoonDataAsset;
    public Material night;
    public LightingDataAsset nightDataAsset;
    public int totaldays = 10;
    public int goalWorkPrgoress = 50;
    
    void Start()
    {
        EventManager.AddListener<string>(EventCommon.PREPARE_CHANGE_TIME, PrepareChangeTime);
        EventManager.AddListener<bool>(EventCommon.CHANGE_TIME, SendAnimatorPostSignal);
        _animator = GetComponent<Animator>();
    }
    private void OnDestroy()
    {
        EventManager.RemoveListener<string>(EventCommon.PREPARE_CHANGE_TIME, PrepareChangeTime);
        EventManager.RemoveListener<bool>(EventCommon.CHANGE_TIME, SendAnimatorPostSignal);
    }

    private void PrepareChangeTime(string type)
    {
        if (type == "play")
        {
            DataCenter.Instance.GetFavorability(DataCenter.Instance.GetTotalFavorabilityAbility());
        }
        else if (type == "work")
        {
            DataCenter.Instance.GetWorkProgress(DataCenter.Instance.GetTotalWorkEfficiency());
            EventManager.DispatchEvent(EventCommon.UPDATE_MONITOR);
        }

    }
    private void SendAnimatorPostSignal(bool flag)
    {
        _animator.SetBool("post", flag);
    }
    private void ChangeTime()//动画事件触发
    {
        //需要重置小鼠的状态（各种bool，动画状态
        //需要重置玩家的状态
        //需要重置键盘的状态（可互动
        //重置所有物体的位置
        if (curTimeStage ==0)
        {
            RenderSettings.skybox = afternoon;
            Lightmapping.lightingDataAsset = afternoonDataAsset;
            curTimeStage++;
        }
        else if(curTimeStage == 1)
        {
            RenderSettings.skybox = night;
            Lightmapping.lightingDataAsset = nightDataAsset;
            curTimeStage++;
        }
        else if (curTimeStage == 2)//一天过去了
        {
            RenderSettings.skybox = morning;
            Lightmapping.lightingDataAsset = morningDataAsset;
            //小鼠回复血量
            DataCenter.Instance.GameData.HamsterData.hp = 10;
            //如果吃过零食了，随机道具
            if (HamsterController.Instance.isOut) { MainItemManager.Instance.RandomItem(); }
            //重置小鼠状态
            HamsterController.Instance.ResetMoveAnimation();
            curTimeStage = 0;
            if (SnackManager.Instance.isPlayer)            //玩家如果吃了零食，减去1点工作效率
            {
                DataCenter.Instance.GetWorkEfficiency(-1);
            }
            //玩家需要在床边醒来
            PlayerManager.Instance.ResetLocation();
            //随机新的零食
            SnackManager.Instance.RandomSnack();
        }
        EventManager.DispatchEvent(EventCommon.UPDATE_MONITOR);
        EventManager.DispatchEvent(EventCommon.NEXT_STAGE);
        TimeManager.Instance.AddTask(1, false, () => { SendAnimatorPostSignal(false); }, this);

    }


    // Update is called once per frame
    void Update()
    {
        
    }

}
