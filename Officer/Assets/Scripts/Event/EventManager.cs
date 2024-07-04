using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager 
{

    public delegate void EventCallBack();
    public delegate void EventCallBack<T>(T arg1);
    public delegate void EventCallBack<T, W>(T arg1, W arg2);
    public delegate void EventCallBack<T, W, E>(T arg1, W arg2, E arg3);
    public delegate void EventCallBack<T, W, E, R>(T arg1, W arg2, E arg3, R arg4);
    public delegate void EventCallBack<T, W, E, R, Y>(T arg1, W arg2, E arg3, R arg4, Y arg5);
    private static Dictionary<string, Delegate> m_EventTable = new Dictionary<string, Delegate>();

    //提取相同的代码段,使用Delegate就可以让方法通用(无论函数是否带参)
    static void AddEventListener(string eventType, Delegate callBack)
    {
        //如果类型已经存在(键相等)
        if (!m_EventTable.ContainsKey(eventType))
        {
            m_EventTable.Add(eventType, null);
        }
        //获取该回掉函数
        Delegate d = m_EventTable[eventType];
        //判断委托对象(函数)是不是空值，类型是否一致(带一个参，还是两个或者更多)
        if (d != null && d.GetType() != callBack.GetType())
        {
            throw new Exception(string.Format("尝试为事件{0}添加不同类型的委托，当前事件所对应的委托是{1}，要添加的委托是{2}", eventType, d.GetType(), callBack.GetType()));
        }
    }


    static void RemoveEventListener(string eventType, Delegate callBack)
    {
        if (m_EventTable.ContainsKey(eventType))
        {
            Delegate d = m_EventTable[eventType];
            if (d == null)
            {
                throw new Exception(string.Format("移除监听错误：事件{0}没有对应的委托", eventType));
            }
            else if (d.GetType() != callBack.GetType())
            {
                throw new Exception(string.Format("移除监听错误：尝试为事件{0}移除不同的类型的委托，当前委托类型为{1}，要移除的对象为{2}", eventType, d, callBack));
            }
        }
        else
        {
            throw new Exception(string.Format("移除监听错误：没有事件码{0}", eventType));
        }
    }

    //移除
    static void OnListenerRemove(string eventType)
    {
        if (m_EventTable[eventType] == null)
        {
            m_EventTable.Remove(eventType);
        }
    }
    /// <summary>
    /// 添加事件
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="callBack"></param>

    //无参的添加方法
    public static void AddListener(string eventType, EventCallBack callBack)
    {
        AddEventListener(eventType, callBack);
        //多播，当键相同后会产生多重广播(调用多个事件)
        m_EventTable[eventType] = (EventCallBack)m_EventTable[eventType] + callBack;
    }

    //一个参的添加方法
    public static void AddListener<T>(string eventType, EventCallBack<T> callBack)
    {
        AddEventListener(eventType, callBack);
        //多播，当键相同后会产生多重广播(调用多个事件)
        m_EventTable[eventType] = (EventCallBack<T>)m_EventTable[eventType] + callBack;
    }
    //两个参的添加方法
    public static void AddListener<T, W>(string eventType, EventCallBack<T, W> callBack)
    {
        AddEventListener(eventType, callBack);
        //多播，当键相同后会产生多重广播(调用多个事件)
        m_EventTable[eventType] = (EventCallBack<T, W>)m_EventTable[eventType] + callBack;
    }
    //三个参的添加方法
    public static void AddListener<T, W, E>(string eventType, EventCallBack<T, W, E> callBack)
    {
        AddEventListener(eventType, callBack);
        //多播，当键相同后会产生多重广播(调用多个事件)
        m_EventTable[eventType] = (EventCallBack<T, W, E>)m_EventTable[eventType] + callBack;
    }
    //四个参的添加方法
    public static void AddListener<T, W, E, R>(string eventType, EventCallBack<T, W, E, R> callBack)
    {
        AddEventListener(eventType, callBack);
        //多播，当键相同后会产生多重广播(调用多个事件)
        m_EventTable[eventType] = (EventCallBack<T, W, E, R>)m_EventTable[eventType] + callBack;
    }
    //五个参的添加方法
    public static void AddListener<T, W, E, R, Y>(string eventType, EventCallBack<T, W, E, R, Y> callBack)
    {
        AddEventListener(eventType, callBack);
        //多播，当键相同后会产生多重广播(调用多个事件)
        m_EventTable[eventType] = (EventCallBack<T, W, E, R, Y>)m_EventTable[eventType] + callBack;
    }


    /// <summary>
    /// 移除事件
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="callBack"></param>
    //无参的移除方法
    public static void RemoveListener(string eventType, EventCallBack callBack)
    {
        AddEventListener(eventType, callBack);
        m_EventTable[eventType] = (EventCallBack)m_EventTable[eventType] - callBack;
        OnListenerRemove(eventType);
    }
    //一个参的移除方法
    public static void RemoveListener<T>(string eventType, EventCallBack<T> callBack)
    {
        AddEventListener(eventType, callBack);
        m_EventTable[eventType] = (EventCallBack<T>)m_EventTable[eventType] - callBack;
        OnListenerRemove(eventType);
    }
    //两个参的移除方法
    public static void RemoveListener<T, W>(string eventType, EventCallBack<T, W> callBack)
    {
        AddEventListener(eventType, callBack);
        m_EventTable[eventType] = (EventCallBack<T, W>)m_EventTable[eventType] - callBack;
        OnListenerRemove(eventType);
    }
    //三个参的移除方法
    public static void RemoveListener<T, W, E>(string eventType, EventCallBack<T, W, E> callBack)
    {
        AddEventListener(eventType, callBack);
        m_EventTable[eventType] = (EventCallBack<T, W, E>)m_EventTable[eventType] - callBack;
        OnListenerRemove(eventType);
    }
    //四个参的移除方法
    public static void RemoveListener<T, W, E, R>(string eventType, EventCallBack<T, W, E, R> callBack)
    {
        AddEventListener(eventType, callBack);
        m_EventTable[eventType] = (EventCallBack<T, W, E, R>)m_EventTable[eventType] - callBack;
        OnListenerRemove(eventType);
    }
    //五个参的移除方法
    public static void RemoveListener<T, W, E, R, Y>(string eventType, EventCallBack<T, W, E, R, Y> callBack)
    {
        AddEventListener(eventType, callBack);
        m_EventTable[eventType] = (EventCallBack<T, W, E, R, Y>)m_EventTable[eventType] - callBack;
        OnListenerRemove(eventType);
    }

    /// <summary>
    /// 广播事件
    /// </summary>
    /// <param name="eventType"></param>

    //无参广播事件
    public static void DispatchEvent(string eventType)
    {
        Delegate d;
        if (m_EventTable.TryGetValue(eventType, out d))
        {
            EventCallBack callBack = d as EventCallBack;
            if (callBack != null)
            {
                callBack();
            }
            else
            {
                throw new Exception(string.Format("广播事件错误：事件{0}对应委托具有不同的类型", eventType));
            }
        }
    }

    //一个参广播事件
    public static void DispatchEvent<T>(string eventType, T arg)
    {
        Delegate d;
        if (m_EventTable.TryGetValue(eventType, out d))
        {
            EventCallBack<T> callBack = d as EventCallBack<T>;
            if (callBack != null)
            {
                callBack(arg);
            }
            else
            {
                throw new Exception(string.Format("广播事件错误：事件{0}对应委托具有不同的类型", eventType));
            }
        }
    }

    //两个参广播事件
    public static void DispatchEvent<T, W>(string eventType, T arg1, W arg2)
    {
        Delegate d;
        if (m_EventTable.TryGetValue(eventType, out d))
        {
            EventCallBack<T, W> callBack = d as EventCallBack<T, W>;
            if (callBack != null)
            {
                callBack(arg1, arg2);
            }
            else
            {
                throw new Exception(string.Format("广播事件错误：事件{0}对应委托具有不同的类型", eventType));
            }
        }
    }

    //三个参广播事件
    public static void DispatchEvent<T, W, E>(string eventType, T arg1, W arg2, E arg3)
    {
        Delegate d;
        if (m_EventTable.TryGetValue(eventType, out d))
        {
            EventCallBack<T, W, E> callBack = d as EventCallBack<T, W, E>;
            if (callBack != null)
            {
                callBack(arg1, arg2, arg3);
            }
            else
            {
                throw new Exception(string.Format("广播事件错误：事件{0}对应委托具有不同的类型", eventType));
            }
        }
    }

    //四个参广播事件
    public static void DispatchEvent<T, W, E, R>(string eventType, T arg1, W arg2, E arg3, R arg4)
    {
        Delegate d;
        if (m_EventTable.TryGetValue(eventType, out d))
        {
            EventCallBack<T, W, E, R> callBack = d as EventCallBack<T, W, E, R>;
            if (callBack != null)
            {
                callBack(arg1, arg2, arg3, arg4);
            }
            else
            {
                throw new Exception(string.Format("广播事件错误：事件{0}对应委托具有不同的类型", eventType));
            }
        }
    }

    //五个参广播事件
    public static void DispatchEvent<T, W, E, R, Y>(string eventType, T arg1, W arg2, E arg3, R arg4, Y arg5)
    {
        Delegate d;
        if (m_EventTable.TryGetValue(eventType, out d))
        {
            EventCallBack<T, W, E, R, Y> callBack = d as EventCallBack<T, W, E, R, Y>;
            if (callBack != null)
            {
                callBack(arg1, arg2, arg3, arg4, arg5);
            }
            else
            {
                throw new Exception(string.Format("广播事件错误：事件{0}对应委托具有不同的类型", eventType));
            }
        }
    }
}
