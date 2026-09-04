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

    public void Ending()
    {
        LockTeleportAreasForEnding();
        SetDeadBodyVisible(false);
        HandleEndingTextAndGameObjects();
        arrowIcon.SetActive(false);
        endingTxt.text = string.IsNullOrEmpty(hamsterEnding) ? workEnding : workEnding + "\r" + hamsterEnding;
        Container.SetActive(true);
    }

    public void EndingDeath()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsLowStressVersion)
        {
            Ending();
            return;
        }

        LockTeleportAreasForEnding();
        SetDeadBodyVisible(true);
        ResetHamsterEndingVisuals();
        hamsterEnding = string.Empty;
        LoadWorkEndingText("TTS/Ending/Work/WorkDead");
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
            LoadWorkEndingText("TTS/Ending/Work/WorkStandard");
            TTSManager.Instance.PlayTTS("TTS/Ending/Work/WorkStandard");
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
            LoadHamsterEndingText("TTS/Ending/Hamster/HamsterLover");
            LoadWorkEndingText("TTS/Ending/Work/Workaholic");
            hamsterLove.gameObject.SetActive(true);
            TTSManager.Instance.PlayTTS("TTS/Ending/Work/Workaholic");
            TTSManager.Instance.EnqueueTTS("TTS/Ending/Hamster/HamsterLover");
            return;
        }

        if (workSuccess)
        {
            LoadWorkEndingText("TTS/Ending/Work/WorkStandard");
            TTSManager.Instance.PlayTTS("TTS/Ending/Work/WorkStandard");
        }
        else
        {
            LoadWorkEndingText("TTS/Ending/Work/WorkFailed");
            TTSManager.Instance.PlayTTS("TTS/Ending/Work/WorkFailed");
        }

        if (!hamsterGameplayEnabled)
        {
            return;
        }

        if (hamsterLoveEnding)
        {
            LoadHamsterEndingText("TTS/Ending/Hamster/HamsterLover");
            hamsterLove.gameObject.SetActive(true);
            TTSManager.Instance.EnqueueTTS("TTS/Ending/Hamster/HamsterLover");
        }
        else if (hamsterDeadEnding)
        {
            LoadHamsterEndingText("TTS/Ending/Hamster/HamsterBad");
            hamsterDead.gameObject.SetActive(true);
            TTSManager.Instance.EnqueueTTS("TTS/Ending/Hamster/HamsterBad");
        }
        else
        {
            LoadHamsterEndingText("TTS/Ending/Hamster/HamsterNormal");
            hamsterNormal.gameObject.SetActive(true);
            TTSManager.Instance.EnqueueTTS("TTS/Ending/Hamster/HamsterNormal");
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
