using System;
using System.Collections;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class BlinkClient : MonoBehaviour
{
    private TcpClient client;
    private NetworkStream stream;
    private Thread receiveThread;
    private bool isConnected = false;
    private string latestMessage = "";
    public MeatGenerator m;
    void Start()
    {
        ConnectToServer("127.0.0.1", 9999);
    }

    void Update()
    {
        if (!string.IsNullOrEmpty(latestMessage))
        {
            Debug.Log("Received: " + latestMessage);

            if (latestMessage.Contains("Blink"))
            {
                m.SpawnM();

                // هنا تسوي أي شيء في يونتي
                Debug.Log("👀 Player blink detected!");
            }

            latestMessage = ""; // مسح الرسالة بعد الاستخدام
        }
    }

    void OnApplicationQuit()
    {
        Disconnect();
    }

    private void ConnectToServer(string host, int port)
    {
        try
        {
            client = new TcpClient(host, port);
            stream = client.GetStream();
            isConnected = true;

            receiveThread = new Thread(ReceiveData);
            receiveThread.IsBackground = true;
            receiveThread.Start();

            Debug.Log("Connected to server " + host + ":" + port);
        }
        catch (Exception e)
        {
            Debug.LogError("Connection error: " + e.Message);
        }
    }

    private void ReceiveData()
    {
        byte[] buffer = new byte[1024];
        while (isConnected)
        {
            try
            {
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead == 0) continue;

                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                latestMessage = message; // نخزنها عشان نقراها في Update
            }
            catch (Exception e)
            {
                Debug.LogError("Receive error: " + e.Message);
                isConnected = false;
            }
        }
    }


    private void Disconnect()
    {
        isConnected = false;
        if (stream != null) stream.Close();
        if (client != null) client.Close();
        if (receiveThread != null) receiveThread.Abort();
    }
}
