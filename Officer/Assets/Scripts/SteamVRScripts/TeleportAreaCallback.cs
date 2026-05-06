using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR.InteractionSystem;

public class TeleportAreaCallbackRunner : MonoBehaviour
{
}

[RequireComponent(typeof(TeleportArea))]
public class TeleportAreaCallback : MonoBehaviour
{
    [Header("回调设置")]
    [Tooltip("传送完成后触发的回调事件")]
    public UnityEvent onTeleportComplete;

    [Tooltip("玩家离开当前传送区域后触发的回调事件")]
    public UnityEvent onPlayerExitArea;

    [Tooltip("触发回调的延迟时间（秒）")]
    [Range(0f, 10f)]
    public float callbackDelay = 0f;

    [Tooltip("是否只触发一次回调（一次性）")]
    public bool triggerOnce = false;

    [Tooltip("玩家离开当前区域后，自动锁定当前 TeleportArea")]
    public bool lockAreaOnExit = false;

    [Header("调试")]
    [Tooltip("是否在控制台输出调试信息")]
    public bool debugLog = false;

    private TeleportArea teleportArea;
    private Collider areaCollider;
    private Coroutine delayedCallbackCoroutine;
    private Coroutine delayedExitCallbackCoroutine;
    private bool hasTriggered;
    private bool hasExitTriggered;
    private bool isPlayerInsideArea;
    private MonoBehaviour coroutineRunner;
    private const float ExitCheckTolerance = 0.01f;

    private void Awake()
    {
        teleportArea = GetComponent<TeleportArea>();
        areaCollider = GetComponent<Collider>();
        if (teleportArea == null)
        {
            Debug.LogError("TeleportAreaCallback: 未找到 TeleportArea 组件！", this);
            enabled = false;
            return;
        }

        Teleport.Player.AddListener(OnPlayerTeleported);

        GameObject coroutineRunnerObject = new GameObject("TeleportAreaCallback_Runner_" + gameObject.name);
        coroutineRunnerObject.hideFlags = HideFlags.HideAndDontSave;
        DontDestroyOnLoad(coroutineRunnerObject);
        coroutineRunner = coroutineRunnerObject.AddComponent<TeleportAreaCallbackRunner>();
    }

    private void OnDestroy()
    {
        Teleport.Player.RemoveListener(OnPlayerTeleported);

        if (delayedCallbackCoroutine != null && coroutineRunner != null)
        {
            coroutineRunner.StopCoroutine(delayedCallbackCoroutine);
        }

        if (delayedExitCallbackCoroutine != null && coroutineRunner != null)
        {
            coroutineRunner.StopCoroutine(delayedExitCallbackCoroutine);
        }

        if (coroutineRunner != null && coroutineRunner.gameObject != null)
        {
            Destroy(coroutineRunner.gameObject);
        }
    }

    private void Update()
    {
        if (!isPlayerInsideArea)
        {
            return;
        }

        if (!TryGetPlayerAreaCheckPosition(out Vector3 playerPosition))
        {
            return;
        }

        if (!IsInsideArea(playerPosition))
        {
            HandlePlayerExitedArea();
        }
    }

    private void OnPlayerTeleported(TeleportMarkerBase teleportedMarker)
    {
        if (teleportedMarker == teleportArea)
        {
            isPlayerInsideArea = true;
            hasExitTriggered = false;

            if (debugLog)
            {
                Debug.Log($"TeleportAreaCallback: 玩家已传送到 {gameObject.name}", this);
            }

            TriggerCallback();
            return;
        }

        if (isPlayerInsideArea)
        {
            HandlePlayerExitedArea();
        }
    }

    private IEnumerator DelayedCallbackCoroutine()
    {
        if (callbackDelay > 0f)
        {
            if (debugLog)
            {
                Debug.Log($"TeleportAreaCallback: 等待 {callbackDelay} 秒后触发回调", this);
            }

            yield return new WaitForSeconds(callbackDelay);
        }

        if (debugLog)
        {
            Debug.Log("TeleportAreaCallback: 触发回调事件", this);
        }

        onTeleportComplete?.Invoke();
        hasTriggered = true;
        delayedCallbackCoroutine = null;
    }

    private IEnumerator DelayedExitCallbackCoroutine()
    {
        if (callbackDelay > 0f)
        {
            if (debugLog)
            {
                Debug.Log($"TeleportAreaCallback: 等待 {callbackDelay} 秒后触发离开区域回调", this);
            }

            yield return new WaitForSeconds(callbackDelay);
        }

        if (debugLog)
        {
            Debug.Log("TeleportAreaCallback: 触发离开区域回调事件", this);
        }

        if (lockAreaOnExit)
        {
            LockCurrentTeleportArea();
        }

        onPlayerExitArea?.Invoke();
        hasExitTriggered = true;
        delayedExitCallbackCoroutine = null;
    }

    public void SetCallbackDelay(float delay)
    {
        callbackDelay = Mathf.Max(0f, delay);
    }

    public void TriggerCallback()
    {
        if (triggerOnce && hasTriggered)
        {
            if (debugLog)
            {
                Debug.Log("TeleportAreaCallback: 回调已触发过（一次性模式），跳过", this);
            }

            return;
        }

        if (delayedCallbackCoroutine != null && coroutineRunner != null)
        {
            coroutineRunner.StopCoroutine(delayedCallbackCoroutine);
        }

        if (coroutineRunner != null)
        {
            delayedCallbackCoroutine = coroutineRunner.StartCoroutine(DelayedCallbackCoroutine());
        }
        else
        {
            delayedCallbackCoroutine = StartCoroutine(DelayedCallbackCoroutine());
        }
    }

    public void TriggerCallbackImmediate()
    {
        if (triggerOnce && hasTriggered)
        {
            if (debugLog)
            {
                Debug.Log("TeleportAreaCallback: 回调已触发过（一次性模式），跳过", this);
            }

            return;
        }

        if (delayedCallbackCoroutine != null && coroutineRunner != null)
        {
            coroutineRunner.StopCoroutine(delayedCallbackCoroutine);
            delayedCallbackCoroutine = null;
        }

        if (debugLog)
        {
            Debug.Log("TeleportAreaCallback: 立即触发回调事件", this);
        }

        onTeleportComplete?.Invoke();
        hasTriggered = true;
    }

    public void TriggerExitCallback()
    {
        if (triggerOnce && hasExitTriggered)
        {
            if (debugLog)
            {
                Debug.Log("TeleportAreaCallback: 离开区域回调已触发过（一次性模式），跳过", this);
            }

            return;
        }

        if (delayedExitCallbackCoroutine != null && coroutineRunner != null)
        {
            coroutineRunner.StopCoroutine(delayedExitCallbackCoroutine);
        }

        if (coroutineRunner != null)
        {
            delayedExitCallbackCoroutine = coroutineRunner.StartCoroutine(DelayedExitCallbackCoroutine());
        }
        else
        {
            delayedExitCallbackCoroutine = StartCoroutine(DelayedExitCallbackCoroutine());
        }
    }

    public void TriggerExitCallbackImmediate()
    {
        if (triggerOnce && hasExitTriggered)
        {
            if (debugLog)
            {
                Debug.Log("TeleportAreaCallback: 离开区域回调已触发过（一次性模式），跳过", this);
            }

            return;
        }

        if (delayedExitCallbackCoroutine != null && coroutineRunner != null)
        {
            coroutineRunner.StopCoroutine(delayedExitCallbackCoroutine);
            delayedExitCallbackCoroutine = null;
        }

        if (debugLog)
        {
            Debug.Log("TeleportAreaCallback: 立即触发离开区域回调事件", this);
        }

        if (lockAreaOnExit)
        {
            LockCurrentTeleportArea();
        }

        onPlayerExitArea?.Invoke();
        hasExitTriggered = true;
    }

    public void ResetCallback()
    {
        hasTriggered = false;
        hasExitTriggered = false;
        isPlayerInsideArea = false;

        if (debugLog)
        {
            Debug.Log("TeleportAreaCallback: 已重置回调状态，可以再次触发", this);
        }
    }

    public void SetTriggerOnce(bool once)
    {
        triggerOnce = once;
    }

    public void SetLockAreaOnExit(bool shouldLock)
    {
        lockAreaOnExit = shouldLock;
    }

    public bool HasTriggered()
    {
        return hasTriggered;
    }

    public bool HasExitTriggered()
    {
        return hasExitTriggered;
    }

    private void HandlePlayerExitedArea()
    {
        isPlayerInsideArea = false;

        if (debugLog)
        {
            Debug.Log($"TeleportAreaCallback: 玩家已离开 {gameObject.name}", this);
        }

        TriggerExitCallback();
    }

    private bool TryGetPlayerAreaCheckPosition(out Vector3 playerPosition)
    {
        if (Player.instance != null)
        {
            playerPosition = Player.instance.feetPositionGuess;
            return true;
        }

        if (Camera.main != null)
        {
            playerPosition = Camera.main.transform.position;
            return true;
        }

        playerPosition = Vector3.zero;
        return false;
    }

    private bool IsInsideArea(Vector3 worldPosition)
    {
        if (CanUseClosestPoint(areaCollider))
        {
            Vector3 closestPoint = areaCollider.ClosestPoint(worldPosition);
            return (closestPoint - worldPosition).sqrMagnitude <= ExitCheckTolerance * ExitCheckTolerance;
        }

        Vector3 localPosition = transform.InverseTransformPoint(worldPosition);
        Bounds localBounds = teleportArea.meshBounds;
        localBounds.Expand(ExitCheckTolerance * 2f);
        return localBounds.Contains(localPosition);
    }

    private bool CanUseClosestPoint(Collider targetCollider)
    {
        if (targetCollider == null || !targetCollider.enabled)
        {
            return false;
        }

        if (targetCollider is BoxCollider || targetCollider is SphereCollider || targetCollider is CapsuleCollider)
        {
            return true;
        }

        MeshCollider meshCollider = targetCollider as MeshCollider;
        return meshCollider != null && meshCollider.convex;
    }

    public void LockCurrentTeleportArea()
    {
        if (teleportArea == null)
        {
            return;
        }

        teleportArea.SetLocked(true);

        if (debugLog)
        {
            Debug.Log($"TeleportAreaCallback: 已锁定 {gameObject.name} 的 TeleportArea", this);
        }
    }

    public void UnlockCurrentTeleportArea()
    {
        if (teleportArea == null)
        {
            return;
        }

        teleportArea.SetLocked(false);

        if (debugLog)
        {
            Debug.Log($"TeleportAreaCallback: 已解锁 {gameObject.name} 的 TeleportArea", this);
        }
    }
}
