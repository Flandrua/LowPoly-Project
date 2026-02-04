using UnityEngine;
using Valve.VR;

public class VRInputController : MonoBehaviour
{
    [Header("动作引用")]
    public SteamVR_Action_Vector2 joystickAction; 
    public SteamVR_Action_Vector2 joystickRotate; 
    [Header("配置参数")]
    public float moveSpeed = 3.0f;
    public float rotateSpeed = 100.0f; // 平滑转向速度
    public float snapTurnAngle = 45.0f; // 瞬间转向角度

    [Header("对象引用")]
    public Transform playerTransform; // 玩家根物体
    public Transform headTransform;   // 摄像头/头部

    private bool canSnapTurn = true; // 用于瞬间转向的冷却防止连续旋转

    void Update()
    {
        HandleLeftHandMovement();
        HandleRightHandRotation();
    }

    // --- 左手：平滑移动 ---
    private void HandleLeftHandMovement()
    {
        Vector2 axis = joystickAction.GetAxis(SteamVR_Input_Sources.LeftHand);

        if (axis.sqrMagnitude > 0.05f)
        {
            // 1. 获取相机前向向量并强行抹平 Y 轴（最直接的方法）
            Vector3 forward = headTransform.forward;
            forward.y = 0;
            forward.Normalize(); // 重新归一化，确保速度恒定

            // 2. 根据水平的前方计算出水平的右方
            Vector3 right = headTransform.right;
            right.y = 0;
            right.Normalize();

            // 3. 计算移动向量
            // 前后 (axis.y) + 左右 (axis.x)
            Vector3 moveDirection = (forward * axis.y + right * axis.x);

            // 4. 应用位移
            playerTransform.position += moveDirection * moveSpeed * Time.deltaTime;
        }
    }

    // --- 右手：转向逻辑 ---
    private void HandleRightHandRotation()
    {
        Vector2 axis = joystickRotate.GetAxis(SteamVR_Input_Sources.RightHand);

        // 方式 A: 平滑转向 (类似推摇杆慢慢转动)
        if (Mathf.Abs(axis.x) > 0.3f)
        {
            playerTransform.Rotate(0, axis.x * rotateSpeed * Time.deltaTime, 0);
        }

        /* // 方式 B: 瞬间转向 (Snap Turn - 比较不容易晕车)
        if (Mathf.Abs(axis.x) > 0.7f && canSnapTurn)
        {
            float angle = axis.x > 0 ? snapTurnAngle : -snapTurnAngle;
            playerTransform.Rotate(0, angle, 0);
            canSnapTurn = false; // 锁定，直到摇杆回到中心
        }
        else if (Mathf.Abs(axis.x) < 0.2f)
        {
            canSnapTurn = true; // 重置锁定
        }
        */
    }
}