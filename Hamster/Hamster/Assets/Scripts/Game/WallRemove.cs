using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRemove : MonoBehaviour
{
    public float transitionSpeed = 8;
    public float targetAlpha = 0f;
    public float raycastDistance = 2000f; // ���߼��ľ���
    public string wallTag = "Wall"; // ����ı�ǩ

    bool isChangeingOcpacity;
    void Update()
    {
        // ���������λ����ǰ��������
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        // �ڳ����л�������
        //Debug.DrawRay(ray.origin, ray.direction * raycastDistance, Color.red);

        // ���������ײ������
        if (Physics.Raycast(ray, out hit, raycastDistance))
        {
            // ��������Ƿ����ָ���ı�ǩ
            if (hit.collider.CompareTag(wallTag))
            {
                // ��ȡ�����Renderer���
                Renderer wallRenderer = hit.collider.GetComponent<Renderer>();

                // ����Ƿ����Renderer���
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
