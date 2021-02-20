using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    public PlayerMovement movementScript;

    private void OnCollisionEnter(Collision collision) {
        if(collision.collider.CompareTag("Obstacle")) {
            movementScript.enabled = false;
            FindObjectOfType<GameManager>().GameOver();
        }
    }
}
