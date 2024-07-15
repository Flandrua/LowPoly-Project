using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemData : MonoBehaviour
{
    public string itemName;
    public string desc;
    public int workEfficiency = 0;
    public int extraFavorability = 0;

    private TextMeshProUGUI _content = null;
    private TextMeshProUGUI _name = null;
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    // Start is called before the first frame update
    void Start()
    {
        EventManager.AddListener(EventCommon.NEXT_STAGE, ResetToDefault);
        _content = UIMonitorController.Instance.content;
        _name = UIMonitorController.Instance.name;
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
        _content.text = desc;
        _name.text = itemName;
        UIMonitorController.Instance.Show(flag);
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
}
