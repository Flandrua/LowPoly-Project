using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    private int curTimeStage = 0;//0���ϣ�1���磬2����
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
        //��Ҫ����С���״̬������bool������״̬
        //��Ҫ������ҵ�״̬
        //��Ҫ���ü��̵�״̬���ɻ���
        //�������������λ��
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
        else if (curTimeStage == 2)//һ���ȥ��
        {
            RenderSettings.skybox = morning;
            //С��ظ�Ѫ��
            DataCenter.Instance.GameData.HamsterData.hp = 10;
            curTimeStage = 0;
            if (SnackManager.Instance.isPlayer)            //������������ʳ����ȥ1�㹤��Ч��
            {
                DataCenter.Instance.GetWorkEfficiency(-1);
            }
            //�����Ҫ�ڴ�������
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
