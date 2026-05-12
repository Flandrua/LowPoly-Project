using UnityEditor;

using UnityEngine;

using UnityEngine.Serialization;



public class GameManager : MonoSingleton<GameManager>

{

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

    public int currentWorkProgress = 0;

    public bool debugDecreaseWorkProgress = false;

    public GameObject ending;

    [SerializeField] private bool enableHamster = true;

    [SerializeField] private GameObject hamsterRoot;

    [SerializeField] private HamsterController hamsterController;

    private float _remainingCountDown;

    private bool _hasTimedOutThisStage;

    private bool _hasTimedOutToday;

    private int _lastSyncedWorkProgress;

    public int TotalDays => Mathf.Max(1, totaldays);

    public bool IsHamsterGameplayEnabled => enableHamster;

    public string CurrentTimeDisplay => GetCurrentTimeDisplay();



    private void Awake()

    {

        DataCenter.Instance.InitData();

        ResolveHamsterReferences();

    }



    void Start()

    {

        EventManager.AddListener<string>(EventCommon.PREPARE_CHANGE_TIME, PrepareChangeTime);

        EventManager.AddListener<bool>(EventCommon.CHANGE_TIME, SendAnimatorPostSignal);

        _animator = GetComponent<Animator>();

        ApplyHamsterFeatureState();
        SyncExposedWorkProgressFromData(false);
        ResetStageCountDown();

    }



    private void OnDestroy()

    {

        EventManager.RemoveListener<string>(EventCommon.PREPARE_CHANGE_TIME, PrepareChangeTime);

        EventManager.RemoveListener<bool>(EventCommon.CHANGE_TIME, SendAnimatorPostSignal);

    }



    private void OnValidate()

    {

        if (!Application.isPlaying)

        {

            return;

        }



        SetHamsterEnabled(enableHamster);
        SyncExposedWorkProgressFromData(true);
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

            DataCenter.Instance.GetWorkProgress(DataCenter.Instance.GetTotalWorkEfficiency());
            SyncExposedWorkProgressFromData(false);

            EventManager.DispatchEvent(EventCommon.UPDATE_MONITOR);

        }

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
                }

                _hasTimedOutToday = false;

            }

        }

        ResetStageCountDown();

        EventManager.DispatchEvent(EventCommon.UPDATE_MONITOR);

        TimeManager.Instance.AddTask(0.5f, false, () => { EventManager.DispatchEvent(EventCommon.NEXT_STAGE); }, this);



        TimeManager.Instance.AddTask(1, false, () => { SendAnimatorPostSignal(false); }, this);

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



    private void TryConsumeDebugDecreaseWorkProgress()

    {

        if (!debugDecreaseWorkProgress)

        {

            return;

        }



        debugDecreaseWorkProgress = false;
        SetCurrentWorkProgress(currentWorkProgress - 1);

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

}

