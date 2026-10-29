#if UNITY_EDITOR
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;

public static class LocalizationFontTools
{
    private const string DefaultFontAssetPath = "Assets/TextMesh Pro/Resources/Fonts & Materials/Chinese SDF.asset";

    [MenuItem("Tools/Localization/Generate Chinese TMP Font Asset")]
    public static void GenerateChineseTmpFontAsset()
    {
        string fontPath = EditorUtility.OpenFilePanel("Select Chinese Font", "Assets", "ttf,otf");
        if (string.IsNullOrEmpty(fontPath))
        {
            return;
        }

        // Convert absolute path to project-relative path when possible.
        string projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
        if (fontPath.StartsWith(projectRoot))
        {
            fontPath = fontPath.Substring(projectRoot.Length + 1).Replace('\\', '/');
        }

        Font sourceFont = AssetDatabase.LoadAssetAtPath<Font>(fontPath);
        if (sourceFont == null)
        {
            EditorUtility.DisplayDialog("Localization", "无法加载字体资源，请先将字体文件放入 Assets 目录。", "OK");
            return;
        }

        TMP_FontAsset fontAsset = TMP_FontAsset.CreateFontAsset(
            sourceFont,
            90,
            9,
            UnityEngine.TextCore.LowLevel.GlyphRenderMode.SDFAA,
            1024,
            1024);

        if (fontAsset == null)
        {
            EditorUtility.DisplayDialog("Localization", "生成 TMP Font Asset 失败。", "OK");
            return;
        }

        string directory = Path.GetDirectoryName(DefaultFontAssetPath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        AssetDatabase.CreateAsset(fontAsset, DefaultFontAssetPath);
        if (fontAsset.atlasTexture != null)
        {
            fontAsset.atlasTexture.name = "Chinese SDF Atlas";
            AssetDatabase.AddObjectToAsset(fontAsset.atlasTexture, fontAsset);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("Localization", $"已生成字体资产：{DefaultFontAssetPath}", "OK");
    }
}
#endif