using UnityEngine;
using System.Collections;

public class PlayerJump : MonoBehaviour
{
    public float jumpPower = 10;
    public PlayerGroundDetecter groundDetecter;
    PlayerMovePosition _movePosition;
    bool _isFloating { get { return !groundDetecter.isGrounded; } }
    bool _isAttacking;

    PlayerAttackBehaviour _attack;
    PlayerHealthBehaviour _health;

    void Awake()
    {
        _movePosition = GetComponent<PlayerMovePosition>();
        _attack = GetComponent<PlayerAttackBehaviour>();
        _health = GetComponent<PlayerHealthBehaviour>();
    }

    void Update()
    {
        ReadInput();
    }

    public bool IsJumping { get { return _isFloating; } }

    void ReadInput()
    {
        if (com.GameTime.timeScale == 0)
            return;
        if (_attack.isAttacking)
            return;
        if (_health.isDead)
            return;
        if (Input.GetKeyDown(KeyCode.Space))
            TryJump();
    }

    void TryJump()
    {
        if (canNotJump)
            return;

        DoJump();
    }

    bool canNotJump { get { return _isFloating || _isAttacking || PlayerBehaviour.Instance.health.isDead || !PlayerBehaviour.Instance.move.canInput||PlayerClimb.Instance.isClimb|| PlayerClimb.Instance.canClimb; } }

    void DoJump()
    {
        //_speedY = jumpPower;
        PlayerBehaviour.Instance.SetBool("walk", false);
        PlayerBehaviour.Instance.SetBool("onGround", false);
        PlayerBehaviour.Instance.animator[0].SetTrigger("jump");
        _movePosition.rb.AddForce(new Vector2(0, jumpPower));
    }

    public void OnGrounded()
    {
        Debug.Log("OnGrounded");
        //_speedY = 0;
        var v = _movePosition.rb.velocity;
        if (v.y > 0)
            return;
        v.y = 0;
        _movePosition.rb.velocity = v;
        _movePosition.StopInputMovement();

        //PlayerBehaviour.Instance.animator.SetBool("walk", false);
    }
}