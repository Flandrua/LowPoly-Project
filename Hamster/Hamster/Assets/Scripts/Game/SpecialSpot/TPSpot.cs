using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPSpot : MonoBehaviour
{
    //如果玩家在TP box的里面按下了E，则进入TP，传送到初始地点
    // Start is called before the first frame update
    public GameObject tpPos;
    public TPSpot targetSpot;
    public bool isTarget = false;
    public bool isBase = false;
    public AudioClip open;
    public AudioClip close;
    private AudioSource _as;
    private Animator _animator;
    private bool playerExist = false;
    private PlayerBehaviour _playerBehaviour;
    void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        _as = GetComponent<AudioSource>();
        EventManager.AddListener(EventCommon.TELEPORT, Teleport);
        if (isBase)
        {
            EventManager.AddListener(EventCommon.START_GAME, StartTp);
        }
        _playerBehaviour = PlayerBehaviour.Instance;
        _animator.SetBool("Open", true);
        TimeManager.Instance.AddTask(2f, false, () => { _animator.SetBool("Open", false); }, this);
    }
    private void OnDestroy()
    {
        EventManager.RemoveListener(EventCommon.TELEPORT, Teleport);
        if (isBase)
        {
            EventManager.RemoveListener(EventCommon.START_GAME, StartTp);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            playerExist = true;
            if (Input.GetKey(KeyCode.E) && _playerBehaviour.move.canInput)
            {
                targetSpot.isTarget = true;
                if (targetSpot.isBase) _playerBehaviour.animator[0].SetBool("base", true);
                _animator.SetBool("Open", true);
                _as.clip = open;
                _as.Play();
                TimeManager.Instance.AddTask(1f, false, () =>
                {
                    _as.clip = close;
                    _as.Play();

                }, this);
                TimeManager.Instance.AddTask(1.5f, false, () =>
                {
                    _animator.SetBool("Open", false);

                }, this);      
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
        if (isTarget)
        {
            isTarget = false;
            StartTp();
        }
    }

    private void StartTp()
    {

        _playerBehaviour.gameObject.transform.position = tpPos.transform.position;
        _animator.SetBool("Open", true);
        _as.clip = open;
        _as.Play();

        TimeManager.Instance.AddTask(2f, false, () =>
        {
            _animator.SetBool("Open", false);
            _as.clip = close;
            _as.Play();

        }, this);
    }
}
