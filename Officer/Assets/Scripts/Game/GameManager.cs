using UnityEditor;
using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using UnityEngine.Serialization;



public class GameManager : MonoSingleton<GameManager>

{

    private enum TimeStage
    {
        Morning = 0,
        Afternoon = 1,
        Night = 2
    }

    [Serializable]
    private class TimeStageCallbackEntry
    {
        public TimeStage stage = TimeStage.Morning;
        [Tooltip("0 means every day. 1+ means trigger only on that day.")]
        [Min(0)] public int day = 0;
        public UnityEvent onTriggered;

        public bool Matches(int currentStage, int currentDay)
        {
            bool stageMatches = (int)stage == currentStage;
            bool dayMatches = day <= 0 || day == currentDay;
            return stageMatches && dayMatches;
        }
    }

    private int curTimeStage = 0;//0 = morning, 1 = afternoon, 2 = night
    public float countDown = 60f;

    private Animator _animator;

    public ReflectionProbe rp;

    public Texture morningProbe;

    public Texture afternoonProbe;

    public Texture nightProbe;

    public Material morning;

    public Material afternoon;

    public Material night;

    public int totaldays = 10;

    [FormerlySerializedAs("goalWorkPrgoress")]

    public int goalWorkProgress = 50;
    [Min(0)] public int hamsterLoveEndingFavorabilityThreshold = 10;

    public int currentWorkProgress = 0;
    public int currentFatigue = 0;

    public bool debugDecreaseWorkProgress = false;

    public GameObject ending;

    [SerializeField] private bool enableHamster = true;
    [SerializeField] private List<TimeStageCallbackEntry> stageAdvanceCallbacks = new List<TimeStageCallbackEntry>();

    [SerializeField] private GameObject hamsterRoot;

    [SerializeField] private HamsterController hamsterController;

    [SerializeField] private GameObject noSnackObject;
    [Header("Player Ray")]
    [Min(0.05f)] public float playerRayLength = 100f;
    [SerializeField] private List<LaserPointerHandler> playerLaserPointers = new List<LaserPointerHandler>();

    private float _remainingCountDown;

    private bool _hasTimedOutThisStage;

    private bool _hasTimedOutToday;

    private bool _hasShownNoSnackObject;

    private int _lastSyncedWorkProgress;
    private int _lastSyncedFatigue;
    private bool _advanceBySleeping;
    private bool _hasTriggeredDeathEnding;
    private bool _isStageAdvanceRequested;

    public int TotalDays => Mathf.Max(1, totaldays);
    public int HamsterLoveEndingFavorabilityThreshold => Mathf.Max(0, hamsterLoveEndingFavorabilityThreshold);

    public bool IsHamsterGameplayEnabled => enableHamster;
    // True while a stage advance is in progress (the time-switching gap). Used to lock work/sleep/pet inputs.
    public bool IsStageAdvanceRequested => _isStageAdvanceRequested;
    public bool IsNightStage => curTimeStage == (int)TimeStage.Night;
    public int CurrentFatigue => GetCurrentFatigueSafe();

    public string CurrentTimeDisplay => GetCurrentTimeDisplay();



    private void Awake()

    {

        DataCenter.Instance.InitData();

        ResolveHamsterReferences();
        ResolvePlayerRayPointers();
        ApplyPlayerRayLength();

    }
    public void DecreaseWorkProcess()
    {
        debugDecreaseWorkProgress = true;
    }


    void Start()

    {

        EventManager.AddListener<string>(EventCommon.PREPARE_CHANGE_TIME, PrepareChangeTime);

        EventManager.AddListener<bool>(EventCommon.CHANGE_TIME, SendAnimatorPostSignal);

        _animator = GetComponent<Animator>();

        ResolveNoSnackObjectReference();
        _hasShownNoSnackObject = noSnackObject != null && noSnackObject.activeSelf;
        ApplyHamsterFeatureState();
        ResolvePlayerRayPointers();
        ApplyPlayerRayLength();
        SyncExposedWorkProgressFromData(false);
        SyncExposedFatigueFromData();
        ResetStageCountDown();
        ApplyHalfOnByFatigue();

    }



    private void OnDestroy()

    {

        EventManager.RemoveListener<string>(EventCommon.PREPARE_CHANGE_TIME, PrepareChangeTime);

        EventManager.RemoveListener<bool>(EventCommon.CHANGE_TIME, SendAnimatorPostSignal);

    }



    private void OnValidate()

    {

        ResolveNoSnackObjectReference();
        ResolvePlayerRayPointers();

        if (!Application.isPlaying)

        {

            return;

        }



        SetHamsterEnabled(enableHamster);
        ApplyPlayerRayLength();
        SyncExposedWorkProgressFromData(true);
        ApplyInspectorFatigueDebugValue();
        TryConsumeDebugDecreaseWorkProgress();

    }



    public void SetHamsterEnabled(bool enabled)

    {

        enableHamster = enabled;

        ApplyHamsterFeatureState();

    }



    public void OnHamsterFeatureChanged(bool enabled)

    {

        SetHamsterEnabled(enabled);

    }

    public void SetPlayerRayLength(float rayLength)
    {
        playerRayLength = Mathf.Max(0.05f, rayLength);
        ApplyPlayerRayLength();
    }

    private void ResolvePlayerRayPointers()
    {
        if (playerLaserPointers == null)
        {
            playerLaserPointers = new List<LaserPointerHandler>();
        }

        playerLaserPointers.RemoveAll(pointer => pointer == null);
        if (playerLaserPointers.Count > 0)
        {
            return;
        }

        LaserPointerHandler[] foundPointers = FindObjectsOfType<LaserPointerHandler>(true);
        if (foundPointers == null || foundPointers.Length == 0)
        {
            return;
        }

        playerLaserPointers.AddRange(foundPointers);
    }

    private void ApplyPlayerRayLength()
    {
        ResolvePlayerRayPointers();
        float clampedLength = Mathf.Max(0.05f, playerRayLength);
        playerRayLength = clampedLength;

        for (int i = 0; i < playerLaserPointers.Count; i++)
        {
            LaserPointerHandler pointer = playerLaserPointers[i];
            if (pointer == null)
            {
                continue;
            }

            pointer.SetMaxRayDistance(clampedLength);
        }
    }



    public bool TryGetHamsterController(out HamsterController controller)

    {

        ResolveHamsterReferences();

        controller = enableHamster ? hamsterController : null;

        return controller != null;

    }



    public bool IsHamsterDead()

    {

        return TryGetHamsterController(out HamsterController controller) && controller.isDead;

    }



    public bool IsHamsterOut()

    {

        return TryGetHamsterController(out HamsterController controller) && controller.isOut;

    }



    private void ResolveHamsterReferences()

    {

        if (hamsterController == null)

        {

            hamsterController = FindObjectOfType<HamsterController>(true);

        }



        if (hamsterRoot == null && hamsterController != null)

        {

            Transform hamsterTransform = hamsterController.transform.parent != null ? hamsterController.transform.parent : hamsterController.transform;

            hamsterRoot = hamsterTransform.gameObject;

        }

    }



    private void ApplyHamsterFeatureState()
    {
        ResolveHamsterReferences();

        if (Application.isPlaying && !enableHamster)
        {
            TTSManager ttsManager = FindObjectOfType<TTSManager>(true);
            if (ttsManager != null)
            {
                ttsManager.StopTTS();
            }

            EventManager.DispatchEvent<bool>(EventCommon.HAMSTER_EATING, false);
        }

        if (hamsterRoot != null && hamsterRoot.activeSelf != enableHamster)
        {
            hamsterRoot.SetActive(enableHamster);
        }
    }

    private void PrepareChangeTime(string type)

    {
        if (_isStageAdvanceRequested)
        {
            return;
        }

        if (type == "play")

        {

            if (!IsHamsterGameplayEnabled)

            {

                return;

            }



            DataCenter.Instance.GetFavorability(DataCenter.Instance.GetTotalFavorabilityAbility());

        }

        else if (type == "work")

        {
            ApplyWorkProgressFromMouseClick();
        }

        _isStageAdvanceRequested = true;
        EventManager.DispatchEvent<bool>(EventCommon.CHANGE_TIME, true);

    }



    private void SendAnimatorPostSignal(bool flag)

    {

        _animator.SetBool("post", flag);

    }



    private void ChangeTime()//Advance the time-of-day stage

    {

        if (curTimeStage == 0)

        {

            RenderSettings.skybox = afternoon;



            curTimeStage++;

        }

        else if (curTimeStage == 1)

        {

            RenderSettings.skybox = night;

            curTimeStage++;

        }

        else if (curTimeStage == 2)//Advance to the next day

        {

            bool sleptThisNight = _advanceBySleeping;
            _advanceBySleeping = false;

            RenderSettings.skybox = morning;

            if (IsHamsterGameplayEnabled)

            {

                DataCenter.Instance.GameData.HamsterData.hp = 10;

                if (IsHamsterOut())

                {

                    MainItemManager.Instance.RandomItem();

                }



                if (TryGetHamsterController(out HamsterController controller))

                {

                    controller.ResetMoveAnimation();

                }

            }



            curTimeStage = 0;

            // Reset the player location after a full day cycle.

            PlayerSteamVRManager.Instance.ResetLocation();

            if (sleptThisNight)
            {
                DataCenter.Instance.ResetFatigue();
            }
            else
            {
                DataCenter.Instance.AddFatigue(1);
            }
            SyncExposedFatigueFromData();

            bool isDeathEnding = EvaluateDailyFatigueState();
            if (!isDeathEnding)
            {
                if (DataCenter.Instance.GameData.PlayerData.days >= TotalDays)

                {

                    EndingManager.Instance.Ending();

                }

                else

                {

                    bool shouldShowSnackContainerNextDay = !_hasTimedOutToday;
                    DataCenter.Instance.GameData.PlayerData.days++;
                    if (shouldShowSnackContainerNextDay)
                    {
                        SnackManager.Instance.SetContainerVisible(true);
                        SnackManager.Instance.RandomSnack();
                    }
                    else
                    {
                        SnackManager.Instance.ClearCurrentSnack();
                        SnackManager.Instance.SetContainerVisible(false);
                        ShowNoSnackObject();
                    }

                    _hasTimedOutToday = false;

                }
            }

        }

        InvokeStageAdvanceCallbacks();
        ResetStageCountDown();

        EventManager.DispatchEvent(EventCommon.UPDATE_MONITOR);

        TimeManager.Instance.AddTask(0.5f, false, () => { EventManager.DispatchEvent(EventCommon.NEXT_STAGE); }, this);



        TimeManager.Instance.AddTask(1, false, () => { SendAnimatorPostSignal(false); }, this);
        _isStageAdvanceRequested = false;

    }

    private void InvokeStageAdvanceCallbacks()

    {

        if (stageAdvanceCallbacks == null || stageAdvanceCallbacks.Count == 0)

        {

            return;

        }



        int currentDay = GetCurrentDaySafe();

        for (int i = 0; i < stageAdvanceCallbacks.Count; i++)

        {

            TimeStageCallbackEntry entry = stageAdvanceCallbacks[i];

            if (entry == null || !entry.Matches(curTimeStage, currentDay))

            {

                continue;

            }



            entry.onTriggered?.Invoke();

        }

    }

    private int GetCurrentDaySafe()

    {

        if (DataCenter.Instance == null ||
            DataCenter.Instance.GameData == null ||
            DataCenter.Instance.GameData.PlayerData == null)

        {

            return 1;

        }



        return Mathf.Max(1, DataCenter.Instance.GameData.PlayerData.days);

    }

    public bool TrySleepToNextDay()
    {
        if (!IsNightStage)
        {
            return false;
        }

        if (_isStageAdvanceRequested)
        {
            return false;
        }

        _advanceBySleeping = true;
        _isStageAdvanceRequested = true;
        EventManager.DispatchEvent<bool>(EventCommon.CHANGE_TIME, true);
        return true;
    }

    public void SleepToNextDayFromInteraction()
    {
        TrySleepToNextDay();
    }

    public void ApplyWorkProgressFromMouseClick()
    {
        if (DataCenter.Instance == null || DataCenter.Instance.GameData == null || DataCenter.Instance.GameData.PlayerData == null)
        {
            return;
        }

        DataCenter.Instance.GetWorkProgress(DataCenter.Instance.GetTotalWorkEfficiency());
        SyncExposedWorkProgressFromData(false);
        EventManager.DispatchEvent(EventCommon.UPDATE_MONITOR);
    }



    // Update is called once per frame

    void Update()

    {
        TickStageCountDown();
        SyncWorkProgressBinding();
        TryConsumeDebugDecreaseWorkProgress();
    }

    private void SyncWorkProgressBinding()

    {

        if (!HasPlayerProgressData())

        {

            return;

        }



        int dataProgress = Mathf.Max(0, DataCenter.Instance.GameData.PlayerData.workProgress);
        int exposedProgress = Mathf.Max(0, currentWorkProgress);

        if (dataProgress != _lastSyncedWorkProgress)

        {

            currentWorkProgress = dataProgress;
            _lastSyncedWorkProgress = dataProgress;
            return;

        }



        if (exposedProgress == _lastSyncedWorkProgress)

        {

            return;

        }



        SetCurrentWorkProgress(exposedProgress);

    }

    private void TickStageCountDown()

    {

        if (!ShouldRunStageCountDown() || _hasTimedOutThisStage)

        {

            return;

        }



        _remainingCountDown -= Time.deltaTime;

        if (_remainingCountDown > 0f)

        {

            return;

        }



        _remainingCountDown = 0f;
        _hasTimedOutThisStage = true;
        _hasTimedOutToday = true;

    }



    private bool ShouldRunStageCountDown()

    {

        return DataCenter.Instance != null &&
               DataCenter.Instance.GameData != null &&
               DataCenter.Instance.GameData.PlayerData != null &&
               DataCenter.Instance.GameData.PlayerData.days >= 2;

    }



    private void ResetStageCountDown()

    {

        _remainingCountDown = Mathf.Max(0f, countDown);
        _hasTimedOutThisStage = false;

    }



    public void SetCurrentWorkProgress(int progress)

    {

        if (!HasPlayerProgressData())

        {

            currentWorkProgress = Mathf.Max(0, progress);
            _lastSyncedWorkProgress = currentWorkProgress;
            return;

        }



        int clampedProgress = Mathf.Max(0, progress);
        DataCenter.Instance.GameData.PlayerData.workProgress = clampedProgress;
        currentWorkProgress = clampedProgress;
        _lastSyncedWorkProgress = clampedProgress;
        EventManager.DispatchEvent(EventCommon.UPDATE_MONITOR);

    }



    private void SyncExposedWorkProgressFromData(bool dispatchEvent)

    {

        if (!HasPlayerProgressData())

        {

            currentWorkProgress = Mathf.Max(0, currentWorkProgress);
            _lastSyncedWorkProgress = currentWorkProgress;
            return;

        }



        currentWorkProgress = Mathf.Max(0, DataCenter.Instance.GameData.PlayerData.workProgress);
        _lastSyncedWorkProgress = currentWorkProgress;

        if (dispatchEvent)

        {

            EventManager.DispatchEvent(EventCommon.UPDATE_MONITOR);

        }

    }



    private bool HasPlayerProgressData()

    {

        return DataCenter.Instance != null &&
               DataCenter.Instance.GameData != null &&
               DataCenter.Instance.GameData.PlayerData != null;

    }

    private int GetCurrentFatigueSafe()
    {
        if (DataCenter.Instance == null ||
            DataCenter.Instance.GameData == null ||
            DataCenter.Instance.GameData.PlayerData == null)
        {
            return 0;
        }

        return Mathf.Max(0, DataCenter.Instance.GameData.PlayerData.fatigue);
    }

    private void SyncExposedFatigueFromData()
    {
        int dataFatigue = GetCurrentFatigueSafe();
        currentFatigue = dataFatigue;
        _lastSyncedFatigue = dataFatigue;
    }

    private void ApplyInspectorFatigueDebugValue()
    {
        int dataFatigue = GetCurrentFatigueSafe();
        int exposedFatigue = Mathf.Max(0, currentFatigue);

        if (dataFatigue != _lastSyncedFatigue)
        {
            currentFatigue = dataFatigue;
            _lastSyncedFatigue = dataFatigue;
            return;
        }

        if (exposedFatigue == _lastSyncedFatigue)
        {
            return;
        }

        if (DataCenter.Instance == null ||
            DataCenter.Instance.GameData == null ||
            DataCenter.Instance.GameData.PlayerData == null)
        {
            currentFatigue = exposedFatigue;
            _lastSyncedFatigue = exposedFatigue;
            return;
        }

        DataCenter.Instance.GameData.PlayerData.fatigue = exposedFatigue;
        _lastSyncedFatigue = exposedFatigue;
        currentFatigue = exposedFatigue;
        EvaluateDailyFatigueState();
    }

    private void ApplyHalfOnByFatigue()
    {
        if (_animator == null)
        {
            return;
        }

        _animator.SetBool("gray", GetCurrentFatigueSafe() >= 3);
    }

    private bool EvaluateDailyFatigueState()
    {
        ApplyHalfOnByFatigue();

        if (_hasTriggeredDeathEnding || GetCurrentFatigueSafe() < 4)
        {
            return false;
        }

        if (EndingManager.Instance == null)
        {
            return false;
        }

        _hasTriggeredDeathEnding = true;
        EndingManager.Instance.EndingDeath();
        return true;
    }



    private void TryConsumeDebugDecreaseWorkProgress()

    {

        if (!debugDecreaseWorkProgress)

        {

            return;

        }



        debugDecreaseWorkProgress = false;
        SetCurrentWorkProgress(currentWorkProgress - 3);

    }



    private string GetCurrentTimeDisplay()

    {

        switch (curTimeStage)

        {

            case 0:
                return "09:00";

            case 1:
                return "18:00";

            case 2:
                return "21:00";

            default:
                return "09:00";

        }

    }

    private void ShowNoSnackObject()

    {

        ResolveNoSnackObjectReference();

        if (noSnackObject == null)

        {

            return;

        }

        if (_hasShownNoSnackObject)

        {

            return;

        }



        noSnackObject.SetActive(true);
        _hasShownNoSnackObject = true;

    }



    private void ResolveNoSnackObjectReference()

    {

        if (noSnackObject != null)
        {
            return;
        }

        GameObject[] sceneObjects = Resources.FindObjectsOfTypeAll<GameObject>();

        foreach (GameObject candidate in sceneObjects)

        {

            if (candidate == null || candidate.name != "No snack")

            {

                continue;

            }



            if (!candidate.scene.IsValid())

            {

                continue;

            }



            noSnackObject = candidate;
            return;

        }

    }

}

