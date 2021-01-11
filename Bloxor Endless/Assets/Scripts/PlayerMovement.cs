using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public new Rigidbody rigidbody;
    public float playerSpeed = 8000f;
    public float swerveForce = 200f; // use 18 if translate, 200 for addforce
    [Range(1, 50)]
    public int playerSpeedMultiplier = 4;
    public float mapWidth = 15f;

    void FixedUpdate()
    {
        float xPosition = Input.GetAxis("Horizontal") * Time.fixedDeltaTime * swerveForce;
        Vector3 newPosition = rigidbody.position + Vector3.right * xPosition;
        //newPosition.x = Mathf.Clamp(newPosition.x, -mapWidth, mapWidth);
        rigidbody.MovePosition(newPosition);

        var forwardForce = playerSpeed * playerSpeedMultiplier * Time.fixedDeltaTime;
        rigidbody.AddForce(0, 0, forwardForce);
        /*
        var speed = swerveForce * Time.fixedDeltaTime;
        rigidbody.AddForce(Input.GetAxis("Horizontal") * speed, 0, 0, ForceMode.VelocityChange);
        */
        // game over by falling off
        if(rigidbody.position.y < -1) {
            FindObjectOfType<GameManager>().GameOver();
        }
    }
}
