using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

/// <summary>
/// Gives the hand's collision sphere the same grab/trigger ability as the laser pointer, but
/// driven by physical overlap instead of a ray. It grabs the first still-overlapping interactable
/// (in entry order) on GrabGrip and releases it on GrabGrip up, and fires that object's Trigger on
/// InteractUI. It is intentionally independent of the laser: it never touches MyInteractableSteamVR's
/// ray-hover state, so the laser keeps working exactly as before.
/// Attach to the hand object that owns the trigger sphere (RightHand / LeftHand).
/// </summary>
public class HandGrabCollider : MonoBehaviour
{
    [Header("Input")]
    [Tooltip("Grip action used to grab/release, matching the laser pointer grab.")]
    public SteamVR_Action_Boolean grabAction = SteamVR_Actions.default_GrabGrip;

    [Header("References")]
    [Tooltip("SteamVR Hand. Auto-resolved from this object or its parents when left empty.")]
    [SerializeField] private Hand hand;
    [Tooltip("Transform the grabbed object follows. Defaults to the resolved hand transform.")]
    [SerializeField] private Transform handTransform;

    private readonly List<MyInteractableSteamVR> _candidates = new List<MyInteractableSteamVR>();
    // A just-released object sits snapped at the hand (still inside the sphere). Ignore it until it
    // physically leaves, so pressing GrabGrip again doesn't instantly re-grab the same object.
    private readonly HashSet<MyInteractableSteamVR> _ignoreUntilExit = new HashSet<MyInteractableSteamVR>();
    // Guide targets (keyboard / hamster) the sphere is physically touching. These have no
    // MyInteractableSteamVR; the laser fires their guide via a pointer click, the sphere fires it
    // directly via TryTriggerGuideIntro() on InteractUI, so the hand can also start the intro.
    private readonly List<MonoBehaviour> _guideCandidates = new List<MonoBehaviour>();
    private MyInteractableSteamVR _target;
    private GameObject _attachedObject;
    private Vector3 _posOffset;
    private Quaternion _rotOffset;

    private SteamVR_Input_Sources HandSource => hand != null ? hand.handType : SteamVR_Input_Sources.Any;

    private void Awake()
    {
        ResolveReferences();
    }

    private void ResolveReferences()
    {
        if (hand == null)
        {
            hand = GetComponentInParent<Hand>();
        }

        if (handTransform == null)
        {
            handTransform = hand != null ? hand.transform : transform;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == null)
        {
            return;
        }

        MyInteractableSteamVR interactable = ResolveInteractable(other);
        if (interactable != null && interactable.canBeMoved && !_candidates.Contains(interactable))
        {
            // Append so the earliest entry stays first ("grab the first prop that entered").
            _candidates.Add(interactable);
        }

        MonoBehaviour guide = ResolveGuide(other);
        if (guide != null && !_guideCandidates.Contains(guide))
        {
            _guideCandidates.Add(guide);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other == null)
        {
            return;
        }

        MyInteractableSteamVR interactable = ResolveInteractable(other);
        if (interactable != null)
        {
            // Keep a held object as a candidate so its Trigger keeps working while held.
            if (_attachedObject == null || interactable.gameObject != _attachedObject)
            {
                _candidates.Remove(interactable);
                // It left the sphere, so it may be grabbed again next time it enters.
                _ignoreUntilExit.Remove(interactable);
            }
        }

        MonoBehaviour guide = ResolveGuide(other);
        if (guide != null)
        {
            _guideCandidates.Remove(guide);
        }
    }

    private void Update()
    {
        if (!IsPlayerInteractionAllowed())
        {
            ClearState();
            return;
        }

        ResolveReferences();

        // Drop a held object that was destroyed or deactivated (e.g. a snack that got eaten).
        if (_attachedObject != null && !_attachedObject.activeInHierarchy)
        {
            _attachedObject = null;
        }

        PruneCandidates();
        UpdateTarget();

        if (grabAction != null)
        {
            if (grabAction.GetStateDown(HandSource))
            {
                TryGrab();
            }

            if (grabAction.GetStateUp(HandSource))
            {
                Detach();
            }
        }

        if (_attachedObject != null)
        {
            UpdateAttachedPosition();
        }

        DispatchTrigger();
        DispatchGuide();
    }

    private void PruneCandidates()
    {
        for (int i = _candidates.Count - 1; i >= 0; i--)
        {
            MyInteractableSteamVR candidate = _candidates[i];
            if (candidate == null || !candidate.gameObject.activeInHierarchy)
            {
                if (candidate != null)
                {
                    _ignoreUntilExit.Remove(candidate);
                }
                _candidates.RemoveAt(i);
            }
        }

        for (int i = _guideCandidates.Count - 1; i >= 0; i--)
        {
            MonoBehaviour guide = _guideCandidates[i];
            if (guide == null || !guide.gameObject.activeInHierarchy)
            {
                _guideCandidates.RemoveAt(i);
            }
        }
    }

    private void UpdateTarget()
    {
        if (_attachedObject != null)
        {
            _target = null;
            return;
        }

        _target = null;
        for (int i = 0; i < _candidates.Count; i++)
        {
            MyInteractableSteamVR candidate = _candidates[i];
            if (candidate != null && !_ignoreUntilExit.Contains(candidate))
            {
                _target = candidate;
                return;
            }
        }
    }

    private void TryGrab()
    {
        if (_target == null || _attachedObject != null || !_target.canBeMoved)
        {
            return;
        }

        _attachedObject = _target.gameObject;

        if (_target.ShouldSnapSnackToHand())
        {
            _posOffset = _target.GetSnackLocalPositionOffset();
            _rotOffset = _target.GetSnackLocalRotationOffset();
        }
        else
        {
            _posOffset = handTransform.InverseTransformPoint(_attachedObject.transform.position);
            _rotOffset = Quaternion.Inverse(handTransform.rotation) * _attachedObject.transform.rotation;
        }

        Rigidbody rb = _attachedObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        _attachedObject.SendMessage("OnAttachedToHand", hand, SendMessageOptions.DontRequireReceiver);
    }

    private void UpdateAttachedPosition()
    {
        _attachedObject.transform.position = handTransform.TransformPoint(_posOffset);
        _attachedObject.transform.rotation = handTransform.rotation * _rotOffset;
    }

    private void Detach()
    {
        if (_attachedObject == null)
        {
            return;
        }

        Rigidbody rb = _attachedObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        MyInteractableSteamVR released = _attachedObject.GetComponent<MyInteractableSteamVR>();
        _attachedObject.SendMessage("OnDetachedFromHand", hand, SendMessageOptions.DontRequireReceiver);
        _attachedObject = null;

        // Don't re-grab the just-released object until it actually leaves the sphere.
        if (released != null && _candidates.Contains(released))
        {
            _ignoreUntilExit.Add(released);
        }
    }

    private void DispatchTrigger()
    {
        if (hand == null)
        {
            return;
        }

        // Trigger the held object, or the current hover target if nothing is held.
        MyInteractableSteamVR triggerTarget = _attachedObject != null
            ? _attachedObject.GetComponent<MyInteractableSteamVR>()
            : _target;

        if (triggerTarget != null)
        {
            triggerTarget.DispatchTriggerFromExternalHand(hand);
        }
    }

    private void DispatchGuide()
    {
        if (_guideCandidates.Count == 0)
        {
            return;
        }

        // Same release-edge the laser uses for its guide click.
        if (!SteamVR_Actions.default_InteractUI.GetStateUp(HandSource))
        {
            return;
        }

        for (int i = 0; i < _guideCandidates.Count; i++)
        {
            MonoBehaviour guide = _guideCandidates[i];
            if (guide == null)
            {
                continue;
            }

            if (guide is KeyboardController keyboard)
            {
                keyboard.TryTriggerGuideIntro();
                return;
            }

            if (guide is HamsterController hamster)
            {
                hamster.TryTriggerGuideIntro();
                return;
            }
        }
    }

    private void ClearState()
    {
        Detach();
        _target = null;
        _guideCandidates.Clear();
    }

    private MyInteractableSteamVR ResolveInteractable(Collider other)
    {
        if (other == null)
        {
            return null;
        }

        return other.GetComponentInParent<MyInteractableSteamVR>();
    }

    private MonoBehaviour ResolveGuide(Collider other)
    {
        if (other == null)
        {
            return null;
        }

        KeyboardController keyboard = other.GetComponentInParent<KeyboardController>();
        if (keyboard != null)
        {
            return keyboard;
        }

        HamsterController hamster = other.GetComponentInParent<HamsterController>();
        if (hamster != null)
        {
            return hamster;
        }

        return null;
    }

    private bool IsPlayerInteractionAllowed()
    {
        return GameManager.Instance == null || GameManager.Instance.IsPlayerInteractionEnabled;
    }
}
