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
    private string _animationName = "VanishEffect";
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
    private bool _suppressNormalSnackTtsOnce;
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Vector3 initialLocalPosition;
    private Quaternion initialLocalRotation;
    private Vector3 initialLocalScale;
    private Vector3 initialContainerScale;
    private Transform _spawnParent;
    private bool _hasInitialPose;
    private Rigidbody _rb;
    private bool _defaultUseGravity;
    private bool _defaultIsKinematic;
    private bool _hasCachedRigidbodyDefaults;
    private string audioAsset;
    public bool isPlayer = false;
    public bool isHamster = false;
    public GameObject container;
    public bool ShouldShowSpawnOutline => !_spawnOutlineHiddenBySnackGrab;

    public enum TutorialSnackRule
    {
        None = 0,
        PlayerEatOnly = 1,
        HamsterFeedOnly = 2
    }

    private TutorialSnackRule _tutorialSnackRule = TutorialSnackRule.None;

    public SnackGuideIntroTrigger CurrentSnackGuide
    {
        get
        {
            if (_curSnacks == null)
            {
                return null;
            }

            SnackGuideIntroTrigger guide = _curSnacks.GetComponent<SnackGuideIntroTrigger>();
            if (guide == null)
            {
                guide = _curSnacks.GetComponentInChildren<SnackGuideIntroTrigger>(true);
            }

            if (guide == null)
            {
                guide = _curSnacks.GetComponentInParent<SnackGuideIntroTrigger>();
            }

            return guide;
        }
    }

    public bool CanPlayerEatSnack()
    {
        return _tutorialSnackRule != TutorialSnackRule.HamsterFeedOnly;
    }

    public bool CanHamsterEatSnack()
    {
        return _tutorialSnackRule != TutorialSnackRule.PlayerEatOnly;
    }

    public void SetTutorialSnackRule(TutorialSnackRule rule)
    {
        _tutorialSnackRule = rule;
    }

    private void Awake()
    {
        CaptureSpawnPose();
        CacheRigidbodyDefaults();
    }

    void Start()
    {
        EventManager.AddListener<bool>(EventCommon.HAMSTER_EATING, HamsterEating);
        EventManager.AddListener<bool>(EventCommon.PLAYER_EATING, PlayerEating);
        EventManager.AddListener<bool>(EventCommon.PLAYER_SNACK_HINT, OnPlayerSnackHintChanged);
        EventManager.AddListener(EventCommon.NEXT_STAGE, ResetToDefault);
        CaptureSpawnPose();
        CacheRigidbodyDefaults();
        _animation = GetComponent<Animation>();
        _animation.enabled = false;
        _animationName = "VanishEffect";
        _col = GetComponent<Collider>();
        _grabInteractable = GetComponent<XRGrabInteractable>();
        _snacks = GetChildren(transform.Find("Container"));
        _content = UIMonitorController.Instance.content;
        _name = UIMonitorController.Instance.nameTxt;
        RegisterGrabEvents();
        if (ShouldSkipInitialRandomSnack())
        {
            ClearCurrentSnack();
            SetContainerVisible(false);
            return;
        }

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

    public bool SpawnSnackByName(string snackKey)
    {
        EnsureSnackList();
        string normalizedKey = NormalizeSnackKey(snackKey);
        if (string.IsNullOrEmpty(normalizedKey) || container == null)
        {
            return false;
        }

        GameObject match = FindSnackByKey(normalizedKey);
        if (match == null)
        {
            Debug.LogWarning("SnackManager: could not find snack [" + normalizedKey + "].");
            return false;
        }

        ResetSnackRootState(true);
        _spawnOutlineHiddenBySnackGrab = false;
        _hasPlayedPickupTtsForCurrentSnack = false;
        _suppressNormalSnackTtsOnce = false;
        _rayHideOutlineEnableTime = Time.time + Mathf.Max(0f, rayHideOutlineDelayAfterSpawn);

        if (_curSnacks != null && _curSnacks != match)
        {
            _curSnacks.SetActive(false);
        }

        if (_animation != null)
        {
            _animation.enabled = false;
        }

        container.transform.localScale = Vector3.one;
        _curSnacks = match;
        _curSnacks.SetActive(true);
        if (_col != null)
        {
            _col.enabled = true;
        }

        SnackData snackData = _curSnacks.GetComponent<SnackData>();
        _snackName = snackData != null ? snackData.name : _curSnacks.name;
        _desc = snackData != null ? snackData.desc : string.Empty;
        audioAsset = ResolveSnackTtsPath(snackData);
        _lastGrabState = false;
        NotifySnackHint(false);
        SetSpawnObjectOutlineVisible(true);
        return true;
    }

    private void EnsureSnackList()
    {
        if (_snacks != null && _snacks.Count > 0)
        {
            return;
        }

        Transform parent = container != null ? container.transform : transform.Find("Container");
        if (parent != null)
        {
            _snacks = GetChildren(parent);
        }
    }

    private GameObject FindSnackByKey(string normalizedKey)
    {
        Transform parent = container != null ? container.transform : transform.Find("Container");
        if (parent == null)
        {
            return null;
        }

        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            if (child == null)
            {
                continue;
            }

            if (NormalizeSnackKey(child.name) == normalizedKey)
            {
                return child.gameObject;
            }

            SnackData snackData = child.GetComponent<SnackData>();
            if (snackData != null && NormalizeSnackKey(snackData.snackName) == normalizedKey)
            {
                return child.gameObject;
            }
        }

        return null;
    }

    private bool ShouldSkipInitialRandomSnack()
    {
        return GameManager.Instance != null && GameManager.Instance.CurrentDay <= 1;
    }

    public void RandomSnack()
    {
        ResetSnackRootState(true);
        _spawnOutlineHiddenBySnackGrab = false;
        _hasPlayedPickupTtsForCurrentSnack = false;
        _suppressNormalSnackTtsOnce = false;
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
        _suppressNormalSnackTtsOnce = false;
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
        _suppressNormalSnackTtsOnce = false;
        NotifySnackHint(false);
        ResetSnackRootState(true);
        if (_curSnacks != null && !_curSnacks.activeInHierarchy)
        {
            _animation.enabled = false;
            container.transform.localScale = Vector3.one;
        }
    }

    private void CaptureSpawnPose()
    {
        if (_hasInitialPose)
        {
            return;
        }

        _spawnParent = transform.parent;
        initialLocalPosition = transform.localPosition;
        initialLocalRotation = transform.localRotation;
        initialLocalScale = transform.localScale;
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        if (container == null)
        {
            Transform found = transform.Find("Container");
            if (found != null)
            {
                container = found.gameObject;
            }
        }

        initialContainerScale = container != null ? container.transform.localScale : Vector3.one;
        _hasInitialPose = true;
    }

    private void CacheRigidbodyDefaults()
    {
        if (_rb == null)
        {
            _rb = GetComponent<Rigidbody>();
        }

        if (_rb == null)
        {
            return;
        }

        if (_hasCachedRigidbodyDefaults)
        {
            return;
        }

        _defaultUseGravity = _rb.useGravity;
        _defaultIsKinematic = _rb.isKinematic;
        _hasCachedRigidbodyDefaults = true;
    }

    public void ReleaseHeldSnack()
    {
        LaserPointerHandler[] lasers = FindObjectsOfType<LaserPointerHandler>(true);
        for (int i = 0; i < lasers.Length; i++)
        {
            if (lasers[i] != null)
            {
                lasers[i].ForceReleaseIfHolding(gameObject);
            }
        }

        HandGrabCollider[] grabs = FindObjectsOfType<HandGrabCollider>(true);
        for (int i = 0; i < grabs.Length; i++)
        {
            if (grabs[i] != null)
            {
                grabs[i].ForceReleaseIfHolding(gameObject);
            }
        }
    }

    private void ResetSnackRootState(bool resetPose)
    {
        if (resetPose)
        {
            ReleaseHeldSnack();
            StopVanishAnimation();
            if (_hasInitialPose)
            {
                if (_spawnParent != null && transform.parent != _spawnParent)
                {
                    transform.SetParent(_spawnParent, false);
                }

                transform.localPosition = initialLocalPosition;
                transform.localRotation = initialLocalRotation;
                transform.localScale = initialLocalScale;
            }

            if (container != null)
            {
                container.transform.localScale = _hasInitialPose ? initialContainerScale : Vector3.one;
            }
        }

        if (_rb == null)
        {
            _rb = GetComponent<Rigidbody>();
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

        // While snack guide intro is still pending, keep its guide highlight alive until trigger.
        if (IsCurrentSnackGuidePending())
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

        // Keep guide highlight persistent before the snack guide trigger is completed.
        if (IsCurrentSnackGuidePending() && IsCurrentSnackRoot(rayRoot))
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

    // Called when the current snack's guide intro finishes. The guide component may live on a child
    // of the snack, so HideSpawnOutlineForRayTarget(guideObject) can miss the yellow spawn-hint
    // outline on the snack root. Hide it via the known current snack root instead.
    public void HideCurrentSnackSpawnOutline()
    {
        if (_spawnOutlineHiddenBySnackGrab)
        {
            return;
        }

        _spawnOutlineHiddenBySnackGrab = true;
        SetSpawnObjectOutlineVisible(false);
    }

    private bool IsCurrentSnackGuidePending()
    {
        if (_curSnacks == null)
        {
            return false;
        }

        SnackGuideIntroTrigger guide = _curSnacks.GetComponent<SnackGuideIntroTrigger>();
        if (guide == null)
        {
            guide = _curSnacks.GetComponentInParent<SnackGuideIntroTrigger>();
        }

        return guide != null && guide.IsGuidePending();
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
        if (_suppressNormalSnackTtsOnce)
        {
            _suppressNormalSnackTtsOnce = false;
            return;
        }

        if (string.IsNullOrEmpty(audioAsset))
        {
            Debug.LogWarning($"SnackManager: no TTS clip found for snack [{_snackName}].");
            return;
        }

        TTSManager.Instance.PlayTTS(audioAsset);
    }

    // Called before a guide TriggerOnce callback runs, so the same trigger does not
    // also play the snack's normal introduction TTS.
    public void SuppressNextNormalSnackTTS()
    {
        _suppressNormalSnackTtsOnce = true;
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
        if (flag && !CanHamsterEatSnack())
        {
            return;
        }

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
        if (flag && !CanPlayerEatSnack())
        {
            return;
        }

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

    private void StopVanishAnimation()
    {
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.RemoveTask(StopAnimation, this);
        }

        if (_animation == null)
        {
            _animation = GetComponent<Animation>();
        }

        if (_animation == null)
        {
            return;
        }

        if (!string.IsNullOrEmpty(_animationName) && _animation[_animationName] != null)
        {
            AnimationState state = _animation[_animationName];
            state.speed = 0f;
            state.time = 0f;
            _animation.Play(_animationName);
            _animation.Sample();
            state.enabled = false;
        }

        _animation.Stop();
        _animation.enabled = false;
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
