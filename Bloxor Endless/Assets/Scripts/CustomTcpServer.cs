using Assets;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class CustomTcpServer : MonoBehaviour {
    public string LocalIPAdress = ProjectConstants.TCP_IP;
    public int Port = ProjectConstants.TCP_PORT;
    private Thread _serverThread;
    static CustomTcpServer instance = null;

    private void Awake() {
        if (instance != null) {
            Destroy(gameObject);
        } else {
            instance = this;
            GameObject.DontDestroyOnLoad(gameObject);
            if (_serverThread == null) {
                _serverThread = new Thread(() => RunServer());
                _serverThread.Start();
                Debug.Log("Starting server");
            }
        }         
    }
    
    /*
    private void OnDestroy() {
        _serverThread.Join();
        _serverThread = null;
    }
    */

    void RunServer() {
        var localAdd = IPAddress.Parse(LocalIPAdress);
        var listener = new TcpListener(localAdd, Port);
        listener.Start();
        Debug.Log($"TCP server is listening at {localAdd}:{Port}.");

        // client connects
        var client = listener.AcceptTcpClient();
        Debug.Log($"Client connected with IP adress: {client.Client.RemoteEndPoint}");
        
        // get data and read stream (in our case, should be something like "speed")
        var networkStream = client.GetStream();

        while (true) {
            if (networkStream.DataAvailable) {
                var buffer = new byte[client.ReceiveBufferSize];
                var bytesRead = networkStream.Read(buffer, 0, client.ReceiveBufferSize);
                var stringReceived = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                Debug.Log($"Received string `{stringReceived}` from client with IP {client.Client.RemoteEndPoint}.");

                // answer
                //var response = "ACK"; // TODO possible errors -> different message
                //networkStream.Write(buffer, 0, response.Length);
            }
        }
    }
}
