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
        // ��ʼ��ǰһ��λ��Ϊ����ĳ�ʼλ��
        parent = transform.parent;
        previousPosition = parent.position;
    }

    void Update()
    {
        // ��ȡ��ǰ֡��λ��
        currentPosition = transform.position;

        // ����˲ʱ�ٶ�
        instantaneousSpeed = (currentPosition - previousPosition) / Time.deltaTime;

        // ����ǰһ��λ��Ϊ��ǰ֡��λ��
        previousPosition = currentPosition;
    }
}