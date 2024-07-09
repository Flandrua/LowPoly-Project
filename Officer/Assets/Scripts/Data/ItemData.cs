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

    [SerializeField] private TextMeshProUGUI _content = null;
    [SerializeField] private TextMeshProUGUI _name = null;
    // Start is called before the first frame update
    void Start()
    {
        _content = transform.Find("ItemDescription").Find("Content").GetComponent<TextMeshProUGUI>();
        _name = transform.Find("ItemDescription").Find("Name").GetComponent<TextMeshProUGUI>();
        _content.text = desc;
        _name.text = itemName;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
