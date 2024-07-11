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
        debugText.text = "PM Debug";
    }

    // Update is called once per frame
    void Update()
    {
        debugText.text = $"Progress{DataCenter.Instance.GameData.PlayerData.workProgress},Fav {DataCenter.Instance.GameData.HamsterData.favorability}";
    }
    public void DebugMsg(string msg)
    {
        debugText.text += $"   {msg}   ";
    }
}
