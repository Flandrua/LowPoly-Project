using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMonitorData : ItemData
{
    public TextMeshProUGUI day;
    public Scrollbar bar;
    public TextMeshProUGUI workProgressPercent;

    void Start()
    {
        _content = UIMonitorController.Instance.content;
        _name = UIMonitorController.Instance.nameTxt;
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        EventManager.AddListener(EventCommon.UPDATE_MONITOR, UpdateInfo);
        UpdateInfo();
    }

    private void OnDestroy()
    {
        EventManager.RemoveListener(EventCommon.UPDATE_MONITOR, UpdateInfo);
    }

    public void UpdateInfo()
    {
        if (day != null &&
            DataCenter.Instance != null &&
            DataCenter.Instance.GameData != null &&
            DataCenter.Instance.GameData.PlayerData != null &&
            GameManager.Instance != null)
        {
            bool chinese = GameManager.Instance != null && GameManager.Instance.IsChinese;
            int dayValue = DataCenter.Instance.GameData.PlayerData.days;
            int totalDays = GameManager.Instance.TotalDays;
            string time = GameManager.Instance.CurrentTimeDisplay;
            day.text = string.Format(LocalizationManager.GetText("day_format"), dayValue, totalDays, time);
        }

        int percent = GetWorkProgressPercent();
        if (bar != null)
        {
            bar.size = percent / 100f;
        }

        if (workProgressPercent != null)
        {
            workProgressPercent.text = percent + "%";
        }
    }

    private static int GetWorkProgressPercent()
    {
        if (DataCenter.Instance == null ||
            DataCenter.Instance.GameData == null ||
            DataCenter.Instance.GameData.PlayerData == null ||
            GameManager.Instance == null)
        {
            return 0;
        }

        int goal = Mathf.Max(1, GameManager.Instance.goalWorkProgress);
        int progress = Mathf.Max(0, DataCenter.Instance.GameData.PlayerData.workProgress);
        return Mathf.Clamp(Mathf.RoundToInt(progress * 100f / goal), 0, 100);
    }
}
