using Snappy;
using System;
using System.IO;
using System.Text;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEditor;
using UnityEngine;
/// <summary>
/// AssetBundle 打包工具
/// </summary>
public class BuildAssetBundle
{
    /// <summary>
    /// 打包生成所有的AssetBundles（包）
    /// </summary>
    [MenuItem("Card1ac的打加密AB包/开干！")]
    public static void BuildAllAB()
    { 
        string strABOutPathDir = "AssetBundles"; 
        if (!Directory.Exists(strABOutPathDir))
        {
            Directory.CreateDirectory(strABOutPathDir);
        }

#if UNITY_XBOX360
        
#elif UNITY_IPHONE
        BuildPipeline.BuildAssetBundles(strABOutPAthDir, BuildAssetBundleOptions.None, BuildTarget.iOS);
#elif UNITY_ANDROID
        BuildPipeline.BuildAssetBundles(strABOutPAthDir, BuildAssetBundleOptions.None, BuildTarget.Android); 
#elif UNITY_STANDALONE_OSX
       
#elif UNITY_STANDALONE_WIN
        BuildPipeline.BuildAssetBundles(strABOutPathDir, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);
#endif
        if (!Directory.Exists(Application.streamingAssetsPath))
        {
            Directory.CreateDirectory(Application.streamingAssetsPath);
        }
        // 获取文件夹中所有后缀为".pkg"的文件
        string[] pkgFiles = Directory.GetFiles(strABOutPathDir, "*.pkg"); 
        foreach (string pkgFilePath in pkgFiles)
        {

            Debug.Log($"找到AB包{pkgFilePath}，正在加密输出为二进制");
            byte[] binaryData = File.ReadAllBytes(pkgFilePath);
            byte[] encryptedBytes = Encrypt(binaryData, LoadManager.assetBundleKey);
            byte[] compressedData = SnappyCodec.Compress(encryptedBytes);
            string[] filStrArr = pkgFilePath.Split("\\");
            string fileName = filStrArr[1].Split(".")[0];
            File.WriteAllBytes($"{Application.streamingAssetsPath}\\{fileName}.bin", compressedData);
            Debug.Log($"成功加密AB包{pkgFilePath}，在{Application.streamingAssetsPath}里查看");
        }
    }

    static byte[] Encrypt(byte[] bytes, string key)
    { 
        byte[] keyBytes = Encoding.UTF8.GetBytes(key);

        byte[] encryptedBytes = new byte[bytes.Length];
        for (int i = 0; i < bytes.Length; i++)
        {
            encryptedBytes[i] = (byte)(bytes[i] ^ keyBytes[i % keyBytes.Length]);
        } 
        return encryptedBytes;
    }
     
}