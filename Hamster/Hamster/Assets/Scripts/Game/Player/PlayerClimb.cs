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
        rb = PlayerBehaviour.Instance.GetComponent<Rigidbody>();//Player的RB
        go = PlayerBehaviour.Instance.animator[0].gameObject;//模型本身的gameobject

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
                //从墙上爬下来，回正，打开gravity，
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
                //从墙上爬下来，回正，打开gravity，
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
            //当玩家进入box后，canClimb为true，此时检测玩家是否按下空格，如果按下，isClimb=true,强制固定玩家操作方式
            canClimb = true;
        }

        if (other.gameObject.name == "EdgeEnd" && isClimbing)
        {
            //玩家进入顶端
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

    private void RotateTo3DGround()
    {
        PlayerBehaviour.Instance.spider.SetActive(false);
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
    private void On2DClimbingWall()
    {
        PlayerBehaviour.Instance.spider.SetActive(true);
        //开始爬墙，关闭gravity
        isClimbing = true;
        rb.useGravity = false;
        rb.velocity = Vector3.zero;
        Vector3 tempR = go.transform.rotation.eulerAngles;
        tempR.x = -90;//垂直旋转
        Vector3 tempP = PlayerBehaviour.Instance.flip.transform.position;
        if (PlayerBehaviour.Instance.flip.transform.localScale.x == 1)//检测小鼠面对哪个方向，攀爬附着哪个方向，2D里面根据filp的正负来检测,1右边，-1左边
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
        if (PlayerBehaviour.Instance.flip.transform.localScale.x == 1)//检测小鼠面对哪个方向，攀爬附着哪个方向，2D里面根据filp的正负来检测,1右边，-1左边
            tempP.x -= 0.15f;
        else
            tempP.x += 0.15f;
        PlayerBehaviour.Instance.flip.transform.position = tempP;
        Vector3 tempR = go.transform.rotation.eulerAngles;
        tempR.x = 0;
        go.transform.rotation = Quaternion.Euler(tempR);
    }
}
