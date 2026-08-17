using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(Collider))]
public class BedSleepTriggerBox : MonoBehaviour
{
    [SerializeField] private string requiredTag = "Player";
    [SerializeField] private bool requireNightStage = true;
    [SerializeField] private bool autoSleepOnEnter = false;
    [SerializeField] private bool enableKeyboardFallback = true;
    [SerializeField] private bool acceptGrabGripAsInteract = true;
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

        if (string.IsNullOrEmpty(requiredTag) || other.CompareTag(requiredTag))
        {
            return true;
        }

        if (Player.instance != null && other.transform.IsChildOf(Player.instance.transform))
        {
            return true;
        }

        if (PlayerSteamVRManager.Instance != null &&
            PlayerSteamVRManager.Instance.playerGO != null &&
            other.transform.IsChildOf(PlayerSteamVRManager.Instance.playerGO.transform))
        {
            return true;
        }

        return false;
    }

    private bool IsInteractPressed()
    {
        bool leftPressed = IsActionDown(SteamVR_Actions.default_InteractUI, SteamVR_Input_Sources.LeftHand);
        bool rightPressed = IsActionDown(SteamVR_Actions.default_InteractUI, SteamVR_Input_Sources.RightHand);

        if (acceptGrabGripAsInteract)
        {
            leftPressed |= IsActionDown(SteamVR_Actions.default_GrabGrip, SteamVR_Input_Sources.LeftHand);
            rightPressed |= IsActionDown(SteamVR_Actions.default_GrabGrip, SteamVR_Input_Sources.RightHand);
        }

        if (leftPressed || rightPressed)
        {
            return true;
        }

        return enableKeyboardFallback && Input.GetKeyDown(KeyCode.E);
    }

    private static bool IsActionDown(SteamVR_Action_Boolean action, SteamVR_Input_Sources source)
    {
        return action != null && action.GetStateDown(source);
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

        if (DayOneTutorialDirector.Instance != null &&
            DayOneTutorialDirector.Instance.IsRunning)
        {
            if (!DayOneTutorialDirector.Instance.CanSleep)
            {
                return;
            }

            DayOneTutorialDirector.Instance.NotifyBedInteracted();
        }

        if (GameManager.Instance.TrySleepToNextDay())
        {
            _nextInteractTime = Time.time + Mathf.Max(0f, interactCooldown);
        }
    }
}
