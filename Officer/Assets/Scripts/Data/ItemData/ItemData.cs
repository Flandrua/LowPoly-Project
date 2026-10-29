using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class ItemData : MonoBehaviour
{
    private const string ItemGetTtsRoot = "TTS/ItemGet";
    private const string CloneSuffix = "(Clone)";

    public string itemName;
    public string desc;
    public bool isPad= false;
    public int workEfficiency = 0;
    public int extraFavorability = 0;
    [SerializeField] private bool playPickupTtsOnFirstGrab = true;
    [SerializeField] private string pickupTtsKey = string.Empty;

    [SerializeField] protected TextMeshProUGUI _content = null;
    [SerializeField] protected TextMeshProUGUI _name = null;
    //protected Outline _outline = null;
    protected Vector3 initialPosition;
    protected Quaternion initialRotation;
    private bool hasTriedPlayPickupTts;
    // Start is called before the first frame update
    void Start()
    {
        EventManager.AddListener(EventCommon.NEXT_STAGE, ResetToDefault);
        //_content = UIMonitorController.Instance.content;
        //_name = UIMonitorController.Instance.nameTxt;
        //_outline = GetComponent<Outline>();
        //_outline.enabled = false;
        //_content.text = desc;
        //_name.text = itemName;
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }
    private void OnDestroy()
    {
        EventManager.RemoveListener(EventCommon.NEXT_STAGE, ResetToDefault);
    }
    public void ShowUIDec(bool flag)
    {
        string itemNameKey = "item_name_" + LocalizationManager.NormalizeKey(itemName);
        string itemDescKey = "item_desc_" + LocalizationManager.NormalizeKey(itemName);

        string localizedItemName = LocalizationManager.GetText(itemNameKey);
        string localizedItemDesc = LocalizationManager.GetText(itemDescKey);

        _content.text = localizedItemDesc != itemDescKey
            ? localizedItemDesc
            : (IsChinese ? GetLocalizedItemDesc(desc) : desc);
        _name.text = localizedItemName != itemNameKey
            ? localizedItemName
            : (IsChinese ? GetLocalizedItemName(itemName) : itemName);
        //_outline.enabled = flag;
        UIMonitorController.Instance.Show(flag);
    }

    private bool IsChinese => GameManager.Instance != null && GameManager.Instance.IsChinese;

    private string GetLocalizedItemName(string englishName)
    {
        if (string.IsNullOrEmpty(englishName))
        {
            return englishName;
        }

        switch (englishName)
        {
            case "CCTV Monitor": return "监控";
            case "Cellphone": return "手机";
            case "Goblin": return "哥布林";
            case "Main Monitor": return "主监视器";
            case "Info Monitor": return "信息监视器";
            case "Mousepad": return "鼠标垫";
            case "Notepad": return "记事本";
            case "Pen": return "笔";
            case "Viking": return "维京";
            case "Webcam": return "摄像头";
            default: return englishName;
        }
    }

    private string GetLocalizedItemDesc(string englishDesc)
    {
        if (string.IsNullOrEmpty(englishDesc))
        {
            return englishDesc;
        }

        string result = englishDesc;
        result = result.Replace("Work Efficiency", "工作效率");
        result = result.Replace("work efficiency", "工作效率");
        result = result.Replace("Favorability Ability", "好感度能力");
        result = result.Replace("favorability ability", "好感度能力");
        result = result.Replace("Check your progress and time", "查看你的进度和时间");
        return result;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void ResetToDefault()
    {
        transform.position = initialPosition;
        transform.rotation = initialRotation;
    }

    public void TryPlayPickupTTSOnce()
    {
        if (!playPickupTtsOnFirstGrab || hasTriedPlayPickupTts)
        {
            return;
        }

        hasTriedPlayPickupTts = true;

        if (TTSManager.Instance == null)
        {
            Debug.LogWarning($"ItemData: TTSManager missing, skip pickup TTS for [{name}].");
            return;
        }

        string audioPath = ResolvePickupTtsPath();
        if (string.IsNullOrEmpty(audioPath))
        {
            Debug.LogWarning($"ItemData: no pickup TTS found for item [{itemName}] on object [{name}] under Resources/{ItemGetTtsRoot}.");
            return;
        }

        TTSManager.Instance.PlayTTS(audioPath);
    }

    public void PlayPickupTTS()
    {
        if (TTSManager.Instance == null)
        {
            Debug.LogWarning($"ItemData: TTSManager missing, skip trigger TTS for [{name}].");
            return;
        }

        string audioPath = ResolvePickupTtsPath();
        if (string.IsNullOrEmpty(audioPath))
        {
            Debug.LogWarning($"ItemData: no trigger TTS found for item [{itemName}] on object [{name}] under Resources/{ItemGetTtsRoot}.");
            return;
        }

        TTSManager.Instance.PlayTTS(audioPath);
    }

    private string ResolvePickupTtsPath()
    {
        List<string> keyCandidates = new List<string>();
        HashSet<string> uniqueCandidates = new HashSet<string>();

        AddCandidate(keyCandidates, uniqueCandidates, pickupTtsKey);
        AddNameCandidates(keyCandidates, uniqueCandidates, itemName);
        AddNameCandidates(keyCandidates, uniqueCandidates, GetObjectNameWithoutCloneSuffix());

        foreach (string candidate in keyCandidates)
        {
            string resourcePath = $"{ItemGetTtsRoot}/{candidate}";
            AudioClip clip = Resources.Load<AudioClip>(resourcePath);
            if (clip != null)
            {
                return resourcePath;
            }
        }

        return string.Empty;
    }

    private void AddNameCandidates(List<string> candidates, HashSet<string> uniqueCandidates, string rawName)
    {
        AddCandidate(candidates, uniqueCandidates, rawName);

        string normalized = NormalizeName(rawName);
        AddCandidate(candidates, uniqueCandidates, normalized);
        AddAliasCandidates(candidates, uniqueCandidates, normalized);
        AddCandidate(candidates, uniqueCandidates, GetFirstToken(rawName));
    }

    private void AddAliasCandidates(List<string> candidates, HashSet<string> uniqueCandidates, string normalizedName)
    {
        if (string.IsNullOrEmpty(normalizedName))
        {
            return;
        }

        if (normalizedName == "InfoMonitor" || normalizedName == "MainMonitor")
        {
            AddCandidate(candidates, uniqueCandidates, "MonitorUI");
        }
        else if (normalizedName == "CCTVMonitor")
        {
            AddCandidate(candidates, uniqueCandidates, "CCTV");
        }
    }

    private void AddCandidate(List<string> candidates, HashSet<string> uniqueCandidates, string candidate)
    {
        if (string.IsNullOrWhiteSpace(candidate))
        {
            return;
        }

        string trimmed = candidate.Trim();
        if (uniqueCandidates.Add(trimmed))
        {
            candidates.Add(trimmed);
        }
    }

    private string GetObjectNameWithoutCloneSuffix()
    {
        string objectName = gameObject != null ? gameObject.name : string.Empty;
        if (string.IsNullOrWhiteSpace(objectName))
        {
            return string.Empty;
        }

        string trimmed = objectName.Trim();
        if (trimmed.EndsWith(CloneSuffix))
        {
            return trimmed.Substring(0, trimmed.Length - CloneSuffix.Length).Trim();
        }

        return trimmed;
    }

    private string GetFirstToken(string rawName)
    {
        if (string.IsNullOrWhiteSpace(rawName))
        {
            return string.Empty;
        }

        string trimmed = rawName.Trim();
        int splitIndex = trimmed.IndexOf(' ');
        return splitIndex > 0 ? trimmed.Substring(0, splitIndex) : trimmed;
    }

    private string NormalizeName(string rawName)
    {
        if (string.IsNullOrWhiteSpace(rawName))
        {
            return string.Empty;
        }

        StringBuilder builder = new StringBuilder(rawName.Length);
        foreach (char ch in rawName)
        {
            if (char.IsLetterOrDigit(ch))
            {
                builder.Append(ch);
            }
        }

        return builder.ToString();
    }
}
