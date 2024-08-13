using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRemove : MonoBehaviour
{
    public float transitionSpeed = 8;
    public float targetAlpha = 0f;
    public float raycastDistance = 2000f; // 射线检测的距离
    public string wallTag = "Wall"; // 物体的标签

    bool isChangeingOcpacity;
    void Update()
    {
        // 从摄像机的位置向前发射射线
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        // 在场景中绘制射线
        //Debug.DrawRay(ray.origin, ray.direction * raycastDistance, Color.red);

        // 如果射线碰撞到物体
        if (Physics.Raycast(ray, out hit, raycastDistance))
        {
            // 检查物体是否带有指定的标签
            if (hit.collider.CompareTag(wallTag))
            {
                // 获取物体的Renderer组件
                Renderer wallRenderer = hit.collider.GetComponent<Renderer>();

                // 检查是否存在Renderer组件
                if (wallRenderer != null)
                {
                        Color wallColor = wallRenderer.material.color;
                        wallColor.a = Mathf.Lerp(wallRenderer.material.color.a, targetAlpha, Time.deltaTime * transitionSpeed);
                        wallRenderer.material.color = wallColor;  
                }
            }
        }
    }
}
