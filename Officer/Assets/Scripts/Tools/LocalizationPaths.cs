using UnityEngine;

public static class LocalizationPaths
{
    public static string TtsRoot
    {
        get
        {
            return GameManager.Instance != null && GameManager.Instance.IsChinese ? "TTS_CN" : "TTS_EN";
        }
    }

    public static string MsgRoot
    {
        get
        {
            return GameManager.Instance != null && GameManager.Instance.IsChinese ? "MSG_CN" : "MSG_EN";
        }
    }

    public static string Tts(string relativePath)
    {
        return $"{TtsRoot}/{relativePath}";
    }

    public static string Msg(string relativePath)
    {
        return $"{MsgRoot}/{relativePath}";
    }
}
