using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerManager : MonoBehaviour
{
    public GameObject xr;
    private CharacterController characterController;
    private XROrigin xrOrign;
    private CustomCharacterControllerDriver ccd;
    public GameObject cameraOffset;
    // Start is called before the first frame update
    private void Awake()
    {
        DataCenter.Instance.InitData();
    }
    void Start()
    {
        DataCenter.Instance.NewData();

        characterController =xr.GetComponent<CharacterController>();
        xrOrign= xr.GetComponent<XROrigin>();
        ccd = xr.GetComponent<CustomCharacterControllerDriver>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Use slider to change player height
    /// </summary>
    /// <param name="height"></param>
    public void OnHeightChange(float height)
    {
       characterController.height = height;
        xrOrign.CameraYOffset = height;
        cameraOffset.transform.position = cameraOffset.transform.parent.TransformPoint(new Vector3(0,height,0));
        ccd.UpdateHeight();
       //DebugHelper.Instance.DebugMsg($"{xrOrign.CameraYOffset},{characterController.height}");
    }


    public void OnBtnTest()
    {
        Debug.Log("press");
    }
}
