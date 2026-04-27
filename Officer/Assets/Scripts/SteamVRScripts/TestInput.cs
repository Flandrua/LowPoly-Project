using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

public class VRInputController : MonoBehaviour
{
    [Header("Actions")]
    public SteamVR_Action_Vector2 joystickAction;
    public SteamVR_Action_Vector2 joystickRotate;

    [Header("Settings")]
    public float moveSpeed = 3.0f;
    public float rotateSpeed = 100.0f;
    public float snapTurnAngle = 45.0f;
    public bool useSnapTurning;

    [Header("References")]
    public Transform playerTransform;
    public Transform headTransform;
    public CharacterController characterController;

    [Header("Collision")]
    public float colliderRadius = 0.25f;
    public float minColliderHeight = 1.0f;

    [Header("UI")]
    public Toggle snapToggle;

    private bool canSnapTurn = true;

    private void Awake()
    {
        if (characterController == null && playerTransform != null)
        {
            characterController = playerTransform.GetComponent<CharacterController>();
            if (characterController == null)
            {
                characterController = playerTransform.gameObject.AddComponent<CharacterController>();
                characterController.skinWidth = 0.03f;
                characterController.stepOffset = 0.2f;
                characterController.slopeLimit = 45f;
                characterController.minMoveDistance = 0f;
            }
        }

        if (snapToggle == null)
        {
            snapToggle = FindSnapToggle();
        }
    }

    private void OnEnable()
    {
        if (snapToggle != null)
        {
            snapToggle.onValueChanged.AddListener(OnSnapToggleChanged);
            ApplySnapTurning(snapToggle.isOn);
        }
    }

    private void OnDisable()
    {
        if (snapToggle != null)
        {
            snapToggle.onValueChanged.RemoveListener(OnSnapToggleChanged);
        }
    }

    private void Update()
    {
        UpdateCharacterController();
        HandleLeftHandMovement();
        HandleRightHandRotation();
    }

    private void HandleLeftHandMovement()
    {
        Vector2 axis = joystickAction.GetAxis(SteamVR_Input_Sources.LeftHand);
        if (axis.sqrMagnitude <= 0.05f)
        {
            return;
        }

        Vector3 forward = headTransform.forward;
        forward.y = 0;
        forward.Normalize();

        Vector3 right = headTransform.right;
        right.y = 0;
        right.Normalize();

        Vector3 moveDirection = forward * axis.y + right * axis.x;
        Vector3 displacement = moveDirection * moveSpeed * Time.deltaTime;

        if (characterController != null && characterController.enabled)
        {
            characterController.Move(displacement);
            return;
        }

        playerTransform.position += displacement;
    }

    private void HandleRightHandRotation()
    {
        Vector2 axis = joystickRotate.GetAxis(SteamVR_Input_Sources.RightHand);

        if (useSnapTurning)
        {
            HandleSnapTurning(axis);
            return;
        }

        HandleSmoothTurning(axis);
    }

    private void HandleSmoothTurning(Vector2 axis)
    {
        if (Mathf.Abs(axis.x) <= 0.3f)
        {
            return;
        }

        float angle = axis.x * rotateSpeed * Time.deltaTime;
        Vector3 pivot = new Vector3(
            headTransform.position.x,
            playerTransform.position.y,
            headTransform.position.z
        );

        playerTransform.RotateAround(pivot, Vector3.up, angle);
    }

    private void HandleSnapTurning(Vector2 axis)
    {
        if (Mathf.Abs(axis.x) > 0.7f && canSnapTurn)
        {
            float angle = axis.x > 0 ? snapTurnAngle : -snapTurnAngle;
            playerTransform.Rotate(0, angle, 0);
            canSnapTurn = false;
        }

        if (Mathf.Abs(axis.x) < 0.2f)
        {
            canSnapTurn = true;
        }
    }

    public void OnSnapToggleChanged(bool isOn)
    {
        ApplySnapTurning(isOn);
    }

    private void ApplySnapTurning(bool isOn)
    {
        useSnapTurning = isOn;
        canSnapTurn = false;
    }

    private Toggle FindSnapToggle()
    {
        Toggle[] toggles = FindObjectsOfType<Toggle>(true);
        foreach (Toggle toggle in toggles)
        {
            string toggleName = toggle.name.ToLowerInvariant();
            if (toggleName.Contains("snap toggle") || toggleName.Contains("snaptoggle") || toggleName.Contains("snap"))
            {
                return toggle;
            }
        }

        return null;
    }

    private void UpdateCharacterController()
    {
        if (characterController == null || headTransform == null || playerTransform == null)
        {
            return;
        }

        Vector3 localHeadPosition = playerTransform.InverseTransformPoint(headTransform.position);
        float controllerHeight = Mathf.Max(minColliderHeight, localHeadPosition.y);
        float controllerRadius = Mathf.Min(colliderRadius, controllerHeight * 0.5f);

        if (!characterController.enabled)
        {
            characterController.enabled = true;
        }

        characterController.height = controllerHeight;
        characterController.radius = controllerRadius;
        characterController.center = new Vector3(localHeadPosition.x, controllerHeight * 0.5f, localHeadPosition.z);
    }
}
