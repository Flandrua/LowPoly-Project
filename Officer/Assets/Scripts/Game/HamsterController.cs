using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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
public class HamsterController : MonoSingleton<HamsterController>
{

    //�л�����
    //��ײ����
    //��ֵ����


    private Animator _animator;
    private Collider _col;
    private GameObject _favEffect;
    private Scrollbar _bar;//����ͣ��ʱ����
    private ParticleSystem _heart;
    [SerializeField] private bool onTrigger = false;
    [SerializeField] private bool isDead = false;
    [SerializeField] private bool isOut = false;
    [SerializeField] private bool isDamage = false;
    [SerializeField] private bool isPlay = false;
    [SerializeField] public bool isEating = false;

    public float stayRequireTime = 3;//��������ͣ��ʱ��
    private float stayTime = 0;//ͣ��ʱ��



    void Start()
    {
        EventManager.AddListener(EventCommon.DAMAGE, DamageFlag);
        EventManager.AddListener(EventCommon.HAMSTER_TRIGGER, TriggerFlag);
        EventManager.AddListener(EventCommon.HAMSTER_FINISH_EATING, HamsterFinishEating);
        EventManager.AddListener(EventCommon.NEXT_STAGE, ResetToDefault);
        _animator = transform.parent.GetComponent<Animator>();
        _col = GetComponent<Collider>();
        _favEffect = transform.parent.Find("Favorability").gameObject;
        _heart = transform.parent.Find("Favorability").Find("heart").GetComponent<ParticleSystem>();
        _bar = transform.parent.Find("Favorability").Find("Canvas").Find("Scrollbar").GetComponent<Scrollbar>();


    }
    private void OnDestroy()    {
        EventManager.RemoveListener(EventCommon.DAMAGE, DamageFlag);
        EventManager.RemoveListener(EventCommon.HAMSTER_TRIGGER, TriggerFlag);
        EventManager.RemoveListener(EventCommon.HAMSTER_FINISH_EATING, HamsterFinishEating);
        EventManager.RemoveListener(EventCommon.NEXT_STAGE, ResetToDefault);
    }
    private void ResetToDefault()
    {
        stayTime = 0;
        _bar.size = 0;
        isPlay = false;
        onTrigger = false;
        _animator.Play("Sit");
        _animator.Play("Eyes_Normal", _animator.GetLayerIndex("Shapekey"));
    }
    public void ResetMoveAnimation()
    {
        isOut=false; ;
        _animator.Play("Sit");
        _animator.Play("Eyes_Normal", _animator.GetLayerIndex("Shapekey"));
    }
    // Update is called once per frame
    void Update()
    {
        if (isPlay)
        {
            stayTime += (float)Time.deltaTime;
            _bar.size = (stayTime / stayRequireTime);
            if (stayTime >= stayRequireTime)
            {
                isPlay = false;
                _animator.Play("Sit");
                _animator.Play("Eyes_Normal", _animator.GetLayerIndex("Shapekey"));
                _heart.Play();
                _bar.size = 1;
                Debug.Log("get favor");
                //֪ͨGM�������
                EventManager.DispatchEvent(EventCommon.PREPARE_CHANGE_TIME,"play");
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if(isDead|| isOut) return;
        if (other.CompareTag("Player") && !isEating)
        {
            onTrigger = true;
            InstantaneousSpeedCalculator calculator = other.GetComponent<InstantaneousSpeedCalculator>();
            if (calculator != null)
            {
                // ��ȡ�ٶȲ����
                Vector3 velocity = calculator.InstantaneousSpeed;
                float mag = velocity.magnitude;
                if (mag > 2.5)//�����Ϊ
                {
                    GetDamage(-2);
                    GetFavorability(-1);
                    if (DataCenter.Instance.GameData.HamsterData.hp < 5)
                    {
                        _animator.Play("Eyes_Cry", _animator.GetLayerIndex("Shapekey"));
                    }
                }
                else if (stayTime < stayRequireTime&&MouseManager.Instance.canSwitchTime==false)//������Ϊ,�����жϽ����ж��Ƿ����
                {
                    isPlay = true;
                    _animator.Play("Idle_A");
                    _animator.Play("Eyes_Happy", _animator.GetLayerIndex("Shapekey"));
                    TimeManager.Instance.RemoveTask(BarHide, this);//�Ƴ������ʱ��
                    _favEffect.SetActive(true);
                    TimeManager.Instance.AddTask(5,false, BarHide, this);//5�������Bar
                }

                //Debug.Log("Player velocity: " + mag);
            }
        }
        else if (other.CompareTag("Snack") && !isPlay)
        {
            onTrigger = true;
            isEating = true;
            _animator.Play("Eyes_Excited", _animator.GetLayerIndex("Shapekey"));
            EventManager.DispatchEvent(EventCommon.HAMSTER_EATING, true);//��SnackManager���Ϳ�ʼ�Ե�֪ͨ

        }
    }
    private void BarHide()
    {
        _favEffect.SetActive(false);
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            onTrigger = false;
            isPlay = false;
            if (stayTime < stayRequireTime)//���ͣ��ʱ��û�е�3�룬����ʱ��
            {
                stayTime= 0;
                //_favEffect.SetActive(false);
            }
            Debug.Log("player exit ");
        }
        else if (other.CompareTag("Snack"))
        {
            onTrigger = false;
            isEating = false;
            EventManager.DispatchEvent(EventCommon.HAMSTER_EATING, false);//��SnackManager�����жϳԵ�֪ͨ
        }
    }


    /// <summary>
    /// ���ⲿhover�õĸ�������������
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
    /// ���ⲿhover�õĸ��������۾�����
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
        DataCenter.Instance.GetFavorability(value);
    }
    /// <summary>
    /// ������HP,��ʽ��+=value�����Կ�Ѫ��value��Ҫ�Ǹ���
    /// </summary>
    /// <param name="value"></param>
    public void GetDamage(int value)
    {
        //�������Ѫ������Ƿ��������Ƿ񲥷ű��򶯻�
        DataCenter.Instance.GetDamage(value);
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
        isOut = true;
        onTrigger = false;
        GetFavorability(1);
        //¼��һ���߿��Ķ���
        _animator.Play("Walk");
        _animator.SetBool("Move",true);
        TimeManager.Instance.AddTask(3, false, () => { _animator.Play("Jump"); }, this);
        TimeManager.Instance.AddTask(4.1f, false, () => { _animator.Play("Walk"); }, this);
    }
}
