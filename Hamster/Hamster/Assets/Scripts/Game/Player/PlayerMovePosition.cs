using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class PlayerMovePosition : MonoBehaviour
{
    public Rigidbody rb { get; private set; }
    private Vector3 _movement;
    public bool isHurtMove;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (isHurtMove)
        {
            DoMove();
        }
    }

    public void AddInputMovement(Vector3 mm, bool isX = true, bool isY = false)
    {
        if (isX)
        {
            var v = rb.velocity;
            v.x = mm.x;
            rb.velocity = v;
        }
        if (isY)
        {
            var v = rb.velocity;
            v.y =- mm.y;
            rb.velocity = v;
        }
        else
        {
            var v = rb.velocity;
            v.x = -transform.right.x * mm.x;
            v.z = -transform.right.z * mm.x;
            rb.velocity = v;
            //Debug.Log(Vector3.Magnitude(rb.velocity));
        }
    }   

    /// <summary>
    /// 
    /// </summary>
    /// <param name="mm">给的速度向量</param>
    /// <param name="t">持续多少时间后恢复重力和操作</param>
    /// <param name="banGravity">期间是否禁用重力，默认true</param>
    /// <param name="banInput">期间是否禁止操作，默认true</param>
    /// <param name="resetSpeed">期间是否重置已有的速度，默认true</param>
    public void AddMovement(Vector3 mm, float t, bool banGravity = true, bool banInput = true, bool resetSpeed = true)
    {
        if (isHurtMove) return;
        else
        {
            if (banInput)
                PlayerBehaviour.instance.move.canInput = false;
            if (banGravity)
                rb.useGravity = false;
            if (resetSpeed)
                PlayerBehaviour.instance.move.ResetSpeed();
            isHurtMove = true;

        }
        TimeManager.Instance.AddTask(t, false, () =>
        {
            isHurtMove = false;
            PlayerBehaviour.instance.move.canInput = true;
            rb.useGravity = true;
        }, this);
        _movement = mm;
    }
    private void DoMove()
    {
        // 计算移动的目标位置
        Vector3 targetPosition = transform.position + _movement * Time.deltaTime;

        // 使用 MovePosition 方法移动到目标位置
        rb.MovePosition(targetPosition);
    }

    public void StopInputMovement(bool isX = true, bool isY = true  )
    {
        if (isX)
        {
            var v = rb.velocity;
            v.x = 0;
            rb.velocity = v;
        }
        if (isY)
        {
            var v = rb.velocity;
            v.y = 0;
            rb.velocity = v;
        }
        else
        {
            var v = rb.velocity;
            v.x = 0;
            v.z = 0;
            rb.velocity = v;
        }
    }
}