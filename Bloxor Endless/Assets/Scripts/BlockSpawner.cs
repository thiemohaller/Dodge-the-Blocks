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
    private Dictionary<int, List<GameObject>> spawnIterations = new Dictionary<int, List<GameObject>>();
    private Dictionary<int, (GameObject, float)> closestColliderToPlayerInCurrentIteration = new Dictionary<int, (GameObject, float)>();
    private Dictionary<int, ClosestSpawnAndDistanceTravelledDto> dtoPerIterationDictionary = new Dictionary<int, ClosestSpawnAndDistanceTravelledDto>();
    private List<GameObject> collidersHit = new List<GameObject>();
    private List<int> iterationsAlreadyProcessed = new List<int>();
    private GameObject player;

    private void Awake() {
        if (instance != null && instance != this) {
            Destroy(gameObject);
        } else {
            instance = this;
        }

        var tcp = GameObject.Find("TCPServer").GetComponent<CustomTcpServer>();
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

        CheckForXTravel();

        if(spawnedObjects.Count > maximumAmountOfObstacles) {
            DespawnObjects();
        }
    }

    private void CheckForXTravel() {
        var tempList = new List<GameObject>();
        foreach (var collider in collidersHit) {

            if (collider.GetComponent<Rigidbody>().position.z < 0) {
                //SetTcpLatestDelta();
                collidersHit.Remove(collider);
            }

            //var dictEntry = spawnIterations.FirstOrDefault(x => x.Value.Contains(collider));
            var dictEntry2 = dtoPerIterationDictionary.Where(x => x.Value.ClosestSpawn = collider).FirstOrDefault();

            if (!iterationsAlreadyProcessed.Contains(dictEntry2.Key)) {
                Debug.Log($"Found entry in iteration: {dictEntry2.Key}");
                ProcessEntry(dictEntry2.Key);
                iterationsAlreadyProcessed.Add(dictEntry2.Key);
            }

            /*
            if (!iterationsAlreadyProcessed.Contains(dictEntry.Key)) {
                Debug.Log($"Found entry in iteration: {dictEntry.Key}");
                ProcessEntry(dictEntry.Key);
                iterationsAlreadyProcessed.Add(dictEntry.Key);
            }
            */

            tempList.Add(collider);
        }

        foreach (var item in tempList) {
            collidersHit.Remove(item);
        }
    }

    private void ProcessEntry(int key) {
        if (dtoPerIterationDictionary.TryGetValue(key, out var value)) {
            var previousPosition = value.PreviousPlayerPosition;
            var currentPosition = GameObject.Find("Player").GetComponent<Rigidbody>().position;
            var distanceTravelled = value.DistanceTraveledByPlayer + Math.Abs(currentPosition.x - previousPosition.x);
            value.DistanceTraveledByPlayer = distanceTravelled;
            value.PreviousPlayerPosition = currentPosition;
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

        spawnIterations.Add(iterationCounter, spawnWaveGaps);

        var listOfDistances = new List<(GameObject gap, float distanceToPlayer)>();
        foreach (var gap in spawnIterations[iterationCounter]) {
            var distance = Vector3.Distance(gap.transform.position, player.transform.position);
            var tuple = (gap, distance);
            listOfDistances.Add(tuple);
        }

        var entryWithSmallestDistance = listOfDistances.OrderBy(x => x.distanceToPlayer).First();        
        closestColliderToPlayerInCurrentIteration.Add(iterationCounter, entryWithSmallestDistance);

        var currentPlayerPosition = GameObject.Find("Player").GetComponent<Rigidbody>().position;

        var dto = new ClosestSpawnAndDistanceTravelledDto {
            ClosestSpawn = entryWithSmallestDistance.gap,
            DistanceBetweenSpawnAndPlayer = entryWithSmallestDistance.distanceToPlayer
        };

        dtoPerIterationDictionary.Add(iterationCounter, dto);

        Debug.Log($"closest spawnpoint in iteration {iterationCounter} is spawn {entryWithSmallestDistance.gap.transform.position} with a distance of {entryWithSmallestDistance.distanceToPlayer}");
        iterationCounter += 1;
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
    }
}
 