using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class KeyboardController : MonoSingleton<KeyboardController>, IPointerClickHandler
{
    public int requireHit = 6;
    private int actualHit = 0;
    private GameObject _workEffect;
    private Scrollbar _bar; // Interaction progress bar.
    private ParticleSystem _star;
    public List<AudioClip> sounds = new List<AudioClip>();
    private AudioSource _as;
    public bool IsWorkInputCompleted => actualHit >= requireHit;
    [Header("Guide Intro")]
    [SerializeField] private bool enableGuideIntro = true;
    [SerializeField] private bool guideTriggered;
    [SerializeField] private string guideIntroTtsPath = "TTS/Introduce/keyboard";
    [SerializeField] private string guideAnimatorTrigger = "Shining";
    [SerializeField] private Animator guideAnimator;
    [Tooltip("Manual assignment only. Drag scene Outline components here.")]
    public Outline[] guideOutlines;
    [SerializeField] private UnityEvent onGuideTriggered;
    private bool _missingGuideOutlineLogged;
    // Start is called before the first frame update

    void Start()
    {
        _workEffect = transform.parent.Find("Work").gameObject;
        _star = transform.parent.Find("Work").Find("Star").GetComponent<ParticleSystem>();
        _bar = transform.parent.Find("Work").Find("Canvas").Find("Scrollbar").GetComponent<Scrollbar>();
        _as = GetComponent<AudioSource>();
        ResolveGuideReferences();
        SetGuideOutlineVisible(enableGuideIntro && !guideTriggered);
        EventManager.AddListener(EventCommon.NEXT_STAGE, ResetToDefault);
    }

    private void OnDestroy()
    {
        EventManager.RemoveListener(EventCommon.NEXT_STAGE, ResetToDefault);
    }
    public void ResetToDefault()
    {
        actualHit = 0;
        _bar.size= 0;
    }
    // Update is called once per frame
    void Update()
    {

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!IsPlayerInteractionAllowed())
        {
            return;
        }

        TryTriggerGuideIntro();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsPlayerInteractionAllowed())
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
            TimeManager.Instance.RemoveTask(BarHide,this); // Clear any pending hide task.
            _workEffect.SetActive(true);
            TimeManager.Instance.AddTask(5, false, BarHide, this); // Hide the bar after 5 seconds.
            InstantaneousSpeedCalculator calculator = other.GetComponent<InstantaneousSpeedCalculator>();
            if (calculator != null)
            {
                // Read the current hit speed.
                Vector3 velocity = calculator.InstantaneousSpeed;
                float mag = velocity.magnitude;
                if (mag > 1.5) // Treat this as a hit.
                    HitHandle();
            }
        }
    }


    private void OnTriggerExit(Collider other)
    {
        //if (other.CompareTag("Player"))
        //{
        //    _workEffect.SetActive(false);
        //}
    }
    private void BarHide()
    {
        _workEffect?.SetActive(false);
    }

    private void HitHandle()
    {
        // Play a random key hit sound.
        int randomIndex = Random.Range(0, sounds.Count);
        _as.clip = sounds[randomIndex];
        _as.Play();

        if (actualHit < requireHit)
        {
            actualHit++;
            _bar.size = ((float)actualHit / (float)requireHit);
            Debug.Log(actualHit);
        }
        else if(actualHit == requireHit)
        {
            actualHit++; // Prevent this branch from firing again on extra hits.
            _star.Play();
            // Notify that work for this stage is complete.
            EventManager.DispatchEvent<string>(EventCommon.PREPARE_CHANGE_TIME,"work");
        }
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

        if (guideAnimator != null && !string.IsNullOrEmpty(guideAnimatorTrigger))
        {
            guideAnimator.SetTrigger(guideAnimatorTrigger);
        }

        onGuideTriggered?.Invoke();
    }

    private bool IsPlayerInteractionAllowed()
    {
        return GameManager.Instance == null || GameManager.Instance.IsPlayerInteractionEnabled;
    }

    public bool HasTriggeredGuideIntro()
    {
        return guideTriggered;
    }

    public void ForceCompleteGuideIntro()
    {
        enableGuideIntro = false;
        guideTriggered = true;
        SetGuideOutlineVisible(false);
    }

    private void ResolveGuideReferences()
    {
        if (guideAnimator == null)
        {
            guideAnimator = GetComponentInParent<Animator>();
        }
    }

    private void SetGuideOutlineVisible(bool visible)
    {
        ResolveGuideReferences();
        if (guideOutlines == null || guideOutlines.Length == 0)
        {
            if (!_missingGuideOutlineLogged)
            {
                Debug.LogWarning("KeyboardController: Guide outlines are missing. Please add Outline components in scene and assign guideOutlines.");
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
}
