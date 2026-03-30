using UnityEngine;
using UnityEngine.EventSystems;
using Valve.VR;
using Valve.VR.Extras;
using Valve.VR.InteractionSystem;

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

    private Hand hand;

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
        if (e.target == null)
        {
            return;
        }

        currentOverGameObject = e.target.gameObject;
        ExecuteEvents.Execute(currentOverGameObject, CreateEventData(), ExecuteEvents.pointerEnterHandler);

        SetHoveredInteractable(e.target.GetComponentInParent<MyInteractableSteamVR>());
    }

    private void OnPointerOut(object sender, PointerEventArgs e)
    {
        if (currentOverGameObject != null)
        {
            ExecuteEvents.Execute(currentOverGameObject, CreateEventData(), ExecuteEvents.pointerExitHandler);
            currentOverGameObject = null;
        }

        if (e.target == null)
        {
            SetHoveredInteractable(null);
            return;
        }

        var interactable = e.target.GetComponentInParent<MyInteractableSteamVR>();
        if (interactable == lastHoveredInteractable)
        {
            SetHoveredInteractable(null);
        }
    }

    private void Update()
    {
        if (laserPointer == null)
        {
            return;
        }

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
        posOffset = handTransform.InverseTransformPoint(attachedObject.transform.position);
        rotOffset = Quaternion.Inverse(handTransform.rotation) * attachedObject.transform.rotation;

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
            lastHoveredInteractable.SendMessage("OnHandHoverEnd", hand, SendMessageOptions.DontRequireReceiver);
        }

        lastHoveredInteractable = interactable;

        if (lastHoveredInteractable != null)
        {
            lastHoveredInteractable.SendMessage("OnHandHoverBegin", hand, SendMessageOptions.DontRequireReceiver);
        }
    }

    private void OnPointerClick(object sender, PointerEventArgs e)
    {
        if (e.target == null)
        {
            return;
        }

        PointerEventData data = CreateEventData();
        ExecuteEvents.Execute(e.target.gameObject, data, ExecuteEvents.pointerDownHandler);
        ExecuteEvents.Execute(e.target.gameObject, data, ExecuteEvents.pointerClickHandler);
        StartCoroutine(ReleaseButton(e.target.gameObject, data));
    }

    private System.Collections.IEnumerator ReleaseButton(GameObject target, PointerEventData data)
    {
        yield return new WaitForSeconds(0.1f);
        ExecuteEvents.Execute(target, data, ExecuteEvents.pointerUpHandler);
    }
}