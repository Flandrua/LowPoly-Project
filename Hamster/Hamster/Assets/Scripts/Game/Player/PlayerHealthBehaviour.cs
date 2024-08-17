
using com;
using DG.Tweening;
using UnityEngine;

public class PlayerHealthBehaviour : MonoBehaviour
{
    //public HpBarFixedWidthBehaviour hpbar;
    public int hpMax;

    private int _hpMax;
    private int _hp;
    bool _dead;

    public string dieSound;
    public float deathFadeDelay;


    public void FullFill()
    {
        _hpMax = hpMax;
        // hpbar.Set(1, true);
        _hp = _hpMax;
        int[] hplist = { _hp, _hpMax };
        EventManager.DispatchEvent<int[]>(EventCommon.UPDATE_HP, hplist);
    }

    private void Update()
    {
        DoRoutineMove();
    }

    public void TakeDamage(int dmg)
    {
        if (_dead) return;

        //Debug.Log(this.name + "TakeDamage " + dmg);
        //PlayerBehaviour.instance.animator.SetTrigger("damage");
        PlayerBehaviour.instance.animator[0].Play("Hit");
        _hp -= dmg;
        int[] hplist = { _hp, _hpMax };
        EventManager.DispatchEvent<int[]>(EventCommon.UPDATE_HP, hplist);
        if (_hp < 0)
            _hp = 0;

        float ratio = (float)_hp / _hpMax;
        //  hpbar.Set(ratio, false);
        if (_hp <= 0)
            Die();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Kill")
        {
            this.Die(true);
        }
    }

    public void Die(bool fromFall = false)
    {
        if (_dead) return;

        _dead = true;
        //ReviveSystem.instance.QueueDie(fromFall);
        //SoundSystem.instance.Play(dieSound);
        PlayerBehaviour.instance.animator[0].Play("Death");
        //if (!fromFall)
        //    PlayerBehaviour.instance.animator.SetTrigger("die");
        
        SpriteRenderer[] srs = GetComponentsInChildren<SpriteRenderer>();
        PlayerBehaviour.instance.movePosition.StopInputMovement();
        foreach (var sr in srs)
        {
            sr.DOFade(0, 3).SetDelay(deathFadeDelay + Random.Range(1, 3f));
        }
    }

    void DoRoutineMove()
    {
        if (_dead) return;


    }

    public bool isDead
    {
        get { return _dead; }
    }
}