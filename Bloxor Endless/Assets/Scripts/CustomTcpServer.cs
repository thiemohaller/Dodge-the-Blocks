using Assets;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class CustomTcpServer : MonoBehaviour {
    public string LocalIPAdress = ProjectConstants.TCP_IP;
    public int Port = ProjectConstants.TCP_PORT;
    public volatile BlockSpawner spawner;
    public double DeltaDistance { get; set; }

    private Thread serverThread;
    private static CustomTcpServer instance = null;
    private volatile bool stopThread = false;
    private DeathCounter deathCounter;

    private void Awake() {
        // singleton design pattern
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

        deathCounter = GetComponent<DeathCounter>();
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
        var iterationCounter = 0;

        while (!stopThread) {
            try { 
                if (iterationCounter % 10 == 0) {
                    var stringToSend = deathCounter.ResetCounter.ToString() + '/' + DeltaDistance.ToString();
                    var bytesToSend = Encoding.ASCII.GetBytes(stringToSend);
                    networkStream.Write(bytesToSend, 0, bytesToSend.Length);
                    Debug.Log($"Send string `{stringToSend}`.");
                }

                if (networkStream.DataAvailable) {
                    var buffer = new byte[client.ReceiveBufferSize];
                    var bytesRead = networkStream.Read(buffer, 0, client.ReceiveBufferSize);
                    var stringReceived = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    Debug.Log($"Received string `{stringReceived}` from client with IP {client.Client.RemoteEndPoint}.");

                    if (!string.IsNullOrEmpty(stringReceived) && spawner != null) {                    
                        var dataReceived = int.Parse(stringReceived);

                        spawner.objectSpeedMultiplier = dataReceived;

                        // replace this with a `level` system, utilizing timebetweenspawns, spawnpoints, different prefabs, maybe layers (=rapid successive waves of spawns)? 
                        if (dataReceived < 10) {
                            spawner.timeBetweenSpawns = 2f;
                            spawner.freeSpaces = 2;
                        } else if (dataReceived < 12) {
                            spawner.timeBetweenSpawns = 1.5f;
                            spawner.freeSpaces = 2;
                        }else if (dataReceived < 20) {
                            spawner.timeBetweenSpawns = 1.25f;
                            spawner.freeSpaces = 2;
                        } else if (dataReceived < 30) {
                            spawner.timeBetweenSpawns = 1f;
                            spawner.freeSpaces = 2;
                        } else if (dataReceived < 50) {
                            spawner.timeBetweenSpawns = .5f;
                            spawner.freeSpaces = 2;
                        }
                    }
                }

                Thread.Sleep(100);

                iterationCounter += 1;
            } catch (Exception ex) {
                Debug.Log($"Exception occured: {ex.Message}, {ex.StackTrace}");
            }
        }

        networkStream.Close();
        client.Close();
        listener.Stop();
    }

    public void Notify(BlockSpawner newSpawner) {
        spawner = newSpawner;
    }

    void OnApplicationQuit() {
        Destroy(this);
        //stopThread = true;
        //serverThread.Join();
    }
}
