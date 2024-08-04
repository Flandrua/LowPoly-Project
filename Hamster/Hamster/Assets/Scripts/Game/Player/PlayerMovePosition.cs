using UnityEngine;
using System.Collections;

public class PlayerMovePosition : MonoBehaviour
{
    public Rigidbody rb { get; private set; }
    private Vector3 _movement;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
    }

    public void AddXMovement(Vector3 mm)
    {
        //var v = rb.velocity;
        //v.x = mm.x;
        //rb.velocity = v;

        // 假设你希望在 X 轴上移动
        Vector3 moveDirection = new Vector3(1, 0, 0); // X 轴方向

        // 计算移动的目标位置
        Vector3 targetPosition = transform.position + moveDirection * mm.x * Time.deltaTime;

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