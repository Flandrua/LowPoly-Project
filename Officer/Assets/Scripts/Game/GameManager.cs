using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        EventManager.AddListener(EventCommon.CHANGE_TIME, ChangeTime);
    }
    private void OnDestroy()
    {
        EventManager.RemoveListener(EventCommon.CHANGE_TIME, ChangeTime);
    }

    private void ChangeTime()
    {
        //��Ҫ����С���״̬������bool������״̬
        //��Ҫ������ҵ�״̬
        //��Ҫ���ü��̵�״̬���ɻ���

        //���һ���ȥ��
        //С��ظ�Ѫ��
        //������������ʳ����ȥ1�㹤��Ч��
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
