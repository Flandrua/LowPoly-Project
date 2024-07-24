using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoSingleton<MouseManager>
{
    public bool canSwitchTime = false;
    private AudioSource _as;

    // Start is called before the first frame update
    void Start()
    {
        EventManager.AddListener<string>(EventCommon.PREPARE_CHANGE_TIME, CanSwitchTime);
        EventManager.AddListener(EventCommon.NEXT_STAGE, ResetToDefault);
        _as = GetComponent<AudioSource>();
    }
    private void OnDestroy()
    {
        EventManager.RemoveListener<string>(EventCommon.PREPARE_CHANGE_TIME, CanSwitchTime);
        EventManager.RemoveListener(EventCommon.NEXT_STAGE, ResetToDefault);
    }


    // Update is called once per frame
    void Update()
    {

    }
    public void ResetToDefault()
    {
        canSwitchTime = false;
    }
    private void CanSwitchTime(string str)
    {
        //��������и���������ʾ
        canSwitchTime = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            InstantaneousSpeedCalculator calculator = other.GetComponent<InstantaneousSpeedCalculator>();
            if (calculator != null)
            {
                // ��ȡ�ٶȲ����
                Vector3 velocity = calculator.InstantaneousSpeed;
                float mag = velocity.magnitude;
                if (mag > 2.5 && canSwitchTime)//�����Ϊ
                {
                    _as.Play();
                    EventManager.DispatchEvent<bool>(EventCommon.CHANGE_TIME, true);
                }
            }
        }

    }
}


