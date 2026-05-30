using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SnackManager : MonoSingleton<SnackManager>
{
    public const string RayInteractableTag = "RayInteractable";

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
    private bool _spawnOutlineHiddenBySnackGrab;
    [SerializeField] private float rayHideOutlineDelayAfterSpawn = 0.35f;
    private float _rayHideOutlineEnableTime;
    private bool _hasPlayedPickupTtsForCurrentSnack;
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Rigidbody _rb;
    private bool _defaultUseGravity;
    private bool _defaultIsKinematic;
    private string audioAsset;
    public bool isPlayer = false;
    public bool isHamster = false;
    public GameObject container;
    public bool ShouldShowSpawnOutline => !_spawnOutlineHiddenBySnackGrab;

    void Start()
    {
        EventManager.AddListener<bool>(EventCommon.HAMSTER_EATING, HamsterEating);
        EventManager.AddListener<bool>(EventCommon.PLAYER_EATING, PlayerEating);
        EventManager.AddListener<bool>(EventCommon.PLAYER_SNACK_HINT, OnPlayerSnackHintChanged);
        EventManager.AddListener(EventCommon.NEXT_STAGE, ResetToDefault);
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        _animation = GetComponent<Animation>();
        _animation.enabled = false;
        _animationName = "VanishEffect";
        _col = GetComponent<Collider>();
        _rb = GetComponent<Rigidbody>();
        if (_rb != null)
        {
            _defaultUseGravity = _rb.useGravity;
            _defaultIsKinematic = _rb.isKinematic;
        }
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
        EventManager.RemoveListener<bool>(EventCommon.PLAYER_SNACK_HINT, OnPlayerSnackHintChanged);
        EventManager.RemoveListener(EventCommon.NEXT_STAGE, ResetToDefault);
        UnregisterGrabEvents();
    }

    public void RandomSnack()
    {
        ResetSnackRootState(true);
        _spawnOutlineHiddenBySnackGrab = false;
        _hasPlayedPickupTtsForCurrentSnack = false;
        _rayHideOutlineEnableTime = Time.time + Mathf.Max(0f, rayHideOutlineDelayAfterSpawn);

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
        audioAsset = ResolveSnackTtsPath(snackData);
        _lastGrabState = false;
        NotifySnackHint(false);
        SetSpawnObjectOutlineVisible(true);
    }

    public void ClearCurrentSnack()
    {
        _lastGrabState = false;
        _spawnOutlineHiddenBySnackGrab = false;
        _hasPlayedPickupTtsForCurrentSnack = false;
        NotifySnackHint(false);
        ResetSnackRootState(true);

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

        if (visible)
        {
            ResetSnackRootState(true);
        }
    }

    private void ResetToDefault()
    {
        _lastGrabState = false;
        _spawnOutlineHiddenBySnackGrab = false;
        NotifySnackHint(false);
        ResetSnackRootState(true);
        if (_curSnacks != null && !_curSnacks.activeInHierarchy)
        {
            _animation.enabled = false;
            container.transform.localScale = Vector3.one;
        }
    }

    private void ResetSnackRootState(bool resetPose)
    {
        if (resetPose)
        {
            transform.position = initialPosition;
            transform.rotation = initialRotation;
        }

        if (_rb == null)
        {
            return;
        }

        _rb.velocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        _rb.useGravity = _defaultUseGravity;
        _rb.isKinematic = _defaultIsKinematic;
    }

    private void OnPlayerSnackHintChanged(bool visible)
    {
        if (!visible)
        {
            return;
        }

        // Play the current snack's TTS once on the first pickup, before clearing hints.
        if (!_hasPlayedPickupTtsForCurrentSnack)
        {
            _hasPlayedPickupTtsForCurrentSnack = true;
            PlaySnackTTS();
        }

        if (_spawnOutlineHiddenBySnackGrab)
        {
            return;
        }

        _spawnOutlineHiddenBySnackGrab = true;
        SetSpawnObjectOutlineVisible(false);
    }

    // Hide the outline only for the specific object the player's ray actually hit.
    public void HideSpawnOutlineForRayTarget(GameObject rayRoot)
    {
        if (rayRoot == null)
        {
            return;
        }

        // Grace period after spawn so a freshly placed snack isn't cleared instantly.
        if (IsCurrentSnackRoot(rayRoot) && Time.time < _rayHideOutlineEnableTime)
        {
            return;
        }

        SetOutlineVisibleRecursive(rayRoot, false);
    }

    private bool IsCurrentSnackRoot(GameObject rayRoot)
    {
        if (_curSnacks == null || rayRoot == null)
        {
            return false;
        }

        Transform rayTransform = rayRoot.transform;
        Transform snackTransform = _curSnacks.transform;
        return rayTransform == snackTransform
            || snackTransform.IsChildOf(rayTransform)
            || rayTransform.IsChildOf(snackTransform);
    }

    private void SetSpawnObjectOutlineVisible(bool visible)
    {
        // Only the current snack acts as the spawn hint; items are handled by MainItemManager per-item.
        SetOutlineVisibleRecursive(_curSnacks, visible);
    }

    private void SetOutlineVisibleRecursive(GameObject target, bool visible)
    {
        if (target == null)
        {
            return;
        }

        Outline[] outlines = target.GetComponentsInChildren<Outline>(true);
        for (int i = 0; i < outlines.Length; i++)
        {
            if (outlines[i] != null)
            {
                outlines[i].enabled = visible;
            }
        }
    }

    public void PlaySnackTTS()
    {
        if (string.IsNullOrEmpty(audioAsset))
        {
            Debug.LogWarning($"SnackManager: no TTS clip found for snack [{_snackName}].");
            return;
        }

        TTSManager.Instance.PlayTTS(audioAsset);
    }

    // Resolve the snack's TTS clip by trying snackName first, then the GameObject name,
    // so a misnamed/renamed GameObject (e.g. "Chips (1)") never maps to the wrong audio.
    private string ResolveSnackTtsPath(SnackData snackData)
    {
        if (snackData == null)
        {
            return string.Empty;
        }

        string[] candidates =
        {
            snackData.snackName,
            snackData.gameObject != null ? snackData.gameObject.name : null
        };

        for (int i = 0; i < candidates.Length; i++)
        {
            string key = NormalizeSnackKey(candidates[i]);
            if (string.IsNullOrEmpty(key))
            {
                continue;
            }

            string path = $"TTS/SnackIteraction/{key}";
            if (Resources.Load<AudioClip>(path) != null)
            {
                return path;
            }
        }

        return string.Empty;
    }

    private string NormalizeSnackKey(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return string.Empty;
        }

        string key = raw.Trim();
        const string cloneSuffix = "(Clone)";
        if (key.EndsWith(cloneSuffix))
        {
            key = key.Substring(0, key.Length - cloneSuffix.Length).Trim();
        }

        // Strip a trailing " (1)" style duplicate suffix added when instances are cloned/renamed.
        int openIndex = key.LastIndexOf('(');
        if (openIndex > 0 && key.EndsWith(")"))
        {
            string inside = key.Substring(openIndex + 1, key.Length - openIndex - 2);
            if (int.TryParse(inside, out _))
            {
                key = key.Substring(0, openIndex).Trim();
            }
        }

        return key;
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
