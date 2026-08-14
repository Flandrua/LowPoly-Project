using System;
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
    private bool _tutorialGuideActive;
    public bool IsWorkInputCompleted => actualHit >= requireHit;
    public int ActualHit => actualHit;
    public Animator GuideAnimator => guideAnimator;
    public event Action<int> ValidHit;
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
        if (actualHit >= requireHit)
        {
            return;
        }

        // Play a random key hit sound.
        if (sounds != null && sounds.Count > 0 && _as != null)
        {
            int randomIndex = UnityEngine.Random.Range(0, sounds.Count);
            _as.clip = sounds[randomIndex];
            _as.Play();
        }

        actualHit++;
        if (_bar != null)
        {
            _bar.size = (float)actualHit / Mathf.Max(1, requireHit);
        }

        ValidHit?.Invoke(actualHit);

        if (actualHit < requireHit)
        {
            return;
        }

        if (_star != null)
        {
            _star.Play();
        }

        EventManager.DispatchEvent<string>(EventCommon.PREPARE_CHANGE_TIME, "work");
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
