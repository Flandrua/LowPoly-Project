using System;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR.InteractionSystem;

public class DayOneTutorialDirector : MonoBehaviour
{
    public enum Step
    {
        Inactive = 0,
        WaitForWorkArea = 1,
        MorningKeyboard = 2,
        AfternoonHamster = 3,
        NightEatChips = 4,
        NightFeedChips = 5,
        NightLookAtBed = 6,
        Completed = 7
    }

    [Serializable]
    public class StepHook
    {
        public Step step = Step.WaitForWorkArea;
        public UnityEvent onEntered;
        public UnityEvent onGuideDismissed;
        public UnityEvent onCompleted;
    }

    public static DayOneTutorialDirector Instance { get; private set; }

    [Header("Tutorial")]
    [SerializeField] private bool enableTutorial = true;
    [SerializeField] [Min(1)] private int guideDismissHitCount = 3;
    [SerializeField] private string chipsSnackKey = "Chips";
    [SerializeField] [Min(0.1f)] private float bedGazeDuration = 2.5f;
    [SerializeField] private bool debugLog;

    [Header("Scene Refs")]
    [SerializeField] private StartTriggerBoxCallback startTriggerBox;
    [SerializeField] private GameObject keyboardRoot;
    [SerializeField] private KeyboardController keyboardController;
    [SerializeField] private GameObject hamsterRoot;
    [SerializeField] private HamsterController hamsterController;
    [SerializeField] private SnackManager snackManager;
    [SerializeField] private Animator snackGuideAnimator;
    [SerializeField] private GameObject bedGuideTarget;
    [SerializeField] private Animator bedGuideAnimator;

    [Header("TTS Hooks")]
    [SerializeField] private StepHook[] stepHooks;

    [Header("Runtime")]
    [SerializeField] private Step currentStep = Step.Inactive;

    private GuideAnimationLoop _keyboardGuideLoop;
    private GuideAnimationLoop _hamsterGuideLoop;
    private GuideAnimationLoop _snackGuideLoop;
    private GuideAnimationLoop _bedGuideLoop;
    private Collider _bedGazeCollider;
    private PlayerSteamVRManager _playerManager;
    private float _bedGazeStartTime = -1f;
    private bool _isBedGazing;
    private bool _hasFinished;
    private int _ttsLockCount;
    private bool _wantHamsterVisible;
    private bool _sessionStarted;

    public bool IsRunning => enableTutorial && _sessionStarted && !_hasFinished && currentStep != Step.Completed;
    public bool ShouldShowHamster => IsRunning && _wantHamsterVisible && IsHamsterEnabled;
    public bool CanSleep => !IsRunning || currentStep == Step.NightLookAtBed;
    public bool ShouldSkipAfternoonAfterMorningWork => IsRunning && !IsHamsterEnabled;
    public bool ShouldSuppressDayOneNightStageCallbacks => IsRunning && currentStep != Step.NightLookAtBed;
    public bool HasLookedAtBed { get; private set; }
    public Step CurrentStep => currentStep;

    private bool IsHamsterEnabled => GameManager.Instance == null || GameManager.Instance.IsHamsterGameplayEnabled;

    private void Awake()
    {
        Instance = this;
        DisableLegacyGuides();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }

        Unsubscribe();
        ReleaseAllTtsLocks();
    }

    private void Start()
    {
        if (!enableTutorial || GameManager.Instance == null || GameManager.Instance.CurrentDay != 1)
        {
            currentStep = Step.Inactive;
            return;
        }

        ResolveReferences();
        DisableLegacyGuides();
        Subscribe();
        _sessionStarted = true;
        HideAllTutorialTargets();
        if (startTriggerBox == null)
        {
            EnterMorningKeyboard();
        }
        else
        {
            PrepareStartTrigger();
            if (currentStep == Step.Inactive)
            {
                SetStep(Step.WaitForWorkArea);
            }
        }
    }

    private void Update()
    {
        if (!IsRunning)
        {
            return;
        }

        _keyboardGuideLoop?.Tick();
        _hamsterGuideLoop?.Tick();
        _snackGuideLoop?.Tick();
        _bedGuideLoop?.Tick();

        if (currentStep == Step.NightLookAtBed && !HasLookedAtBed)
        {
            TickBedGaze();
        }
    }

    public void PlayTutorialTTS(string resourcePath)
    {
        if (string.IsNullOrEmpty(resourcePath) || TTSManager.Instance == null)
        {
            return;
        }

        PushTtsLock();
        TTSManager.Instance.PlayTTS(resourcePath, PopTtsLock);
    }

    public void PlayTutorialTTS(AudioClip clip)
    {
        if (clip == null || TTSManager.Instance == null)
        {
            return;
        }

        PushTtsLock();
        TTSManager.Instance.PlayTTS(clip, PopTtsLock);
    }

    public void NotifyArrivedAtWorkArea()
    {
        if (currentStep != Step.WaitForWorkArea && currentStep != Step.Inactive)
        {
            return;
        }

        CompleteCurrentStep();
        ReleaseAllTtsLocks();
        EnterMorningKeyboard();
    }

    public void FinishTutorial()
    {
        if (_hasFinished)
        {
            return;
        }

        _hasFinished = true;
        StopAllGuideLoops();
        HasLookedAtBed = true;
        SetSnackRule(SnackManager.TutorialSnackRule.None);
        RestoreHamsterGameplay();
        SetKeyboardVisible(true);
        if (startTriggerBox != null)
        {
            startTriggerBox.gameObject.SetActive(false);
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetPlayerInteractionLock();
            GameManager.Instance.ApplyHamsterFeatureState();
        }

        SetStep(Step.Completed);
        ReleaseAllTtsLocks();
        Log("tutorial finished");
    }

    private void Subscribe()
    {
        if (keyboardController != null)
        {
            keyboardController.ValidHit += OnKeyboardValidHit;
        }

        if (hamsterController != null)
        {
            hamsterController.PetCompleted += OnHamsterPetCompleted;
        }

        EventManager.AddListener<SnackData>(EventCommon.PLAYER_FINISH_EATING, OnPlayerFinishedEating);
        EventManager.AddListener<SnackData>(EventCommon.HAMSTER_FINISH_EATING, OnHamsterFinishedEating);
        EventManager.AddListener(EventCommon.NEXT_STAGE, OnNextStage);
        Teleport.Player.AddListener(OnPlayerTeleported);
    }

    private void Unsubscribe()
    {
        if (keyboardController != null)
        {
            keyboardController.ValidHit -= OnKeyboardValidHit;
        }

        if (hamsterController != null)
        {
            hamsterController.PetCompleted -= OnHamsterPetCompleted;
        }

        if (startTriggerBox != null)
        {
            startTriggerBox.Entered -= NotifyArrivedAtWorkArea;
        }

        EventManager.RemoveListener<SnackData>(EventCommon.PLAYER_FINISH_EATING, OnPlayerFinishedEating);
        EventManager.RemoveListener<SnackData>(EventCommon.HAMSTER_FINISH_EATING, OnHamsterFinishedEating);
        EventManager.RemoveListener(EventCommon.NEXT_STAGE, OnNextStage);
        Teleport.Player.RemoveListener(OnPlayerTeleported);
    }

    private void ResolveReferences()
    {
        if (startTriggerBox == null)
        {
            startTriggerBox = FindObjectByName<StartTriggerBoxCallback>("StartTriggerBox");
        }

        if (keyboardController == null)
        {
            keyboardController = FindObjectOfType<KeyboardController>(true);
        }

        if (keyboardRoot == null && keyboardController != null)
        {
            Transform parent = keyboardController.transform.parent;
            keyboardRoot = parent != null && parent.Find("Work") != null
                ? parent.gameObject
                : keyboardController.gameObject;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.TryGetHamsterController(out hamsterController);
            if (hamsterRoot == null)
            {
                hamsterRoot = GameManager.Instance.HamsterRoot;
            }
        }

        if (hamsterController == null)
        {
            hamsterController = FindObjectOfType<HamsterController>(true);
        }

        if (hamsterRoot == null && hamsterController != null)
        {
            hamsterRoot = hamsterController.transform.parent != null
                ? hamsterController.transform.parent.gameObject
                : hamsterController.gameObject;
        }

        if (snackManager == null)
        {
            snackManager = SnackManager.Instance;
        }

        if (snackGuideAnimator == null && snackManager != null)
        {
            SnackGuideIntroTrigger snackGuide = snackManager.GetComponentInChildren<SnackGuideIntroTrigger>(true);
            if (snackGuide != null)
            {
                snackGuideAnimator = snackGuide.GuideAnimator;
            }
        }

        if (bedGuideTarget == null)
        {
            bedGuideTarget = FindBedGuideTarget();
        }

        if (bedGuideAnimator == null && bedGuideTarget != null)
        {
            bedGuideAnimator = bedGuideTarget.GetComponentInChildren<Animator>(true);
        }

        if (bedGuideTarget != null)
        {
            _bedGazeCollider = bedGuideTarget.GetComponentInChildren<Collider>(true);
        }

        ResolveBedGazeCollider();

        _keyboardGuideLoop = new GuideAnimationLoop(keyboardController != null ? keyboardController.GuideAnimator : null, "Shining");
        _hamsterGuideLoop = new GuideAnimationLoop(hamsterController != null ? hamsterController.GuideAnimator : null, "Shining");
        _snackGuideLoop = new GuideAnimationLoop(snackGuideAnimator, "Shining");
        _bedGuideLoop = new GuideAnimationLoop(bedGuideAnimator, "Shining");
    }

    private void DisableLegacyGuides()
    {
        if (keyboardController != null)
        {
            keyboardController.ForceCompleteGuideIntro();
        }

        if (hamsterController != null)
        {
            hamsterController.ForceCompleteGuideIntro();
        }

        SnackGuideIntroTrigger[] snackGuides = FindObjectsOfType<SnackGuideIntroTrigger>(true);
        for (int i = 0; i < snackGuides.Length; i++)
        {
            if (snackGuides[i] != null)
            {
                snackGuides[i].ForceCompleteGuideIntro();
            }
        }
    }

    private void PrepareStartTrigger()
    {
        startTriggerBox.SetPersistentEventsEnabled(false);
        startTriggerBox.Entered -= NotifyArrivedAtWorkArea;
        startTriggerBox.Entered += NotifyArrivedAtWorkArea;
        startTriggerBox.ResetTriggerState();
        startTriggerBox.gameObject.SetActive(true);
    }

    private void HideAllTutorialTargets()
    {
        SetKeyboardVisible(false);
        SetHamsterVisible(false);
        SetSnackVisible(false);
        SetSnackRule(SnackManager.TutorialSnackRule.None);
        SetKeyboardGuide(false);
        SetHamsterGuide(false);
        SetSnackGuide(false);
        SetBedGuide(false);
        RestoreHamsterGameplay();
    }

    private void EnterMorningKeyboard()
    {
        SetKeyboardVisible(true);
        SetHamsterVisible(false);
        SetSnackVisible(false);
        SetKeyboardGuide(true);
        SetStep(Step.MorningKeyboard);
    }

    private void EnterAfternoonHamster()
    {
        SetKeyboardVisible(false);
        SetKeyboardGuide(false);
        SetSnackVisible(false);
        SetHamsterVisible(true);
        ConfigureHamsterForAfternoon();
        SetHamsterGuide(true);
        SetStep(Step.AfternoonHamster);
    }

    private void EnterNightEatChips()
    {
        SetKeyboardVisible(false);
        SetKeyboardGuide(false);
        SetHamsterGuide(false);
        SetHamsterVisible(false);
        SetSnackVisible(true);
        SetSnackRule(SnackManager.TutorialSnackRule.PlayerEatOnly);
        SpawnChips();
        SetSnackGuide(true);
        SetStep(Step.NightEatChips);
    }

    private void EnterNightFeedChips()
    {
        SetHamsterVisible(IsHamsterEnabled);
        ConfigureHamsterForNight();
        SetSnackRule(SnackManager.TutorialSnackRule.HamsterFeedOnly);
        SpawnChips();
        SetSnackGuide(true);
        SetStep(Step.NightFeedChips);
    }

    private void EnterNightLookAtBed()
    {
        SetSnackGuide(false);
        SetSnackVisible(false);
        SetSnackRule(SnackManager.TutorialSnackRule.None);
        HasLookedAtBed = false;
        _isBedGazing = false;
        _bedGazeStartTime = -1f;
        ResolveBedGazeCollider();
        SetBedGuide(true);
        SetStep(Step.NightLookAtBed);
    }

    private void OnKeyboardValidHit(int actualHit)
    {
        if (currentStep != Step.MorningKeyboard)
        {
            return;
        }

        if (actualHit == guideDismissHitCount)
        {
            SetKeyboardGuide(false);
            InvokeHook(currentStep, hook => hook.onGuideDismissed?.Invoke());
        }
    }

    private void OnHamsterPetCompleted()
    {
        if (currentStep != Step.AfternoonHamster)
        {
            return;
        }

        SetHamsterGuide(false);
        InvokeHook(currentStep, hook => hook.onGuideDismissed?.Invoke());
        CompleteCurrentStep();
    }

    private void OnPlayerFinishedEating(SnackData snack)
    {
        if (currentStep != Step.NightEatChips)
        {
            return;
        }

        SetSnackGuide(false);
        CompleteCurrentStep();
        if (IsHamsterEnabled)
        {
            EnterNightFeedChips();
        }
        else
        {
            EnterNightLookAtBed();
        }
    }

    private void OnHamsterFinishedEating(SnackData snack)
    {
        if (currentStep != Step.NightFeedChips)
        {
            return;
        }

        SetSnackGuide(false);
        CompleteCurrentStep();
        EnterNightLookAtBed();
    }

    private void OnNextStage()
    {
        if (!IsRunning)
        {
            if (GameManager.Instance != null && GameManager.Instance.CurrentDay >= 2)
            {
                FinishTutorial();
            }

            return;
        }

        if (currentStep == Step.MorningKeyboard && GameManager.Instance != null)
        {
            CompleteCurrentStep();
            if (GameManager.Instance.IsNightStage || !IsHamsterEnabled)
            {
                EnterNightEatChips();
            }
            else
            {
                EnterAfternoonHamster();
            }

            return;
        }

        if (currentStep == Step.AfternoonHamster && GameManager.Instance != null && GameManager.Instance.IsNightStage)
        {
            EnterNightEatChips();
        }
    }

    private void OnPlayerTeleported(TeleportMarkerBase teleportedMarker)
    {
        if (currentStep != Step.WaitForWorkArea)
        {
            return;
        }

        startTriggerBox?.DetectOverlappingTargets();
    }

    public void NotifyBedInteracted()
    {
        if (currentStep != Step.NightLookAtBed)
        {
            return;
        }

        if (HasLookedAtBed)
        {
            return;
        }

        HasLookedAtBed = true;
        SetBedGuide(false);
        InvokeHook(currentStep, hook => hook.onGuideDismissed?.Invoke());
        Log("bed interacted");
    }

    private void TickBedGaze()
    {
        if (_bedGazeCollider == null)
        {
            HasLookedAtBed = true;
            InvokeHook(currentStep, hook => hook.onGuideDismissed?.Invoke());
            return;
        }

        if (_playerManager == null)
        {
            _playerManager = PlayerSteamVRManager.Instance;
        }

        if (_playerManager == null ||
            !_playerManager.TryGetCenterGazeState(out _, out bool hasHit, out RaycastHit hitInfo, out _))
        {
            ResetBedGaze();
            return;
        }

        bool looking = hasHit && hitInfo.collider != null && IsBedGazeHit(hitInfo.collider);
        if (!looking)
        {
            ResetBedGaze();
            return;
        }

        if (!_isBedGazing)
        {
            _isBedGazing = true;
            _bedGazeStartTime = Time.time;
        }

        if (Time.time - _bedGazeStartTime < bedGazeDuration)
        {
            return;
        }

        HasLookedAtBed = true;
        InvokeHook(currentStep, hook => hook.onGuideDismissed?.Invoke());
        Log("looked at bed");
    }

    private void ResetBedGaze()
    {
        _isBedGazing = false;
        _bedGazeStartTime = -1f;
    }

    private void ResolveBedGazeCollider()
    {
        if (bedGuideTarget != null)
        {
            _bedGazeCollider = bedGuideTarget.GetComponentInChildren<Collider>(true);
        }

        if (_bedGazeCollider != null)
        {
            return;
        }

        BedSleepTriggerBox bedBox = FindObjectOfType<BedSleepTriggerBox>(true);
        if (bedBox != null)
        {
            _bedGazeCollider = bedBox.GetComponent<Collider>();
            if (bedGuideTarget == null)
            {
                bedGuideTarget = bedBox.gameObject;
            }
        }
    }

    private bool IsBedGazeHit(Collider hitCollider)
    {
        if (hitCollider == null)
        {
            return false;
        }

        if (_bedGazeCollider != null &&
            (hitCollider == _bedGazeCollider ||
             hitCollider.transform.IsChildOf(_bedGazeCollider.transform) ||
             _bedGazeCollider.transform.IsChildOf(hitCollider.transform)))
        {
            return true;
        }

        if (bedGuideTarget != null &&
            (hitCollider.transform == bedGuideTarget.transform ||
             hitCollider.transform.IsChildOf(bedGuideTarget.transform) ||
             bedGuideTarget.transform.IsChildOf(hitCollider.transform)))
        {
            return true;
        }

        if (hitCollider.GetComponentInParent<BedSleepTriggerBox>() != null)
        {
            return true;
        }

        return hitCollider.name.IndexOf("Bed", StringComparison.OrdinalIgnoreCase) >= 0;
    }

    private void SetStep(Step step)
    {
        currentStep = step;
        InvokeHook(step, hook => hook.onEntered?.Invoke());
        Log("step " + step);
    }

    private void CompleteCurrentStep()
    {
        InvokeHook(currentStep, hook => hook.onCompleted?.Invoke());
    }

    private void InvokeHook(Step step, Action<StepHook> invoke)
    {
        if (stepHooks == null || invoke == null)
        {
            return;
        }

        for (int i = 0; i < stepHooks.Length; i++)
        {
            StepHook hook = stepHooks[i];
            if (hook != null && hook.step == step)
            {
                invoke(hook);
            }
        }
    }

    private void SetKeyboardVisible(bool visible)
    {
        if (keyboardRoot != null)
        {
            keyboardRoot.SetActive(visible);
        }
    }

    private void SetHamsterVisible(bool visible)
    {
        _wantHamsterVisible = visible && IsHamsterEnabled;
        if (hamsterRoot != null && hamsterRoot.activeSelf != _wantHamsterVisible)
        {
            hamsterRoot.SetActive(_wantHamsterVisible);
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ApplyHamsterFeatureState();
        }
    }

    private void SetSnackVisible(bool visible)
    {
        if (snackManager == null)
        {
            return;
        }

        if (!visible)
        {
            snackManager.ClearCurrentSnack();
        }

        snackManager.SetContainerVisible(visible);
    }

    private void SpawnChips()
    {
        if (snackManager == null)
        {
            return;
        }

        snackManager.SetContainerVisible(true);
        if (!snackManager.SpawnSnackByName(chipsSnackKey))
        {
            snackManager.RandomSnack();
        }
    }

    private void SetSnackRule(SnackManager.TutorialSnackRule rule)
    {
        if (snackManager != null)
        {
            snackManager.SetTutorialSnackRule(rule);
        }
    }

    private void SetKeyboardGuide(bool visible)
    {
        keyboardController?.SetTutorialGuideActive(visible);
        _keyboardGuideLoop?.SetLooping(visible);
    }

    private void SetHamsterGuide(bool visible)
    {
        hamsterController?.SetTutorialGuideActive(visible);
        _hamsterGuideLoop?.SetLooping(visible);
    }

    private void SetSnackGuide(bool visible)
    {
        if (snackManager != null)
        {
            SnackGuideIntroTrigger guide = snackManager.CurrentSnackGuide;
            if (guide != null)
            {
                guide.SetTutorialGuideActive(visible);
                if (guide.GuideAnimator != null)
                {
                    _snackGuideLoop = new GuideAnimationLoop(guide.GuideAnimator, "Shining");
                }
            }
        }

        _snackGuideLoop?.SetLooping(visible);
    }

    private void SetBedGuide(bool visible)
    {
        _bedGuideLoop?.SetLooping(visible);
    }

    private void StopAllGuideLoops()
    {
        SetKeyboardGuide(false);
        SetHamsterGuide(false);
        SetSnackGuide(false);
        SetBedGuide(false);
    }

    private void ConfigureHamsterForAfternoon()
    {
        if (hamsterController == null)
        {
            return;
        }

        hamsterController.SetTutorialIgnoreHit(true);
        hamsterController.SetTutorialPettingEnabled(true);
    }

    private void ConfigureHamsterForNight()
    {
        if (hamsterController == null)
        {
            return;
        }

        hamsterController.SetTutorialIgnoreHit(true);
        hamsterController.SetTutorialPettingEnabled(false);
    }

    private void RestoreHamsterGameplay()
    {
        if (hamsterController == null)
        {
            return;
        }

        hamsterController.SetTutorialIgnoreHit(false);
        hamsterController.SetTutorialPettingEnabled(true);
    }

    private void PushTtsLock()
    {
        if (GameManager.Instance == null)
        {
            return;
        }

        GameManager.Instance.PushPlayerInteractionLock();
        _ttsLockCount++;
    }

    private void PopTtsLock()
    {
        if (GameManager.Instance == null || _ttsLockCount <= 0)
        {
            _ttsLockCount = 0;
            return;
        }

        GameManager.Instance.PopPlayerInteractionLock();
        _ttsLockCount--;
    }

    private void ReleaseAllTtsLocks()
    {
        while (_ttsLockCount > 0)
        {
            PopTtsLock();
        }
    }

    private static T FindObjectByName<T>(string objectName) where T : Component
    {
        T[] found = FindObjectsOfType<T>(true);
        for (int i = 0; i < found.Length; i++)
        {
            if (found[i] != null && found[i].name == objectName)
            {
                return found[i];
            }
        }

        return found.Length > 0 ? found[0] : null;
    }

    private static GameObject FindBedGuideTarget()
    {
        Animator[] animators = FindObjectsOfType<Animator>(true);
        for (int i = 0; i < animators.Length; i++)
        {
            Animator animator = animators[i];
            if (animator == null || animator.runtimeAnimatorController == null)
            {
                continue;
            }

            if (animator.runtimeAnimatorController.name == "Bed")
            {
                return animator.gameObject;
            }
        }

        Transform[] transforms = Resources.FindObjectsOfTypeAll<Transform>();
        for (int i = 0; i < transforms.Length; i++)
        {
            Transform transform = transforms[i];
            if (transform == null || !transform.gameObject.scene.IsValid())
            {
                continue;
            }

            if (transform.name.IndexOf("Bed", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return transform.gameObject;
            }
        }

        return null;
    }

    private void Log(string message)
    {
        if (debugLog)
        {
            Debug.Log("DayOneTutorialDirector: " + message, this);
        }
    }
}
