using UnityEngine;
using Valve.VR;

[RequireComponent(typeof(Collider))]
public class BedSleepTriggerBox : MonoBehaviour
{
    [SerializeField] private string requiredTag = "Player";
    [SerializeField] private bool requireNightStage = true;
    [SerializeField] private bool autoSleepOnEnter = false;
    [SerializeField] private bool enableKeyboardFallback = true;
    [SerializeField] private float interactCooldown = 0.25f;

    private int _insideCount;
    private float _nextInteractTime;

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
        if (!IsRequiredTarget(other))
        {
            return;
        }

        _insideCount++;
        if (autoSleepOnEnter)
        {
            TrySleep();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsRequiredTarget(other))
        {
            return;
        }

        _insideCount = Mathf.Max(0, _insideCount - 1);
    }

    private void Update()
    {
        if (_insideCount <= 0 || autoSleepOnEnter)
        {
            return;
        }

        if (Time.time < _nextInteractTime)
        {
            return;
        }

        if (!IsInteractPressed())
        {
            return;
        }

        TrySleep();
    }

    private bool IsRequiredTarget(Collider other)
    {
        if (other == null)
        {
            return false;
        }

        return string.IsNullOrEmpty(requiredTag) || other.CompareTag(requiredTag);
    }

    private bool IsInteractPressed()
    {
        bool leftPressed = SteamVR_Actions.default_InteractUI.GetStateDown(SteamVR_Input_Sources.LeftHand);
        bool rightPressed = SteamVR_Actions.default_InteractUI.GetStateDown(SteamVR_Input_Sources.RightHand);

        if (leftPressed || rightPressed)
        {
            return true;
        }

        return enableKeyboardFallback && Input.GetKeyDown(KeyCode.E);
    }

    private void TrySleep()
    {
        if (GameManager.Instance == null)
        {
            return;
        }

        if (requireNightStage && !GameManager.Instance.IsNightStage)
        {
            return;
        }

        if (GameManager.Instance.TrySleepToNextDay())
        {
            _nextInteractTime = Time.time + Mathf.Max(0f, interactCooldown);
        }
    }
}
