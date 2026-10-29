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
        bool chinese = GameManager.Instance != null && GameManager.Instance.IsChinese;
        debugText.text = chinese ? "调试" : "PM Debug";
    }

    // Update is called once per frame
    void Update()
    {
        bool chinese = GameManager.Instance != null && GameManager.Instance.IsChinese;
        int progress = DataCenter.Instance != null && DataCenter.Instance.GameData != null && DataCenter.Instance.GameData.PlayerData != null
            ? DataCenter.Instance.GameData.PlayerData.workProgress : 0;
        int favorability = DataCenter.Instance != null && DataCenter.Instance.GameData != null && DataCenter.Instance.GameData.HamsterData != null
            ? DataCenter.Instance.GameData.HamsterData.favorability : 0;
        debugText.text = chinese
            ? $"进度{progress}, 好感 {favorability}"
            : $"Progress{progress}, Fav {favorability}";
    }
    public void DebugMsg(string msg)
    {
        debugText.text += $"   {msg}   ";
    }
}
