using UnityEngine;

public static class LocalizationPaths
{
    public static string TtsRoot
    {
        get
        {
            // Default to Chinese to match LocalizationManager when GameManager is not ready yet.
            if (GameManager.Instance == null)
            {
                return "TTS_CN";
            }
            return GameManager.Instance.IsChinese ? "TTS_CN" : "TTS_EN";
        }
    }

    public static string MsgRoot
    {
        get
        {
            if (GameManager.Instance == null)
            {
                return "MSG_CN";
            }
            return GameManager.Instance.IsChinese ? "MSG_CN" : "MSG_EN";
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
