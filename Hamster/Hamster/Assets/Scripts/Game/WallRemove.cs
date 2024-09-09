using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRemove : MonoBehaviour
{
    public float transitionSpeed = 8;
    public float targetAlpha = 0f;
    public float raycastDistanceOffset = -0.1f; // ���߼��ľ���ƫ��
    public string wallTag = "Wall"; // ����ı�ǩ

    public float rayInvertDistOffset = 2;
    public Transform targetSpot;
    bool isChangeingOcpacity;
    private void Start()
    {
        EventManager.DispatchEvent<float[]>(EventCommon.INIT_TRANS_DATA, new float[] { transitionSpeed, targetAlpha });
    }
    void Update()
    {
        // ���������λ����ǰ��������
        var dir = targetSpot.position - transform.position;
        var dirNormalized = dir.normalized;
        var from = transform.position - dirNormalized * rayInvertDistOffset;
        Ray ray = new Ray(from, dirNormalized);

        // �ڳ����л�������
        var dist = dir.magnitude + raycastDistanceOffset + rayInvertDistOffset;
        Debug.DrawRay(ray.origin, ray.direction * dist, Color.cyan);

        RaycastHit[] hits = Physics.RaycastAll(ray);
        if (hits.Length > 0)
        {
            foreach (RaycastHit hit in hits)
            {
                if (hit.distance > dist)
                    continue;

                if (hit.collider.CompareTag(wallTag))
                {
                    Debug.LogWarning(hit.collider.gameObject);
                    TransparentEffect Te = hit.collider.GetComponent<TransparentEffect>();
                    Te.SetInView(true);
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
