using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float speed;
    public bool canInput;
    public float rotationSpeed = 10;
    private PlayerMovePosition _movePosition;
    private float _speedX;
    private float _speedZ;
    private PlayerJump _jump;
    private PlayerAttackBehaviour _attack;
    private PlayerHealthBehaviour _health;

    public bool sewageInput = false;
    public bool isMoving { get; private set; }

    void Start()
    {
        _movePosition = GetComponent<PlayerMovePosition>();
        _jump = GetComponent<PlayerJump>();
        _attack = GetComponent<PlayerAttackBehaviour>();
        _health = GetComponent<PlayerHealthBehaviour>();
        EventManager.AddListener(EventCommon.CAN_INPUT, CanInput);
    }
    private void OnDestroy()
    {
        EventManager.RemoveListener(EventCommon.CAN_INPUT, CanInput);
    }
    public void CanInput()
    {
        canInput = true;
    }

    void Update()
    {
        ReadInput();
    }

    /// <summary>
    /// FixedUpdate is called once per fixed interval
    /// This interval can be set by user
    /// the default value is 0.02
    /// the FixedUpdate is the Unity physics system's interval of calculating collisions
    /// </summary>
    private void FixedUpdate()
    {
        Move();
    }

    /// <summary>
    /// read the input from keyboard to set the move parameters
    /// </summary>
    void ReadInput()
    {
        if (_health.isDead || !canInput)
            return;

        if (_attack.isAttacking)
        {
            if (!_jump.IsJumping)
            {
                _speedX = 0;
            }
            PlayerBehaviour.instance.animator?.SetBool("walk", false);
            return;
        }
        _speedX = 0;
        if (!sewageInput)
        {

            if (Input.GetKey(KeyCode.A))
                _speedX = _speedX - 1;
            if (Input.GetKey(KeyCode.D))
                _speedX = _speedX + 1;


        }
        else
        {
            _speedZ = 0;
            //3D�ƶ�����
            if (Input.GetKey(KeyCode.A))//��A D�����ƽ�ɫ��ת�ĽǶȺ��� _SpeedZ��ʱ����
            {
                transform.Rotate(0, -Time.deltaTime * rotationSpeed, 0);
                //_speedZ = _speedZ - 1;
            }
            if (Input.GetKey(KeyCode.D))
            {
                transform.Rotate(0, Time.deltaTime * rotationSpeed, 0);
                //_speedZ = _speedZ + 1;
            }
            if (Input.GetKey(KeyCode.W))
                _speedX = _speedX - 1;
            if (Input.GetKey(KeyCode.S))
                _speedX = _speedX + 1;
        }

        if (_speedX > 0)
        {
            isMoving = true;
            if (!_jump.IsJumping)
                PlayerBehaviour.instance.animator?.SetBool("walk", true);

            if (!sewageInput)
                FlipRight();
        }
        else if (_speedX < 0)
        {
            isMoving = true;
            if (!sewageInput)
                FlipLeft();
            if (!_jump.IsJumping)
                PlayerBehaviour.instance.animator?.SetBool("walk", true);
        }
        else
        {
            isMoving = false;
            if (!_jump.IsJumping)
                PlayerBehaviour.instance.animator?.SetBool("walk", false);
        }


    }

    /// <summary>
    /// Use the move parameters to move the player
    /// </summary>
    void Move()
    {

        if (!sewageInput)
        {
            if (_speedX != 0)
                _movePosition.AddInputMovement(Vector3.right * _speedX * speed);
            else
                _movePosition.StopInputMovement();
        }
        if (sewageInput)
        {
            if (_speedZ != 0 || _speedX != 0)
                _movePosition.AddInputMovement(new Vector3(_speedX, 0, _speedZ) * speed, false);
            else
                _movePosition.StopInputMovement(false);
        }
    }
    public void ResetSpeed()
    {
        _speedX = 0;
    }
    public void FlipRight()//��Ҫ���л�����ˮ����ʱ�򣬵���һ�£���ԭscale
    {
        PlayerBehaviour.instance.flip.localScale = new Vector3(1, 1, 1);
    }

    void FlipLeft()
    {
        PlayerBehaviour.instance.flip.localScale = new Vector3(-1, 1, 1);
    }
}