using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SubMonitorStatusController : MonoBehaviour
{
    [Tooltip("workInfo 根节点；留空时会在场景中按名称查找。")]
    [SerializeField] private Transform workInfoRoot;

    [Tooltip("Day1..Day6 对应的状态文本/对象根；留空时按 workInfo 下的 Day1..Day6 自动查找。")]
    [SerializeField] private Transform[] dayRoots;

    private Transform[] _unhappyTransforms;
    private Transform[] _smileTransforms;
    private bool _resolved;

    private void Start()
    {
        ResolveReferences();
        Refresh();
        EventManager.AddListener(EventCommon.UPDATE_MONITOR, Refresh);
    }

    private void OnDestroy()
    {
        EventManager.RemoveListener(EventCommon.UPDATE_MONITOR, Refresh);
    }

    private void ResolveReferences()
    {
        if (_resolved && _unhappyTransforms != null && _smileTransforms != null)
        {
            return;
        }

        if (workInfoRoot == null)
        {
            Transform foundTransform = transform.Find("workInfo");
            if (foundTransform == null)
            {
                Transform[] descendants = GetComponentsInChildren<Transform>(true);
                for (int i = 0; i < descendants.Length; i++)
                {
                    if (descendants[i] != null && descendants[i].name == "workInfo")
                    {
                        foundTransform = descendants[i];
                        break;
                    }
                }
            }

            if (foundTransform != null)
            {
                workInfoRoot = foundTransform;
            }
        }

        if ((dayRoots == null || dayRoots.Length == 0) && workInfoRoot != null)
        {
            List<Transform> roots = new List<Transform>();
            for (int i = 1; i <= 6; i++)
            {
                Transform day = FindDescendant(workInfoRoot, $"Day{i}");
                if (day != null)
                {
                    roots.Add(day);
                }
            }
            dayRoots = roots.ToArray();
        }

        if (dayRoots == null || dayRoots.Length == 0)
        {
            return;
        }

        _unhappyTransforms = new Transform[dayRoots.Length];
        _smileTransforms = new Transform[dayRoots.Length];

        for (int i = 0; i < dayRoots.Length; i++)
        {
            Transform day = dayRoots[i];
            if (day == null)
            {
                continue;
            }

            _unhappyTransforms[i] = FindDescendant(day, "Unhappy");
            _smileTransforms[i] = FindDescendant(day, "Smile");
        }

        _resolved = true;
    }

    private static Transform FindDescendant(Transform root, string name)
    {
        if (root == null || string.IsNullOrEmpty(name))
        {
            return null;
        }

        if (root.name == name)
        {
            return root;
        }

        for (int i = 0; i < root.childCount; i++)
        {
            Transform found = FindDescendant(root.GetChild(i), name);
            if (found != null)
            {
                return found;
            }
        }

        return null;
    }

    public void Refresh()
    {
        ResolveReferences();

        if (_unhappyTransforms == null || _smileTransforms == null)
        {
            return;
        }

        List<bool> data = null;
        if (DataCenter.Instance != null &&
            DataCenter.Instance.GameData != null &&
            DataCenter.Instance.GameData.PlayerData != null &&
            DataCenter.Instance.GameData.PlayerData.dailyTimedOut != null)
        {
            data = DataCenter.Instance.GameData.PlayerData.dailyTimedOut;
        }

        int count = Mathf.Min(_unhappyTransforms.Length, _smileTransforms.Length);
        for (int i = 0; i < count; i++)
        {
            Transform unhappy = _unhappyTransforms[i];
            Transform smile = _smileTransforms[i];

            bool hasRecord = data != null && i < data.Count;

            if (!hasRecord)
            {
                if (unhappy != null) unhappy.gameObject.SetActive(false);
                if (smile != null) smile.gameObject.SetActive(false);
                continue;
            }

            bool isUnhappy = data[i];
            if (unhappy != null) unhappy.gameObject.SetActive(isUnhappy);
            if (smile != null) smile.gameObject.SetActive(!isUnhappy);
        }
    }
}
