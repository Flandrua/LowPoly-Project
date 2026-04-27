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

    [Tooltip("触发回调的延迟时间（秒）")]
    [Range(0f, 10f)]
    public float callbackDelay = 0f;

    [Tooltip("是否只触发一次回调（一次性）")]
    public bool triggerOnce = false;

    [Header("调试")]
    [Tooltip("是否在控制台输出调试信息")]
    public bool debugLog = false;

    private TeleportArea teleportArea;
    private Coroutine delayedCallbackCoroutine;
    private bool hasTriggered;
    private MonoBehaviour coroutineRunner;

    private void Awake()
    {
        teleportArea = GetComponent<TeleportArea>();
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

        if (coroutineRunner != null && coroutineRunner.gameObject != null)
        {
            Destroy(coroutineRunner.gameObject);
        }
    }

    private void OnPlayerTeleported(TeleportMarkerBase teleportedMarker)
    {
        if (teleportedMarker != teleportArea)
        {
            return;
        }

        if (debugLog)
        {
            Debug.Log($"TeleportAreaCallback: 玩家已传送到 {gameObject.name}", this);
        }

        TriggerCallback();
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

    public void ResetCallback()
    {
        hasTriggered = false;

        if (debugLog)
        {
            Debug.Log("TeleportAreaCallback: 已重置回调状态，可以再次触发", this);
        }
    }

    public void SetTriggerOnce(bool once)
    {
        triggerOnce = once;
    }

    public bool HasTriggered()
    {
        return hasTriggered;
    }
}
