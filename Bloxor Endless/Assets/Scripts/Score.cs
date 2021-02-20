using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour {

    private double score;
    private double timer;
    public Text scoreText;
    public int multiplier = 10;

    private void Start() {
        scoreText.text = "0";
        score = 0;
        timer = 0;
    }

    void Update() {
        timer += Time.deltaTime;

        if (timer > 0.5) {
            score += 0.5 * multiplier;
            scoreText.text = score.ToString();
            timer = 0;
        }
    }
}
