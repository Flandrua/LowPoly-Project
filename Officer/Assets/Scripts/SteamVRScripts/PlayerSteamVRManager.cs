using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using Valve.VR.InteractionSystem;

public class PlayerSteamVRManager : MonoSingleton<PlayerSteamVRManager>
{
    public GameObject playerGO;
    public GameObject HeightGO;
    [Header("Center Gaze")]
    [SerializeField] private Transform centerGazeHeadTransform;
    [SerializeField] private LayerMask centerGazeLayers = Physics.DefaultRaycastLayers;
    [SerializeField] [Range(0.01f, 100f)] private float centerGazeMaxDistance = 20f;
    [SerializeField] [Range(0f, 1f)] private float centerGazeRayRadius = 0f;
    public float height = 0.5f;
    private ParticleSystem _flame;
    public int tempEfficiency = 0;
    private AudioSource _as;
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    [SerializeField] private Color snackHintColor = new Color(0.68f, 0.87f, 1f, 0.1f);
    [SerializeField] private Material snackHintMaterialTemplate;
    private MeshRenderer _snackHintRenderer;
    private Material _snackHintMaterial;
    private Ray _centerGazeRay;
    private RaycastHit _centerGazeHit;
    private bool _hasCenterGazeHit;
    private int _centerGazeFrame = -1;
    private float _nextResolveCenterGazeHeadTime;

    void Start()
    {
        _flame = GetComponentInChildren<ParticleSystem>();
        _as = GetComponent<AudioSource>();
        initialPosition = playerGO.transform.position;
        initialRotation = playerGO.transform.rotation;
        EventManager.AddListener<SnackData>(EventCommon.PLAYER_FINISH_EATING, PlayerFinishEating);
        EventManager.AddListener<bool>(EventCommon.PLAYER_SNACK_HINT, SetSnackHintVisible);
        InitSnackHintZone();
    }

    private void OnDestroy()
    {
        EventManager.RemoveListener<SnackData>(EventCommon.PLAYER_FINISH_EATING, PlayerFinishEating);
        EventManager.RemoveListener<bool>(EventCommon.PLAYER_SNACK_HINT, SetSnackHintVisible);
        if (_snackHintMaterial != null)
        {
            Destroy(_snackHintMaterial);
        }
    }

    public void ResetLocation()
    {
        DataCenter.Instance.GetWorkEfficiency(-tempEfficiency);
        _flame.Stop();
        tempEfficiency = 0;
        playerGO.transform.position = initialPosition;
        playerGO.transform.rotation = initialRotation;
        SetSnackHintVisible(false);
    }

    public void OnHeightUp()
    {
        Vector3 pos = HeightGO.transform.position;
        height += 0.1f;
        pos.y = height;
        HeightGO.transform.position = pos;
        UIManager.Instance.UpdateHeight(height);
    }

    public void OnHeightDown()
    {
        Vector3 pos = HeightGO.transform.position;
        height -= 0.1f;
        pos.y = height;
        HeightGO.transform.position = pos;
        UIManager.Instance.UpdateHeight(height);
    }

    private void InitSnackHintZone()
    {
        Transform headCollider = FindHeadColliderTransform();
        if (headCollider == null)
        {
            return;
        }

        SphereCollider sphereCollider = headCollider.GetComponent<SphereCollider>();
        float radius = sphereCollider != null ? sphereCollider.radius : 0.12f;

        Transform visual = headCollider.Find("HeadColliderVisual");
        if (visual == null)
        {
            GameObject visualGo = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            visualGo.name = "HeadColliderVisual";
            visualGo.transform.SetParent(headCollider, false);
            visual = visualGo.transform;
        }

        visual.localPosition = new Vector3(0f, 0f, radius * 2f);
        visual.localRotation = Quaternion.identity;
        visual.localScale = Vector3.one * radius * 2f;

        Collider visualCollider = visual.GetComponent<Collider>();
        if (visualCollider != null)
        {
            visualCollider.enabled = false;
        }

        MeshFilter meshFilter = visual.GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            meshFilter = visual.gameObject.AddComponent<MeshFilter>();
        }

        if (meshFilter.sharedMesh == null)
        {
            GameObject tempSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            meshFilter.sharedMesh = tempSphere.GetComponent<MeshFilter>().sharedMesh;
            Destroy(tempSphere);
        }

        _snackHintRenderer = visual.GetComponent<MeshRenderer>();
        if (_snackHintRenderer == null)
        {
            _snackHintRenderer = visual.gameObject.AddComponent<MeshRenderer>();
        }

        Material baseMaterial = _snackHintRenderer.sharedMaterial;
        _snackHintMaterial = CreateSnackHintMaterial(baseMaterial);
        if (_snackHintMaterial == null)
        {
            return;
        }

        _snackHintMaterial.name = "SnackHintRuntimeMaterial";
        ApplySnackHintStyle(_snackHintMaterial);
        _snackHintRenderer.material = _snackHintMaterial;
        _snackHintRenderer.shadowCastingMode = ShadowCastingMode.Off;
        _snackHintRenderer.receiveShadows = false;
        SetSnackHintVisible(false);
    }

    private Transform FindHeadColliderTransform()
    {
        if (playerGO == null)
        {
            return null;
        }

        Transform[] transforms = playerGO.transform.root.GetComponentsInChildren<Transform>(true);
        foreach (Transform child in transforms)
        {
            if (child.name == "HeadCollider")
            {
                return child;
            }
        }

        return null;
    }

    private Material CreateSnackHintMaterial(Material baseMaterial)
    {
        if (snackHintMaterialTemplate != null)
        {
            return new Material(snackHintMaterialTemplate);
        }

        Shader shader = FindPreferredSnackHintShader();
        if (shader != null)
        {
            return new Material(shader);
        }

        if (baseMaterial != null)
        {
            return new Material(baseMaterial);
        }

        return null;
    }

    private Shader FindPreferredSnackHintShader()
    {
        RenderPipelineAsset currentPipeline = GraphicsSettings.currentRenderPipeline;
        bool isUrp = currentPipeline != null
            && currentPipeline.GetType().Name.Contains("UniversalRenderPipelineAsset");
        string[] shaderNames = isUrp
            ? new[]
            {
                "Universal Render Pipeline/Unlit",
                "Unlit/Transparent",
                "Legacy Shaders/Transparent/Diffuse",
                "Standard"
            }
            : new[]
            {
                "Legacy Shaders/Transparent/Diffuse",
                "Unlit/Transparent",
                "Standard"
            };

        for (int i = 0; i < shaderNames.Length; i++)
        {
            Shader shader = Shader.Find(shaderNames[i]);
            if (shader != null)
            {
                return shader;
            }
        }

        return null;
    }

    private void ApplySnackHintStyle(Material material)
    {
        if (material == null)
        {
            return;
        }

        if (material.HasProperty("_Color"))
        {
            material.color = snackHintColor;
        }

        if (material.HasProperty("_BaseColor"))
        {
            material.SetColor("_BaseColor", snackHintColor);
        }

        if (material.HasProperty("_Mode"))
        {
            material.SetFloat("_Mode", 3f);
        }

        if (material.HasProperty("_Surface"))
        {
            material.SetFloat("_Surface", 1f);
        }

        if (material.HasProperty("_Blend"))
        {
            material.SetFloat("_Blend", 0f);
        }

        if (material.HasProperty("_AlphaClip"))
        {
            material.SetFloat("_AlphaClip", 0f);
        }

        if (material.HasProperty("_SrcBlend"))
        {
            material.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
        }

        if (material.HasProperty("_DstBlend"))
        {
            material.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
        }

        if (material.HasProperty("_ZWrite"))
        {
            material.SetInt("_ZWrite", 0);
        }

        material.DisableKeyword("_ALPHATEST_ON");
        material.EnableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.SetOverrideTag("RenderType", "Transparent");
        material.renderQueue = (int)RenderQueue.Transparent;
    }

    public void SetSnackHintVisible(bool visible)
    {
        if (_snackHintRenderer == null)
        {
            return;
        }

        _snackHintRenderer.enabled = visible;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Snack"))
        {
            if (SnackManager.Instance != null && !SnackManager.Instance.CanPlayerEatSnack())
            {
                return;
            }

            _as.Play();
            EventManager.DispatchEvent(EventCommon.PLAYER_EATING, true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Snack"))
        {
            EventManager.DispatchEvent(EventCommon.PLAYER_EATING, false);
        }
    }

    private void PlayerFinishEating(SnackData snack)
    {
        tempEfficiency = snack.workEfficiency;
        DataCenter.Instance.GetWorkEfficiency(snack.workEfficiency);
        SetSnackHintVisible(false);
        if (snack.isSpicy)
        {
            _flame.Play();
        }
    }

    public bool TryGetCenterGazeState(out Ray gazeRay, out bool hasHit, out RaycastHit hitInfo, out Transform headTransform)
    {
        UpdateCenterGazeStateIfNeeded();

        gazeRay = _centerGazeRay;
        hasHit = _hasCenterGazeHit;
        hitInfo = _centerGazeHit;
        headTransform = centerGazeHeadTransform;
        return centerGazeHeadTransform != null;
    }

    public float GetCenterGazeMaxDistance()
    {
        return centerGazeMaxDistance;
    }

    private void UpdateCenterGazeStateIfNeeded()
    {
        if (_centerGazeFrame == Time.frameCount)
        {
            return;
        }

        _centerGazeFrame = Time.frameCount;
        _hasCenterGazeHit = false;
        _centerGazeHit = default;

        if (!TryResolveCenterGazeHeadTransform())
        {
            _centerGazeRay = default;
            return;
        }

        _centerGazeRay = new Ray(centerGazeHeadTransform.position, centerGazeHeadTransform.forward);

        if (centerGazeRayRadius > 0f)
        {
            _hasCenterGazeHit = Physics.SphereCast(_centerGazeRay, centerGazeRayRadius, out _centerGazeHit, centerGazeMaxDistance, centerGazeLayers, QueryTriggerInteraction.Collide);
            return;
        }

        _hasCenterGazeHit = Physics.Raycast(_centerGazeRay, out _centerGazeHit, centerGazeMaxDistance, centerGazeLayers, QueryTriggerInteraction.Collide);
    }

    private bool TryResolveCenterGazeHeadTransform()
    {
        if (centerGazeHeadTransform != null)
        {
            return true;
        }

        if (Time.time < _nextResolveCenterGazeHeadTime)
        {
            return false;
        }

        _nextResolveCenterGazeHeadTime = Time.time + 1f;

        if (Player.instance != null && Player.instance.hmdTransform != null)
        {
            centerGazeHeadTransform = Player.instance.hmdTransform;
            return true;
        }

        if (Camera.main != null)
        {
            centerGazeHeadTransform = Camera.main.transform;
            return true;
        }

        return false;
    }
}


