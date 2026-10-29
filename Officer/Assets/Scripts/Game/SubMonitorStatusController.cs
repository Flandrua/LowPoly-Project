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
        if (workInfoRoot == null)
        {
            GameObject found = GameObject.Find("workInfo");
            if (found != null)
            {
                workInfoRoot = found.transform;
            }
        }

        if (dayRoots == null || dayRoots.Length == 0)
        {
            if (workInfoRoot == null)
            {
                return;
            }

            List<Transform> roots = new List<Transform>();
            for (int i = 1; i <= 6; i++)
            {
                Transform day = workInfoRoot.Find($"Day{i}");
                if (day != null)
                {
                    roots.Add(day);
                }
            }
            dayRoots = roots.ToArray();
        }
    }

    public void Refresh()
    {
        if (dayRoots == null || dayRoots.Length == 0)
        {
            ResolveReferences();
        }

        if (dayRoots == null || dayRoots.Length == 0)
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

        for (int i = 0; i < dayRoots.Length; i++)
        {
            Transform day = dayRoots[i];
            if (day == null)
            {
                continue;
            }

            Transform unhappy = day.Find("Unhappy");
            Transform smile = day.Find("Smile");

            int recordIndex = i;
            bool hasRecord = data != null && recordIndex < data.Count;

            if (!hasRecord)
            {
                if (unhappy != null) unhappy.gameObject.SetActive(false);
                if (smile != null) smile.gameObject.SetActive(false);
                continue;
            }

            bool isUnhappy = data[recordIndex];
            if (unhappy != null) unhappy.gameObject.SetActive(isUnhappy);
            if (smile != null) smile.gameObject.SetActive(!isUnhappy);
        }
    }
}
