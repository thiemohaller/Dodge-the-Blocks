using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public new Rigidbody rigidbody;
    public float swerveForce = 5f;
    public float mapWidth = 15f;
    [Range(1, 50)]
    public int playerSpeedMultiplier = 4;

    void FixedUpdate() {
        float xPosition = Input.GetAxis("Horizontal") * Time.fixedDeltaTime * swerveForce * playerSpeedMultiplier;
        Vector3 newPosition = rigidbody.position + Vector3.right * xPosition;
        newPosition.x = Mathf.Clamp(newPosition.x, -mapWidth, mapWidth);
        rigidbody.MovePosition(newPosition);

        // game over by falling off
        if(rigidbody.position.y < -1) {
            NotifyGameManagerOfGameOver();
        }

        if (rigidbody.position.x < -8 || rigidbody.position.x > 8) {
            NotifyGameManagerOfGameOver();
        }
    }

    void NotifyGameManagerOfGameOver() {
        FindObjectOfType<GameManager>().GameOver();
    }
}
