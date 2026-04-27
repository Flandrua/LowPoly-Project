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
    [SerializeField] private UnityEvent onTriggerEntered;
    [SerializeField] private TriggerColliderEvent onTriggerEnteredWithCollider;

    public bool HasTriggered { get; private set; }
    public Collider LastEnteredCollider { get; private set; }

    private void Reset()
    {
        Collider triggerCollider = GetComponent<Collider>();
        if (triggerCollider != null)
        {
            triggerCollider.isTrigger = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggerOnce && HasTriggered)
        {
            return;
        }

        if (!string.IsNullOrEmpty(requiredTag) && !other.CompareTag(requiredTag))
        {
            return;
        }

        LastEnteredCollider = other;
        HasTriggered = true;
        onTriggerEntered?.Invoke();
        onTriggerEnteredWithCollider?.Invoke(other);
    }

    public void ResetTriggerState()
    {
        HasTriggered = false;
        LastEnteredCollider = null;
    }

    public void TriggerCallback()
    {
        if (triggerOnce && HasTriggered)
        {
            return;
        }

        HasTriggered = true;
        onTriggerEntered?.Invoke();

        if (LastEnteredCollider != null)
        {
            onTriggerEnteredWithCollider?.Invoke(LastEnteredCollider);
        }
    }
}
