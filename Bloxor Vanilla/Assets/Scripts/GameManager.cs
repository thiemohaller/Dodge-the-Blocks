using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    bool gameHasEnded = false;
    public float restartDelay = 1.3f;

    public float slowDown = 10f;

    public GameObject completeLevelUI;

    public void CompleteLevel()
    {
        Debug.Log("Level finished!");
        completeLevelUI.SetActive(true);
    }

    public void EndGame()
    {
        if (gameHasEnded == false)
        {
            Debug.Log("Game Over.");
            gameHasEnded = true;
            Invoke("Restart", restartDelay);
            StartCoroutine(Restart());
        }
    }

    /*
    void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    */

    IEnumerator Restart()
    {
        // slow down
        Time.timeScale = 1f / slowDown;
        Time.fixedDeltaTime = Time.fixedDeltaTime / slowDown;

        // before wait
        yield return new WaitForSeconds(restartDelay / slowDown);
        // after wait

        // speed back up
        Time.timeScale = 1f;
        Time.fixedDeltaTime = Time.fixedDeltaTime * slowDown;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
