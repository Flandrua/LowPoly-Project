using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyboardController : MonoSingleton<KeyboardController>
{
    public int requireHit = 6;
    private int actualHit = 0;
    private GameObject _workEffect;
    private Scrollbar _bar;//交互条
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
            TimeManager.Instance.RemoveTask(BarHide,this);//移除该类计时器
            _workEffect.SetActive(true);
            TimeManager.Instance.AddTask(5, false, BarHide, this);//5秒后隐藏Bar
            InstantaneousSpeedCalculator calculator = other.GetComponent<InstantaneousSpeedCalculator>();
            if (calculator != null)
            {
                // 获取速度并输出
                Vector3 velocity = calculator.InstantaneousSpeed;
                float mag = velocity.magnitude;
                if (mag > 2.5)//打击行为
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
            actualHit++;//让再次打击键盘的时候，不触发这个if
            _star.Play();
            //发送工作完成通知
            EventManager.DispatchEvent<string>(EventCommon.PREPARE_CHANGE_TIME,"work");
        }
    }
}