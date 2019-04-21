using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using System.Text;
using System.Net;
using System;

public class SocketClient : MonoBehaviour
{
    public string Host = "192.168.31.180";
    public int Port = 8888;

    private IPAddress ipAddress;
    private IPEndPoint ipEndPoint;

    private Socket sendSocket;
    private Socket receiveSocket;

    private Thread sendThread;
    private Thread receiveThread;

    private List<string> messages = new List<string>();
    public delegate void ReceiveHandler(string msg);
    public event ReceiveHandler OnReceived;
    private bool isConnected;

    private void Start()
    {
        ipAddress = IPAddress.Parse(Host);
        ipEndPoint = new IPEndPoint(ipAddress, Port);

        sendSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        receiveSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        StartClient();
    }

    private void StartClient()
    {
        sendThread = new Thread(Send);
        sendThread.IsBackground = true;
        sendThread.Start();
        receiveThread = new Thread(Receive);
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    public void Send(string msg)
    {
        messages.Add(msg);
    }

    private void Send()
    {
        while (true)
        {
            if (!sendSocket.Connected)
            {
                sendSocket.Connect(ipEndPoint);
            }
            isConnected = sendSocket.Connected;
            try
            {
                if (messages.Count > 0)
                {
                    var msgs = messages.ToArray();
                    messages.Clear();
                    foreach (var msg in msgs)
                    {
                        var count = sendSocket.Send(Encoding.ASCII.GetBytes(msg));
                    }
                }
            }
            catch (Exception err)
            {
                Debug.LogError(err.Message);
            }
        }
    }

    private void Receive()
    {
        while (true)
        {
            if (!receiveSocket.Connected)
            {
                receiveSocket.Connect(ipEndPoint);
            }
            try
            {
                byte[] raw = new byte[4096];
                int byteCount = receiveSocket.Receive(raw);
                OnReceived?.Invoke(Encoding.ASCII.GetString(raw, 0, byteCount));
            }
            catch (Exception err)
            {
                Debug.LogError(err.Message);
                break;
            }
        }
    }

    private void OnDestroy()
    {
        sendThread.Abort();
        receiveThread.Abort();
        sendSocket.Close();
        receiveSocket.Close();
    }
}
