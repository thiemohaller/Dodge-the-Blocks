using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class BlockSpawner : MonoBehaviour {
    public Transform[] spawnPoints;
    public GameObject blockPrefab;
    public int freeSpaces = 1;
    [Range(1, 50)]
    public int objectSpeedMultiplier = 4;
    public float objectSpeed = 1000f;
    public List<GameObject> spawnedObjects;
    public float timeBetweenSpawns = 2f;
    private float timeToSpawn = 2f;
    private int maximumAmountOfObstacles = 80;

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
        var amountOfSpawnPoints = spawnPoints.Length;
        var randomNumber = new System.Random();
        var filteredList = Enumerable.Range(0, amountOfSpawnPoints).OrderBy(x => randomNumber.Next()).Take(3).ToList();
        var randomIndex = Random.Range(0, amountOfSpawnPoints);
        var randomIndex2 = Random.Range(0, amountOfSpawnPoints);
        var randomIndex3 = Random.Range(0, amountOfSpawnPoints);
        GameObject currentObstacle;

        for (int i = 0; i < amountOfSpawnPoints; i++) {
            if (!filteredList.Contains(i)) {
                currentObstacle = Instantiate(blockPrefab, spawnPoints[i].position, Quaternion.identity);
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
 