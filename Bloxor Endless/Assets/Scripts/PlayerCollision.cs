using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    public PlayerMovement movementScript;
    public BlockSpawner blockSpawner;

    public void Awake() {
        blockSpawner = GameObject.Find("BlockSpawner").GetComponent<BlockSpawner>();
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.collider.CompareTag("GapTrigger")) {
            var otherGameobject = collision.gameObject;
            otherGameobject.SetActive(false);
            Debug.Log("Hit gap collider");
            blockSpawner.Notify(collision.gameObject);
            //Destroy(collision.gameObject);
        }

        if(collision.collider.CompareTag("Obstacle")) {
            movementScript.enabled = false;
            FindObjectOfType<GameManager>().GameOver();
        }
    }
}
