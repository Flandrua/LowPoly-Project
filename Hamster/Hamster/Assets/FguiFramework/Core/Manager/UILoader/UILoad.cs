using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FguiFramework;
using FairyGUI;


public class UILoad 
{
    public UILoad()
    {
        UIConfig.modalLayerColor = new Color(0, 0, 0, 150);
        Init();
    }

    private void Init()
    {
        new UICfg();
        loadUIPacket();
    }

    private void loadUIPacket()
    {
        var globalEntryArr = new List<PanelEntry>();
        var globalPanel = new List<string>();
        foreach(var entry in UICfg.RegisterPanels.Values)
        {
            foreach(var panelEntry in entry)
            {
                var temp = panelEntry.PkgName.Split('_');
                if (temp[0].Replace("UI/", "").Equals("Global"))
                {
                    globalEntryArr.Add(panelEntry);
                    var panelName = panelEntry.PkgName.Replace("UI/", "");
                    globalPanel.Add(panelName);
                }
            }
           
        }
        UIManager2D.Instance.GlobalPanels = globalPanel;
        for (var i = 0; i < globalEntryArr.Count; i++)
        {
            UIPackage.AddPackage(globalEntryArr[i].PkgName);
        }
        LoadManager.Instance.LoadGameScene("GameScene", () =>
         {
             UIManager2D.Instance.OpenPanel(MainView.Name);
         });
        //UIManager2D.Instance.OpenPanel(ToastView.Name);
    }

}
