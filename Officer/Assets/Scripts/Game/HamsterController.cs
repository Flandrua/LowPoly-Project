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
    [SerializeField] private bool isPlay = false;
    [SerializeField] private bool isEating = false;

    public float stayTime = 3;//����ͣ��ʱ��



    void Start()
    {
        EventManager.AddListener(EventCommon.DAMAGE, DamageFlag);
        EventManager.AddListener(EventCommon.HAMSTER_TRIGGER, TriggerFlag);
        EventManager.AddListener(EventCommon.HAMSTER_FINISH_EATING, HamsterFinishEating);
        _animator = GetComponent<Animator>();
        _col = GetComponent<Collider>();


    }
    private void OnDestroy()
    {
        EventManager.RemoveListener(EventCommon.DAMAGE, DamageFlag);
        EventManager.RemoveListener(EventCommon.HAMSTER_TRIGGER, TriggerFlag);
        EventManager.RemoveListener(EventCommon.HAMSTER_FINISH_EATING, HamsterFinishEating);
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlay)
        {
            stayTime -= (float)Time.deltaTime;
            if (stayTime <= 0)
            {
                GetFavorability(1);//�˴���Ӧ����1��������Ҫ����ҵ�����
                isPlay = false;
                Debug.Log("get favor");
                //֪ͨGM�л�ʱ��
                EventManager.DispatchEvent(EventCommon.CHANGE_TIME);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isEating)
        {
            onTrigger = true;
            InstantaneousSpeedCalculator calculator = other.GetComponent<InstantaneousSpeedCalculator>();
            if (calculator != null)
            {
                // ��ȡ�ٶȲ����
                Vector3 velocity = calculator.InstantaneousSpeed;
                float mag = velocity.magnitude;
                if (mag > 3)//�����Ϊ
                {
                    GetDamage(-2);
                    Debug.Log("hit");
                }
                else //������Ϊ
                    isPlay = true;

                Debug.Log("Player velocity: " + mag);
            }
        }
        else if (other.CompareTag("Snack") && !isPlay)
        {
            onTrigger = true;
            isEating = true;
            EventManager.DispatchEvent(EventCommon.HAMSTER_EATING, true);//��SnackManager���Ϳ�ʼ�Ե�֪ͨ

        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            onTrigger = false;
            isPlay = false;
            Debug.Log("player exit ");
        }
        else if (other.CompareTag("Snack"))
        {
            onTrigger = false;
            isEating = false;
            EventManager.DispatchEvent(EventCommon.HAMSTER_EATING, false);//��SnackManager�����жϳԵ�֪ͨ
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
        if (!onTrigger && !isDead && !isDamage && !isPlay)
        {
            _animator.Play(animationName);
            //Debug.Log(animationName);
        }

    }
    /// <summary>
    /// ���������۾�����
    /// </summary>
    /// <param name="animationName"></param>
    public void ChangeEyesAnimationByStr(string animationName)
    {
        if (!onTrigger && !isDead && !isDamage && !isPlay)
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
        Debug.Log("favorability:" + DataCenter.Instance.GameData.HamsterData.favorability);
    }
    /// <summary>
    /// ������HP,��ʽ��+=value�����Կ�Ѫ��value��Ҫ�Ǹ���
    /// </summary>
    /// <param name="value"></param>
    public void GetDamage(int value)
    {
        //�������Ѫ������Ƿ��������Ƿ񲥷ű��򶯻�
        DataCenter.Instance.GameData.HamsterData.hp += value;
        Debug.Log("hp:"+DataCenter.Instance.GameData.HamsterData.hp);
        if (DataCenter.Instance.GameData.HamsterData.hp <= 0)
            Death();
        else
        {
            isDamage = true;
            _animator.SetTrigger("damage");

        }
    }
    public void Death()
    {
        //������������������
        DataCenter.Instance.GameData.HamsterData.hp = 0;
        _animator.Play("Eyes_Dead", _animator.GetLayerIndex("Shapekey"));
        _animator.Play("Death");
        isDead = true;
    }
    public void DamageFlag()
    {
        isDamage = false;
        //Debug.Log("damage false");
    }
    public void TriggerFlag()
    {
        onTrigger = false;
        //Debug.Log("trigger false");
    }
    public void HamsterFinishEating()
    {
        onTrigger = false;
        GetFavorability(1);
    }
}
