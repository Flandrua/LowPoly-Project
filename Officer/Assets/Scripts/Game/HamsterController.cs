using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum HamsterBehavior
{
    Attack,
    Bounce,
    Clicked,
    Death,
    Eat,
    Fear,
    Fly,
    Hit,
    Idle_A,
    Idle_B,
    Idle_C,
    Jump,
    Roll,
    Run,
    Sit,
    Spin,
    Swim,
    Walk,
};
public enum HamsterEyes
{
    Eyes_Normal,
    Eyes_Annoyed,
    Eyes_Blink,
    Eyes_Cry,
    Eyes_Dead,
    Eyes_Excited,
    Eyes_Happy,
    Eyes_LookDown,
    Eyes_LookIn,
    Eyes_LookOut,
    Eyes_LookUp,
    Eyes_Rabid,
    Eyes_Sad,
    Eyes_Shrink,
    Eyes_Sleep,
    Eyes_Spin,
    Eyes_Squint,
    Eyes_Trauma,
    Sweat_L,
    Sweat_R,
    Teardrop_L,
    Teardrop_R
};
public class HamsterController : MonoBehaviour
{

    //�л�����
    //��ײ����
    //��ֵ����


    private Animator _animator;
    private Collider _col;
   [SerializeField] private bool onTrigger = false;
    [SerializeField] private bool isDead = false;
    [SerializeField] private bool isDamage = false;

    public float stayTime = 3;//����ͣ��ʱ��



    void Start()
    {
        EventManager.AddListener(EventCommon.DAMAGE, DamageFlag);
        _animator = GetComponent<Animator>();
        _col = GetComponent<Collider>();


    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            onTrigger = true;
            InstantaneousSpeedCalculator calculator = other.GetComponent<InstantaneousSpeedCalculator>();
            if (calculator != null)
            {
                // ��ȡ�ٶȲ����
                Vector3 velocity = calculator.InstantaneousSpeed;
                Debug.Log("Player velocity: " + velocity);
                DebugHelper.Instance.DebugMsg("Player velocity: " + velocity);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            onTrigger = false;
            Debug.Log("player exit ");
        }
    }

    //private void OnCollisionEnter(Collision other)
    //{
    //    if (other.gameObject.CompareTag("Player"))
    //    {
    //        onTrigger = true;
    //        Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
    //        if (rb != null)
    //        {
    //            // ��ȡ�ٶȲ����
    //            Vector3 velocity = rb.velocity;
    //            Debug.Log("Player velocity: " + velocity);
    //            DebugHelper.Instance.DebugMsg("Player velocity: " + velocity);
    //        }
    //    }
    //}
    //private void OnCollisionExit(Collision other)
    //{
    //    if (other.gameObject.CompareTag("Player"))
    //    {
    //        onTrigger = false;
    //        Debug.Log("player exit ");
    //    }
    //}
    /// <summary>
    /// ��������������
    /// </summary>
    /// <param name="animationName"></param>

    public void ChangeBehaviorAnimationByStr(string animationName)
    {
        if (!onTrigger&&!isDead&&!isDamage)
        {
            _animator.Play(animationName);
            Debug.Log(animationName);
        }

    }
    /// <summary>
    /// ���������۾�����
    /// </summary>
    /// <param name="animationName"></param>
    public void ChangeEyesAnimationByStr(string animationName)
    {
        if (!onTrigger && !isDead && !isDamage)
        {
            _animator.Play(animationName, _animator.GetLayerIndex("Shapekey"));
        }
    }

    //public void ChangeBehaviorAnimation(HamsterBehavior animationName)
    //{
    //    if (!onTrigger && !isDead)
    //    {
    //        string animation = animationName.ToString();
    //        _animator.Play(animation);
    //    }
    //}
    //public void ChangeEyesAnimation(HamsterEyes animationName)
    //{
    //    if (!onTrigger && !isDead)
    //    {
    //        string animation = animationName.ToString();
    //        _animator.Play(animation, _animator.GetLayerIndex("Shapekey"));
    //    }
    //}

    /// <summary>
    /// �Ӻøж�
    /// </summary>
    /// <param name="value"></param>
    public void GetFavorability(int value)
    {
        DataCenter.Instance.GameData.HamsterData.favorability += value;
    }
    /// <summary>
    /// ������HP,����˺�����Ļ�����Ҫ��ȭ
    /// </summary>
    /// <param name="value">��ʽ��+=value�����Կ�Ѫ��value��Ҫ�Ǹ���</param>
    public void GetDamage(int value)
    {
        DataCenter.Instance.GameData.HamsterData.hp += value;
        if (DataCenter.Instance.GameData.HamsterData.hp <= 0)
            Death();
        else
        {
            isDamage = true;
            _animator.SetTrigger("damage");
            
        }
    }
    public void DamageFlag()
    {
        isDamage = false;
        Debug.Log("damage exit");
    }
   public void Death()
    {
        DataCenter.Instance.GameData.HamsterData.hp = 0;
        _animator.Play("Eyes_Dead", _animator.GetLayerIndex("Shapekey"));
        _animator.Play("Death");
        isDead = true;
    }

}
