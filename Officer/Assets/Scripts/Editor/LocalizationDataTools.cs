#if UNITY_EDITOR
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public static class LocalizationDataTools
{
    [MenuItem("Tools/Localization/Export Item & Snack Data (CSV)")]
    public static void ExportData()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Type,ObjectName,Name,Description");

        ItemData[] items = Object.FindObjectsOfType<ItemData>(true);
        foreach (ItemData item in items)
        {
            sb.AppendLine($"ItemData,\"{item.gameObject.name}\",\"{Escape(item.itemName)}\",\"{Escape(item.desc)}\"");
        }

        SnackData[] snacks = Object.FindObjectsOfType<SnackData>(true);
        foreach (SnackData snack in snacks)
        {
            sb.AppendLine($"SnackData,\"{snack.gameObject.name}\",\"{Escape(snack.snackName)}\",\"{Escape(snack.desc)}\"");
        }

        string path = Path.Combine(Application.dataPath, "..", "LocalizationData.csv");
        File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Localization", $"已导出到：{path}", "OK");
    }

    private static string Escape(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        return value.Replace("\"", "\"\"");
    }
}
#endif