using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        EventManager.AddListener(EventCommon.CHANGE_TIME, ChangeTime);
    }
    private void OnDestroy()
    {
        EventManager.RemoveListener(EventCommon.CHANGE_TIME, ChangeTime);
    }

    private void ChangeTime()
    {
        //需要重置小鼠的状态（各种bool，动画状态
        //需要重置玩家的状态
        //需要重置键盘的状态（可互动

        //如果一天过去了
        //小鼠回复血量
        //玩家如果吃了零食，减去1点工作效率
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
