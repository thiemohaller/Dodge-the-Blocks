using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    bool gameHasEnded = false;
    public float restartDelay = 1f;
    public float slowDown = 10f;

    public void GameOver() {
        if (gameHasEnded == false) {
            GameObject.Find("TCPServer").GetComponent<DeathCounter>().IncreaseDeathCounter();
            Debug.Log("Game Over :(");
            gameHasEnded = true;
            Invoke(nameof(Restart), restartDelay);
            StartCoroutine(Restart());
        }
    }

    IEnumerator Restart() {
        // slow down
        Time.timeScale = 1f / slowDown;
        Time.fixedDeltaTime /= slowDown;
        yield return new WaitForSeconds(restartDelay / slowDown);

        // speed back up
        Time.timeScale = 1f;
        Time.fixedDeltaTime *= slowDown;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
