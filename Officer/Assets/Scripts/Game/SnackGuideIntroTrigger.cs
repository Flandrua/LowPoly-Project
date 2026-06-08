using UnityEngine;
using UnityEngine.Events;

public class SnackGuideIntroTrigger : MonoBehaviour
{
    [Header("Guide Intro")]
    [SerializeField] private bool enableGuideIntro = true;
    [SerializeField] private bool guideTriggered;

    [Header("Guide Content")]
    [SerializeField] private string guideIntroTtsPath = "TTS/Introduce/Snack";
    [SerializeField] private string guideAnimatorTrigger = "Shining";
    [SerializeField] private Animator guideAnimator;
    [SerializeField] private Outline[] guideOutlines;
    [SerializeField] private UnityEvent onGuideTriggered;

    [Header("Guide vs Normal Snack TTS")]
    [SerializeField] private bool suppressNormalSnackTts = true;

    private void Awake()
    {
        ResolveGuideReferences();
    }

    private void OnEnable()
    {
        SetGuideOutlineVisible(enableGuideIntro && !guideTriggered);
    }

    public void TryTriggerGuideIntro()
    {
        if (!enableGuideIntro || guideTriggered)
        {
            return;
        }

        guideTriggered = true;

        if (suppressNormalSnackTts && SnackManager.Instance != null)
        {
            SnackManager.Instance.SuppressNextNormalSnackTTS();
        }

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

        // Guide done: immediately hand over to original snack outline behavior.
        if (SnackManager.Instance != null)
        {
            SnackManager.Instance.HideSpawnOutlineForRayTarget(gameObject);
        }

        SetGuideOutlineVisible(false);
        onGuideTriggered?.Invoke();
    }

    public void ResetGuideIntro()
    {
        guideTriggered = false;
        SetGuideOutlineVisible(enableGuideIntro);
    }

    public bool ShouldSuppressNormalSnackTtsOnFirstTrigger()
    {
        return suppressNormalSnackTts && enableGuideIntro && !guideTriggered;
    }

    public bool IsGuidePending()
    {
        return enableGuideIntro && !guideTriggered;
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

        if (guideOutlines == null || guideOutlines.Length == 0)
        {
            guideOutlines = GetComponentsInChildren<Outline>(true);
        }

        if ((guideOutlines == null || guideOutlines.Length == 0) && transform.parent != null)
        {
            guideOutlines = transform.parent.GetComponentsInChildren<Outline>(true);
        }
    }

    private void SetGuideOutlineVisible(bool visible)
    {
        if (guideOutlines == null)
        {
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
