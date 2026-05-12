using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SnackManager : MonoSingleton<SnackManager>
{
    // Random snack selection without pooling.
    private Animation _animation;
    private string _animationName;
    private Collider _col;
    private XRGrabInteractable _grabInteractable;
    [SerializeField] private List<GameObject> _snacks = new List<GameObject>();
    [SerializeField] private GameObject _curSnacks;
    private string _snackName;
    private string _desc;
    [SerializeField] private TextMeshProUGUI _content = null;
    [SerializeField] private TextMeshProUGUI _name = null;
    private bool isEating = false;
    private bool _lastGrabState = false;
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private string audioAsset;
    public bool isPlayer = false;
    public bool isHamster = false;
    public GameObject container;

    void Start()
    {
        EventManager.AddListener<bool>(EventCommon.HAMSTER_EATING, HamsterEating);
        EventManager.AddListener<bool>(EventCommon.PLAYER_EATING, PlayerEating);
        EventManager.AddListener(EventCommon.NEXT_STAGE, ResetToDefault);
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        _animation = GetComponent<Animation>();
        _animation.enabled = false;
        _animationName = "VanishEffect";
        _col = GetComponent<Collider>();
        _grabInteractable = GetComponent<XRGrabInteractable>();
        _snacks = GetChildren(transform.Find("Container"));
        _content = UIMonitorController.Instance.content;
        _name = UIMonitorController.Instance.nameTxt;
        RegisterGrabEvents();
        RandomSnack();
    }

    public void ShowUIDec(bool flag)
    {
        _content.text = _desc;
        _name.text = _snackName;
        UIMonitorController.Instance.Show(flag);
    }

    private List<GameObject> GetChildren(Transform parent)
    {
        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in parent)
        {
            children.Add(child.gameObject);
        }
        return children;
    }

    private void OnDestroy()
    {
        EventManager.RemoveListener<bool>(EventCommon.HAMSTER_EATING, HamsterEating);
        EventManager.RemoveListener<bool>(EventCommon.PLAYER_EATING, PlayerEating);
        EventManager.RemoveListener(EventCommon.NEXT_STAGE, ResetToDefault);
        UnregisterGrabEvents();
    }

    public void RandomSnack()
    {
        if (_curSnacks != null)
        {
            _curSnacks.SetActive(false);
        }

        if (_snacks.Count == 0)
        {
            Debug.LogWarning("snacks已用完");
            _lastGrabState = false;
            NotifySnackHint(false);
            return;
        }

        _animation.enabled = false;
        container.transform.localScale = Vector3.one;
        int randomIndex = Random.Range(0, _snacks.Count);
        _curSnacks = _snacks[randomIndex];
        _curSnacks.SetActive(true);
        _snacks.RemoveAt(randomIndex);
        _col.enabled = true;
        SnackData snackData = _curSnacks.GetComponent<SnackData>();
        _snackName = snackData.name;
        _desc = snackData.desc;
        audioAsset = $"TTS/SnackIteraction/{_snackName}";
        _lastGrabState = false;
        NotifySnackHint(false);
    }

    public void ClearCurrentSnack()
    {
        _lastGrabState = false;
        NotifySnackHint(false);

        if (_curSnacks != null)
        {
            _curSnacks.SetActive(false);
        }

        if (_animation != null)
        {
            _animation.enabled = false;
        }

        if (container != null)
        {
            container.transform.localScale = Vector3.one;
        }

        if (_col != null)
        {
            _col.enabled = false;
        }
    }

    public void SetContainerVisible(bool visible)
    {
        _lastGrabState = false;
        NotifySnackHint(false);

        if (container == null)
        {
            return;
        }

        if (container.activeSelf == visible)
        {
            return;
        }

        container.SetActive(visible);
    }

    private void ResetToDefault()
    {
        _lastGrabState = false;
        NotifySnackHint(false);
        transform.position = initialPosition;
        transform.rotation = Quaternion.identity;
        if (_curSnacks != null && !_curSnacks.activeInHierarchy)
        {
            _animation.enabled = false;
            container.transform.localScale = Vector3.one;
        }
    }

    public void PlaySnackTTS()
    {
        TTSManager.Instance.PlayTTS(audioAsset);
    }

    void Update()
    {
        SyncGrabHintState();
    }

    public void HamsterEating(bool flag)
    {
        if (isPlayer) { return; }
        isEating = flag;
        if (flag)
        {
            isHamster = true;
            _animation[_animationName].speed = 1;
            _animation.enabled = true;
            _animation.Play();
            TimeManager.Instance.RemoveTask(StopAnimation, this);
        }
        else
        {
            _animation[_animationName].speed = 0;
            TimeManager.Instance.AddTask(3, false, StopAnimation, this);
        }
    }

    public void PlayerEating(bool flag)
    {
        if (isHamster) { return; }
        isEating = flag;
        if (flag)
        {
            isPlayer = true;
            _animation[_animationName].speed = 1;
            _animation.enabled = true;
            _animation.Play();
            TimeManager.Instance.RemoveTask(StopAnimation, this);
        }
        else
        {
            _animation[_animationName].speed = 0;
            TimeManager.Instance.AddTask(3, false, StopAnimation, this);
        }
    }

    public void StopAnimation()
    {
        isHamster = false;
        isPlayer = false;
        ResetAnimation(_animation, _animationName);
    }

    private void ResetAnimation(Animation ani, string name)
    {
        AnimationState state = ani[name];
        ani.Play(name);
        state.time = 0;
        ani.Sample();
        state.enabled = false;
    }

    public void FinishEating()
    {
        _lastGrabState = false;
        NotifySnackHint(false);
        _col.enabled = false;
        _curSnacks.SetActive(false);
        SnackData snack = _curSnacks.GetComponent<SnackData>();
        if (isHamster)
        {
            HamsterController.Instance.isEating = false;
            EventManager.DispatchEvent<SnackData>(EventCommon.HAMSTER_FINISH_EATING, snack);
        }

        if (isPlayer)
        {
            EventManager.DispatchEvent<SnackData>(EventCommon.PLAYER_FINISH_EATING, snack);
        }

        isPlayer = false;
        isHamster = false;
    }

    private void RegisterGrabEvents()
    {
        if (_grabInteractable == null)
        {
            return;
        }

        _grabInteractable.selectEntered.AddListener(OnSnackGrabbed);
        _grabInteractable.selectExited.AddListener(OnSnackReleased);
    }

    private void UnregisterGrabEvents()
    {
        if (_grabInteractable == null)
        {
            return;
        }

        _grabInteractable.selectEntered.RemoveListener(OnSnackGrabbed);
        _grabInteractable.selectExited.RemoveListener(OnSnackReleased);
    }

    private void OnSnackGrabbed(SelectEnterEventArgs args)
    {
        _lastGrabState = true;
        NotifySnackHint(true);
    }

    private void OnSnackReleased(SelectExitEventArgs args)
    {
        _lastGrabState = false;
        NotifySnackHint(false);
    }

    private void NotifySnackHint(bool showHint)
    {
        EventManager.DispatchEvent(EventCommon.PLAYER_SNACK_HINT, showHint);
    }

    private void SyncGrabHintState()
    {
        if (_grabInteractable == null)
        {
            return;
        }

        bool isGrabbed = _grabInteractable.isSelected;
        if (_lastGrabState == isGrabbed)
        {
            return;
        }

        _lastGrabState = isGrabbed;
        NotifySnackHint(isGrabbed);
    }
}
