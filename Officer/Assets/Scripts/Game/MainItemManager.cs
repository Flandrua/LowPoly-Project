using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainItemManager : MonoSingleton<MainItemManager>
{
    [SerializeField] private List<GameObject> _item = new List<GameObject>();
    [SerializeField] private GameObject _nextItem;
    // Start is called before the first frame update
    void Start()
    {
        _item = GetChildren(this.transform);
    }
    private List<GameObject> GetChildren(Transform parent)
    {
        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in parent)
        {
            children.Add(child.gameObject);
        }
        return children;
    }

    public void RandomItem()
    {
        if (_item.Count == 0)
        {
            Debug.LogWarning("item 已用完");
            return;
        }
        int randomIndex = Random.Range(0, _item.Count);
        _nextItem = _item[randomIndex];
        _nextItem.SetActive(true);
        _item.RemoveAt(randomIndex);
        ItemData itemData = _nextItem.GetComponent<ItemData>();
        if (itemData.isPad) {
            //如果是鼠标垫，就让实际需要工作的次数除以2
            KeyboardController.Instance.requireHit = (KeyboardController.Instance.requireHit / 2);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
