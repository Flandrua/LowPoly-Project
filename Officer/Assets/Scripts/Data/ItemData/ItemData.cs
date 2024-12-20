using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemData : MonoBehaviour
{
    public string itemName;
    public string desc;
    public bool isPad= false;
    public int workEfficiency = 0;
    public int extraFavorability = 0;

    [SerializeField] protected TextMeshProUGUI _content = null;
    [SerializeField] protected TextMeshProUGUI _name = null;
    protected Outline _outline = null;
    protected Vector3 initialPosition;
    protected Quaternion initialRotation;
    // Start is called before the first frame update
    void Start()
    {
        EventManager.AddListener(EventCommon.NEXT_STAGE, ResetToDefault);
        _content = UIMonitorController.Instance.content;
        _name = UIMonitorController.Instance.name;
        _outline = GetComponent<Outline>();
        _outline.enabled = false;
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
        _outline.enabled = flag;
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
