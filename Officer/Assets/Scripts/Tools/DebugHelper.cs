using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugHelper : MonoSingleton<DebugHelper>
{
    private TextMeshProUGUI debugText;

    // Start is called before the first frame update
    void Start()
    {
        debugText = GameObject.Find("TxtDebug").GetComponent<TextMeshProUGUI>();
        debugText.text = LocalizationManager.GetText("debug_title");
    }

    // Update is called once per frame
    void Update()
    {
        int progress = DataCenter.Instance != null && DataCenter.Instance.GameData != null && DataCenter.Instance.GameData.PlayerData != null
            ? DataCenter.Instance.GameData.PlayerData.workProgress : 0;
        int favorability = DataCenter.Instance != null && DataCenter.Instance.GameData != null && DataCenter.Instance.GameData.HamsterData != null
            ? DataCenter.Instance.GameData.HamsterData.favorability : 0;
        debugText.text = string.Format(LocalizationManager.GetText("debug_progress"), progress, favorability);
    }
    public void DebugMsg(string msg)
    {
        debugText.text += $"   {msg}   ";
    }
}
