using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;
using static UnityEngine.Rendering.DebugUI;

public class PlayerBehaviour : MonoSingleton<PlayerBehaviour>
{
    [HideInInspector]
    public PlayerHealthBehaviour health;
    [HideInInspector]
    public PlayerMove move;
    [HideInInspector]
    public PlayerAttackBehaviour attack;
    [HideInInspector]
    public PlayerMovePosition movePosition;

    [Header("第一个放角色的控制器")]
    public List<Animator> animator;

    public Transform flip;
    public GameObject StartPos;

    public bool isSewage;

    public GameObject spider;
    public GameObject fish;
    public GameObject[] fishHideObject;

    public GameObject rig;
    private bool _isFish;
    private bool _isSpider;
    private bool _isSwim;


    public bool IsSwim
    {
        get { return _isSwim; }
        set
        {
            animator[0].SetBool("swim", value);
            _isSwim = value;
        }
    }
    //NpcController _npcController;

    override public void Init()
    {
        attack = GetComponent<PlayerAttackBehaviour>();
        move = GetComponent<PlayerMove>();
        health = GetComponent<PlayerHealthBehaviour>();
        movePosition = GetComponent<PlayerMovePosition>();
        EventManager.AddListener(EventCommon.START_GAME, InitPlayer);
        //_npcController = GetComponent<NpcController>();
        InitPlayer();

    }
    private void InitPlayer()
    {
        //重置数据中心的数据
        //DataCenter.Instance.NewGameData();
        //重置判断标签
        health.InitHealth();
        move.canInput = true;
        isSewage = false;
        ResetToTarget(this.gameObject, StartPos);
        animator[0].Play("Idle_A");
        animator[0].Play("Eyes_Normal",1);
    }

    public void SetBool(string name, bool value)
    {
        foreach (var anim in animator)
        {
            anim.SetBool(name, value);
        }

    }
    void ResetToTarget(GameObject go, GameObject target)
    {
        // 暂时将当前物体的父物体设置为 targetEntrance
        Transform originalParent = transform.parent;
        go.transform.SetParent(target.transform);

        // 将局部旋转设置为零
        go.transform.localRotation = Quaternion.Euler(Vector3.zero);

        go.transform.position = target.transform.position;

        // 恢复父物体
        go.transform.SetParent(target.transform.parent.parent.parent);
    }
}