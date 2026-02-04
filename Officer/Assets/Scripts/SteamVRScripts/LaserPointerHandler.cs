using UnityEngine;
using UnityEngine.EventSystems;
using Valve.VR;
using Valve.VR.Extras;
using Valve.VR.InteractionSystem; // 必须引用交互系统

public class LaserPointerHandler : MonoBehaviour
{
    private SteamVR_LaserPointer laserPointer;

    // 声明缺失的变量
    [SerializeField] private GameObject currentOverGameObject;
    private Interactable lastHoveredInteractable; // 建议直接存储 Interactable 组件

    [Header("输入设置")]
    public SteamVR_Action_Boolean grabAction = SteamVR_Actions.default_GrabGrip;
    public SteamVR_Input_Sources currentHand;

    [Header("抓取设置")]
    public Transform handTransform;
    private GameObject attachedObject;
    private Vector3 posOffset;
    private Quaternion rotOffset;

    private Hand hand;

    private void Start()
    {
        hand = GetComponent<Hand>();
        laserPointer = GetComponent<SteamVR_LaserPointer>();
        if (laserPointer == null) laserPointer = GetComponentInChildren<SteamVR_LaserPointer>();

        if (laserPointer != null)
        {
            laserPointer.PointerClick += OnPointerClick;
            laserPointer.PointerIn += OnPointerIn;
            laserPointer.PointerOut += OnPointerOut;
        }
    }

    private void OnPointerIn(object sender, PointerEventArgs e)
    {
        currentOverGameObject = e.target.gameObject;

        // 触发 UI 进入事件
        ExecuteEvents.Execute(currentOverGameObject, CreateEventData(), ExecuteEvents.pointerEnterHandler);

        // 核心修复 1：使用 GetComponentInParent，因为射线可能射中子物体
        var interactable = e.target.GetComponentInParent<Interactable>();
        if (interactable != null)
        {
            lastHoveredInteractable = interactable;
            // 核心修复 2：模拟高亮开始
            // 注意：Interactable 的 OnHandHoverBegin 需要 Hand 参数
            // 如果没有 Hand，我们传递 null，但要确保脚本内有判空处理
            interactable.SendMessage("OnHandHoverBegin", hand, SendMessageOptions.DontRequireReceiver);
        }
    }

    private void OnPointerOut(object sender, PointerEventArgs e)
    {
        if (currentOverGameObject != null)
        {
            ExecuteEvents.Execute(currentOverGameObject, CreateEventData(), ExecuteEvents.pointerExitHandler);
            currentOverGameObject = null;
        }

        if (lastHoveredInteractable != null)
        {
            // 模拟高亮结束
            lastHoveredInteractable.SendMessage("OnHandHoverEnd", hand, SendMessageOptions.DontRequireReceiver);
            lastHoveredInteractable = null;
        }
    }

    private void Update()
    {
        if (laserPointer == null) return;


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

    // ... 其余的 OnPointerClick、TryGrabRemote、UpdateRemotePosition 等函数保持不变 ...
    // 但确保在 TryGrabRemote 中使用 lastHoveredInteractable.gameObject 进行抓取
    private void TryGrabRemote()
    {
        if (lastHoveredInteractable != null && attachedObject == null)
        {
            attachedObject = lastHoveredInteractable.gameObject;
            posOffset = handTransform.InverseTransformPoint(attachedObject.transform.position);
            rotOffset = Quaternion.Inverse(handTransform.rotation) * attachedObject.transform.rotation;

            Rigidbody rb = attachedObject.GetComponent<Rigidbody>();
            if (rb != null) { rb.isKinematic = true; rb.useGravity = false; }

            attachedObject.SendMessage("OnAttachedToHand", null, SendMessageOptions.DontRequireReceiver);
        }
    }

    private void UpdateRemotePosition()
    {
        attachedObject.transform.position = handTransform.TransformPoint(posOffset);
        attachedObject.transform.rotation = handTransform.rotation * rotOffset;
    }

    private void DetachRemote()
    {
        if (attachedObject != null)
        {
            Rigidbody rb = attachedObject.GetComponent<Rigidbody>();
            if (rb != null) { rb.isKinematic = false; rb.useGravity = true; }
            attachedObject.SendMessage("OnDetachedFromHand", null, SendMessageOptions.DontRequireReceiver);
            attachedObject = null;
        }
    }

    private PointerEventData CreateEventData()
    {
        PointerEventData data = new PointerEventData(EventSystem.current);
        data.button = PointerEventData.InputButton.Left;
        return data;
    }

    private void OnPointerClick(object sender, PointerEventArgs e)
    {
        if (e.target != null)
        {
            PointerEventData data = CreateEventData();
            ExecuteEvents.Execute(e.target.gameObject, data, ExecuteEvents.pointerDownHandler);
            ExecuteEvents.Execute(e.target.gameObject, data, ExecuteEvents.pointerClickHandler);
            StartCoroutine(ReleaseButton(e.target.gameObject, data));
        }
    }

    private System.Collections.IEnumerator ReleaseButton(GameObject target, PointerEventData data)
    {
        yield return new WaitForSeconds(0.1f);
        ExecuteEvents.Execute(target, data, ExecuteEvents.pointerUpHandler);
    }
}