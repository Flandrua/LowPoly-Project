using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>  
/// 定时任务封装类  
/// </summary>  
public class TimeTask
{
    /// <summary>  
    /// 延迟时间  
    /// </summary>  
    private float _timeDelay;
    /// <summary>  
    /// 延迟时间  
    /// </summary>  
    private float _timeDelayOnly;
    /// <summary>  
    /// 是否需要重复执行  
    /// </summary>  
    private bool _repeat;
    /// <summary>
    /// 是否忽略引擎TimeScale
    /// </summary>
    private bool _ignoreTimeScale;
    /// <summary>  
    /// 回调函数  
    /// </summary>  
    private TimeTaskDelegate _timeTaskCallBack;
    /// <summary>
    /// 本体
    /// </summary>
    private object _target;


    public float timeDelay
    {
        get { return _timeDelay; }
        set { _timeDelay = value; }
    }
    public float timeDelayOnly
    {
        get { return _timeDelayOnly; }
    }
    public bool repeat
    {
        get { return _repeat; }
        set { _repeat = value; }
    }
    public TimeTaskDelegate timeTaskCallBack
    {
        get { return _timeTaskCallBack; }
    }

    public bool IgnoreTimeScale { get => _ignoreTimeScale; set => _ignoreTimeScale = value; }
    public object Target { get => _target; set => _target = value; }

    //构造函数  
    public TimeTask(float timeDelay, bool repeat, TimeTaskDelegate timeTaskCallBack, object target,bool ignoreTimeScale=true)
    {
        _timeDelay = timeDelay;
        _timeDelayOnly = timeDelay;
        _repeat = repeat;
        _timeTaskCallBack = timeTaskCallBack;
        _ignoreTimeScale = ignoreTimeScale;
        Target = target;
    }

    public TimeTask(float timeDelay, TimeTaskDelegate timeTaskCallBack, object target) : this(timeDelay, true, timeTaskCallBack,target) { }
}
public delegate void TimeTaskDelegate();
public class TimeManager : MonoSingleton<TimeManager>
{
    /// <summary>  
    /// 定时任务列表  
    /// </summary>  
    private List<TimeTask> taskList = new List<TimeTask>();

    public override void Init()
    {
        DontDestroyOnLoad(this);
    }

    void Start()
    {

    }

    /// <summary>  
    /// 添加定时任务  
    /// </summary>  
    /// <param name="timeDelay">延时执行时间间隔</param>  
    /// <param name="repeat">是否可以重复执行</param>  
    /// <param name="timeTaskCallback">执行回调</param>  
    /// <param name="ignoreTimeScale">是否试用真实时间</param>  
    public void AddTask(float timeDelay, bool repeat, TimeTaskDelegate timeTaskCallback,object target,bool ignoreTimeScale = true)
    {
        AddTask(new TimeTask(timeDelay, repeat, timeTaskCallback, target,ignoreTimeScale));
    }
    public void AddTask(TimeTask taskToAdd)
    {
        if (taskList.Contains(taskToAdd) || taskToAdd == null) return;
        taskList.Add(taskToAdd);
    }

    /// <summary>  
    /// 移除定时任务  
    /// </summary>  
    /// <param name="taskToRemove"></param>  
    /// <returns></returns>  
    public bool RemoveTask(TimeTaskDelegate taskToRemove,object target)
    {
        if (taskList.Count == 0 || taskToRemove == null) return false;
        for (var i = 0; i < taskList.Count; i++)
        {
            TimeTask item = taskList[i];
            if (item.timeTaskCallBack == taskToRemove && item.Target == target)
                return taskList.Remove(item);
        }
        return false;
    }

    void Update()
    {
        Tick();
    }

    /// <summary>  
    /// 执行定时任务  
    /// </summary>  
    private void Tick()
    {
        if (taskList == null) return;
        for (var i = 0; i < taskList.Count;)
        {
            TimeTask task = taskList[i];
            if (task.IgnoreTimeScale)
            {
                task.timeDelay -= Time.unscaledDeltaTime;
            }
            else
            {
                task.timeDelay -= Time.deltaTime;
            }
            if (task.timeDelay <= 0)
            {
                if (task.timeTaskCallBack != null)
                {
                    task.timeTaskCallBack();
                }
                if (!task.repeat)
                {
                    taskList.Remove(task);
                    continue;
                }
                task.timeDelay = task.timeDelayOnly;
            }
            i++;
        }
    }
}
