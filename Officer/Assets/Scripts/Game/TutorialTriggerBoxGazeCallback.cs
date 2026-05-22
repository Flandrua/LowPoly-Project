using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]
public class TutorialTriggerBoxGazeCallback : MonoBehaviour
{
    [SerializeField] [Min(0f)] private float gazeDuration = 3f;
    [SerializeField] private bool triggerOnce = true;
    [SerializeField] private UnityEvent onGazeStayComplete;
    [SerializeField] private bool debugLog = false;

    public bool HasTriggered { get; private set; }

    private PlayerSteamVRManager _playerManager;
    private Collider _targetCollider;
    private float _gazeStartTime = -1f;
    private float _nextResolvePlayerManagerTime;
    private bool _isGazing;

    private void Awake()
    {
        _targetCollider = GetComponent<Collider>();
        if (_targetCollider != null)
        {
            _targetCollider.isTrigger = true;
        }

        ResolvePlayerManager();
    }

    private void OnEnable()
    {
        _gazeStartTime = -1f;
        _isGazing = false;
    }

    private void Update()
    {
        if (triggerOnce && HasTriggered)
        {
            return;
        }

        if (!TryGetIsLookingAtTarget(out bool isLookingAtTarget))
        {
            ResetGazeState();
            return;
        }

        if (!isLookingAtTarget)
        {
            ResetGazeState();
            return;
        }

        if (!_isGazing)
        {
            _isGazing = true;
            _gazeStartTime = Time.time;
        }

        if (Time.time - _gazeStartTime < gazeDuration)
        {
            return;
        }

        TriggerCallback();
    }

    public void ResetTriggerState()
    {
        HasTriggered = false;
        ResetGazeState();
    }

    public void TriggerCallback()
    {
        if (triggerOnce && HasTriggered)
        {
            return;
        }

        HasTriggered = true;
        ResetGazeState();
        onGazeStayComplete?.Invoke();

        if (debugLog)
        {
            Debug.Log($"TutorialTriggerBoxGazeCallback: gaze completed on {name}", this);
        }
    }

    private void ResetGazeState()
    {
        _isGazing = false;
        _gazeStartTime = -1f;
    }

    private bool TryGetIsLookingAtTarget(out bool isLookingAtTarget)
    {
        isLookingAtTarget = false;

        if (_targetCollider == null)
        {
            _targetCollider = GetComponent<Collider>();
            if (_targetCollider == null)
            {
                return false;
            }
        }

        if (!ResolvePlayerManager())
        {
            return false;
        }

        if (!_playerManager.TryGetCenterGazeState(out _, out bool hasHit, out RaycastHit hitInfo, out _))
        {
            return false;
        }

        if (!hasHit || hitInfo.collider == null)
        {
            return true;
        }

        Collider hitCollider = hitInfo.collider;
        isLookingAtTarget = hitCollider == _targetCollider || hitCollider.transform.IsChildOf(transform);
        return true;
    }

    private bool ResolvePlayerManager()
    {
        if (_playerManager != null)
        {
            return true;
        }

        if (Time.time < _nextResolvePlayerManagerTime)
        {
            return false;
        }

        _nextResolvePlayerManagerTime = Time.time + 1f;
        _playerManager = FindObjectOfType<PlayerSteamVRManager>();
        return _playerManager != null;
    }
}
