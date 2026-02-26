using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSteamVRManager : MonoSingleton<PlayerSteamVRManager>
{
    public GameObject playerGO;
    public GameObject HeightGO;
    private float height = 1;
    private ParticleSystem _flame;
    public int tempEfficiency = 0;
    private AudioSource _as;
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    // Start is called before the first frame update
    void Start()
    {
        _flame = GetComponentInChildren<ParticleSystem>();
        _as = GetComponent<AudioSource>();
        initialPosition = playerGO.transform.position;
        initialRotation = playerGO.transform.rotation;
        EventManager.AddListener<SnackData>(EventCommon.PLAYER_FINISH_EATING, PlayerFinishEating);
    }

    // Update is called once per frame
    void Update()
    {
        EventManager.RemoveListener<SnackData>(EventCommon.PLAYER_FINISH_EATING, PlayerFinishEating);
    }
    public void ResetLocation()
    {
        DataCenter.Instance.GetWorkEfficiency(-tempEfficiency);
        _flame.Stop();
        tempEfficiency = 0;
        playerGO.transform.position = initialPosition;
        playerGO.transform.rotation = initialRotation;
    }

    public void OnHeightUp()
    {
        Vector3 pos = HeightGO.transform.position;
        height += 0.1f;
        pos.y = height;
        HeightGO.transform.position = pos;
        UIManager.Instance.UpdateHeight(height);
    }
    public void OnHeightDown()
    {
        Vector3 pos = HeightGO.transform.position;
        height -= 0.1f;
        pos.y = height;
        HeightGO.transform.position = pos;
        UIManager.Instance.UpdateHeight(height);
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
