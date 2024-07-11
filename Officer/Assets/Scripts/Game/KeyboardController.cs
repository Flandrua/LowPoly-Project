using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyboardController : MonoSingleton<KeyboardController>
{
    public int requireHit = 6;
    private int actualHit = 0;
    private GameObject _workEffect;
    private Scrollbar _bar;//������
    private ParticleSystem _star;
    // Start is called before the first frame update
    void Start()
    {
        _workEffect = transform.parent.Find("Work").gameObject;
        _star = transform.parent.Find("Work").Find("Star").GetComponent<ParticleSystem>();
        _bar = transform.parent.Find("Work").Find("Canvas").Find("Scrollbar").GetComponent<Scrollbar>();
        EventManager.AddListener(EventCommon.NEXT_STAGE, ResetToDefault);
    }
    private void OnDestroy()
    {
        EventManager.RemoveListener(EventCommon.NEXT_STAGE, ResetToDefault);
    }
    public void ResetToDefault()
    {
        actualHit = 0;
        _bar.size= 0;
    }
    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TimeManager.Instance.RemoveTask(BarHide,this);//�Ƴ������ʱ��
            _workEffect.SetActive(true);
            TimeManager.Instance.AddTask(5, false, BarHide, this);//5�������Bar
            InstantaneousSpeedCalculator calculator = other.GetComponent<InstantaneousSpeedCalculator>();
            if (calculator != null)
            {
                // ��ȡ�ٶȲ����
                Vector3 velocity = calculator.InstantaneousSpeed;
                float mag = velocity.magnitude;
                if (mag > 2.5)//�����Ϊ
                    HitHandle();
            }
        }
    }


    private void OnTriggerExit(Collider other)
    {
        //if (other.CompareTag("Player"))
        //{
        //    _workEffect.SetActive(false);
        //}
    }
    private void BarHide()
    {
        _workEffect?.SetActive(false);
    }

    private void HitHandle()
    {
        if (actualHit < requireHit)
        {
            actualHit++;
            _bar.size = ((float)actualHit / (float)requireHit);
            Debug.Log(actualHit);
        }
        else if(actualHit == requireHit)
        {
            actualHit++;//���ٴδ�����̵�ʱ�򣬲��������if
            _star.Play();
            //���͹������֪ͨ
            EventManager.DispatchEvent<string>(EventCommon.PREPARE_CHANGE_TIME,"work");
        }
    }
}