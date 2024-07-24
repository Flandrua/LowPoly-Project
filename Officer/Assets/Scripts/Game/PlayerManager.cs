using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerManager :MonoSingleton<PlayerManager>
{
    public GameObject xr;
    public int tempEfficiency = 0;
    private CharacterController characterController;
    private XROrigin xrOrign;
    private CustomCharacterControllerDriver ccd;
    public GameObject cameraOffset;
    public Vector3 initCameraOffset;
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private ParticleSystem _flame;
    private AudioSource _as;
    // Start is called before the first frame update
    private void Awake()
    {
        DataCenter.Instance.InitData();
    }
    void Start()
    {
        DataCenter.Instance.NewData();
        EventManager.AddListener<SnackData>(EventCommon.PLAYER_FINISH_EATING, PlayerFinishEating);
        characterController = xr.GetComponent<CharacterController>();
        xrOrign = xr.GetComponent<XROrigin>();
        ccd = xr.GetComponent<CustomCharacterControllerDriver>();
        initialPosition = xr.transform.position;
        initialRotation = xr.transform.rotation;
        _flame = GetComponentInChildren<ParticleSystem>();
        _as = GetComponent<AudioSource>();
        cameraOffset.transform.localPosition = initCameraOffset;
    }
    private void OnDestroy()
    {
        EventManager.RemoveListener<SnackData>(EventCommon.PLAYER_FINISH_EATING, PlayerFinishEating);
    }
    public void ResetLocation()
    {
        DataCenter.Instance.GetWorkEfficiency(-tempEfficiency);
        _flame.Stop();
        tempEfficiency = 0;
        xr.transform.position = initialPosition;
        xr.transform.rotation = initialRotation;
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
        cameraOffset.transform.position = cameraOffset.transform.parent.TransformPoint(new Vector3(0, height, 0));
        ccd.UpdateHeight();
        //DebugHelper.Instance.DebugMsg($"{xrOrign.CameraYOffset},{characterController.height}");
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Snack"))
        {
            _as.Play();
            EventManager.DispatchEvent(EventCommon.PLAYER_EATING, true);//给SnackManager发送开始吃的通知

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Snack"))
        {

            EventManager.DispatchEvent(EventCommon.PLAYER_EATING, false);//给SnackManager发送中断吃的通知
        }
    }
    private void PlayerFinishEating(SnackData snack)
    {
        tempEfficiency = snack.workEfficiency;
        DataCenter.Instance.GetWorkEfficiency(snack.workEfficiency);
        //判断是否吃了特殊零食
        if (snack.isSpicy)
        {
            _flame.Play();
        }

    }
}
