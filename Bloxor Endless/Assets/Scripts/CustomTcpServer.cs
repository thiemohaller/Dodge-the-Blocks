using Assets;
using System;
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
    public volatile BlockSpawner spawner;
    
    private Thread serverThread;
    private static CustomTcpServer instance = null;

    private void Awake() {
        if (instance != null) {
            Destroy(gameObject);
        } else {
            instance = this;
            DontDestroyOnLoad(gameObject);
            if (serverThread == null) {
                serverThread = new Thread(() => RunServer());
                serverThread.Start();
                Debug.Log("Starting server");
            }
        }

        spawner = GameObject.Find("BlockSpawner").GetComponent<BlockSpawner>();
        if (spawner == null) {
            serverThread.Join();
            serverThread = null;
            Debug.Log("Spawner is required.");
        }
    }

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

                if (!string.IsNullOrEmpty(stringReceived) && spawner != null) {                    
                    var dataReceived = int.Parse(stringReceived);

                    // replace this with a `level` system, utilizing timebetweenspawns, spawnpoints, different prefabs, maybe layers? 
                    if (dataReceived < 10) {
                        spawner.timeBetweenSpawns = 2f;
                    } else if (dataReceived < 20) {
                        spawner.timeBetweenSpawns = 1.25f;
                    } else if (dataReceived < 30) {
                        spawner.timeBetweenSpawns = 0.9f;
                    } else if (dataReceived < 50) {
                        spawner.timeBetweenSpawns = .5f;
                    }

                    spawner.objectSpeedMultiplier = dataReceived;
                }
            }
        }
    }

    public void Notify(BlockSpawner newSpawner) {
        spawner = newSpawner;
    }
}
