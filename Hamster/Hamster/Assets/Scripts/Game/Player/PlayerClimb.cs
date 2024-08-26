using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClimb : MonoSingleton<PlayerClimb>
{
    //用一个trigger box检测，如果玩家的trigger on enter有tag climb的box进入的话，关闭gravity，打开is kinematic，旋转角色；当角色到edge的时候，强制触发动画，让角色回正(应该按空格，触发攀爬功能）
    public float raycastDistance = 10f; // 射线检测的距离
    //public string wallTag = "Climb"; // 物体的标签
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
            //从墙上爬下来，回正，打开gravity，
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
            //当玩家进入box后，canClimb为true，此时检测玩家是否按下空格，如果按下，isClimb=true,强制固定玩家操作方式
            canClimb = true;
        }

        if (other.gameObject.name == "EdgeEnd" && isClimbing)
        {
            //玩家进入顶端
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
        //开始爬墙，关闭gravity
        isClimbing = true;
        rb.useGravity = false;
        rb.velocity = Vector3.zero;
        if (rb.transform.eulerAngles.y >= 180 && rb.transform.eulerAngles.y <= 360)//检测小鼠面对哪个方向，攀爬附着哪个方向，但是这样背身攀爬会产生bug
        {
            rb.transform.rotation = Quaternion.Euler(new Vector3(0, -90, 0));//平面旋转
        }
        else
        {
            rb.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));//平面旋转
        }
        Vector3 tempR = go.transform.rotation.eulerAngles;
        tempR.x = -90;//垂直旋转
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
        if (rb.transform.eulerAngles.y >= 180 && rb.transform.eulerAngles.y <= 360)//检测小鼠面对哪个方向，攀爬附着哪个方向，但是这样背身攀爬会产生bug
        {
            rb.transform.rotation = Quaternion.Euler(new Vector3(0, -90, 0));//平面旋转
        }
        else
        {
            rb.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));//平面旋转
        }
        Vector3 tempR = go.transform.rotation.eulerAngles;
        tempR.x = 0;
        go.transform.rotation = Quaternion.Euler(tempR);
    }
}
