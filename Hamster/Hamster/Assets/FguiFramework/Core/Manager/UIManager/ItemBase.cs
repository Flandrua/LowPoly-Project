using FairyGUI;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public abstract class ItemBase 
{
	public GComponent comp;
	public Dictionary<string, GObject> UiObjs = new Dictionary<string, GObject>();
	public Dictionary<string, Controller> UiCtrls = new Dictionary<string, Controller>();
	public Dictionary<string, Transition> UiTions = new Dictionary<string, Transition>();
	public Dictionary<string, int> CompNum = new Dictionary<string, int>();
	private Dictionary<string, GObject> tempObjList = new Dictionary<string, GObject>();
	public ItemBase(GComponent comp)
    {
		this.comp = comp;
		for (var i = 0; i < comp.numChildren; i++)
		{
			var gObject = comp.GetChildAt(i);
			UiObjs.Add(gObject.name, gObject);
		}
		for (var i = 0; i < comp.Controllers.Count; i++)
		{
			UiCtrls.Add(comp.Controllers[i].name, comp.Controllers[i]);
		}
        for (var i = 0; i < comp._transitions.Count; i++)
        {
            UiTions.Add(comp._transitions[i].name, comp._transitions[i]);
        }
        foreach (var compName in UiObjs.Keys)
        {
            GObject gObj = UiObjs[compName];

            GComponent gComp = gObj.asCom;

            if (gComp != null && gComp.Controllers != null)
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
		AutoBindChild();
		Init();
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
			if (tempComp != null && tempComp.asCom != null && tempComp.asCom.Controllers != null)
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

	public abstract void Init();
}
