using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class TriggerColliderEvent : UnityEvent<Collider>
{
}

[RequireComponent(typeof(Collider))]
public class StartTriggerBoxCallback : MonoBehaviour
{
    [SerializeField] private string requiredTag = "Player";
    [SerializeField] private bool triggerOnce = true;
    [SerializeField] private bool invokePersistentEvents = true;
    [SerializeField] private UnityEvent onTriggerEntered;
    [SerializeField] private TriggerColliderEvent onTriggerEnteredWithCollider;

    public event Action Entered;

    public bool HasTriggered { get; private set; }
    public Collider LastEnteredCollider { get; private set; }

    private Collider _triggerCollider;

    private void Reset()
    {
        Collider triggerCollider = GetComponent<Collider>();
        if (triggerCollider != null)
        {
            triggerCollider.isTrigger = true;
        }
    }

    private void Awake()
    {
        _triggerCollider = GetComponent<Collider>();
        if (_triggerCollider != null)
        {
            _triggerCollider.isTrigger = true;
        }
    }

    private void OnEnable()
    {
        TryTriggerOverlappingTargets();
    }

    private void OnTriggerEnter(Collider other)
    {
        TryHandleEnter(other);
    }

    public void SetPersistentEventsEnabled(bool enabled)
    {
        invokePersistentEvents = enabled;
    }

    public void ResetTriggerState()
    {
        HasTriggered = false;
        LastEnteredCollider = null;
    }

    public void DetectOverlappingTargets()
    {
        TryTriggerOverlappingTargets();
    }

    public void TriggerCallback()
    {
        if (triggerOnce && HasTriggered)
        {
            return;
        }

        HasTriggered = true;
        Entered?.Invoke();
        if (invokePersistentEvents)
        {
            onTriggerEntered?.Invoke();
            if (LastEnteredCollider != null)
            {
                onTriggerEnteredWithCollider?.Invoke(LastEnteredCollider);
            }
        }
    }

    private void TryTriggerOverlappingTargets()
    {
        if (_triggerCollider == null)
        {
            _triggerCollider = GetComponent<Collider>();
        }

        if (_triggerCollider == null || !_triggerCollider.enabled)
        {
            return;
        }

        BoxCollider boxCollider = _triggerCollider as BoxCollider;
        Collider[] hits;
        if (boxCollider != null)
        {
            Vector3 worldCenter = transform.TransformPoint(boxCollider.center);
            Vector3 worldHalfExtents = Vector3.Scale(boxCollider.size * 0.5f, transform.lossyScale);
            hits = Physics.OverlapBox(worldCenter, worldHalfExtents, transform.rotation);
        }
        else
        {
            hits = Physics.OverlapSphere(transform.position, 0.5f);
        }

        for (int i = 0; i < hits.Length; i++)
        {
            TryHandleEnter(hits[i]);
            if (HasTriggered && triggerOnce)
            {
                return;
            }
        }
    }

    private void TryHandleEnter(Collider other)
    {
        if (other == null)
        {
            return;
        }

        if (triggerOnce && HasTriggered)
        {
            return;
        }

        if (!IsRequiredTarget(other))
        {
            return;
        }

        LastEnteredCollider = other;
        HasTriggered = true;
        Entered?.Invoke();
        if (invokePersistentEvents)
        {
            onTriggerEntered?.Invoke();
            onTriggerEnteredWithCollider?.Invoke(other);
        }
    }

    private bool IsRequiredTarget(Collider other)
    {
        if (string.IsNullOrEmpty(requiredTag) || other.CompareTag(requiredTag))
        {
            return true;
        }

        return other.transform.root.CompareTag(requiredTag);
    }
}
