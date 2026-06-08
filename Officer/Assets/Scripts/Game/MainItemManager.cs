using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainItemManager : MonoSingleton<MainItemManager>
{
    [SerializeField] private List<GameObject> _item = new List<GameObject>();
    [SerializeField] private GameObject _nextItem;

    void Start()
    {
        _item = GetChildren(transform);
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
            Debug.LogWarning("Item pool is empty.");
            return;
        }

        int randomIndex = Random.Range(0, _item.Count);
        _nextItem = _item[randomIndex];
        _nextItem.SetActive(true);
        // Only the freshly spawned item is highlighted as the current hint; the ray clears it per-object.
        SetOutlineVisibleRecursive(_nextItem, true);
        _item.RemoveAt(randomIndex);

        ItemData itemData = _nextItem.GetComponent<ItemData>();
        if (itemData != null)
        {
            RegisterItemOwnership(itemData);

            if (itemData.isPad)
            {
                // A mouse pad halves the required number of keyboard hits.
                KeyboardController.Instance.requireHit = (KeyboardController.Instance.requireHit / 2);
            }
        }
    }

    private void RegisterItemOwnership(ItemData itemData)
    {
        if (itemData == null ||
            DataCenter.Instance == null ||
            DataCenter.Instance.GameData == null ||
            DataCenter.Instance.GameData.PlayerData == null)
        {
            return;
        }

        List<ItemData> ownedItem = DataCenter.Instance.GameData.PlayerData.ownedItem;
        if (ownedItem == null)
        {
            return;
        }

        if (!ownedItem.Contains(itemData))
        {
            ownedItem.Add(itemData);
        }

        // Refresh the work-efficiency display to reflect the newly owned item bonus.
        EventManager.DispatchEvent(EventCommon.UPDATE_MONITOR);
    }

    public void SetAllActiveItemOutlineVisible(bool visible)
    {
        foreach (Transform child in transform)
        {
            if (child == null || !child.gameObject.activeInHierarchy)
            {
                continue;
            }

            SetOutlineVisibleRecursive(child.gameObject, visible);
        }
    }

    public void HideSpawnOutlineForRayTarget(GameObject rayRoot)
    {
        if (rayRoot == null)
        {
            return;
        }

        SetOutlineVisibleRecursive(rayRoot, false);
    }

    private void SetOutlineVisibleRecursive(GameObject target, bool visible)
    {
        if (target == null)
        {
            return;
        }

        Outline[] outlines = target.GetComponentsInChildren<Outline>(true);
        for (int i = 0; i < outlines.Length; i++)
        {
            if (outlines[i] != null)
            {
                outlines[i].enabled = visible;
            }
        }
    }
}
