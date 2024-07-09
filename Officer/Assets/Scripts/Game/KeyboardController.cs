using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyboardController : MonoBehaviour
{
    public int requireHit = 6;
    private int actualHit = 0;
    private GameObject _particle;
    private Scrollbar _bar;//������
    // Start is called before the first frame update
    void Start()
    {
        _particle = transform.parent.Find("Work").gameObject;
        _bar = transform.parent.Find("Favorability").Find("Canvas").Find("Scrollbar").GetComponent<Scrollbar>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _particle.SetActive(true);
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
        if (other.CompareTag("Player"))
        {
            _particle.SetActive(false);
        }
    }

    private void HitHandle()
    {
        if (actualHit < requireHit)
        {
            actualHit++;
            _bar.size = actualHit / requireHit;
        }
        else if(actualHit == requireHit)
        {
            //���͹������֪ͨ
        }
    }
}