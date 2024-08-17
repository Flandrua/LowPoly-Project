using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPSpot : MonoBehaviour
{
    //如果玩家在TP box的里面按下了E，则进入TP，传送到初始地点
    // Start is called before the first frame update
    public GameObject tpPos;
    public bool isBase = false;
    private Animator _animator;
    private bool playerExist = false;
    private PlayerBehaviour _playerBehaviour;
    void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        EventManager.AddListener(EventCommon.TELEPORT, Teleport);
        _playerBehaviour = PlayerBehaviour.instance;
    }
    private void OnDestroy()
    {
        EventManager.RemoveListener(EventCommon.TELEPORT, Teleport);
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player") && !isBase)
        {
            playerExist = true;
            if (Input.GetKey(KeyCode.E)&& _playerBehaviour.move.canInput)
            {
                _animator.SetBool("Open", true);
                TimeManager.Instance.AddTask(1.5f, false, () => { _animator.SetBool("Open", false); }, this);
                _playerBehaviour.move.FlipRight();
                _playerBehaviour.animator[0].SetTrigger("Move");
                _playerBehaviour.move.canInput = false;
                _playerBehaviour.move.ResetSpeed();
                _playerBehaviour.transform.position = tpPos.transform.position;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        playerExist = false;
    }

    private void Teleport()
    {
        if (isBase)
        {
            _playerBehaviour.gameObject.transform.position = tpPos.transform.position;
            _animator.SetBool("Open", true);
            TimeManager.Instance.AddTask(2f, false, () => { _animator.SetBool("Open", false); }, this);
        }
    }
}
