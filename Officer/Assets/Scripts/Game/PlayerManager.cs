using System.Collections;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR.Interaction.Toolkit;

//启用就改成MonoSingleton<PlayerManager>
public class PlayerManager : MonoBehaviour
{
    public GameObject xr;
    public int tempEfficiency = 0;
    private CharacterController characterController;
    private XROrigin xrOrign;
    private CustomCharacterControllerDriver ccd;
    public GameObject cameraOffset;
    public Vector3 initCameraOffset;
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private ParticleSystem _flame;
    private AudioSource _as;
    [SerializeField] private Color snackHintColor = new Color(0.68f, 0.87f, 1f, 0.1f);
    [SerializeField] private float snackHintRadius = 0.12f;
    [SerializeField] private Material snackHintMaterialTemplate;
    private GameObject _snackHintVisual;
    private MeshRenderer _snackHintRenderer;
    private Material _snackHintMaterial;

    private void Awake()
    {
        DataCenter.Instance.InitData();
    }

    void Start()
    {
        DataCenter.Instance.NewData();
        EventManager.AddListener<SnackData>(EventCommon.PLAYER_FINISH_EATING, PlayerFinishEating);
        EventManager.AddListener<bool>(EventCommon.PLAYER_SNACK_HINT, SetSnackHintVisible);
        characterController = xr.GetComponent<CharacterController>();
        xrOrign = xr.GetComponent<XROrigin>();
        ccd = xr.GetComponent<CustomCharacterControllerDriver>();
        initialPosition = xr.transform.position;
        initialRotation = xr.transform.rotation;
        _flame = GetComponentInChildren<ParticleSystem>();
        _as = GetComponent<AudioSource>();
        cameraOffset.transform.localPosition = initCameraOffset;
        InitSnackHintZone();
    }

    private void OnDestroy()
    {
        EventManager.RemoveListener<SnackData>(EventCommon.PLAYER_FINISH_EATING, PlayerFinishEating);
        EventManager.RemoveListener<bool>(EventCommon.PLAYER_SNACK_HINT, SetSnackHintVisible);
        if (_snackHintVisual != null)
        {
            Destroy(_snackHintVisual);
        }
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
        xr.transform.position = initialPosition;
        xr.transform.rotation = initialRotation;
    }

    void Update()
    {
    }

    private void InitSnackHintZone()
    {
        MeshRenderer baseRenderer = GetComponent<MeshRenderer>();
        Material baseMaterial = baseRenderer != null ? baseRenderer.sharedMaterial : null;
        _snackHintMaterial = CreateSnackHintMaterial(baseMaterial);
        if (_snackHintMaterial == null)
        {
            return;
        }

        if (baseRenderer != null)
        {
            baseRenderer.enabled = false;
        }

        Transform headRoot = transform.parent != null ? transform.parent : transform;
        _snackHintVisual = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        _snackHintVisual.name = "HeadCollider";
        _snackHintVisual.transform.SetParent(headRoot, false);
        _snackHintVisual.transform.localPosition = transform.localPosition;
        _snackHintVisual.transform.localRotation = Quaternion.identity;
        _snackHintVisual.transform.localScale = Vector3.one * snackHintRadius * 2f;

        Collider visualCollider = _snackHintVisual.GetComponent<Collider>();
        if (visualCollider != null)
        {
            visualCollider.enabled = false;
        }

        _snackHintRenderer = _snackHintVisual.GetComponent<MeshRenderer>();
        if (_snackHintRenderer == null)
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

        if (material.HasProperty("_BaseColor"))
        {
            material.SetColor("_BaseColor", snackHintColor);
        }

        if (material.HasProperty("_Color"))
        {
            material.color = snackHintColor;
        }

        if (material.HasProperty("_SrcBlend"))
        {
            material.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
        }

        if (material.HasProperty("_DstBlend"))
        {
            material.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
        }

        if (material.HasProperty("_Blend"))
        {
            material.SetFloat("_Blend", 0f);
        }

        if (material.HasProperty("_AlphaClip"))
        {
            material.SetFloat("_AlphaClip", 0f);
        }

        if (material.HasProperty("_ZWrite"))
        {
            material.SetInt("_ZWrite", 0);
        }

        if (material.HasProperty("_Cull"))
        {
            material.SetInt("_Cull", (int)CullMode.Off);
        }

        if (material.HasProperty("_EmissionColor"))
        {
            material.EnableKeyword("_EMISSION");
            material.SetColor("_EmissionColor", snackHintColor * 0.35f);
        }

        material.DisableKeyword("_ALPHATEST_ON");
        material.EnableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.SetOverrideTag("RenderType", "Transparent");
        material.renderQueue = (int)RenderQueue.Transparent;

        if (material.HasProperty("_Mode"))
        {
            material.SetFloat("_Mode", 3f);
        }

        if (material.HasProperty("_Surface"))
        {
            material.SetFloat("_Surface", 1f);
        }
    }

    public void SetSnackHintVisible(bool visible)
    {
        if (_snackHintRenderer == null)
        {
            return;
        }

        _snackHintRenderer.enabled = visible;
    }

    /// <summary>
    /// Use slider to change player height
    /// </summary>
    /// <param name="height"></param>
    public void OnHeightChange(float height)
    {
        characterController.height = height;
        xrOrign.CameraYOffset = height;
        cameraOffset.transform.position = cameraOffset.transform.parent.TransformPoint(new Vector3(0, height, 0));
        ccd.UpdateHeight();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Snack"))
        {
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
        if (snack.isSpicy)
        {
            TTSManager.Instance.PlayTTS("TTS/Special/PeperPlayer");
            _flame.Play();
        }

        if (snack.isWine)
        {
            TTSManager.Instance.PlayTTS("TTS/Special/BeerPlayer");
        }
    }
}


