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
    public float timeBetweenDespawn = 6f; // TODO further testing
    private float timeToSpawn = 2f;
    private float timeToDespawn = 15f;

    void FixedUpdate() {
        if(Time.time >= timeToSpawn) {
            SpawnerLogic();
            timeToSpawn = Time.time + timeBetweenSpawns;
        }

        var forwardForce = objectSpeed * objectSpeedMultiplier * Time.fixedDeltaTime;
        foreach(var block in spawnedObjects) {
            block.GetComponent<Rigidbody>().AddForce(0, 0, -forwardForce);
        }

        if(Time.time >= timeToDespawn) {
            DespawnObject();
            timeToDespawn = Time.time + timeBetweenDespawn;
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

    private void DespawnObject() {
        var amountOfObjectsToDespawn = spawnPoints.Length;
        var listToDespawn = spawnedObjects.GetRange(0, amountOfObjectsToDespawn);
        foreach (var item in listToDespawn) {
            Destroy(item);
        }
        spawnedObjects.RemoveRange(0, amountOfObjectsToDespawn);
    }

}
 