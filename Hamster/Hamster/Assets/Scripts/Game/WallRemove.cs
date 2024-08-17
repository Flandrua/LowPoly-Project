using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRemove : MonoBehaviour
{
    public float transitionSpeed = 8;
    public float targetAlpha = 0f;
    public float raycastDistance = 2000f; // ���߼��ľ���
    public string wallTag = "Wall"; // ����ı�ǩ

    public Vector3 rayPosOffset;
    public Vector3 rayRoateOffset;
    bool isChangeingOcpacity;
    private void Start()
    {
        EventManager.DispatchEvent<float[]>(EventCommon.INIT_TRANS_DATA, new float[] { transitionSpeed, targetAlpha });
    }
    void Update()
    {
        // ���������λ����ǰ��������

        Ray ray = new Ray(transform.position + rayPosOffset, transform.forward + rayRoateOffset);
        //RaycastHit hit;

        // �ڳ����л�������
        Debug.DrawRay(ray.origin, ray.direction * raycastDistance, Color.red);

        RaycastHit[] hits = Physics.RaycastAll(ray);
        if (hits.Length > 0)
        {
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.CompareTag(wallTag))
                {
                    TransparentEffect Te = hit.collider.GetComponent<TransparentEffect>();
                    Te.inView = true;
                }
            }
        }
        // ���������ײ������
        //    if (Physics.RaycastAll(ray, out hit, raycastDistance))
        //    {
        //        // ��������Ƿ����ָ���ı�ǩ
        //        if (hit.collider.CompareTag(wallTag))
        //        {
        //            TransparentEffect Te = hit.collider.GetComponent<TransparentEffect>();
        //            Te.inView = true;
        //        }
        //    }
    }
}
