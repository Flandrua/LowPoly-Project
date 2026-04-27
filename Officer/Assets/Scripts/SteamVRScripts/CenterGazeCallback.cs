using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class CenterGazeCallback : MonoBehaviour
{
    [Header("Target")]
    public Transform targetRoot;
    public Collider[] targetColliders;
    public Renderer[] targetRenderers;
    public Animator animator;
    public string animatorBoolName;

    [Header("Gaze")]
    [Range(0f, 3f)]
    public float enterDelay = 0f;
    [Min(0f)]
    public float triggerDistance = 0f;
    public bool useRendererBoundsFallback = true;

    [Header("Callbacks")]
    public UnityEvent onGazeEnter;
    public UnityEvent onGazeExit;

    [Header("Debug")]
    public bool debugLog = false;

    private PlayerSteamVRManager playerManager;
    private bool isGazing;
    private float gazeStartTime = -1f;

    private void Awake()
    {
        if (targetRoot == null)
        {
            targetRoot = transform;
        }

        CacheTargets();
        playerManager = FindObjectOfType<PlayerSteamVRManager>();
        TryResolveAnimator();
    }

    private void OnEnable()
    {
        gazeStartTime = -1f;
        isGazing = false;
    }

    private void OnDisable()
    {
        if (isGazing)
        {
            InvokeExitCallback();
        }

        gazeStartTime = -1f;
        isGazing = false;
    }

    private void Update()
    {
        if (!TryGetCenterGazeState(out Ray gazeRay, out bool hasHit, out RaycastHit hitInfo))
        {
            return;
        }

        bool isLookingAtTarget = IsLookingAtTarget(gazeRay, hasHit, hitInfo);

        if (isLookingAtTarget)
        {
            if (isGazing)
            {
                return;
            }

            if (gazeStartTime < 0f)
            {
                gazeStartTime = Time.time;
            }

            if (Time.time - gazeStartTime >= enterDelay)
            {
                InvokeEnterCallback();
            }

            return;
        }

        gazeStartTime = -1f;

        if (isGazing)
        {
            InvokeExitCallback();
        }
    }

    public void RefreshTargets()
    {
        CacheTargets();
        TryResolveAnimator();
    }

    public void SetAnimBoolTrue(string boolName)
    {
        if (string.IsNullOrEmpty(boolName))
        {
            return;
        }

        if (!TryResolveAnimator())
        {
            return;
        }

        animator.SetBool(boolName, true);
    }

    public void SetAnimBoolFalse(string boolName)
    {
        if (string.IsNullOrEmpty(boolName))
        {
            return;
        }

        if (!TryResolveAnimator())
        {
            return;
        }

        animator.SetBool(boolName, false);
    }

    private void CacheTargets()
    {
        if (targetRoot == null)
        {
            targetRoot = transform;
        }

        if (targetColliders == null || targetColliders.Length == 0)
        {
            targetColliders = targetRoot.GetComponentsInChildren<Collider>(true);
        }

        if (targetRenderers == null || targetRenderers.Length == 0)
        {
            targetRenderers = targetRoot.GetComponentsInChildren<Renderer>(true);
        }
    }

    private bool TryResolveAnimator()
    {
        if (animator != null)
        {
            return true;
        }

        if (targetRoot != null)
        {
            animator = targetRoot.GetComponentInChildren<Animator>(true);
        }

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>(true);
        }

        return animator != null;
    }

    private bool TryGetCenterGazeState(out Ray gazeRay, out bool hasHit, out RaycastHit hitInfo)
    {
        gazeRay = default;
        hasHit = false;
        hitInfo = default;

        if (playerManager == null)
        {
            playerManager = FindObjectOfType<PlayerSteamVRManager>();
        }

        if (playerManager == null)
        {
            return false;
        }

        return playerManager.TryGetCenterGazeState(out gazeRay, out hasHit, out hitInfo, out _);
    }

    private bool IsLookingAtTarget(Ray gazeRay, bool hasHit, RaycastHit hitInfo)
    {
        bool hitTarget;

        if (TryHitTarget(hasHit, hitInfo))
        {
            hitTarget = true;
        }
        else
        {
            if (!useRendererBoundsFallback)
            {
                return false;
            }

            hitTarget = TryHitRendererBounds(gazeRay, hasHit, hitInfo);
        }

        if (!hitTarget)
        {
            return false;
        }

        return IsWithinTriggerDistance(gazeRay.origin);
    }

    private bool TryHitTarget(bool hasHit, RaycastHit hitInfo)
    {
        if (!hasHit || hitInfo.collider == null)
        {
            return false;
        }

        return IsTargetTransform(hitInfo.collider.transform);
    }

    private bool TryHitRendererBounds(Ray gazeRay, bool hasHit, RaycastHit hitInfo)
    {
        if (!TryGetTargetBounds(out Bounds targetBounds))
        {
            return false;
        }

        float boundsDistance;
        if (!targetBounds.IntersectRay(gazeRay, out boundsDistance))
        {
            return false;
        }

        if (playerManager == null)
        {
            playerManager = FindObjectOfType<PlayerSteamVRManager>();
        }

        float maxDistance = playerManager != null ? playerManager.GetCenterGazeMaxDistance() : 0f;
        if (maxDistance <= 0f || boundsDistance > maxDistance)
        {
            return false;
        }

        if (hasHit && hitInfo.distance < boundsDistance)
        {
            return IsTargetTransform(hitInfo.collider.transform);
        }

        return true;
    }

    private bool TryGetTargetBounds(out Bounds combinedBounds)
    {
        combinedBounds = default;
        bool hasBounds = false;

        if (targetColliders != null)
        {
            for (int index = 0; index < targetColliders.Length; index++)
            {
                Collider targetCollider = targetColliders[index];
                if (targetCollider == null || !targetCollider.enabled)
                {
                    continue;
                }

                if (!hasBounds)
                {
                    combinedBounds = targetCollider.bounds;
                    hasBounds = true;
                }
                else
                {
                    combinedBounds.Encapsulate(targetCollider.bounds);
                }
            }
        }

        if (!hasBounds && targetRenderers != null)
        {
            for (int index = 0; index < targetRenderers.Length; index++)
            {
                Renderer renderer = targetRenderers[index];
                if (renderer == null || !renderer.enabled)
                {
                    continue;
                }

                if (!hasBounds)
                {
                    combinedBounds = renderer.bounds;
                    hasBounds = true;
                }
                else
                {
                    combinedBounds.Encapsulate(renderer.bounds);
                }
            }
        }

        if (!hasBounds)
        {
            return false;
        }

        return true;
    }

    private bool IsWithinTriggerDistance(Vector3 playerPosition)
    {
        if (triggerDistance <= 0f)
        {
            return true;
        }

        Vector3 targetPosition = GetTargetReferencePosition();
        float distance = Vector3.Distance(playerPosition, targetPosition);
        return distance <= triggerDistance;
    }

    private Vector3 GetTargetReferencePosition()
    {
        if (TryGetTargetBounds(out Bounds targetBounds))
        {
            return targetBounds.center;
        }

        if (targetRoot != null)
        {
            return targetRoot.position;
        }

        return transform.position;
    }

    private bool IsTargetTransform(Transform hitTransform)
    {
        if (hitTransform == null)
        {
            return false;
        }

        Transform current = hitTransform;
        while (current != null)
        {
            if (current == targetRoot)
            {
                return true;
            }

            current = current.parent;
        }

        return false;
    }

    private void InvokeEnterCallback()
    {
        isGazing = true;
        SetAnimatorBool(true);

        if (debugLog)
        {
            Debug.Log($"CenterGazeCallback: 视线进入 {targetRoot.name}", this);
        }

        onGazeEnter?.Invoke();
    }

    private void InvokeExitCallback()
    {
        isGazing = false;
        SetAnimatorBool(false);

        if (debugLog)
        {
            Debug.Log($"CenterGazeCallback: 视线离开 {targetRoot.name}", this);
        }

        onGazeExit?.Invoke();
    }

    private void SetAnimatorBool(bool value)
    {
        if (string.IsNullOrEmpty(animatorBoolName))
        {
            return;
        }

        if (!TryResolveAnimator())
        {
            return;
        }

        animator.SetBool(animatorBoolName, value);
    }
}
