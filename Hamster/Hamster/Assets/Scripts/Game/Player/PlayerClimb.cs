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
        rb = PlayerBehaviour.Instance.GetComponent<Rigidbody>();//Player��RB
        go = PlayerBehaviour.Instance.animator[0].gameObject;//ģ�ͱ����gameobject

    }
    private void OnDestroy()
    {
        EventManager.RemoveListener(EventCommon.MOVE_TO_TOP, MoveToTop);
    }

    // Update is called once per frame
    void Update()
    {
        if (!DataCenter.Instance.GameData.Abilities.Contains(Ability.Spider)) return;
        //Debug.Log(rb.transform.eulerAngles.y);
        if (PlayerBehaviour.Instance.isSewage)
        {
            if (canClimb && !isClimbing && Input.GetKeyDown(KeyCode.Space))
            {      
                On3DClimbingWall();
            }
            else if (canClimb && isClimbing && Input.GetKeyDown(KeyCode.Space))
            {
                //��ǽ������������������gravity��
                RotateTo3DGround();
            }
        }
        else
        {
            if (canClimb && !isClimbing && Input.GetKeyDown(KeyCode.Space))
            {
                On2DClimbingWall();
            }
            else if (canClimb && isClimbing && Input.GetKeyDown(KeyCode.Space))
            {
                //��ǽ������������������gravity��
                RotateTo2DGround();
            }
        }
    }
    private void MoveToTop()
    {
        PlayerBehaviour.Instance.move.canInput = true;
        Vector3 temp = curTop.transform.position;
        if (PlayerBehaviour.Instance.isSewage)
        {
            temp.x = rb.transform.position.x;
            RotateTo3DGround();
        }
        else
            RotateTo2DGround();

        rb.transform.position = temp;
        PlayerBehaviour.Instance.spider.SetActive(false);
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
            PlayerBehaviour.Instance.move.canInput = false;
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

    private void On3DClimbingWall()
    {
        PlayerBehaviour.Instance.spider.SetActive(true);
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

    private void RotateTo3DGround()
    {
        PlayerBehaviour.Instance.spider.SetActive(false);
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
    private void On2DClimbingWall()
    {
        PlayerBehaviour.Instance.spider.SetActive(true);
        //��ʼ��ǽ���ر�gravity
        isClimbing = true;
        rb.useGravity = false;
        rb.velocity = Vector3.zero;
        Vector3 tempR = go.transform.rotation.eulerAngles;
        tempR.x = -90;//��ֱ��ת
        Vector3 tempP = PlayerBehaviour.Instance.flip.transform.position;
        if (PlayerBehaviour.Instance.flip.transform.localScale.x == 1)//���С������ĸ��������������ĸ�����2D�������filp�����������,1�ұߣ�-1���
            tempP.x += 0.15f;
        else
            tempP.x -= 0.15f;

        PlayerBehaviour.Instance.flip.transform.position = tempP;
        Vector3 tempRbP = rb.transform.position;
        tempRbP.y += 0.15f;
        rb.transform.position = tempRbP;
        go.transform.rotation = Quaternion.Euler(tempR);
        PlayerBehaviour.Instance.animator[0].Play("Idle_A");
    }

    private void RotateTo2DGround()
    {
        PlayerBehaviour.Instance.spider.SetActive(false);
        isClimbing = false;
        rb.useGravity = true;
        Vector3 tempP = PlayerBehaviour.Instance.flip.transform.position;
        if (PlayerBehaviour.Instance.flip.transform.localScale.x == 1)//���С������ĸ��������������ĸ�����2D�������filp�����������,1�ұߣ�-1���
            tempP.x -= 0.15f;
        else
            tempP.x += 0.15f;
        PlayerBehaviour.Instance.flip.transform.position = tempP;
        Vector3 tempR = go.transform.rotation.eulerAngles;
        tempR.x = 0;
        go.transform.rotation = Quaternion.Euler(tempR);
    }
}
