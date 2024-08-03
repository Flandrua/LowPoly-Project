using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCTVData : ItemData
{
    public List<RenderTexture> cameraTexture;
    public Material material;
    private int count = 0;
    // Start is called before the first frame update


    // Update is called once per frame
    void Update()
    {

    }
    public void SwitchCCTV()
    {
        if (count + 2 <= cameraTexture.Count)
            count++;
        else
            count = 0;
        material.SetTexture("_MainTex", cameraTexture[count]);
    }
}
