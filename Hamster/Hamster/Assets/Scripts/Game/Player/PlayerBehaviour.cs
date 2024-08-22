using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    public bool isSewage;

    public GameObject spider;
    public GameObject fish;
    public GameObject[] fishHideObject;
    private bool _isFish;
    private bool _isSpider;
    private bool _isSwim;

    public bool IsFish
    {
        get { return _isFish; }
        set
        {
            fish.SetActive(value);
            _isFish = value;
            if(fishHideObject.Length > 0)
            {
                foreach(var obj in fishHideObject)
                {
                    obj.SetActive(value);
                }
            }
        }
    }

    public bool IsSpider
    {
        get { return _isSpider; }
        set
        {
            spider.SetActive(value);
            _isSpider = value;
        }
    }

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
        //_npcController = GetComponent<NpcController>();

        //_npcController.Reinit(animator, flip);
        health.FullFill();

        IsFish = true;
        IsSpider = true;
    }

    public void SetBool(string name, bool value)
    {
        foreach (var anim in animator)
        {
            anim.SetBool(name, value);
        }

    }
}