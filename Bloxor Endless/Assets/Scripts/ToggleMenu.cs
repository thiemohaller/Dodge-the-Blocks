using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleMenu : MonoBehaviour {

    public static bool GameIsPaused = false;
    public GameObject Ui;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (GameIsPaused) {
                Hide();
            } else {
                Show();
            }
        }
    }

    private void Show() {
        Ui.SetActive(true);
        //Time.timeScale = 0f;
        GameIsPaused = true;
    }

    private void Hide() {
        Ui.SetActive(false);
        //Time.timeScale = 1f;
        GameIsPaused = false;
    }
}
