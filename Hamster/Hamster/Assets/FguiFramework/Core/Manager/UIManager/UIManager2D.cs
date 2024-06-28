

using System;
using System.Collections.Generic;
using DG.Tweening;
using FairyGUI;
using UnityEngine;

namespace FguiFramework
{

	/// <summary>
	/// time:2020/1/24
	/// author:Card1ac
	/// description:UI窗口基类
	/// </summary>
	public class UIManager2D : Singleton<UIManager2D> {

		/// <summary>
		/// 所有Panel
		/// </summary>
		private Dictionary<string, UIBase> _uIArray = new Dictionary<string, UIBase>();
		private List<string> _globalPanels = new List<string>();

        public List<string> GlobalPanels { get => _globalPanels; set => _globalPanels = value; }



		public override void Init()
		{
			//设置FGUI的分辨率
			GRoot.inst.SetContentScaleFactor(UICfg.DefaultResolutionX, UICfg.DefaultResolutionY);
			//关闭窗口自动排序
			FairyGUI.UIConfig.bringWindowToFrontOnClick = false;
			//设置RenderMode
			GRoot.inst.container.renderMode = RenderMode.WorldSpace;
		}

		/// <summary>
		/// 获取窗口页面
		/// </summary>
		/// <param name="uiName"></param>
		/// <returns></returns>
		public UIBase GetPanel(string uiName)
		{
			UIBase panel = null;
            if (_uIArray.ContainsKey(uiName))
            {
				panel = _uIArray[uiName];
            }
			return panel;
		}

		/// <summary>
		/// 创建UI
		/// </summary>
		/// <param name="uiName"></param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		public UIBase CreatePanel(string uiName)
		{
			UIBase panel = null;
			panel = Activator.CreateInstance(Type.GetType(uiName, true)) as UIBase;
			if (panel == null)
			{
				throw new Exception("不存在" + uiName + "页面");
			}
			return panel;
		}

		/// <summary>
		/// 得到所有处于打开状态的窗口页面
		/// </summary>
		/// <returns></returns>
		public List<string> GetAllOpenWindows()
		{
			List<string> list = new List<string>();
			foreach (string uiName in _uIArray.Keys)
			{
				if (IsOpenPanel(uiName))
				{
					list.Add(uiName);
				}
			}
			return list;
		}
		/// <summary>
		/// 关闭所有打开的面板
		/// </summary>
		public void CloseAllPanel()
		{
			GRoot.inst.CloseAllWindows();
		}


		/// <summary>
		/// 面板是否处于打开状态
		/// </summary>
		/// <param name="uiName"></param>
		/// <returns></returns>
		public bool IsOpenPanel(string uiName)
		{
			UIBase panel = GetPanel(uiName);
			if (panel != null)
			{
				return panel.isShowing;
			}
			return false;
		}

		/// <summary>
		/// 展示面板
		/// </summary>
		/// <param name="baseUi"></param>
		public void OpenPanel(string panelName,object param=null)
		{
			UIBase baseUi = GetPanel(panelName);
			if (baseUi == null)
			{
				baseUi = CreatePanel(panelName);
				_uIArray.Add(panelName, baseUi);
			}
			Debug.Log(string.Format("打开{0}面板", panelName));
			if (IsOpenPanel(panelName))
			{
				return;
			}
			PanelEntry panelEntry = null;
			foreach (var entrys in UICfg.RegisterPanels.Values)
            {
				foreach(var entry in entrys)
                {
                    if (entry.PanelName.Equals(panelName))
                    {
						panelEntry = entry;
						break;
                    }
                }
            }
            if (panelEntry == null)
            {
				Debug.LogError("没有" + panelName + "面板");
				return;
            }

			baseUi.ParamData = param;
			baseUi.PanelName = panelName;
			baseUi.PkgName = panelEntry.PkgName.Replace("UI/","");
			baseUi.Order = panelEntry.Order;
			baseUi.IsCloseDestroy = panelEntry.IsCloseDestroy;
			baseUi.IsCloseRemovePkg = panelEntry.IsCloseRemovePkg;
			var pkgObj = UIPackage.GetByName(baseUi.PkgName);
			if (UICfg.IsDynamic)
			{
				if (pkgObj == null)
				{	
					foreach(var entry in UICfg.RegisterPanels.Values)
                    {
						foreach(var temp in entry)
                        {
							if (temp.PanelName.Equals(panelName))
							{
								UIPackage.AddPackage(temp.PkgName);
								baseUi.Show();
								break;
							}
						}
                       
                    }
				}
				else
				{
					UIPackage.AddPackage(panelEntry.PkgName);
					baseUi.Show();
				}
			}
			else
			{
				if (pkgObj == null)
				{
					UIPackage.AddPackage(panelEntry.PkgName);
				}
				baseUi.Show();
			}
		}
		/// <summary>
		/// 根据包名获取资源URL，默认为公共包
		/// </summary>
		/// <param name="resName"></param>
		/// <param name="pkgName"></param>
		/// <returns></returns>
        public string GetTextureUrlByPkgName(string resName, string pkgName = null)
        {
            if (pkgName == null)
            {
                for (var i = 0; i < GlobalPanels.Count; i++)
                {
                    var url = UIPackage.GetItemURL(GlobalPanels[i], resName);
                    if (!string.IsNullOrEmpty(url))
                    {
                        return url;
                    }
                }
				return "";
            }
            else
            {
                var pkgObj = UIPackage.GetByName(pkgName);
                if (pkgObj == null)
                {
                    UIPackage.AddPackage(pkgName);
                }
                return UIPackage.GetItemURL(pkgName, resName);
            }
        }

        public void DisposeWind(string panelName)
        {
            _uIArray.Remove(panelName);
        }
		/// <summary>
		/// 关闭面板
		/// </summary>
		/// <param name="panelName"></param>
        public void ClosePanel(string panelName)
        {
            var baseUi = GetPanel(panelName);
            if (baseUi == null)
            {
                Debug.LogWarningFormat("{0}页面不存在！", panelName);
                if (_uIArray.ContainsKey(panelName))
                    _uIArray.Remove(panelName);
                return;
            }
            if (!IsOpenPanel(panelName))
            {
                return;
            }
            baseUi.HideImmediately();
        }

        /// <summary>
        /// 播放特效
        /// </summary>
        /// <param name="prefabName"></param>
        /// <param name="holder"></param>
        /// <param name="supportStencil"></param>
        public void PlayEffect(string prefabName, GGraph holder,Vector3 localPosition,Vector3 localScale, bool supportStencil = true)
		{
			UnityEngine.Object prefab = Resources.Load(string.Format("FguiEffect/{0}", prefabName));
			GameObject go = (GameObject)UnityEngine.Object.Instantiate(prefab);
			go.transform.localPosition = localPosition;
			go.transform.localScale = localScale;
			GoWrapper wrapper = new GoWrapper(go);
			holder.SetNativeObject(wrapper);
			wrapper.supportStencil = supportStencil;
		}

		/// <summary>
		/// 文本数值滚动动效
		/// </summary>
		/// <param name="compText">文本对象</param>
		/// <param name="str">字符串 "你的值：{0}"</param>
		/// <param name="startValue">开始的值</param>
		/// <param name="endValue">结束的值</param>
		/// <param name="duration">时间</param>
		public void DoTweenTextValue(GTextField compText, string str, int startValue, int endValue, float duration)
		{
			var start = startValue;
			DOTween.To(() => start, value => start = value, endValue, duration).OnUpdate(() => {
				compText.text = string.Format(str, start);
			}).SetUpdate(true);
		}
	}
 
}

