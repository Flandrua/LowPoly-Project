using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class PlayerBehaviour : MonoSingleton<PlayerBehaviour>
{
    public static PlayerBehaviour instance;

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

    //NpcController _npcController;

    private void Awake()
    {
        instance = this;
        attack = GetComponent<PlayerAttackBehaviour>();
        move = GetComponent<PlayerMove>();
        health = GetComponent<PlayerHealthBehaviour>();
        movePosition = GetComponent<PlayerMovePosition>();
        //_npcController = GetComponent<NpcController>();

        //_npcController.Reinit(animator, flip);
        health.FullFill();
    }

    public void SetBool(string name, bool value)
    {
        foreach (var anim in animator)
        {
                anim.SetBool(name, value);
        }
    }



}