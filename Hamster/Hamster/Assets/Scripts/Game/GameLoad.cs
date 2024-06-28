using FairyGUI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityDecryptDllWaterDrop;
using UnityEngine;

public class GameLoad : MonoBehaviour
{
    public Font font = null;
    
    void Awake()
    {
        CheckMac();
        var configs = Resources.LoadAll("Config");
        DataManager.Instance.LoadConfig(configs);
        DataCenter.Instance.InitData();
        LanguageManager.Instance.InitLang();

        //设置字体
        DynamicFont dynamicFont = new DynamicFont("font", font);
        FontManager.RegisterFont(dynamicFont, "font");
        UIConfig.defaultFont = "font";
        //GameSocketManager.Instance.Connect("ws://192.168.200.125:8049/game/1");

        new UILoad();
    }

    private void CheckMac()
    {
        string localMac = MachineCodeHandler.GetMachineCode();
        byte[] encryptedByte = GetEncryptedByte();
        if (encryptedByte != null)
        {
            string decryptMac = MachineCodeHandler.Decrypt(encryptedByte, "abcd1234");
            LogManager.Instance.Log($"decryptMac:{decryptMac}");
            LogManager.Instance.Log($"localMac:{localMac}");
            if (!localMac.Equals(decryptMac))
            {
                Application.Quit();
            }
            else
            {
                LogManager.Instance.Log($"正确的地址，可以运行");
            }
        }
        else
        {
            Application.Quit();
        }
    }

    private byte[] GetEncryptedByte()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "encrypted.bin");
        byte[] binaryData = null;

        if (File.Exists(filePath))
        {
            binaryData = File.ReadAllBytes(filePath);
        }
        else
        {
            LogManager.Instance.Log($"未找到encrypted.bin");
        }
        return binaryData;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
