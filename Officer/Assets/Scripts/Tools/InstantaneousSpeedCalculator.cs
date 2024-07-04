using UnityEngine;

public class InstantaneousSpeedCalculator : MonoBehaviour
{
    private Transform parent;
    private Vector3 previousPosition;
    private Vector3 currentPosition;
    private Vector3 instantaneousSpeed;

    public Vector3 InstantaneousSpeed { get => instantaneousSpeed; set => instantaneousSpeed = value; }

    void Start()
    {
        // 初始化前一个位置为物体的初始位置
        parent = transform.parent;
        previousPosition = parent.position;
    }

    void Update()
    {
        // 获取当前帧的位置
        currentPosition = transform.position;

        // 计算瞬时速度
        instantaneousSpeed = (currentPosition - previousPosition) / Time.deltaTime;

        // 更新前一个位置为当前帧的位置
        previousPosition = currentPosition;
    }
}