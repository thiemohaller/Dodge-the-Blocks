using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public float speed = 20f;
    public float mapWidth = 10f;

    private Rigidbody rigidBody;

    private void Start() {
        rigidBody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate() {
        float x = Input.GetAxis("Horizontal") * Time.fixedDeltaTime * speed;

        Vector3 newPosition = rigidBody.position + Vector3.right * x;

        newPosition.x = Mathf.Clamp(newPosition.x, -mapWidth, mapWidth);

        rigidBody.MovePosition(newPosition);

        if (rigidBody.position.y < -1f) {
            FindObjectOfType<GameManager>().EndGame();
        }

        if (rigidBody.position.x < -8 || rigidBody.position.x > 8) {
            FindObjectOfType<GameManager>().EndGame();
        }

    }

    void OnCollisionEnter2D() {
        //Debug.Log("Man Down!");
        FindObjectOfType<GameManager>().EndGame();
    }
}
