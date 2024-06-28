using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;
using DG.Tweening;
using FairyGUI;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace FguiFramework
{
	/// <summary>
	/// time:2020/1/26
	/// author:Card1ac
	/// description:UI面板基类
	/// </summary>
	public abstract class UIBase: Window
	{

		private string _panelName;
		private string closeBtnName = "btnClose";
		private object _paramData;
		private string _pkgName;
		private UILayerEnum _order;
		private bool _isCloseDestroy;
		private bool _isCloseRemovePkg;
		public Dictionary<string, GObject> UiObjs = new Dictionary<string, GObject>();
		public Dictionary<string, Controller> UiCtrls = new Dictionary<string, Controller>();
		public Dictionary<string, Transition> UiTions = new Dictionary<string, Transition>();
		public Dictionary<string, int> CompNum = new Dictionary<string, int>();
		public bool IsNeedShowAni = false;
		public bool IsNeedHideAni = false;
		public bool IsNeedModel = false;
		private Dictionary<string, GObject> tempObjList = new Dictionary<string, GObject> ();
		

		public string PanelName { get => _panelName; set => _panelName = value; }
        public object ParamData { get => _paramData; set => _paramData = value; }
        public string PkgName { get => _pkgName; set => _pkgName = value; }
        public UILayerEnum Order { get => _order; set => _order = value; }
        public bool IsCloseDestroy { get => _isCloseDestroy; set => _isCloseDestroy = value; }
        public bool IsCloseRemovePkg { get => _isCloseRemovePkg; set => _isCloseRemovePkg = value; }

		/// <summary>
		/// 初始化UI生命周期
		/// </summary>
		public abstract void OnInitUI();
		/// <summary>
		/// 添加事件生命周期
		/// </summary>
		public abstract void OnAddListener();
		/// <summary>
		/// 移除事件生命周期
		/// </summary>
		public abstract void OnRemoveListener();
		/// <summary>
		/// 每次打开面板生命周期
		/// </summary>
		/// <param name="ParamData">自定义传参</param>
		public abstract void OnOpen(object ParamData);
		/// <summary>
		/// 每次关闭面板生命周期
		/// </summary>
		public abstract void OnClose();
		/// <summary>
		/// 销毁面板生命周期
		/// </summary>
		public abstract void OnDestroy();
		
        protected override void OnShown()
        {
            base.OnShown();
            OnAddListener();
            OnOpen(ParamData);
            visible = true;
        }
        public override void Dispose()
        {
            OnDestroy();
            base.Dispose();
        }

		protected override void OnHide()
		{
			base.OnHide();
            OnRemoveListener();
            OnClose(); 
        }


	/// <summary>
	/// 面板初始化
	/// </summary>
	protected override void OnInit()
		{
			base.OnInit();
			var windObj = UIPackage.CreateObject(PkgName, PanelName);
			if (windObj == null)
			{
				throw new Exception("创建" + PanelName + "窗口页面失败");
			}
			contentPane = windObj.asCom;
			contentPane.SetSize(GRoot.inst.width, GRoot.inst.height);
			for (var i = 0; i < contentPane.numChildren; i++)
			{
				var gObject = contentPane.GetChildAt(i);
				UiObjs.Add(gObject.name, gObject);
			}
			for (var i = 0; i < contentPane.Controllers.Count; i++)
			{
				UiCtrls.Add(contentPane.Controllers[i].name, contentPane.Controllers[i]);
			}
			for (var i = 0; i < contentPane._transitions.Count; i++)
			{
				UiTions.Add(contentPane._transitions[i].name, contentPane._transitions[i]);
            }
			
            foreach (var compName in UiObjs.Keys)
            {
				GObject gObj = UiObjs[compName];

				GComponent gComp = gObj.asCom;

                if (gComp!=null && gComp.Controllers != null)
                    for (var i = 0; i < gComp.Controllers.Count; i++)
                    {
						if (UiCtrls.ContainsKey(gComp.Controllers[i].name))
						{
							if (CompNum.ContainsKey(gComp.asCom.Controllers[i].name))
							{
								CompNum[gComp.Controllers[i].name]++;
							}
							else
							{
								CompNum.Add(gComp.Controllers[i].name, 0);
							}
							UiCtrls.Add(string.Format("{0}_{1}", gComp.Controllers[i].name, CompNum[gComp.Controllers[i].name]), gComp.Controllers[i]);
						}
						else
						{
							UiCtrls.Add(gComp.Controllers[i].name, gComp.Controllers[i]);
						}
                    }
                if (gComp != null && gComp._transitions != null)
                    for (var i = 0; i < gComp._transitions.Count; i++)
                    {
						if (UiTions.ContainsKey(gComp._transitions[i].name))
						{
							if (CompNum.ContainsKey(gComp._transitions[i].name))
							{
								CompNum[gComp._transitions[i].name]++;
							}
							else
							{
								CompNum.Add(gComp._transitions[i].name, 0);
							}
							UiTions.Add(string.Format("{0}_{1}", gComp._transitions[i].name, CompNum[gComp._transitions[i].name]), gComp._transitions[i]);
						}
						else
						{
							UiTions.Add(gComp._transitions[i].name, gComp._transitions[i]);
						}
					}
				if (gComp != null)
					GetAllComps(gComp);
            }


			foreach (KeyValuePair<string, GObject> dic in tempObjList)
			{
				UiObjs.Add(dic.Key, dic.Value);
			}

            foreach (var compName in UiObjs.Keys)
            {
                var obj = UiObjs[compName];
                if (closeBtnName.Equals(obj.name))
                {
                    closeButton = obj;
                    break;
                }
            }

			AutoBindChild();
             
            pivotX = 0.5f;
            pivotY = 0.5f;
            sortingOrder = (int)_order;


            OnInitUI();
			modal = IsNeedModel;

		}

        protected void AutoBindChild()
        {
            FieldInfo[] fields = this.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			foreach (FieldInfo field in fields)
			{
				if (field.FieldType != typeof(Controller) && field.FieldType != typeof(Transition))
				{
					string childName = field.Name;
					if (UiObjs.ContainsKey(childName))
                    {
                        GObject child = UiObjs[childName];
                        field.SetValue(this, child);
                    } 
				}
                if (field.FieldType == typeof(Controller))
                {
                    string childName = field.Name;
                    if (UiCtrls.ContainsKey(childName))
                    {
                        Controller child = UiCtrls[childName];
                        field.SetValue(this, child);
                    }
                }
                if (field.FieldType == typeof(Transition))
                {
                    string childName = field.Name;
                    if (UiTions.ContainsKey(childName))
                    {
                        Transition child = UiTions[childName];
                        field.SetValue(this, child);
                    }
                }
            }
        }

        private void GetAllComps(GComponent comp)
        {
            for (var i = 0; i < comp.numChildren; i++)
            {
                var tempComp = comp.GetChildAt(i);
                if (UiObjs.ContainsKey(tempComp.name) || tempObjList.ContainsKey(tempComp.name))
                {
                    if (CompNum.ContainsKey(tempComp.name))
                    {
                        CompNum[tempComp.name]++;
                    }
                    else
                    {
                        CompNum.Add(tempComp.name, 0);
                    }
                    tempObjList.Add(string.Format("{0}_{1}", tempComp.name, CompNum[tempComp.name]), tempComp);
					if (tempComp.asCom != null)
						GetAllComps(tempComp.asCom);
                }
                else
                {
					tempObjList.Add(tempComp.name, tempComp);
					if (tempComp.asCom != null)
						GetAllComps(tempComp.asCom);
                }
				if (tempComp!=null && tempComp.asCom!=null && tempComp.asCom.Controllers != null)
				{
					for (var j = 0; j < tempComp.asCom.Controllers.Count; j++)
					{
						if (UiCtrls.ContainsKey(tempComp.asCom.Controllers[j].name))
						{
							if (CompNum.ContainsKey(tempComp.asCom.Controllers[j].name))
							{
								CompNum[tempComp.asCom.Controllers[j].name]++;
							}
							else
							{
								CompNum.Add(tempComp.asCom.Controllers[j].name, 0);
							}
							UiCtrls.Add(string.Format("{0}_{1}", tempComp.asCom.Controllers[j].name, CompNum[tempComp.asCom.Controllers[j].name]), tempComp.asCom.Controllers[j]);
						}
						else
						{
							UiCtrls.Add(tempComp.asCom.Controllers[j].name, tempComp.asCom.Controllers[j]);
						}
					}
				}
				if (tempComp != null && tempComp.asCom != null && tempComp.asCom._transitions != null)
				{
					for (var j = 0; j < tempComp.asCom._transitions.Count; j++)
					{
						if (UiTions.ContainsKey(tempComp.asCom._transitions[j].name))
						{
							if (CompNum.ContainsKey(tempComp.asCom._transitions[j].name))
							{
								CompNum[tempComp.asCom._transitions[j].name]++;
							}
							else
							{
								CompNum.Add(tempComp.asCom._transitions[j].name, 0);
							}
							UiTions.Add(string.Format("{0}_{1}", tempComp.asCom._transitions[j].name, CompNum[tempComp.asCom._transitions[j].name]), tempComp.asCom._transitions[j]);
						}
						else
						{
							UiTions.Add(tempComp.asCom._transitions[j].name, tempComp.asCom._transitions[j]);
						}
					}
				}
            }
        }

        /// <summary>
        /// 显示页面动画,可重写
        /// </summary>
        protected override void DoShowAnimation()
		{
			if (IsNeedShowAni)
			{
				if(!string.IsNullOrEmpty(FairyGUI.UIConfig.globalModalWaiting))
					GRoot.inst.ShowModalWait ();
				scale =  new Vector2(0.6f,0.6f);
				DOTween.To(() => scale, a => scale = a, Vector2.one, 0.3f)
					.SetEase(Ease.OutBounce).OnComplete(() =>
					{
						if (!string.IsNullOrEmpty(FairyGUI.UIConfig.globalModalWaiting))
						{
							GRoot.inst.CloseModalWait();
						}
						OnShown();
					})
					.SetUpdate(true)
					.SetTarget(this);
			}
			else
			{
				scale = Vector2.one;
				OnShown();
			}
		}

		/// <summary>
		/// 隐藏页面动画，可重写
		/// </summary>
		protected override void DoHideAnimation()
		{
			if (IsNeedHideAni)
			{
				DOTween.To(() => scale, a => scale = a, Vector2.zero, 0.3f)
					.OnComplete(() => { base.DoHideAnimation(); });
			}
			else
			{
				HideImmediately();
			}
		}


		public void Close()
		{
			if (this._isCloseDestroy)
			{
				Debug.LogError(PanelName);
	
			 UIManager2D.Instance.DisposeWind(PanelName);
				if (_isCloseRemovePkg)
				{
					UIPackage.RemovePackage(PkgName);
				}
				Dispose();
			}
			else
			{
				HideImmediately();
			}
		
		}



	}
}
