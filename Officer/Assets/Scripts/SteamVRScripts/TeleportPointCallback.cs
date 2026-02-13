using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using Valve.VR.InteractionSystem;

public class MonoTemp:MonoBehaviour
{
    
}
/// <summary>
/// TeleportPoint回调组件
/// 当玩家传送到该TeleportPoint后，触发回调并支持延时触发
/// 即使GameObject被禁用也能正常工作
/// </summary>
[RequireComponent(typeof(TeleportPoint))]
public class TeleportPointCallback : MonoBehaviour
{
    [Header("回调设置")]
    [Tooltip("传送完成后的回调事件")]
    public UnityEvent onTeleportComplete;

    [Tooltip("触发回调的延时时间（秒）")]
    [Range(0f, 10f)]
    public float callbackDelay = 0f;

    [Tooltip("是否只触发一次回调（一次性）")]
    public bool triggerOnce = false;

    [Header("调试")]
    [Tooltip("是否在控制台输出调试信息")]
    public bool debugLog = false;

    private TeleportPoint teleportPoint;
    private Coroutine delayedCallbackCoroutine;
    private bool hasTriggered = false; // 标记是否已经触发过回调
    private MonoBehaviour coroutineRunner; // 用于在GameObject被禁用时运行协程

    private void Awake()
    {
        teleportPoint = GetComponent<TeleportPoint>();
        if (teleportPoint == null)
        {
            Debug.LogError("TeleportPointCallback: 未找到TeleportPoint组件！", this);
            enabled = false;
            return;
        }

        // 在Awake中订阅事件，确保即使GameObject被禁用也能接收事件
        Teleport.Player.AddListener(OnPlayerTeleported);

        // 创建一个独立的GameObject来运行协程，避免GameObject被禁用时协程停止
        GameObject coroutineRunnerObj = new GameObject("TeleportPointCallback_Runner_" + gameObject.name);
        coroutineRunnerObj.hideFlags = HideFlags.HideAndDontSave;
        DontDestroyOnLoad(coroutineRunnerObj);
        coroutineRunner = coroutineRunnerObj.AddComponent<MonoTemp>();
    }

    private void Start()
    {

    }

    private void OnDestroy()
    {
        // 取消订阅传送事件
        Teleport.Player.RemoveListener(OnPlayerTeleported);

        // 停止协程并清理协程运行器
        if (delayedCallbackCoroutine != null && coroutineRunner != null)
        {
            coroutineRunner.StopCoroutine(delayedCallbackCoroutine);
        }

        // 清理协程运行器
        if (coroutineRunner != null && coroutineRunner.gameObject != null)
        {
            Destroy(coroutineRunner.gameObject);
        }
    }

    /// <summary>
    /// 当玩家传送时调用
    /// </summary>
    /// <param name="teleportedMarker">传送到的标记点</param>
    private void OnPlayerTeleported(TeleportMarkerBase teleportedMarker)
    {
        // 检查是否是传送到当前TeleportPoint
        if (teleportedMarker == teleportPoint)
        {
            // 如果是一次性回调且已经触发过，则不再触发
            if (triggerOnce && hasTriggered)
            {
                if (debugLog)
                {
                    Debug.Log($"TeleportPointCallback: 回调已触发过（一次性模式），跳过", this);
                }
                return;
            }

            if (debugLog)
            {
                Debug.Log($"TeleportPointCallback: 玩家已传送到 {gameObject.name}", this);
            }

            // 停止之前的协程（如果有）
            if (delayedCallbackCoroutine != null && coroutineRunner != null)
            {
                coroutineRunner.StopCoroutine(delayedCallbackCoroutine);
            }

            // 启动延时回调协程（使用独立的协程运行器）
            if (coroutineRunner != null)
            {
                delayedCallbackCoroutine = coroutineRunner.StartCoroutine(DelayedCallbackCoroutine());
            }
            else
            {
                // 备用方案：如果协程运行器不存在，使用当前对象
                delayedCallbackCoroutine = StartCoroutine(DelayedCallbackCoroutine());
            }
        }
    }

    /// <summary>
    /// 延时回调协程
    /// </summary>
    private IEnumerator DelayedCallbackCoroutine()
    {
        // 等待指定的延时时间
        if (callbackDelay > 0f)
        {
            if (debugLog)
            {
                Debug.Log($"TeleportPointCallback: 等待 {callbackDelay} 秒后触发回调", this);
            }
            yield return new WaitForSeconds(callbackDelay);
        }

        // 触发回调
        if (debugLog)
        {
            Debug.Log($"TeleportPointCallback: 触发回调事件", this);
        }

        onTeleportComplete?.Invoke();

        // 标记已触发
        hasTriggered = true;

        delayedCallbackCoroutine = null;
        TimeManager.Instance.AddTask(0.5f, false, () => { this.gameObject.SetActive(false); }, this);
    }

    /// <summary>
    /// 设置回调延时时间（运行时调用）
    /// </summary>
    /// <param name="delay">延时时间（秒）</param>
    public void SetCallbackDelay(float delay)
    {
        callbackDelay = Mathf.Max(0f, delay);
    }

    /// <summary>
    /// 手动触发回调（不检查传送状态）
    /// </summary>
    public void TriggerCallback()
    {
        // 如果是一次性回调且已经触发过，则不再触发
        if (triggerOnce && hasTriggered)
        {
            if (debugLog)
            {
                Debug.Log($"TeleportPointCallback: 回调已触发过（一次性模式），跳过", this);
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

    /// <summary>
    /// 立即触发回调（忽略延时）
    /// </summary>
    public void TriggerCallbackImmediate()
    {
        // 如果是一次性回调且已经触发过，则不再触发
        if (triggerOnce && hasTriggered)
        {
            if (debugLog)
            {
                Debug.Log($"TeleportPointCallback: 回调已触发过（一次性模式），跳过", this);
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
            Debug.Log($"TeleportPointCallback: 立即触发回调事件", this);
        }

        onTeleportComplete?.Invoke();
        hasTriggered = true;
    }

    /// <summary>
    /// 重置回调状态（允许再次触发一次性回调）
    /// </summary>
    public void ResetCallback()
    {
        hasTriggered = false;
        if (debugLog)
        {
            Debug.Log($"TeleportPointCallback: 已重置回调状态，可以再次触发", this);
        }
    }

    /// <summary>
    /// 设置是否只触发一次
    /// </summary>
    /// <param name="once">是否只触发一次</param>
    public void SetTriggerOnce(bool once)
    {
        triggerOnce = once;
    }

    /// <summary>
    /// 检查回调是否已经触发过
    /// </summary>
    /// <returns>是否已触发</returns>
    public bool HasTriggered()
    {
        return hasTriggered;
    }
}