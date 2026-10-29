using UnityEngine;
using UnityEngine.Animations;

/// <summary>
/// 限制 CCTV 在 Y 轴上的旋转范围。
/// 挂载在需要限制的 CCTV 子对象（带 LookAtConstraint 的 pivot）上。
/// 默认接管 LookAt 跟踪并在 Y 轴上钳制，确保限制一定生效；
/// 也可关闭接管，仅做 LateUpdate 钳制（保留原有 LookAtConstraint）。
/// </summary>
public class CCTVYRotationLimiter : MonoBehaviour
{
    [Tooltip("Y 轴最小旋转角（局部欧拉角，度）")]
    public float minY = -120f;

    [Tooltip("Y 轴最大旋转角（局部欧拉角，度）")]
    public float maxY = 120f;

    [Tooltip("是否接管 LookAt 跟踪（禁用 LookAtConstraint，自行旋转并钳制 Y）")]
    public bool overrideLookAt = true;

    [Tooltip("跟踪目标（留空则从 LookAtConstraint 或场景中 FollowHead 自动获取）")]
    public Transform target;

    private LookAtConstraint _constraint;

    private void Awake()
    {
        _constraint = GetComponent<LookAtConstraint>();

        if (target == null && _constraint != null && _constraint.sourceCount > 0)
        {
            target = _constraint.GetSource(0).sourceTransform;
        }

        if (target == null)
        {
            GameObject go = GameObject.Find("FollowHead");
            if (go != null) target = go.transform;
        }

        if (overrideLookAt && _constraint != null)
        {
            _constraint.constraintActive = false;
            _constraint.enabled = false;
        }
    }

    private void LateUpdate()
    {
        if (!overrideLookAt)
        {
            // 仅钳制：LookAtConstraint 已先更新变换
            Vector3 e = transform.localEulerAngles;
            float y = e.y > 180f ? e.y - 360f : e.y;
            y = Mathf.Clamp(y, Mathf.Min(minY, maxY), Mathf.Max(minY, maxY));
            e.y = y;
            transform.localEulerAngles = e;
            return;
        }

        if (target == null) return;

        Vector3 toTarget = target.position - transform.position;
        if (toTarget.sqrMagnitude < 0.0001f) return;

        Quaternion desiredWorld = Quaternion.LookRotation(toTarget, Vector3.up);
        Quaternion parentRot = transform.parent != null ? transform.parent.rotation : Quaternion.identity;
        Quaternion desiredLocal = Quaternion.Inverse(parentRot) * desiredWorld;

        Vector3 euler = desiredLocal.eulerAngles;
        float yy = euler.y > 180f ? euler.y - 360f : euler.y;
        yy = Mathf.Clamp(yy, Mathf.Min(minY, maxY), Mathf.Max(minY, maxY));
        euler.y = yy;

        transform.localRotation = Quaternion.Euler(euler.x, euler.y, euler.z);
    }
}
