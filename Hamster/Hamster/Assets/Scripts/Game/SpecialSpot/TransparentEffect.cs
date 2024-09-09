using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransparentEffect : MonoBehaviour
{
    public bool inView = false;
    public Material opaque;
    public Material transparent;
    private Renderer wallRenderer;
    private float transitionSpeed = 8;
    private float targetAlpha = 0f;
    public bool doAppear = false;
    private bool isTransMat = false;

    void Start()
    {
        wallRenderer = GetComponent<Renderer>();
        EventManager.AddListener<float[]>(EventCommon.INIT_TRANS_DATA, Disappear);
    }
    private void OnDestroy()
    {
        EventManager.RemoveListener<float[]>(EventCommon.INIT_TRANS_DATA, Disappear);
    }

    void Update()
    {
        SetDoAppear(inView);

        if (doAppear)
        {
            if (!isTransMat)
            {
                wallRenderer.material = transparent;
                isTransMat = true;
            }
            Color wallColor = wallRenderer.material.color;
            wallColor.a = Mathf.Lerp(wallRenderer.material.color.a, targetAlpha, Time.deltaTime * transitionSpeed);
            wallRenderer.material.color = wallColor;
            if (wallColor.a <= targetAlpha + 0.05f)
            {
                SetDoAppear(false);
                SetInView(false);
            }

        }
        else if (!inView)
        {
            if (wallRenderer.material.color.a < 0.99)
            {
                Color wallColor = wallRenderer.material.color;
                wallColor.a = Mathf.Lerp(wallRenderer.material.color.a, 1, Time.deltaTime * transitionSpeed);
                wallRenderer.material.color = wallColor;
            }
            else
            {
                if (isTransMat)
                {
                    wallRenderer.material = opaque;
                    isTransMat = false;
                }
            }
        }
    }

    public void SetInView(bool b)
    {

        if (inView == b)
            return;
        Debug.Log("Set InView " + b);
        inView = b;
    }
    public void SetDoAppear(bool b)
    {
        if (doAppear == b)
            return;
        Debug.Log("Set DoAppear " + b);
        doAppear = b;
    }

    public void Disappear(float[] data)
    {
        if (this.transitionSpeed != data[0])
            this.transitionSpeed = data[0];
        if (this.targetAlpha != data[1])
            this.targetAlpha = data[1];
    }
}