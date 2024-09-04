using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System;

public class GameData
{
    private long _saveTime = 0;
    private List<LockerType> _lockerTypes = new List<LockerType>();
    private List<Ability>_abilities= new List<Ability>();
    public long SaveTime { get => _saveTime; set => _saveTime = value; }
    public List<LockerType> LockerTypes { get => _lockerTypes; set => _lockerTypes = value; }
    public List<Ability> Abilities { get => _abilities; set => _abilities = value; }
}

public class DataCenter : Singleton<DataCenter>
{
    GameData _gameData;

    public GameData GameData { get => _gameData; set => _gameData = value; }
  
    public void InitData()
    {
        //PlayerPrefs.DeleteKey("GameData");
        string str = PlayerPrefs.GetString("GameData");
        if (string.IsNullOrEmpty(str))
        {
            _gameData = new GameData();
            SaveData();
        }
        else
        {
            _gameData = JsonMapper.ToObject<GameData>(str);
        }
}

    public void SaveData()
    {
        _gameData.SaveTime = GetTimeStamp(); 
        var json = JsonMapper.ToJson(_gameData);
        PlayerPrefs.SetString("GameData", json);
        PlayerPrefs.Save();
;    }

    private long GetTimeStamp()
    {
        TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return Convert.ToInt64(ts.TotalSeconds);
    }

    public Color HexToColor(string hex)
    {
        Color nowColor;
        ColorUtility.TryParseHtmlString(hex, out nowColor);
        return nowColor;
    }

}
