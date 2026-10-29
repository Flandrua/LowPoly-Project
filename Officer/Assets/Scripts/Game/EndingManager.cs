using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR.InteractionSystem;

public class EndingManager : MonoSingleton<EndingManager>
{
    private const string TeleportAreaStartName = "TeleportAreaStart";
    private string hamsterEnding;
    private string workEnding;

    public GameObject Container;
    public GameObject hamsterLove;
    public GameObject hamsterNormal;
    public GameObject hamsterDead;
    public GameObject arrowIcon;
    public Text endingTxt;
    [SerializeField] private GameObject deadBodyObject;

    void Start()
    {
    }

    private void LoadHamsterEndingText(string path)
    {
        TextAsset textAsset = Resources.Load<TextAsset>($"{path}");
        if (textAsset != null)
        {
            hamsterEnding = textAsset.text;
        }
        else
        {
            Debug.LogError("LoadHamsterEndingText cant find");
        }
    }

    private void LoadWorkEndingText(string path)
    {
        TextAsset textAsset = Resources.Load<TextAsset>($"{path}");
        if (textAsset != null)
        {
            workEnding = textAsset.text;
        }
        else
        {
            Debug.LogError("LoadWorkEndingText cant find");
        }
    }

    void Update()
    {
    }

    public void TriggerWorkFailedEnding()
    {
        LockTeleportAreasForEnding();
        TeleportPlayerToEndingPanelFront();
        SetDeadBodyVisible(false);
        ResetHamsterEndingVisuals();
        hamsterEnding = string.Empty;
        LoadWorkEndingText(LocalizationPaths.Tts("Ending/Work/WorkFailed"));
        TTSManager.Instance.PlayTTS(LocalizationPaths.Tts("Ending/Work/WorkFailed"));
        arrowIcon.SetActive(false);
        endingTxt.text = workEnding;
        Container.SetActive(true);
    }
    public void Ending()
    {
        HandleEndingTextAndGameObjects();
        LockTeleportAreasForEnding();
        TeleportPlayerToEndingPanelFront();
        arrowIcon.SetActive(false);
        endingTxt.text = string.IsNullOrEmpty(hamsterEnding) ? workEnding : workEnding + "\r" + hamsterEnding;
        Container.SetActive(true);
    }

    public void EndingDeath()
    {
        if (GameManager.Instance != null && (GameManager.Instance.IsLowStressVersion || GameManager.Instance.IsSuddenDeathEndingDisabled))
        {
            Ending();
            return;
        }

        LockTeleportAreasForEnding();
        TeleportPlayerToEndingPanelFront();
        SetDeadBodyVisible(true);
        ResetHamsterEndingVisuals();
        hamsterEnding = string.Empty;
        LoadWorkEndingText(LocalizationPaths.Tts("Ending/Work/WorkDead"));
        arrowIcon.SetActive(false);
        endingTxt.text = workEnding;
        Container.SetActive(true);
    }

    private void HandleEndingTextAndGameObjects()
    {
        ResetHamsterEndingVisuals();
        hamsterEnding = string.Empty;

        if (GameManager.Instance != null && GameManager.Instance.IsLowStressVersion)
        {
            LoadWorkEndingText(LocalizationPaths.Tts("Ending/Work/LowPressure"));
            TTSManager.Instance.PlayTTS(LocalizationPaths.Tts("Ending/Work/LowPressure"));
            return;
        }

        bool hamsterGameplayEnabled = GameManager.Instance.IsHamsterGameplayEnabled;
        bool hamsterLoveEnding = hamsterGameplayEnabled &&
                                 DataCenter.Instance.GameData.HamsterData.favorability >= GameManager.Instance.HamsterLoveEndingFavorabilityThreshold &&
                                 !GameManager.Instance.IsHamsterDead();
        bool hamsterDeadEnding = hamsterGameplayEnabled && GameManager.Instance.IsHamsterDead();
        bool workSuccess = DataCenter.Instance.GameData.PlayerData.workProgress >= GameManager.Instance.goalWorkProgress;

        if (workSuccess && hamsterLoveEnding)
        {
            LoadHamsterEndingText(LocalizationPaths.Tts("Ending/Hamster/HamsterLover"));
            LoadWorkEndingText(LocalizationPaths.Tts("Ending/Work/Workaholic"));
            hamsterLove.gameObject.SetActive(true);
            TTSManager.Instance.PlayTTS(LocalizationPaths.Tts("Ending/Work/Workaholic"));
            TTSManager.Instance.EnqueueTTS(LocalizationPaths.Tts("Ending/Hamster/HamsterLover"));
            return;
        }

        if (workSuccess)
        {
            LoadWorkEndingText(LocalizationPaths.Tts("Ending/Work/WorkStandard"));
            TTSManager.Instance.PlayTTS(LocalizationPaths.Tts("Ending/Work/WorkStandard"));
        }
        else
        {
            LoadWorkEndingText(LocalizationPaths.Tts("Ending/Work/WorkFailed"));
            TTSManager.Instance.PlayTTS(LocalizationPaths.Tts("Ending/Work/WorkFailed"));
        }

        if (!hamsterGameplayEnabled)
        {
            return;
        }

        if (hamsterLoveEnding)
        {
            LoadHamsterEndingText(LocalizationPaths.Tts("Ending/Hamster/HamsterLover"));
            hamsterLove.gameObject.SetActive(true);
            TTSManager.Instance.EnqueueTTS(LocalizationPaths.Tts("Ending/Hamster/HamsterLover"));
        }
        else if (hamsterDeadEnding)
        {
            LoadHamsterEndingText(LocalizationPaths.Tts("Ending/Hamster/HamsterBad"));
            hamsterDead.gameObject.SetActive(true);
            TTSManager.Instance.EnqueueTTS(LocalizationPaths.Tts("Ending/Hamster/HamsterBad"));
        }
        else
        {
            LoadHamsterEndingText(LocalizationPaths.Tts("Ending/Hamster/HamsterNormal"));
            hamsterNormal.gameObject.SetActive(true);
            TTSManager.Instance.EnqueueTTS(LocalizationPaths.Tts("Ending/Hamster/HamsterNormal"));
        }
    }

    private void ResetHamsterEndingVisuals()
    {
        hamsterLove.gameObject.SetActive(false);
        hamsterNormal.gameObject.SetActive(false);
        hamsterDead.gameObject.SetActive(false);
    }

    private void SetDeadBodyVisible(bool visible)
    {
        if (deadBodyObject == null)
        {
            Debug.LogWarning("EndingManager: deadBodyObject is not assigned in Inspector.");
            return;
        }

        deadBodyObject.SetActive(visible);
    }

    private void TeleportPlayerToEndingPanelFront()
    {
        if (Player.instance == null)
        {
            return;
        }

        TeleportArea startArea = null;
        TeleportArea[] areas = FindObjectsOfType<TeleportArea>(true);
        for (int i = 0; i < areas.Length; i++)
        {
            TeleportArea area = areas[i];
            if (area != null && area.name == TeleportAreaStartName)
            {
                startArea = area;
                break;
            }
        }

        if (startArea == null)
        {
            return;
        }

        Transform trackingOrigin = Player.instance.trackingOriginTransform;
        if (trackingOrigin == null)
        {
            return;
        }

        Vector3 feetOffset = trackingOrigin.position - Player.instance.feetPositionGuess;
        trackingOrigin.position = startArea.transform.position + feetOffset;
        trackingOrigin.rotation = Quaternion.Euler(0f, 180f, 0f);
    }

    private void LockTeleportAreasForEnding()
    {
        TeleportArea[] areas = FindObjectsOfType<TeleportArea>(true);
        for (int i = 0; i < areas.Length; i++)
        {
            TeleportArea area = areas[i];
            if (area == null || !area.gameObject.scene.IsValid())
            {
                continue;
            }

            // Inactive areas never ran Awake(), so their internal areaMesh is null and
            // SetLocked() -> UpdateVisuals() would throw a NullReferenceException. They also
            // can't be teleported to while inactive, so there is nothing to lock.
            if (!area.gameObject.activeInHierarchy)
            {
                continue;
            }

            if (area.name == TeleportAreaStartName)
            {
                area.SetLocked(false);
                continue;
            }

            area.SetLocked(true);
        }
    }
}
