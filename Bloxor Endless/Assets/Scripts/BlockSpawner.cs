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
    public float obstacleSpeed = 8000f;
    public float timeBetweenSpawns = 2f;
    private float timeToSpawn = 2f;
    // Start is called before the first frame update
    void Update() {
        if(Time.time >= timeToSpawn) {
            SpawnerLogic();
            timeToSpawn = Time.time + timeBetweenSpawns;
        }
    }

    void SpawnerLogic() {
        var amountOfSpawnPoints = spawnPoints.Length;
        var randomNumber = new System.Random();
        var filteredList = Enumerable.Range(0, amountOfSpawnPoints).OrderBy(x => randomNumber.Next()).Take(3).ToList();
        var randomIndex = Random.Range(0, amountOfSpawnPoints);
        var randomIndex2 = Random.Range(0, amountOfSpawnPoints);
        var randomIndex3 = Random.Range(0, amountOfSpawnPoints);
        var forwardForce = obstacleSpeed * Time.fixedDeltaTime;
        GameObject currentObstacle;

        for (int i = 0; i < amountOfSpawnPoints; i++) {
            if (!filteredList.Contains(i)) {
                currentObstacle = Instantiate(blockPrefab, spawnPoints[i].position, Quaternion.identity);
            }
        }
    }
}
 