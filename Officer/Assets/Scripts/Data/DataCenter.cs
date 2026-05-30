using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System;
using System.Security.Cryptography;

public class GameData
{
    private long _saveTime = 0;
    public long SaveTime { get => _saveTime; set => _saveTime = value; }

    private PlayerData _playerData = new PlayerData();
    public PlayerData PlayerData { get => _playerData; set => _playerData = value; }

    private HamsterData _hamsterData = new HamsterData();
    public HamsterData HamsterData { get => _hamsterData; set => _hamsterData = value; }


}

public class HamsterData
{
    public int hp = 10;
    public int favorability = 0;
}
public class PlayerData
{
    public int workProgress = 0;
    public int days = 1;
    public int fatigue = 0;
    public int workEfficiency = 1;
    public int favorabilityAbility = 1;
    public List<ItemData> ownedItem = new List<ItemData>();
}
public class DataCenter : Singleton<DataCenter>
{
    GameData _gameData;
    public GameData GameData { get => _gameData; set => _gameData = value; }
    /// <summary>
    /// Initialize player save data.
    /// </summary>
    public void InitData()
    {

        //PlayerPrefs.DeleteKey("GameData");
        string str = PlayerPrefs.GetString("GameData");
        if (string.IsNullOrEmpty(str))
        {
            NewData();
        }
        else
        {
            _gameData = JsonMapper.ToObject<GameData>(str);
            EnsureDataIntegrity();
        }
    }

    /// <summary>
    /// Save data to PlayerPrefs.
    /// </summary>
    public void SaveData()
    {
        _gameData.SaveTime = GetTimeStamp();
        var json = JsonMapper.ToJson(_gameData);
        PlayerPrefs.SetString("GameData", json);
        PlayerPrefs.Save();
        ;
    }

    /// <summary>
    /// Create a new game data snapshot.
    /// </summary>
    public void NewData()
    {
        _gameData = new GameData();
        EnsureDataIntegrity();
        SaveData();
    }

    private void EnsureDataIntegrity()
    {
        if (_gameData == null)
        {
            _gameData = new GameData();
        }

        if (_gameData.PlayerData == null)
        {
            _gameData.PlayerData = new PlayerData();
        }

        if (_gameData.HamsterData == null)
        {
            _gameData.HamsterData = new HamsterData();
        }

        if (_gameData.PlayerData.ownedItem == null)
        {
            _gameData.PlayerData.ownedItem = new List<ItemData>();
        }

        _gameData.PlayerData.days = Mathf.Max(1, _gameData.PlayerData.days);
        _gameData.PlayerData.fatigue = Mathf.Max(0, _gameData.PlayerData.fatigue);
        _gameData.PlayerData.workProgress = Mathf.Max(0, _gameData.PlayerData.workProgress);
    }

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

    public int GetTotalWorkEfficiency()
    {
        int totalWork = GameData.PlayerData.workEfficiency;
        if (GameData.PlayerData.ownedItem.Count > 0)
        {
            foreach (ItemData item in GameData.PlayerData.ownedItem)
            {
                totalWork += item.workEfficiency;
            }
        }
        return totalWork;
    }

    public int GetTotalFavorabilityAbility()
    {
        int totalFav = GameData.PlayerData.favorabilityAbility;
        if (GameData.PlayerData.ownedItem.Count > 0)
        {
            foreach (ItemData item in GameData.PlayerData.ownedItem)
            {
                totalFav += item.extraFavorability;
            }
        }
        return totalFav;
    }

    public void GetFavorability(int value)
    {
        GameData.HamsterData.favorability += value;
        Debug.Log($"Favorability:{value}");
        EventManager.DispatchEvent(EventCommon.UPDATE_MONITOR);
    }

    public void GetDamage(int value)
    {
        GameData.HamsterData.hp += value;
        Debug.Log($"Damage:{value}");
    }
    public void GetWorkEfficiency(int value)
    {
        GameData.PlayerData.workEfficiency += value;
        Debug.Log($"Efficiency:{value}");
        EventManager.DispatchEvent(EventCommon.UPDATE_MONITOR);
    }
    public void GetWorkProgress(int value)
    {
        GameData.PlayerData.workProgress += value;
        Debug.Log($"Progress:{value}");
    }

    public void AddFatigue(int value = 1)
    {
        if (GameData == null || GameData.PlayerData == null)
        {
            return;
        }

        GameData.PlayerData.fatigue = Mathf.Max(0, GameData.PlayerData.fatigue + value);
    }

    public void ResetFatigue()
    {
        if (GameData == null || GameData.PlayerData == null)
        {
            return;
        }

        GameData.PlayerData.fatigue = 0;
    }
}
