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
    private float _speedY = 0;
    private float _speedZ;
    private PlayerJump _jump;
    private PlayerAttackBehaviour _attack;
    private PlayerHealthBehaviour _health;

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
        PlayerBehaviour pb = PlayerBehaviour.Instance;
        if (_attack.isAttacking)
        {
            if (!_jump.IsJumping)
            {
                _speedX = 0;
            }
            pb?.SetBool("walk", false);
            return;
        }
        _speedX = 0;
        if (!pb.isSewage)
        {

            if (Input.GetKey(KeyCode.A))
                _speedX = _speedX - 1;
            if (Input.GetKey(KeyCode.D))
                _speedX = _speedX + 1;


        }
        else
        {
            //3D移动操作
            if (!PlayerClimb.Instance.isClimb)
            {
                _speedZ = 0;
                if (Input.GetKey(KeyCode.A))//让A D来控制角色旋转的角度好了 _SpeedZ暂时不用
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
            else
            {
                _speedY = 0;
                if (Input.GetKey(KeyCode.W))
                    _speedY = _speedY - 1;
                if (Input.GetKey(KeyCode.S))
                    _speedY = _speedY + 1;
            }
        }

        if (_speedX > 0)
        {
            isMoving = true;
            if (!_jump.IsJumping)
                pb.SetBool("walk", true);

            if (!pb.isSewage)
                FlipRight();
        }
        else if (_speedX < 0)
        {
            isMoving = true;
            if (!pb.isSewage)
                FlipLeft();
            if (!_jump.IsJumping)
                pb.SetBool("walk", true);
        }
        else
        {
            isMoving = false;
            if (!_jump.IsJumping)
                pb.SetBool("walk", false);
        }

        if (_speedY != 0&& PlayerClimb.Instance.isClimb)
        {
            isMoving = true;
            pb.SetBool("walk", true);
        }
        else if(_speedY == 0 && PlayerClimb.Instance.isClimb)
        {
            isMoving = false;
            pb.SetBool("walk", false);
        }



    }

    /// <summary>
    /// Use the move parameters to move the player
    /// </summary>
    void Move()
    {

        if (!PlayerBehaviour.Instance.isSewage)
        {
            if (_speedX != 0)
                _movePosition.AddInputMovement(Vector3.right * _speedX * speed);
            else
                _movePosition.StopInputMovement();
        }
        if (PlayerBehaviour.Instance.isSewage)
        {
            if (!PlayerClimb.Instance.isClimb)
            {
                if (_speedZ != 0 || _speedX != 0)
                    _movePosition.AddInputMovement(new Vector3(_speedX, 0, _speedZ) * speed, false);
                else
                    _movePosition.StopInputMovement(false,false);
            }
            else
            {
                if (_speedY != 0)
                    _movePosition.AddInputMovement(new Vector3(0, _speedY, 0) * speed, false,true);
                else
                    _movePosition.StopInputMovement(false,true);
            }
        }
    }
    public void ResetSpeed()
    {
        _speedX = 0;
    }
    public void FlipRight()//需要在切换到下水道的时候，调用一下，还原scale
    {
        PlayerBehaviour.Instance.flip.localScale = new Vector3(1, 1, 1);
    }

    void FlipLeft()
    {
        PlayerBehaviour.Instance.flip.localScale = new Vector3(-1, 1, 1);
    }
}