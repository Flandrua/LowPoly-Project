using System.Collections;

using System.Collections.Generic;

using Unity.VisualScripting;

using UnityEngine;

using UnityEngine.Events;
using UnityEngine.EventSystems;

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

public class HamsterController : MonoSingleton<HamsterController>, IPointerClickHandler

{



    // Cached references.

    // Runtime state.

    // Timers and audio.





    private Animator _animator;

    private Collider _col;

    private GameObject _favEffect;

    private Scrollbar _bar; // Favorability progress bar.

    private ParticleSystem _heart;

    private ParticleSystem _flame;

    [SerializeField] private bool onTrigger = false;

    [SerializeField] public bool isDead = false;

    [SerializeField] public bool isOut = false;

    [SerializeField] private bool isDamage = false;

    [SerializeField] private bool isPlay = false;

    [SerializeField] public bool isEating = false;



    public float stayRequireTime = 3; // Required petting time before the interaction completes.

    private float stayTime = 0; // Accumulated petting time.
    private bool _tutorialIgnoreHit;
    private bool _tutorialPettingEnabled = true;
    private bool _tutorialGuideActive;

    private AudioSource _as;

    private GameManager _gameManager;

    private bool _eventsRegistered = false;

    public AudioClip hit;

    public AudioClip eat;
    [Header("Guide Intro")]
    [SerializeField] private bool enableGuideIntro = true;
    [SerializeField] private bool guideTriggered;
    [SerializeField] private string guideIntroTtsPath = "TTS/Introduce/Hamster";
    [SerializeField] private string guideAnimatorTrigger = "Shining";
    [SerializeField] private Animator guideAnimator;
    [Tooltip("Manual assignment only. Drag scene Outline components here.")]
    public Outline[] guideOutlines;
    [SerializeField] private UnityEvent onGuideTriggered;
    [Header("Trigger Enter Callback")]
    [Tooltip("Invoked once on the first valid OnTriggerEnter interaction with Player.")]
    [SerializeField] private UnityEvent onTriggerEnterOnce;
    [SerializeField] private bool hasTriggeredOnTriggerEnterOnce;
    private bool _missingGuideOutlineLogged;
    public Animator GuideAnimator => guideAnimator;
    public event System.Action PetCompleted;

    private void Awake()

    {

        CacheComponents();

    }



    void Start()

    {

        CacheComponents();
        ResolveGuideReferences();
        SetGuideOutlineVisible(_tutorialGuideActive || (enableGuideIntro && !guideTriggered));

    }

    private void OnEnable()

    {

        CacheComponents();
        ResolveGuideReferences();
        SetGuideOutlineVisible(_tutorialGuideActive || (enableGuideIntro && !guideTriggered));

        RegisterEvents();

    }



    private void OnDisable()
    {
        UnregisterEvents();
        stayTime = 0;
        isPlay = false;
        onTrigger = false;
        isEating = false;
        isDamage = false;

        if (_favEffect != null)
        {
            _favEffect.SetActive(false);
        }

        if (_bar != null)
        {
            _bar.size = 0;
        }

        if (_heart != null)
        {
            _heart.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        if (_flame != null)
        {
            _flame.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        if (_as != null && _as.isPlaying)
        {
            _as.Stop();
        }
    }

    private void OnDestroy()

    {

        UnregisterEvents();

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!IsPlayerInteractionAllowed())
        {
            return;
        }

        TryTriggerGuideIntro();
    }



    private void CacheComponents()

    {

        if (_gameManager == null)

        {

            _gameManager = FindObjectOfType<GameManager>(true);

        }



        Transform hamsterTransform = transform.parent != null ? transform.parent : transform;

        if (_animator == null)

        {

            _animator = hamsterTransform.GetComponent<Animator>();

        }



        if (_col == null)

        {

            _col = GetComponent<Collider>();

        }



        if (_favEffect == null)

        {

            Transform favorability = hamsterTransform.Find("Favorability");

            if (favorability != null)

            {

                _favEffect = favorability.gameObject;

                Transform heart = favorability.Find("heart");

                if (heart != null)

                {

                    _heart = heart.GetComponent<ParticleSystem>();

                }



                Transform scrollbar = favorability.Find("Canvas/Scrollbar");

                if (scrollbar != null)

                {

                    _bar = scrollbar.GetComponent<Scrollbar>();

                }

            }

        }



        if (_flame == null)

        {

            Transform flame = hamsterTransform.Find("Flame");

            if (flame != null)

            {

                _flame = flame.GetComponent<ParticleSystem>();

            }

        }



        if (_as == null)

        {

            _as = GetComponent<AudioSource>();

        }

    }



    private bool IsHamsterGameplayEnabled()

    {

        return _gameManager == null || _gameManager.IsHamsterGameplayEnabled;

    }



    private void RegisterEvents()

    {

        if (_eventsRegistered || !IsHamsterGameplayEnabled())

        {

            return;

        }



        EventManager.AddListener(EventCommon.DAMAGE, DamageFlag);

        EventManager.AddListener(EventCommon.HAMSTER_TRIGGER, TriggerFlag);

        EventManager.AddListener<SnackData>(EventCommon.HAMSTER_FINISH_EATING, HamsterFinishEating);

        EventManager.AddListener(EventCommon.NEXT_STAGE, ResetToDefault);

        _eventsRegistered = true;

    }



    private void UnregisterEvents()

    {

        if (!_eventsRegistered)

        {

            return;

        }



        EventManager.RemoveListener(EventCommon.DAMAGE, DamageFlag);

        EventManager.RemoveListener(EventCommon.HAMSTER_TRIGGER, TriggerFlag);

        EventManager.RemoveListener<SnackData>(EventCommon.HAMSTER_FINISH_EATING, HamsterFinishEating);

        EventManager.RemoveListener(EventCommon.NEXT_STAGE, ResetToDefault);

        _eventsRegistered = false;

    }

    private void ResetToDefault()

    {

        if (!IsHamsterGameplayEnabled())

        {

            return;

        }



        stayTime = 0;

        if (_bar != null)

        {

            _bar.size = 0;

        }

        isPlay = false;

        onTrigger = false;

        if (!isDead)

        {

            _animator.Play("Sit");

            _animator.Play("Eyes_Normal", _animator.GetLayerIndex("Shapekey"));

            _animator.SetBool("Sour", false);

        }

        if (!isOut)

        {

            _animator.SetBool("Move", false);

        }

        if (_flame != null)

        {

            _flame.Stop();

        }



    }

    public void ResetMoveAnimation()

    {

        if (!IsHamsterGameplayEnabled())

        {

            return;

        }



        isOut = false;

        if (!isDead)

        {

            _animator.Play("Sit");

            _animator.Play("Eyes_Normal", _animator.GetLayerIndex("Shapekey"));

        }

    }

    // Update is called once per frame

    void Update()

    {

        if (!IsHamsterGameplayEnabled())

        {

            return;

        }



        if (isPlay)

        {

            // Stop accumulating petting progress during the time-switching gap.

            if (GameManager.Instance != null && GameManager.Instance.IsStageAdvanceRequested)

            {

                return;

            }

            if (!_tutorialPettingEnabled)
            {
                isPlay = false;
                return;
            }

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

                PetCompleted?.Invoke();

                // Notify the GameManager that this stage can advance.

                EventManager.DispatchEvent(EventCommon.PREPARE_CHANGE_TIME,"play");

            }

        }

    }





    private void OnTriggerEnter(Collider other)

    {
        if (!IsHamsterGameplayEnabled() || isDead || isOut || !IsPlayerInteractionAllowed()) return;

        if (other.CompareTag("Player") && !isEating)
        {
            if (!_tutorialPettingEnabled)
            {
                return;
            }

            InvokeTriggerEnterOnceIfNeeded();

            onTrigger = true;

            InstantaneousSpeedCalculator calculator = other.GetComponent<InstantaneousSpeedCalculator>();

            if (calculator != null)

            {

                // Read the player's current movement speed.

                Vector3 velocity = calculator.InstantaneousSpeed;

                float mag = velocity.magnitude;

                if (mag > 2.5) // Treat a fast collision as a hit.

                {
                    if (_tutorialIgnoreHit)
                    {
                        return;
                    }

                    GetDamage(-2);

                    GetFavorability(-1);

                    if (DataCenter.Instance.GameData.HamsterData.hp < 5)

                    {

                        _animator.Play("Eyes_Cry", _animator.GetLayerIndex("Shapekey"));

                    }

                }

                else if (stayTime < stayRequireTime) // Enter play mode only before petting is complete.

                {

                    isPlay = true;

                    _animator.Play("Idle_A");

                    _animator.Play("Eyes_Happy", _animator.GetLayerIndex("Shapekey"));

                    TimeManager.Instance.RemoveTask(BarHide, this); // Cancel any pending bar hide task.

                    _favEffect.SetActive(true);

                    TimeManager.Instance.AddTask(5,false, BarHide, this); // Hide the bar after 5 seconds.

                }



                //Debug.Log("Player velocity: " + mag);

            }

        }

        else if (other.CompareTag("Snack") && !isPlay)

        {
            if (SnackManager.Instance != null && !SnackManager.Instance.CanHamsterEatSnack())
            {
                return;
            }

            onTrigger = true;

            isEating = true;

            _animator.Play("Eyes_Excited", _animator.GetLayerIndex("Shapekey"));

            _as.clip = eat;

            _as.Play();

            EventManager.DispatchEvent(EventCommon.HAMSTER_EATING, true); // Pause snack visuals while the hamster is eating.



        }

    }

    private void InvokeTriggerEnterOnceIfNeeded()
    {
        if (hasTriggeredOnTriggerEnterOnce)
        {
            return;
        }

        hasTriggeredOnTriggerEnterOnce = true;
        onTriggerEnterOnce?.Invoke();
    }

    private void BarHide()

    {

        _favEffect.SetActive(false);

    }

    private void OnTriggerExit(Collider other)

    {

        if (!IsHamsterGameplayEnabled())

        {

            return;

        }

        if (other.CompareTag("Player"))

        {

            onTrigger = false;

            isPlay = false;

            if (stayTime < stayRequireTime) // Reset progress if the player leaves before finishing the petting timer.

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

            EventManager.DispatchEvent(EventCommon.HAMSTER_EATING, false); // Resume snack visuals after eating ends.

        }

    }





    /// <summary>

    /// Switch the body animation while the hamster is idle and not being interacted with.

    /// </summary>

    /// <param name="animationName"></param>



    public void ChangeBehaviorAnimationByStr(string animationName)

    {

        if (!IsHamsterGameplayEnabled() || _animator == null)

        {

            return;

        }



        if (!onTrigger && !isDead && !isDamage && !isPlay&&!isOut)

        {

            _animator.Play(animationName);

            //Debug.Log(animationName);

        }



    }

    /// <summary>

    /// Switch the eye animation while the hamster is idle and not being interacted with.

    /// </summary>

    /// <param name="animationName"></param>

    public void ChangeEyesAnimationByStr(string animationName)

    {

        if (!IsHamsterGameplayEnabled() || _animator == null)

        {

            return;

        }



        if (!onTrigger && !isDead && !isDamage && !isPlay&&!isOut)

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

    /// Adjust favorability.

    /// </summary>

    /// <param name="value"></param>

    public void GetFavorability(int value)

    {

        if (!IsHamsterGameplayEnabled())

        {

            return;

        }



        DataCenter.Instance.GetFavorability(value);

    }

    /// <summary>

    /// Adjust HP. Positive values heal, negative values deal damage.

    /// </summary>

    /// <param name="value"></param>

    public void GetDamage(int value)

    {

        if (!IsHamsterGameplayEnabled() || _as == null || _animator == null)

        {

            return;

        }



        // Play hit feedback before applying the HP change.

        _as.clip = hit;

        _as.Play();

        DataCenter.Instance.GetDamage(value);

        if (DataCenter.Instance.GameData.HamsterData.hp <= 0)

        {

            PlayRandomTTS("TTS/HamsterDead");

            Death();

        }

        else

        {

            isDamage = true;

            _animator.SetTrigger("damage");

            // Play a random HamsterHit voice line.

            PlayRandomTTS("TTS/HamsterHit");

        }

    }

    public void Death()

    {

        if (!IsHamsterGameplayEnabled() || _animator == null)

        {

            return;

        }



        // Keep the saved HP in sync with the death state.

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

    public void HamsterFinishEating(SnackData snack)

    {

        if (!IsHamsterGameplayEnabled() || snack == null || _animator == null)

        {

            return;

        }



        isOut = true;

        onTrigger = false;

        GetFavorability(snack.extraFavorability);

        // Apply special snack effects before the hamster walks away.

        if(snack.isPoisonous)

        {

            TTSManager.Instance.PlayTTS("TTS/Special/ChocolateDead");

            Death();

            return;

        }

        else if (snack.isSour)

        {

            TTSManager.Instance.PlayTTS("TTS/Special/LemonHamster");

            _animator.SetBool("Sour", true);

            _animator.Play("Eyes_Trauma", _animator.GetLayerIndex("Shapekey"));

            _animator.Play("Walk");

            _animator.SetBool("Move", true);



            TimeManager.Instance.AddTask(8, false, () => { _animator.Play("Jump"); }, this);

            TimeManager.Instance.AddTask(9.1f, false, () => { _animator.Play("Walk"); }, this);

            return;

        }

        else if (snack.isWine)

        {

            TTSManager.Instance.PlayTTS("TTS/Special/BeerHamster");

            _animator.Play("Eyes_Spin", _animator.GetLayerIndex("Shapekey"));

        }

        else if (snack.isSpicy)

        {

            TTSManager.Instance.PlayTTS("TTS/Special/PeperHamster");

            _animator.Play("Eyes_Shrink", _animator.GetLayerIndex("Shapekey"));

            _flame.Play();

        }

        // Default finish-eating movement sequence.

        _animator.Play("Walk");

        _animator.SetBool("Move",true);

        

        TimeManager.Instance.AddTask(3, false, () => { _animator.Play("Jump"); }, this);

        TimeManager.Instance.AddTask(4.1f, false, () => { _animator.Play("Walk"); }, this);

    }
    public void TryTriggerGuideIntro()
    {
        if (!enableGuideIntro || guideTriggered)
        {
            return;
        }

        guideTriggered = true;
        SetGuideOutlineVisible(false);

        if (!string.IsNullOrEmpty(guideIntroTtsPath) && TTSManager.Instance != null)
        {
            AudioClip guideClip = Resources.Load<AudioClip>(guideIntroTtsPath);
            if (guideClip != null)
            {
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.PushPlayerInteractionLock();
                    TTSManager.Instance.PlayTTS(guideClip, () =>
                    {
                        if (GameManager.Instance != null)
                        {
                            GameManager.Instance.PopPlayerInteractionLock();
                        }
                    });
                }
                else
                {
                    TTSManager.Instance.PlayTTS(guideClip);
                }
            }
        }

        Animator targetAnimator = guideAnimator != null ? guideAnimator : _animator;
        if (targetAnimator != null && !string.IsNullOrEmpty(guideAnimatorTrigger))
        {
            targetAnimator.SetTrigger(guideAnimatorTrigger);
        }

        onGuideTriggered?.Invoke();
    }

    public bool HasTriggeredGuideIntro()
    {
        return guideTriggered;
    }

    public void ForceCompleteGuideIntro()
    {
        enableGuideIntro = false;
        guideTriggered = true;
        if (!_tutorialGuideActive)
        {
            SetGuideOutlineVisible(false);
        }
    }

    public void SetTutorialGuideActive(bool active)
    {
        enableGuideIntro = false;
        _tutorialGuideActive = active;
        SetGuideOutlineVisible(active);
    }

    public void SetTutorialIgnoreHit(bool ignoreHit)
    {
        _tutorialIgnoreHit = ignoreHit;
    }

    public void SetTutorialPettingEnabled(bool enabled)
    {
        _tutorialPettingEnabled = enabled;
        if (!enabled)
        {
            isPlay = false;
        }
    }

    private void ResolveGuideReferences()
    {
        if (guideAnimator == null)
        {
            guideAnimator = _animator != null ? _animator : GetComponentInParent<Animator>();
        }
    }

    private bool IsPlayerInteractionAllowed()
    {
        return GameManager.Instance == null || GameManager.Instance.IsPlayerInteractionEnabled;
    }

    private void SetGuideOutlineVisible(bool visible)
    {
        ResolveGuideReferences();
        if (guideOutlines == null || guideOutlines.Length == 0)
        {
            if (!_missingGuideOutlineLogged)
            {
                Debug.LogWarning("HamsterController: Guide outlines are missing. Please add Outline components in scene and assign guideOutlines.");
                _missingGuideOutlineLogged = true;
            }
            return;
        }

        for (int i = 0; i < guideOutlines.Length; i++)
        {
            if (guideOutlines[i] != null)
            {
                guideOutlines[i].enabled = visible;
            }
        }
    }

    /// <summary>

    /// Play a random TTS clip from a Resources folder.

    /// </summary>

    /// <param name="resourcePath">Resources path without the file extension.</param>

    private void PlayRandomTTS(string resourcePath)

    {

        // Load every candidate clip in the folder.

        AudioClip[] clips = Resources.LoadAll<AudioClip>(resourcePath);

        

        if (clips == null || clips.Length == 0)

        {

            Debug.LogWarning($"HamsterController: no TTS clips found at {resourcePath}.");

            return;

        }



        // Pick one clip at random.

        int randomIndex = Random.Range(0, clips.Length);

        AudioClip selectedClip = clips[randomIndex];



        // Hand the clip off to the shared TTS manager.

        if (TTSManager.Instance != null && selectedClip != null)

        {

            TTSManager.Instance.PlayTTS(selectedClip);

        }

        else

        {

            Debug.LogWarning("HamsterController: TTSManager is missing or the selected clip is null.");

        }

    }

}

