using System.Collections;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    public static PlayerBehaviour instance;

    [HideInInspector]
    public PlayerHealthBehaviour health;
    [HideInInspector]
    public PlayerMove move;
    [HideInInspector]
    public PlayerAttackBehaviour attack;

    public Animator animator;

    public Transform flip;

    //NpcController _npcController;

    private void Awake()
    {
        instance = this;
        attack = GetComponent<PlayerAttackBehaviour>();
        move = GetComponent<PlayerMove>();
        health = GetComponent<PlayerHealthBehaviour>();
        //_npcController = GetComponent<NpcController>();

        //_npcController.Reinit(animator, flip);
        health.FullFill();
    }
}