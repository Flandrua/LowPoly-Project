using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRemove : MonoBehaviour
{
    public float transitionSpeed = 8;
    public float targetAlpha = 0f;
    public float raycastDistanceOffset = -0.1f; // 射线检测的距离偏移
    public string wallTag = "Wall"; // 物体的标签

    public float rayInvertDistOffset = 2;
    public Transform targetSpot;
    bool isChangeingOcpacity;
    private void Start()
    {
        EventManager.DispatchEvent<float[]>(EventCommon.INIT_TRANS_DATA, new float[] { transitionSpeed, targetAlpha });
    }
    void Update()
    {
        // 从摄像机的位置向前发射射线
        var dir = targetSpot.position - transform.position;
        var dirNormalized = dir.normalized;
        var from = transform.position - dirNormalized * rayInvertDistOffset;
        Ray ray = new Ray(from, dirNormalized);

        // 在场景中绘制射线
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
        // 如果射线碰撞到物体
        //    if (Physics.RaycastAll(ray, out hit, raycastDistance))
        //    {
        //        // 检查物体是否带有指定的标签
        //        if (hit.collider.CompareTag(wallTag))
        //        {
        //            TransparentEffect Te = hit.collider.GetComponent<TransparentEffect>();
        //            Te.inView = true;
        //        }
        //    }
    }
}
