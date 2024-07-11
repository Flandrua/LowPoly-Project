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
    public bool haveUIDesc = false;

    [SerializeField] private TextMeshProUGUI _content = null;
    [SerializeField] private TextMeshProUGUI _name = null;
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    // Start is called before the first frame update
    void Start()
    {
        EventManager.AddListener(EventCommon.NEXT_STAGE, ResetToDefault);
        if (haveUIDesc)
        {
            _content = transform.Find("ItemDescription").Find("Content").GetComponent<TextMeshProUGUI>();
            _name = transform.Find("ItemDescription").Find("Name").GetComponent<TextMeshProUGUI>();
            _content.text = desc;
            _name.text = itemName;
        }
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }
    private void OnDestroy()
    {
        EventManager.RemoveListener(EventCommon.NEXT_STAGE, ResetToDefault);
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
