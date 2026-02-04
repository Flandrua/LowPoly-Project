using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSteamVRManager : MonoBehaviour
{
    private float height = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnHeightUp()
    {
        Vector3 pos = transform.position;
        height += 0.1f;
        pos.y = height;
        transform.position = pos;
        UIManager.Instance.UpdateHeight(height);
    }
    public void OnHeightDown()
    {
        Vector3 pos = transform.position;
        height -= 0.1f;
        pos.y = height;
        transform.position = pos;
        UIManager.Instance.UpdateHeight(height);
    }
}
