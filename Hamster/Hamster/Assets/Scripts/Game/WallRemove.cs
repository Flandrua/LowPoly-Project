using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRemove : MonoBehaviour
{
    public float transitionSpeed = 8;
    public float targetAlpha = 0f;
    public float raycastDistance = 2000f; // 射线检测的距离
    public string wallTag = "Wall"; // 物体的标签

    public Vector3 rayPosOffset;
    public Vector3 rayRoateOffset;
    bool isChangeingOcpacity;
    private void Start()
    {
        EventManager.DispatchEvent<float[]>(EventCommon.INIT_TRANS_DATA, new float[] { transitionSpeed, targetAlpha });
    }
    void Update()
    {
        // 从摄像机的位置向前发射射线

        Ray ray = new Ray(transform.position + rayPosOffset, transform.forward + rayRoateOffset);
        //RaycastHit hit;

        // 在场景中绘制射线
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
