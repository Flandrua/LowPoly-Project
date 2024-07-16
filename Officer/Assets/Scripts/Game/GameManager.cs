using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class GameManager : MonoSingleton<GameManager>
{
    private int curTimeStage = 0;//0���ϣ�1���磬2����
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
    private void ChangeTime()//�����¼�����
    {
        //��Ҫ����С���״̬������bool������״̬
        //��Ҫ������ҵ�״̬
        //��Ҫ���ü��̵�״̬���ɻ���
        //�������������λ��
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
        else if (curTimeStage == 2)//һ���ȥ��
        {
            RenderSettings.skybox = morning;
            Lightmapping.lightingDataAsset = morningDataAsset;
            //С��ظ�Ѫ��
            DataCenter.Instance.GameData.HamsterData.hp = 10;
            //����Թ���ʳ�ˣ��������
            if (HamsterController.Instance.isOut) { MainItemManager.Instance.RandomItem(); }
            //����С��״̬
            HamsterController.Instance.ResetMoveAnimation();
            curTimeStage = 0;
            if (SnackManager.Instance.isPlayer)            //������������ʳ����ȥ1�㹤��Ч��
            {
                DataCenter.Instance.GetWorkEfficiency(-1);
            }
            //�����Ҫ�ڴ�������
            PlayerManager.Instance.ResetLocation();
            //����µ���ʳ
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
