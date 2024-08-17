using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClimb : MonoSingleton<PlayerClimb>
{
    //��һ��trigger box��⣬�����ҵ�trigger on enter��tag climb��box����Ļ����ر�gravity����is kinematic����ת��ɫ������ɫ��edge��ʱ��ǿ�ƴ����������ý�ɫ����(Ӧ�ð��ո񣬴����������ܣ�
    public float raycastDistance = 10f; // ���߼��ľ���
    public string wallTag = "Climb"; // ����ı�ǩ
    private Rigidbody rb;
    private GameObject go;

    public bool isClimb = false;
    public bool canClimb = false;
    private bool canJudgeClimb = true;
    // Start is called before the first frame update
    void Start()
    {
        rb = PlayerBehaviour.instance.GetComponent<Rigidbody>();
        go = PlayerBehaviour.instance.animator[0].gameObject;

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "EdgeStart")
        {
            //����ҽ���box��canClimbΪtrue����ʱ�������Ƿ��¿ո�������£�isClimb=true,ǿ�ƹ̶���Ҳ�����ʽ
            canClimb = true;
        }

        if (other.gameObject.name == "EdgeEnd")
        {
            //��ҽ��붥��

        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.name == "EdgeStart" && canClimb && !isClimb && Input.GetKeyDown(KeyCode.Space))
        {
            //��ʼ��ǽ���ر�gravity
            isClimb = true;
            rb.useGravity = false;
            if (rb.transform.rotation.y >= 0 && rb.transform.rotation.y <= 180)//���С������ĸ��������������ĸ����򣬵��������������������bug
            {
                rb.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));//ƽ����ת
            }
            else
            {
                rb.transform.rotation = Quaternion.Euler(new Vector3(0, -90, 0));//ƽ����ת
            }
            Vector3 tempR = go.transform.rotation.eulerAngles;
            tempR.x = -90;//��ֱ��ת
            Vector3 tempP = rb.transform.position;
            tempP.y += 0.15f;
            rb.transform.position = tempP;
            go.transform.rotation = Quaternion.Euler(tempR);
            PlayerBehaviour.instance.animator[0].Play("Idle_A");

        }
        else if (other.gameObject.name == "EdgeStart" && canClimb && isClimb && Input.GetKeyDown(KeyCode.Space))
        {
            //��ǽ������������������gravity��
            isClimb = false;
            rb.useGravity = true;
            rb.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
            Vector3 tempR = go.transform.rotation.eulerAngles;
            tempR.x = 0;
            go.transform.rotation = Quaternion.Euler(tempR);

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "EdgeStart")
        {
            canClimb = false;
        }
    }
}
