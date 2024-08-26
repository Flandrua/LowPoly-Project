using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClimb : MonoSingleton<PlayerClimb>
{
    //��һ��trigger box��⣬�����ҵ�trigger on enter��tag climb��box����Ļ����ر�gravity����is kinematic����ת��ɫ������ɫ��edge��ʱ��ǿ�ƴ����������ý�ɫ����(Ӧ�ð��ո񣬴����������ܣ�
    public float raycastDistance = 10f; // ���߼��ľ���
    //public string wallTag = "Climb"; // ����ı�ǩ
    private Rigidbody rb;
    private GameObject go;
    private GameObject curTop;

    public bool isClimbing = false;
    public bool canClimb = false;
    private bool canJudgeClimb = true;
    // Start is called before the first frame update
    void Start()
    {
        EventManager.AddListener(EventCommon.MOVE_TO_TOP, MoveToTop);
        rb = PlayerBehaviour.Instance.GetComponent<Rigidbody>();
        go = PlayerBehaviour.Instance.animator[0].gameObject;

    }
    private void OnDestroy()
    {
        EventManager.RemoveListener(EventCommon.MOVE_TO_TOP, MoveToTop);
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(rb.transform.eulerAngles.y);
        if (canClimb && !isClimbing && Input.GetKeyDown(KeyCode.Space))
        {
            OnClimbingWall();
        }
        else if (canClimb && isClimbing && Input.GetKeyDown(KeyCode.Space))
        {
            //��ǽ������������������gravity��
            RotateToGround();
        }
    }
    private void MoveToTop()
    {
        Vector3 temp = curTop.transform.position;
        temp.x = rb.transform.position.x;
        rb.transform.position = temp;
        RotateToGround();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "EdgeStart")
        {
            //����ҽ���box��canClimbΪtrue����ʱ�������Ƿ��¿ո�������£�isClimb=true,ǿ�ƹ̶���Ҳ�����ʽ
            canClimb = true;
        }

        if (other.gameObject.name == "EdgeEnd" && isClimbing)
        {
            //��ҽ��붥��
            curTop = other.GetComponent<ClimbBoxSpot>().TopPos;
            PlayerBehaviour.Instance.animator[0].SetTrigger("MoveToTop");
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "EdgeStart")
        {
            canClimb = false;
        }
    }

    private void OnClimbingWall()
    {
        //��ʼ��ǽ���ر�gravity
        isClimbing = true;
        rb.useGravity = false;
        rb.velocity = Vector3.zero;
        if (rb.transform.eulerAngles.y >= 180 && rb.transform.eulerAngles.y <= 360)//���С������ĸ��������������ĸ����򣬵��������������������bug
        {
            rb.transform.rotation = Quaternion.Euler(new Vector3(0, -90, 0));//ƽ����ת
        }
        else
        {
            rb.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));//ƽ����ת
        }
        Vector3 tempR = go.transform.rotation.eulerAngles;
        tempR.x = -90;//��ֱ��ת
        Vector3 tempP = rb.transform.position;
        tempP.y += 0.15f;
        rb.transform.position = tempP;
        go.transform.rotation = Quaternion.Euler(tempR);
        PlayerBehaviour.Instance.animator[0].Play("Idle_A");
    }

    private void RotateToGround()
    {
        isClimbing = false;
        rb.useGravity = true;
        if (rb.transform.eulerAngles.y >= 180 && rb.transform.eulerAngles.y <= 360)//���С������ĸ��������������ĸ����򣬵��������������������bug
        {
            rb.transform.rotation = Quaternion.Euler(new Vector3(0, -90, 0));//ƽ����ת
        }
        else
        {
            rb.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));//ƽ����ת
        }
        Vector3 tempR = go.transform.rotation.eulerAngles;
        tempR.x = 0;
        go.transform.rotation = Quaternion.Euler(tempR);
    }
}
