using UnityEngine;
using UnityEngine.EventSystems;
using Valve.VR;
using Valve.VR.Extras;
using Valve.VR.InteractionSystem;
using System;

public class LaserPointerHandler : MonoBehaviour
{
    private SteamVR_LaserPointer laserPointer;

    [SerializeField] private GameObject currentOverGameObject;
    private MyInteractableSteamVR lastHoveredInteractable;

    [Header("Input Settings")]
    public SteamVR_Action_Boolean grabAction = SteamVR_Actions.default_GrabGrip;
    public SteamVR_Input_Sources currentHand;

    [Header("Grab Settings")]
    public Transform handTransform;
    private GameObject attachedObject;
    private Vector3 posOffset;
    private Quaternion rotOffset;

    [Header("Raycast Filter")]
    [SerializeField] private string rayInteractableTag = SnackManager.RayInteractableTag;
    [SerializeField] private string snackRootTag = "Snack";
    [SerializeField] private float maxRayDistance = 100f;
    [SerializeField] private LayerMask raycastLayers = Physics.DefaultRaycastLayers;
    [SerializeField] private QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Collide;

    private Hand hand;
    private Transform currentFilteredHitTarget;
    private float currentFilteredHitDistance = 100f;

    public void SetMaxRayDistance(float distance)
    {
        maxRayDistance = Mathf.Max(0.05f, distance);
        if (currentFilteredHitTarget == null)
        {
            currentFilteredHitDistance = maxRayDistance;
        }
    }

    public float GetMaxRayDistance()
    {
        return maxRayDistance;
    }

    private void Start()
    {
        hand = GetComponent<Hand>();
        laserPointer = GetComponent<SteamVR_LaserPointer>();
        if (laserPointer == null)
        {
            laserPointer = GetComponentInChildren<SteamVR_LaserPointer>();
        }

        if (laserPointer != null)
        {
            // Use custom RaycastAll filtering so non-target-tag objects do not block interaction.
            laserPointer.PointerClick += OnPointerClick;
            laserPointer.PointerIn += OnPointerIn;
            laserPointer.PointerOut += OnPointerOut;
        }
    }

    private void OnDestroy()
    {
        if (laserPointer == null)
        {
            return;
        }

        laserPointer.PointerClick -= OnPointerClick;
        laserPointer.PointerIn -= OnPointerIn;
        laserPointer.PointerOut -= OnPointerOut;
    }

    private void OnPointerIn(object sender, PointerEventArgs e)
    {
        // Intentionally handled in UpdateFilteredRayTarget.
    }

    private void OnPointerOut(object sender, PointerEventArgs e)
    {
        // Intentionally handled in UpdateFilteredRayTarget.
    }

    private void Update()
    {
        if (laserPointer == null)
        {
            return;
        }

        UpdateFilteredRayTarget();
        HandleFilteredClick();

        if (grabAction.GetStateDown(currentHand))
        {
            TryGrabRemote();
        }

        if (grabAction.GetStateUp(currentHand))
        {
            DetachRemote();
        }

        if (attachedObject != null)
        {
            UpdateRemotePosition();
        }
    }

    private void LateUpdate()
    {
        UpdateLaserVisualDistance();
    }

    private void TryGrabRemote()
    {
        if (lastHoveredInteractable == null || attachedObject != null)
        {
            return;
        }

        if (!lastHoveredInteractable.canBeMoved)
        {
            return;
        }

        attachedObject = lastHoveredInteractable.gameObject;
        if (lastHoveredInteractable.ShouldSnapSnackToHand())
        {
            posOffset = lastHoveredInteractable.GetSnackLocalPositionOffset();
            rotOffset = lastHoveredInteractable.GetSnackLocalRotationOffset();
        }
        else
        {
            posOffset = handTransform.InverseTransformPoint(attachedObject.transform.position);
            rotOffset = Quaternion.Inverse(handTransform.rotation) * attachedObject.transform.rotation;
        }

        Rigidbody rb = attachedObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        attachedObject.SendMessage("OnAttachedToHand", hand, SendMessageOptions.DontRequireReceiver);
    }

    private void UpdateRemotePosition()
    {
        attachedObject.transform.position = handTransform.TransformPoint(posOffset);
        attachedObject.transform.rotation = handTransform.rotation * rotOffset;
    }

    private void DetachRemote()
    {
        if (attachedObject == null)
        {
            return;
        }

        Rigidbody rb = attachedObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        attachedObject.SendMessage("OnDetachedFromHand", hand, SendMessageOptions.DontRequireReceiver);
        attachedObject = null;
    }

    private PointerEventData CreateEventData()
    {
        PointerEventData data = new PointerEventData(EventSystem.current);
        data.button = PointerEventData.InputButton.Left;
        return data;
    }

    private void SetHoveredInteractable(MyInteractableSteamVR interactable)
    {
        if (lastHoveredInteractable == interactable)
        {
            return;
        }

        if (lastHoveredInteractable != null)
        {
            lastHoveredInteractable.OnRayHoverEnd(hand);
        }

        lastHoveredInteractable = interactable;

        if (lastHoveredInteractable != null)
        {
            lastHoveredInteractable.OnRayHoverBegin(hand);
        }
    }

    private void OnPointerClick(object sender, PointerEventArgs e)
    {
        // Intentionally handled by HandleFilteredClick so click also passes through non-tag objects.
    }

    private System.Collections.IEnumerator ReleaseButton(GameObject target, PointerEventData data)
    {
        yield return new WaitForSeconds(0.1f);
        ExecuteEvents.Execute(target, data, ExecuteEvents.pointerUpHandler);
    }

    private bool TryGetRayInteractableTarget(Transform target, out GameObject rayTarget)
    {
        rayTarget = null;
        if (target == null)
        {
            return false;
        }

        Transform current = target;
        while (current != null)
        {
            bool isRayInteractable = !string.IsNullOrEmpty(rayInteractableTag) && current.tag == rayInteractableTag;
            bool isSnackRootFromInspector = !string.IsNullOrEmpty(snackRootTag) && current.tag == snackRootTag;
            bool isSnackRoot = current.tag == "Snack" || current.tag == "Snacks";
            if (isRayInteractable || isSnackRootFromInspector || isSnackRoot)
            {
                rayTarget = current.gameObject;
                return true;
            }

            current = current.parent;
        }

        return false;
    }

    private void UpdateFilteredRayTarget()
    {
        Transform nextHit = FindFirstAllowedHit(out float nextHitDistance);
        currentFilteredHitDistance = nextHit == null ? maxRayDistance : nextHitDistance;
        if (nextHit == currentFilteredHitTarget)
        {
            return;
        }

        if (currentOverGameObject != null)
        {
            ExecuteEvents.Execute(currentOverGameObject, CreateEventData(), ExecuteEvents.pointerExitHandler);
            currentOverGameObject = null;
        }

        currentFilteredHitTarget = nextHit;
        if (currentFilteredHitTarget == null)
        {
            SetHoveredInteractable(null);
            return;
        }

        currentOverGameObject = currentFilteredHitTarget.gameObject;
        ExecuteEvents.Execute(currentOverGameObject, CreateEventData(), ExecuteEvents.pointerEnterHandler);
        SetHoveredInteractable(currentFilteredHitTarget.GetComponentInParent<MyInteractableSteamVR>());

        // Only clear the outline for the specific object the ray actually hit, not every spawn hint.
        if (SnackManager.Instance != null && TryGetRayInteractableTarget(currentFilteredHitTarget, out GameObject rayRoot))
        {
            SnackManager.Instance.HideSpawnOutlineForRayTarget(rayRoot);
        }
    }

    private Transform FindFirstAllowedHit(out float hitDistance)
    {
        hitDistance = maxRayDistance;
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit[] hits = Physics.RaycastAll(ray, maxRayDistance, raycastLayers, queryTriggerInteraction);
        if (hits == null || hits.Length == 0)
        {
            return null;
        }

        Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
        for (int i = 0; i < hits.Length; i++)
        {
            Transform hitTarget = hits[i].transform;
            if (hitTarget == null)
            {
                continue;
            }

            if (TryGetRayInteractableTarget(hitTarget, out _))
            {
                hitDistance = hits[i].distance;
                return hitTarget;
            }
        }

        return null;
    }

    private void HandleFilteredClick()
    {
        if (currentOverGameObject == null || laserPointer == null || laserPointer.interactWithUI == null || laserPointer.pose == null)
        {
            return;
        }

        if (!laserPointer.interactWithUI.GetStateUp(laserPointer.pose.inputSource))
        {
            return;
        }

        PointerEventData data = CreateEventData();
        ExecuteEvents.Execute(currentOverGameObject, data, ExecuteEvents.pointerDownHandler);
        ExecuteEvents.Execute(currentOverGameObject, data, ExecuteEvents.pointerClickHandler);
        StartCoroutine(ReleaseButton(currentOverGameObject, data));
    }

    private void UpdateLaserVisualDistance()
    {
        if (laserPointer == null || laserPointer.pointer == null)
        {
            return;
        }

        float dist = currentFilteredHitTarget != null ? currentFilteredHitDistance : maxRayDistance;
        if (dist <= 0f)
        {
            dist = maxRayDistance;
        }

        Vector3 localScale = laserPointer.pointer.transform.localScale;
        localScale.z = dist;
        laserPointer.pointer.transform.localScale = localScale;

        Vector3 localPos = laserPointer.pointer.transform.localPosition;
        localPos.z = dist * 0.5f;
        laserPointer.pointer.transform.localPosition = localPos;
    }
}
