using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Ball_script : MonoBehaviour
{
    public float horizontalSpeed = 5.0f; // Set the horizontal speed
    public float speedIncrease = 0.2f;
    public Rigidbody2D rb;
    [SerializeField] private GameManagerScript gameManager;
    [SerializeField] private AudioManagerScript AudioManager;

    private Vector2 defaultPosition;

    void Start()
    {
        // Find the GameManager in the scene and cache it
        //gameManager = FindObjectOfType<GameManagerScript>();

        rb = GetComponent<Rigidbody2D>();

        // Set the default starting position
        defaultPosition = transform.position;

        SetInitialVelocity();
    }


    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("LeftWall"))
        {
            AudioManager.HandlePaddleAudio("LeftRightWall");
            gameManager.BallHitWall("Left");
        }
        else if (col.gameObject.CompareTag("RightWall"))
        {
            AudioManager.HandlePaddleAudio("LeftRightWall");
            gameManager.BallHitWall("Right");
        }
        else
        {
            if (col.gameObject.CompareTag("PlayerPaddle"))
            {
                //AudioManager.PlaySound(SoundType.PaddleHit);
                Debug.Log("Ball script registers PlayerPaddle hit");
                AudioManager.HandlePaddleAudio("PlayerPaddle"); // make sound if ball hits player paddle
                HandlePaddleCollision(col);
            }

            // Make paddle sound if ball hits a computer paddle
            if (col.gameObject.CompareTag("ComputerPaddle"))
            {
                //AudioManager.PlaySound(SoundType.PaddleHit);
                Debug.Log("Ball script registers all ComputerPaddle hit");
                AudioManager.HandlePaddleAudio("ComputerPaddle");
            }
            // Make top/bot wall sound if ball hits top/bot wall
            else if (col.gameObject.CompareTag("TopBotWall"))
            {
                Debug.Log("Ball script registers top/bot wall hit");
                AudioManager.HandlePaddleAudio("TopBotWall");
            }
            IncreaseVelocitySpeed();
        }

    }


    private void SetInitialVelocity()
    {
        // Randomly decide the x and y direction
        int xDirection = Random.Range(0, 2) * 2 - 1;
        int yDirection = Random.Range(0, 2) * 2 - 1;
        rb.velocity = new Vector2(horizontalSpeed * xDirection, yDirection);
    }


    private void HandlePaddleCollision(Collision2D col)
    {
        
        // Save the initial speed (magnitude of velocity)
        float initialSpeed = rb.velocity.magnitude;

        // Get the collision point
        Vector2 contactPoint = col.GetContact(0).point;

        // Get the center point of the paddle
        Vector2 paddleCenter = col.collider.bounds.center;

        // Get the size of the paddle
        float paddleHeight = col.collider.bounds.size.y;

        // Calculate the relative position of the collision from the center of the paddle
        float relativePosition = (contactPoint.y - paddleCenter.y) / (paddleHeight / 2);

        // Convert the relative position to an angle (from -50 to 50 degrees)
        float angle = relativePosition * 50;

        // Clamp the angle to be within the desired range just in case
        angle = Mathf.Clamp(angle, -50f, 50f);

        // Calculate the direction vector from the angle
        float radians = angle * Mathf.Deg2Rad; // Convert degrees to radians
        Vector2 direction = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));

        // Assuming ballSpeed is the desired speed of the ball after collision
        rb.velocity = direction.normalized * initialSpeed;
        
    }


    private void IncreaseVelocitySpeed()
    {
        // Normalize the current velocity to get the direction
        Vector2 direction = rb.velocity.normalized;

        // Calculate the new speed by adding speedIncrease to the current speed
        float newSpeed = rb.velocity.magnitude + speedIncrease;

        // Apply the new speed to the direction
        rb.velocity = direction * newSpeed;
    }

    public void resetPaddlePosition()
    {
        transform.position = defaultPosition;
        SetInitialVelocity();
    }
}
