using UnityEngine;
using UnityEngine.Events;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class MyInteractableSteamVR : Interactable
{
    [Header("悬停回调")]
    public UnityEvent onHoverBegin;
    [Tooltip("只触发一次的悬停开始回调")]
    public UnityEvent onHoverBeginOnce;
    public UnityEvent onHoverEnd;

    [Header("触发器回调")]
    public UnityEvent onTriggerDown;
    [Tooltip("只触发一次的触发器按下回调")]
    public UnityEvent onTriggerDownOnce;

    [Header("是否可移动")]
    public bool canBeMoved = true;

    private bool hasTriggeredHoverBeginOnce = false;
    private bool hasTriggeredTriggerDownOnce = false;
    private bool lastAttachedState = false;

    protected override void OnHandHoverBegin(Hand hand)
    {
        base.OnHandHoverBegin(hand);

        ItemData itemData = GetComponent<ItemData>();
        if (itemData != null) itemData.ShowUIDec(true);

        if (onHoverBegin != null) onHoverBegin.Invoke();

        if (!hasTriggeredHoverBeginOnce && onHoverBeginOnce != null)
        {
            onHoverBeginOnce.Invoke();
            hasTriggeredHoverBeginOnce = true;
        }
    }

    protected override void OnHandHoverEnd(Hand hand)
    {
        base.OnHandHoverEnd(hand);

        ItemData itemData = GetComponent<ItemData>();
        if (itemData != null) itemData.ShowUIDec(false);

        if (onHoverEnd != null) onHoverEnd.Invoke();
    }

    protected override void OnAttachedToHand(Hand hand)
    {
        base.OnAttachedToHand(hand);
        lastAttachedState = true;
        NotifySnackHint(true);
    }

    protected override void OnDetachedFromHand(Hand hand)
    {
        base.OnDetachedFromHand(hand);
        lastAttachedState = false;
        NotifySnackHint(false);
    }

    protected override void Update()
    {
        base.Update();
        SyncAttachedHintState();

        if (isHovering)
        {
            foreach (Hand hand in hoveringHands)
            {
                if (SteamVR_Actions.default_InteractUI.GetStateUp(hand.handType))
                {
                    OnTriggerPressed();
                }
            }
        }
    }

    private void SyncAttachedHintState()
    {
        bool isAttached = attachedToHand != null;
        if (isAttached == lastAttachedState)
        {
            return;
        }

        lastAttachedState = isAttached;
        NotifySnackHint(isAttached);
    }

    private void NotifySnackHint(bool visible)
    {
        if (CompareTag("Snack"))
        {
            EventManager.DispatchEvent(EventCommon.PLAYER_SNACK_HINT, visible);
        }
    }

    private void OnTriggerPressed()
    {
        if (onTriggerDown != null)
        {
            onTriggerDown.Invoke();
        }

        if (!hasTriggeredTriggerDownOnce && onTriggerDownOnce != null)
        {
            onTriggerDownOnce.Invoke();
            hasTriggeredTriggerDownOnce = true;
        }
    }

    public void ResetHoverBeginOnce()
    {
        hasTriggeredHoverBeginOnce = false;
    }

    public bool HasTriggeredHoverBeginOnce()
    {
        return hasTriggeredHoverBeginOnce;
    }

    public void ResetTriggerDownOnce()
    {
        hasTriggeredTriggerDownOnce = false;
    }

    public bool HasTriggeredTriggerDownOnce()
    {
        return hasTriggeredTriggerDownOnce;
    }
}
