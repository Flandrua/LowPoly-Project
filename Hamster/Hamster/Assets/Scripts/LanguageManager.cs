using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Configs;
public enum LanguageType
{
    CN,
    TW,
    EN,
}
public class LanguageManager : Singleton<LanguageManager>
{
    private LanguageType _langType;
    private CfgLanguage cfgLanguage;

    public LanguageType LangType { get => _langType; set => _langType = value; }

    public void InitLang()
    {
        cfgLanguage = DataManager.Instance.CfgLanguage ;
        SystemLanguage lang = Application.systemLanguage;
        lang = SystemLanguage.English;
        switch (lang)
        {
            case SystemLanguage.ChineseSimplified:
                LangType = LanguageType.CN;
                break;
            case SystemLanguage.ChineseTraditional:
                LangType = LanguageType.TW;
                break;
            case SystemLanguage.English:
                LangType = LanguageType.EN;
                break;
            case SystemLanguage.Chinese:
                LangType = LanguageType.CN;
                break;
            default:
                LangType = LanguageType.EN;
                break;
        }
    }

    public string GetString(string langId)
    {
        string str = "";
        if (cfgLanguage.mDataMap.ContainsKey(langId))
        {
            switch (LangType)
            {
                case LanguageType.CN:
                    str = cfgLanguage.mDataMap[langId].CN;
                    break;
                case LanguageType.TW:
                    //str = cfgLanguage.mDataMap[langId].CN;
                    break;
                case LanguageType.EN:
                    str = cfgLanguage.mDataMap[langId].EN;
                    break;
            }
        }
        else
        {
            str = langId;
        }
        return str;
    }
}
