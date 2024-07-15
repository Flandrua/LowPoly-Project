using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMonitorData : ItemData
{
    // Start is called before the first frame update
    private TextMeshProUGUI day;
    private Scrollbar bar;
    void Start()
    {
        EventManager.AddListener(EventCommon.UPDATE_MONITOR, UpdateInfo);
    }
    private void OnDestroy()
    {
        EventManager.RemoveListener(EventCommon.UPDATE_MONITOR, UpdateInfo);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public void UpdateInfo()
    {
        day.text = $"Day:{DataCenter.Instance.GameData.PlayerData.days}/{GameManager.Instance.totaldays}";
        float size = (DataCenter.Instance.GameData.PlayerData.workProgress / GameManager.Instance.goalWorkPrgoress);
        bar.size = size;
    }
}
