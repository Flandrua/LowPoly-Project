using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMonitorData : ItemData
{
    // Start is called before the first frame update
    public TextMeshProUGUI day;
    public Scrollbar bar;
    void Start()
    {
        _content = UIMonitorController.Instance.content;
        _name = UIMonitorController.Instance.name;
        _outline = GetComponent<Outline>();
        _outline.enabled = false;
        //_content.text = desc;
        //_name.text = itemName;
        initialPosition = transform.position;
        initialRotation = transform.rotation;
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
        float size = (DataCenter.Instance.GameData.PlayerData.workProgress*1.0f) / (GameManager.Instance.goalWorkPrgoress*1.0f);
        bar.size = size;
    }
}
