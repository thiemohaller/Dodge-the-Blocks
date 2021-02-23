using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class BlockSpawner : MonoBehaviour {

    public Transform[] spawnPoints;
    public List<GameObject> blockPrefabs = new List<GameObject>(4);
    [Range(1, 50)]
    public int objectSpeedMultiplier = 4;
    public int freeSpaces = 3;
    public float objectSpeed = 1000f;
    public float timeBetweenSpawns = 2f;
    public List<GameObject> spawnedObjects;

    private float timeToSpawn = 2f;
    private int maximumAmountOfObstacles = 80;
    private BlockSpawner instance;

    private void Awake() {
        if (instance != null && instance != this) {
            Destroy(gameObject);
        } else {
            instance = this;
        }

        var tcp = GameObject.Find("TCPServer").GetComponent<CustomTcpServer>();
        tcp.Notify(this);
    }

    void FixedUpdate() {        
        if(Time.time >= timeToSpawn) {
            SpawnerLogic();
            timeToSpawn = Time.time + timeBetweenSpawns;
        }

        var forwardForce = objectSpeed * objectSpeedMultiplier * Time.fixedDeltaTime;
        foreach(var block in spawnedObjects) {
            block.GetComponent<Rigidbody>().freezeRotation = true;
            block.GetComponent<Rigidbody>().AddForce(0, 0, -forwardForce);
        }

        if(spawnedObjects.Count > maximumAmountOfObstacles) {
            DespawnObjects();
        }
    }
    
    void SpawnerLogic() {
        // Select 3 random empty spawnpoints, fill the rest with random objects
        var amountOfSpawnPoints = spawnPoints.Length;
        var randomNumber = new System.Random();
        var filteredList = Enumerable.Range(0, amountOfSpawnPoints)
            .OrderBy(x => randomNumber.Next())
            .Take(freeSpaces).ToList();
        GameObject currentObstacle;
        
        for (int i = 0; i < amountOfSpawnPoints; i++) {
            if (!filteredList.Contains(i)) {
                var randomPrefab = randomNumber.Next(blockPrefabs.Count);
                currentObstacle = Instantiate(blockPrefabs[randomPrefab], spawnPoints[i].position, Quaternion.identity);
                spawnedObjects.Add(currentObstacle);
            }
        }
    }

    private void DespawnObjects() {        
        var amountOfObjectsToDespawn = maximumAmountOfObstacles / 2;
        var listToDespawn = spawnedObjects.GetRange(0, amountOfObjectsToDespawn);

        foreach (var item in listToDespawn) {
            Destroy(item);
        }

        spawnedObjects.RemoveRange(0, amountOfObjectsToDespawn);
    }

}
 