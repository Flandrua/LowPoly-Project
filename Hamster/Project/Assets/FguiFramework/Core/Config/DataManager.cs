using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using Configs;

public class DataManager : Singleton<DataManager>
{
    private Object[] configRes;
    public void LoadConfig(Object[] objArr)
    {
        configRes = objArr;
        _cfgGlobal = SetConfigData<CfgGlobal>("CfgGlobal");
        _cfgLanguage = SetConfigData<CfgLanguage>("CfgLanguage");
        _cfgQuestion = SetConfigData<CfgQuestion>("CfgQuestion");
    }

    private CfgGlobal _cfgGlobal;

    private CfgLanguage _cfgLanguage;

    private CfgQuestion _cfgQuestion;

    public CfgGlobal CfgGlobal { get => _cfgGlobal; }

    public CfgLanguage CfgLanguage { get => _cfgLanguage; }

    public CfgQuestion CfgQuestion { get => _cfgQuestion; }


    private T SetConfigData<T>(string name)
    {
        for (var i = 0; i < configRes.Length; i++)
        {
            if (configRes[i].name.Equals(name))
            {
                T config = default(T);
                try
                {
                    config = JsonMapper.ToObject<T>(configRes[i].ToString());
                }
                catch
                {
                    Debug.LogError(string.Format("{0} 表出错，请检查！！！！！", name));
                }
                return config;
            }
        }
        return default(T);
    }

}
