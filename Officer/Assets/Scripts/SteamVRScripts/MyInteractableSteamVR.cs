using UnityEngine;
using UnityEngine.Events;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class MyInteractableSteamVR : Interactable
{
    [Header("??????????")]
    public UnityEvent onHoverBegin;
    [Tooltip("????????????")]
    public UnityEvent onHoverBeginOnce;
    public UnityEvent onHoverEnd;

    [Header("???????????")]
    public UnityEvent onTriggerDown; // ????????????????
    [Tooltip("只触发一次的触发器按下回调")]
    public UnityEvent onTriggerDownOnce;

    [Header("???????")]
    public bool canBeMoved = true; // ? false???????????

    private bool hasTriggeredHoverBeginOnce = false; // ????????????
    private bool hasTriggeredTriggerDownOnce = false; // 标记是否已触发一次性触发器回调

    protected override void OnHandHoverBegin(Hand hand)
    {
        base.OnHandHoverBegin(hand); // ????????????????????????????

        ItemData itemData = GetComponent<ItemData>();
        if (itemData != null) itemData.ShowUIDec(true);

        // ??????
        if (onHoverBegin != null) onHoverBegin.Invoke();

        // ????????????????
        if (!hasTriggeredHoverBeginOnce && onHoverBeginOnce != null)
        {
            onHoverBeginOnce.Invoke();
            hasTriggeredHoverBeginOnce = true;
        }
    }

    protected override void OnHandHoverEnd(Hand hand)
    {
        base.OnHandHoverEnd(hand); // ????????????????????

        ItemData itemData = GetComponent<ItemData>();
        if (itemData != null) itemData.ShowUIDec(false);

        if (onHoverEnd != null) onHoverEnd.Invoke();
    }

    // ??? Update ?????????
    protected override void Update()
    {
        base.Update(); // ?????????? Update ???????????????

        // ???????????????????
        if (isHovering)
        {
            // ???????????????????
            foreach (Hand hand in hoveringHands)
            {
                // ?????????????????? (GrabPinch)
                // SteamVR Hand ???????????????????????
                if (SteamVR_Actions.default_InteractUI.GetStateUp(hand.handType))
                {
                    OnTriggerPressed();
                }
            }
        }
    }

    private void OnTriggerPressed()
    {
        Debug.Log(gameObject.name + " ???????????????????");

        // 触发常规回调
        if (onTriggerDown != null)
        {
            onTriggerDown.Invoke();
        }

        // 触发一次性回调（如果还未触发过）
        if (!hasTriggeredTriggerDownOnce && onTriggerDownOnce != null)
        {
            onTriggerDownOnce.Invoke();
            hasTriggeredTriggerDownOnce = true;
        }
    }

    /// <summary>
    /// ???????????????? onHoverBeginOnce?
    /// </summary>
    public void ResetHoverBeginOnce()
    {
        hasTriggeredHoverBeginOnce = false;
    }

    /// <summary>
    /// ????????????
    /// </summary>
    /// <returns>?????</returns>
    public bool HasTriggeredHoverBeginOnce()
    {
        return hasTriggeredHoverBeginOnce;
    }

    /// <summary>
    /// 重置一次性触发器回调状态（允许再次触发 onTriggerDownOnce）
    /// </summary>
    public void ResetTriggerDownOnce()
    {
        hasTriggeredTriggerDownOnce = false;
    }

    /// <summary>
    /// 检查一次性触发器回调是否已触发
    /// </summary>
    /// <returns>是否已触发</returns>
    public bool HasTriggeredTriggerDownOnce()
    {
        return hasTriggeredTriggerDownOnce;
    }
}