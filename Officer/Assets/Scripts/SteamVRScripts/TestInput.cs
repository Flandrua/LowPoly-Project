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

    [Header("UI")]
    public Toggle snapToggle;

    private bool canSnapTurn = true;

    private void Awake()
    {
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
        playerTransform.position += moveDirection * moveSpeed * Time.deltaTime;
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
}