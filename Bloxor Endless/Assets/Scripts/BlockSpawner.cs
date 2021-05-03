using Assets.Scripts.DTO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class BlockSpawner : MonoBehaviour {

    public Transform[] spawnPoints;
    public GameObject gapTriggerPrefab;
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
    private List<int> previousSpawns;
    private int iterationCounter = 0;
    private List<List<GameObject>> listOfSpawnedGapsPerIteration = new List<List<GameObject>>();
    private Dictionary<int, (GameObject, float)> closestColliderToPlayerInCurrentIteration = new Dictionary<int, (GameObject, float)>();
    private Dictionary<int, ClosestSpawnAndDistanceTravelledDto> dtoPerIterationDictionary = new Dictionary<int, ClosestSpawnAndDistanceTravelledDto>();
    private List<GameObject> collidersHit = new List<GameObject>();
    private GameObject player;
    private float xToClosestGap;
    private double distanceTravelled;
    private Vector3 previousPlayerPosition;
    private CustomTcpServer tcp;

    private void Awake() {
        if (instance != null && instance != this) {
            Destroy(gameObject);
        } else {
            instance = this;
        }

        tcp = GameObject.Find("TCPServer").GetComponent<CustomTcpServer>();
        tcp.Notify(this);
        
        previousSpawns = new List<int>();
        player = GameObject.Find("Player");
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

        var currentPlayerPosition = player.transform.position;
        distanceTravelled += Math.Abs(currentPlayerPosition.x - previousPlayerPosition.x);
        previousPlayerPosition = currentPlayerPosition;

        if(spawnedObjects.Count > maximumAmountOfObstacles) {
            DespawnObjects();
        }
    }

    void SpawnerLogic() {
        // Select 3 random empty spawnpoints, fill the rest with random objects
        var amountOfSpawnPoints = spawnPoints.Length;
        var randomNumber = new System.Random();
        var filteredList = Enumerable.Range(0, amountOfSpawnPoints)
            .Except(previousSpawns)
            .OrderBy(x => randomNumber.Next())
            .Take(freeSpaces).ToList();

        GameObject currentObstacle;
        
        previousSpawns.Clear();
        var spawnWaveGaps = new List<GameObject>();

        for (int i = 0; i < amountOfSpawnPoints; i++) {
            if (!filteredList.Contains(i)) {
                var randomPrefab = randomNumber.Next(blockPrefabs.Count);
                currentObstacle = Instantiate(blockPrefabs[randomPrefab], spawnPoints[i].position, Quaternion.identity);
            } else {
                // this yields a 10% chance of having the same hole twice in a row
                currentObstacle = Instantiate(gapTriggerPrefab, spawnPoints[i].position, Quaternion.identity);
                spawnWaveGaps.Add(currentObstacle);

                var rand = new System.Random();
                var chance = rand.Next(1, 101);
                
                if (chance <= 90) {
                    previousSpawns.Add(i);
                }
            }
            
            spawnedObjects.Add(currentObstacle);
        }

        listOfSpawnedGapsPerIteration.Add(spawnWaveGaps);
    }

    private void DespawnObjects() {        
        var amountOfObjectsToDespawn = maximumAmountOfObstacles / 2;
        var listToDespawn = spawnedObjects.GetRange(0, amountOfObjectsToDespawn);

        foreach (var item in listToDespawn) {
            Destroy(item);
        }

        spawnedObjects.RemoveRange(0, amountOfObjectsToDespawn);
    }

    public void Notify(GameObject gameObject) {
        collidersHit.Add(gameObject);
        var spawnedGaps = listOfSpawnedGapsPerIteration.FirstOrDefault();
        tcp.DeltaDistance = Math.Abs(xToClosestGap - distanceTravelled);
        listOfSpawnedGapsPerIteration.Remove(spawnedGaps);

        var listOfDistances = new List<float>();
        foreach (var gap in spawnedGaps) {
            var distance = Math.Abs(gap.transform.position.x - previousPlayerPosition.x);
            listOfDistances.Add(distance);
        }

        Debug.Log($"Previous distance travelled: {distanceTravelled}");
        listOfDistances.Sort();
        xToClosestGap = listOfDistances.FirstOrDefault();
        distanceTravelled = 0;
        Debug.Log($"Distance to closest gap: {xToClosestGap}");
    }
}
 