using LitJson;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public class GameSocketManager : Singleton<GameSocketManager>
{
    private string _url;
    private WebSocket webSocket = null;
    public string Url { get => _url; set => _url = value; }

    public override void Init()
    {

    }

    public void Connect(string url)
    {
        Url = url;
        webSocket = new WebSocket(url);
        webSocket.OnMessage += OnMessage;
        webSocket.OnClose += OnClose;
        webSocket.OnError += OnError;
        webSocket.OnOpen += OnOpen;

        webSocket.Connect();
    }
    private void OnOpen(object sender, EventArgs message)
    {
        LogManager.Instance.Log("连接成功"); 
    } 

    private void OnMessage(object sender,MessageEventArgs message)
    {
        string jsonStr = message.Data;
        LogManager.Instance.Log($"收到消息\n{jsonStr}");
        JObject json = JObject.Parse(jsonStr);
        string id = (string)json["id"];
        switch (id)
        {
            case SocketEvent.S2C_PING_PONG:
                int count = (int)json["count"];
                LogManager.Instance.Log($"总在线人数：{count}");
                break;
        }
    }

    private void SendMessage(object obj)
    {
        string str = JsonConvert.SerializeObject(obj);
        LogManager.Instance.Log($"发送消息\n{str}");
        webSocket.Send(str);
    }

    private void OnClose(object sender,CloseEventArgs message)
    {
        LogManager.Instance.Log("连接关闭"); 

    }

    private void OnError(object sender, ErrorEventArgs message)
    {
        LogManager.Instance.Log("连接出错"); 

    }
}
