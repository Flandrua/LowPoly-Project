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

    [Header("抓取回调")]
    public UnityEvent onObjectAttached;
    [Tooltip("只触发一次的抓取回调")]
    public UnityEvent onObjectAttachedOnce;

    [Header("是否可移动")]
    public bool canBeMoved = true;

    [Header("零食抓取吸附")]
    [Tooltip("开启后，Tag=Snack 的物体在抓起时会吸附到手部锚点。")]
    public bool snapSnackToHandOnAttach = true;
    [Tooltip("吸附时相对手部的本地位置偏移。")]
    public Vector3 snackLocalPositionOffset = Vector3.zero;
    [Tooltip("吸附时相对手部的本地旋转偏移（欧拉角）。")]
    public Vector3 snackLocalEulerOffset = Vector3.zero;

    private bool hasTriggeredHoverBeginOnce = false;
    private bool hasTriggeredTriggerDownOnce = false;
    private bool hasTriggeredAttachOnce = false;
    private bool lastAttachedState = false;
    private int lastTriggerDispatchFrame = -1;
    private bool isRayHovering = false;
    private Hand rayHoverHand;

    protected override void OnHandHoverBegin(Hand hand)
    {
        // Block direct proximity hover from hands; interaction is ray-only.
    }

    protected override void OnHandHoverEnd(Hand hand)
    {
        // Block direct proximity hover from hands; interaction is ray-only.
    }

    protected override void OnAttachedToHand(Hand hand)
    {
        base.OnAttachedToHand(hand);
        SnapSnackToHand(hand);
        if (onObjectAttached != null)
        {
            onObjectAttached.Invoke();
        }

        if (!hasTriggeredAttachOnce && onObjectAttachedOnce != null)
        {
            onObjectAttachedOnce.Invoke();
            hasTriggeredAttachOnce = true;
        }

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

        if (attachedToHand != null)
        {
            TryDispatchTriggerFromHand(attachedToHand);
        }

        if (isRayHovering && rayHoverHand != null)
        {
            TryDispatchTriggerFromHand(rayHoverHand);
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

    private void TryDispatchTriggerFromHand(Hand hand)
    {
        if (hand == null || Time.frameCount == lastTriggerDispatchFrame)
        {
            return;
        }

        if (!SteamVR_Actions.default_InteractUI.GetStateUp(hand.handType))
        {
            return;
        }

        lastTriggerDispatchFrame = Time.frameCount;
        OnTriggerPressed();
    }

    public void OnRayHoverBegin(Hand hand)
    {
        rayHoverHand = hand;
        if (isRayHovering)
        {
            return;
        }

        base.OnHandHoverBegin(hand);
        isRayHovering = true;
        HandleHoverBegin();
    }

    public void OnRayHoverEnd(Hand hand)
    {
        if (!isRayHovering)
        {
            return;
        }

        if (hand != null)
        {
            base.OnHandHoverEnd(hand);
        }
        isRayHovering = false;
        rayHoverHand = null;
        HandleHoverEnd();
    }

    private void HandleHoverBegin()
    {
        ItemData itemData = GetComponent<ItemData>();
        if (itemData != null)
        {
            itemData.ShowUIDec(true);
        }

        onHoverBegin?.Invoke();

        if (!hasTriggeredHoverBeginOnce && onHoverBeginOnce != null)
        {
            onHoverBeginOnce.Invoke();
            hasTriggeredHoverBeginOnce = true;
        }
    }

    private void HandleHoverEnd()
    {
        ItemData itemData = GetComponent<ItemData>();
        if (itemData != null)
        {
            itemData.ShowUIDec(false);
        }

        onHoverEnd?.Invoke();
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

    public void ResetAttachedOnce()
    {
        hasTriggeredAttachOnce = false;
    }

    public bool HasTriggeredAttachedOnce()
    {
        return hasTriggeredAttachOnce;
    }

    public bool ShouldSnapSnackToHand()
    {
        return snapSnackToHandOnAttach && CompareTag("Snack");
    }

    public Vector3 GetSnackLocalPositionOffset()
    {
        return snackLocalPositionOffset;
    }

    public Quaternion GetSnackLocalRotationOffset()
    {
        return Quaternion.Euler(snackLocalEulerOffset);
    }

    private void SnapSnackToHand(Hand hand)
    {
        if (!ShouldSnapSnackToHand() || hand == null)
        {
            return;
        }

        Transform handTransform = hand.transform;
        if (handTransform == null)
        {
            return;
        }

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        transform.position = handTransform.TransformPoint(GetSnackLocalPositionOffset());
        transform.rotation = handTransform.rotation * GetSnackLocalRotationOffset();
    }
}
