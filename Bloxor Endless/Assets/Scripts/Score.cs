using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour {

    public Text scoreText;
    public Text currentSpeedText;
    public Text activeSpawnsText;
    public int multiplier = 10;
    public BlockSpawner spawner;

    private double score;
    private double timer;

    private void Start() {
        scoreText.text = "0";
        score = 0;
        timer = 0;

        currentSpeedText.text = $"Speed multiplier: {spawner.objectSpeedMultiplier}";
        var activeSpawns = spawner.spawnPoints.Length - spawner.freeSpaces;
        activeSpawnsText.text = $"Active Spawns: \n{activeSpawns}";
    }

    void Update() {
        timer += Time.deltaTime;

        if (timer > 0.5) {
            score += 0.5 * multiplier;
            scoreText.text = score.ToString();
            timer = 0;
        }

        currentSpeedText.text = $"Speed multiplier: {spawner.objectSpeedMultiplier}";
        var activeSpawns = spawner.spawnPoints.Length - spawner.freeSpaces;
        activeSpawnsText.text = $"Active Spawns: \n{activeSpawns}";
    }
}
