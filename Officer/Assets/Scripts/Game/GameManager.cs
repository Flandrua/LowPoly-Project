using UnityEditor;

using UnityEngine;

using UnityEngine.Serialization;



public class GameManager : MonoSingleton<GameManager>

{

    private int curTimeStage = 0;//0???1???2??

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

    public GameObject ending;

    [SerializeField] private bool enableHamster = true;

    [SerializeField] private GameObject hamsterRoot;

    [SerializeField] private HamsterController hamsterController;

    public int TotalDays => Mathf.Max(1, totaldays);

    public bool IsHamsterGameplayEnabled => enableHamster;



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

            EventManager.DispatchEvent(EventCommon.UPDATE_MONITOR);

        }

    }



    private void SendAnimatorPostSignal(bool flag)

    {

        _animator.SetBool("post", flag);

    }



    private void ChangeTime()//??????

    {

        //????????????bool?????

        //?????????

        //?????????????

        //?????????

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

        else if (curTimeStage == 2)//?????

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

            //?????????,????????????????

            PlayerSteamVRManager.Instance.ResetLocation();

            //??????

            SnackManager.Instance.RandomSnack();

            if (DataCenter.Instance.GameData.PlayerData.days >= TotalDays)

            {

                EndingManager.Instance.Ending();

            }

            else

            {

                DataCenter.Instance.GameData.PlayerData.days++;

            }

        }

        EventManager.DispatchEvent(EventCommon.UPDATE_MONITOR);

        TimeManager.Instance.AddTask(0.5f, false, () => { EventManager.DispatchEvent(EventCommon.NEXT_STAGE); }, this);



        TimeManager.Instance.AddTask(1, false, () => { SendAnimatorPostSignal(false); }, this);

    }



    // Update is called once per frame

    void Update()

    {

    }

}

