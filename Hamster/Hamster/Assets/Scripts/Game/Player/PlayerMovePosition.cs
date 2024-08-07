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

    public void AddXMovement(Vector3 mm)
    {
        var v = rb.velocity;
        v.x = mm.x;
        rb.velocity = v;

        //// 假设你希望在 X 轴上移动
        //Vector3 moveDirection = new Vector3(1, 0, 0); // X 轴方向

        //// 计算移动的目标位置
        //Vector3 targetPosition = transform.position + moveDirection * mm.x * Time.deltaTime;

        //// 使用 MovePosition 方法移动到目标位置
        //rb.MovePosition(targetPosition);
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

    public void StopXMovement()
    {
        var v = rb.velocity;
        v.x = 0;
        rb.velocity = v;
    }
}