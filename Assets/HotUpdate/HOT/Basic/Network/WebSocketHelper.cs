using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using UnityEngine;
using UnityWebSocket;
using System;

public class WebSocketHelper
{
    public bool isLog = true;
    private string address;
    private IWebSocket socket;

    public delegate void ReceiveDelegate(string message);
    public event ReceiveDelegate ReceiveEvent;

    public WebSocketHelper(string address)
    {
        this.address = address;
    }

    public WebSocketHelper Connect() {
        socket = new WebSocket(address);
        socket.OnOpen += Socket_OnOpen;
        socket.OnMessage += Socket_OnMessage;
        socket.OnClose += Socket_OnClose;
        socket.OnError += Socket_OnError;
        socket.ConnectAsync();
        return this;
    }

    public void Send(string content) {
        socket.SendAsync(content);
    }

    public void Close() {
        if (socket != null && socket.ReadyState != WebSocketState.Closed)
        {
            socket.CloseAsync();
        }
    }

    private void OnApplicationQuit()
    {
        Close();
    }

    private void Socket_OnOpen(object sender, OpenEventArgs e)
    {
        Log.Append("WebSocket连接", address);
    }

    private void Socket_OnMessage(object sender, MessageEventArgs e)
    {
        if (e.IsBinary)
        {
            Log.Append("WebSocket接收", address, e.Data, e.Data.Length.ToString());
        }
        else if (e.IsText)
        {
           Log.Append("WebSocket接收", address, e.Data);
            ReceiveEvent?.Invoke(e.Data);
        }
    }

    private void Socket_OnClose(object sender, CloseEventArgs e)
    {
        Log.Append("WebSocket关闭", address, e.StatusCode + e.Reason);
    }

    private void Socket_OnError(object sender, ErrorEventArgs e)
    {
        Log.Append("WebSocket错误", address, e.Message);
    }
}
