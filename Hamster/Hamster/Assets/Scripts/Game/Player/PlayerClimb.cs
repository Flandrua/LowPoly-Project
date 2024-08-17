using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClimb : MonoSingleton<PlayerClimb>
{
    //用一个trigger box检测，如果玩家的trigger on enter有tag climb的box进入的话，关闭gravity，打开is kinematic，旋转角色；当角色到edge的时候，强制触发动画，让角色回正(应该按空格，触发攀爬功能）
    public float raycastDistance = 10f; // 射线检测的距离
    public string wallTag = "Climb"; // 物体的标签
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
            //当玩家进入box后，canClimb为true，此时检测玩家是否按下空格，如果按下，isClimb=true,强制固定玩家操作方式
            canClimb = true;
        }

        if (other.gameObject.name == "EdgeEnd")
        {
            //玩家进入顶端

        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.name == "EdgeStart" && canClimb && !isClimb && Input.GetKeyDown(KeyCode.Space))
        {
            //开始爬墙，关闭gravity
            isClimb = true;
            rb.useGravity = false;
            if (rb.transform.rotation.y >= 0 && rb.transform.rotation.y <= 180)//检测小鼠面对哪个方向，攀爬附着哪个方向，但是这样背身攀爬会产生bug
            {
                rb.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));//平面旋转
            }
            else
            {
                rb.transform.rotation = Quaternion.Euler(new Vector3(0, -90, 0));//平面旋转
            }
            Vector3 tempR = go.transform.rotation.eulerAngles;
            tempR.x = -90;//垂直旋转
            Vector3 tempP = rb.transform.position;
            tempP.y += 0.15f;
            rb.transform.position = tempP;
            go.transform.rotation = Quaternion.Euler(tempR);
            PlayerBehaviour.instance.animator[0].Play("Idle_A");

        }
        else if (other.gameObject.name == "EdgeStart" && canClimb && isClimb && Input.GetKeyDown(KeyCode.Space))
        {
            //从墙上爬下来，回正，打开gravity，
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
