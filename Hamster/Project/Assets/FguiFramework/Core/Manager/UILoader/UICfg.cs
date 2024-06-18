using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelEntry
{
    private string _panelName;
    private string _pkgName;
    private UILayerEnum _order;
    private bool _isCloseDestroy;
    private bool _isCloseRemovePkg;

    public string PanelName { get => _panelName; set => _panelName = value; }
    public string PkgName { get => _pkgName; set => _pkgName = value; }
    public UILayerEnum Order { get => _order; set => _order = value; }
    public bool IsCloseDestroy { get => _isCloseDestroy; set => _isCloseDestroy = value; }
    public bool IsCloseRemovePkg { get => _isCloseRemovePkg; set => _isCloseRemovePkg = value; }
}

public class UICfg
{
    private static Dictionary<string, HashSet<PanelEntry>> _registerPanels;
    private static bool _isDynamic = true;

    /// <summary>
    /// 设计分辨率X
    /// </summary>
    public static int DefaultResolutionX = 1920;
    /// <summary>
    /// 设计分辨率Y
    /// </summary>
    public static int DefaultResolutionY = 1080;
    public static bool IsDynamic { get => _isDynamic; set => _isDynamic = value; }
    public static Dictionary<string, HashSet<PanelEntry>> RegisterPanels { get => _registerPanels; set => _registerPanels = value; }

    public UICfg()
    {
        initConfig();
    }



    private void initConfig()
    {
        _registerPanels = new Dictionary<string, HashSet<PanelEntry>>();
        //公共包注册，一般不放面板，放置的公共资源均可跨包使用，将会在游戏一开始就加载
        PanelRegister("Global_Common");

        //注册主面板，可以在相同包里加载多个面板，但是保证面板名唯一        
        PanelRegister("Main_LoadView", LoadView.Name);
        PanelRegister("Main_MainView", MainView.Name);
        PanelRegister("Main_MainView", ExitView.Name);


        PanelRegister("Main_ExamView", ErrorView.Name);
        PanelRegister("Main_ExamView", ExamView.Name);
        PanelRegister("Main_ExamView", ExamTipsView.Name);
        PanelRegister("Main_ExamView", ResultView.Name);
    }
    /// <summary>
    /// 面板注册
    /// </summary>
    /// <param name="pkgName">包名</param>
    /// <param name="panelName">面板名</param>
    /// <param name="order">层级</param>
    /// <param name="isCloseDestroy">关闭是否销毁</param>
    /// <param name="isCloseRemovePkg">关闭移除包</param>
    private void PanelRegister(string pkgName, string panelName  = "", UILayerEnum order = UILayerEnum.UI_LAYER_0, bool isCloseDestroy = false, bool isCloseRemovePkg = false)
    {
        var panelEntry = new PanelEntry();
        panelEntry.PkgName = string.Format("UI/{0}", pkgName);
        panelEntry.PanelName = panelName;
        panelEntry.Order = order;
        panelEntry.IsCloseDestroy = isCloseDestroy;
        panelEntry.IsCloseRemovePkg = isCloseRemovePkg;
        HashSet<PanelEntry> tempHash = new HashSet<PanelEntry>();
        if (_registerPanels.ContainsKey(pkgName))
        {
            tempHash = _registerPanels[pkgName];
            tempHash.Add(panelEntry);
        }
        else
        {
            tempHash.Add(panelEntry);
            _registerPanels.Add(pkgName, tempHash);
        }

    }
}
