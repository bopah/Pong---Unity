using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.VFX;

public class Ball_script : MonoBehaviour
{
    private Vector2 defaultPosition;
    public float horizontalSpeed = 5.0f; // Set the horizontal speed
    public float speedIncrease = 0.2f;
    public Rigidbody2D rb;

    [SerializeField] private GameManagerScript gameManager;
    [SerializeField] private AudioManagerScript AudioManager;

    // Fireball effect
    [SerializeField] private GameObject fireballPrefab; // Assign your fireball prefab GameObject in the inspector
    private GameObject activeFireballEffect; // the fireball gameobject that is intantiated and destroyed (we cant directly do stuff with a prefab)
    
    private float allFireActivationMagnitude = 6f; // Set your desired velocity threshold

    // Fire trail effect
    [SerializeField] private GameObject fireTrailPrefab; // Assign your Fire_B prefab GameObject in the inspector
    private GameObject activeFireTrail; // The Fire_B GameObject that is instantiated and destroyed

    

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        defaultPosition = transform.position;
        SetInitialVelocity();
        Debug.Log("rb.velocity.magnitude: " + rb.velocity.magnitude);
    }


    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("LeftWall"))
        {
            AudioManager.HandlePaddleAudio("LeftRightWall");
            gameManager.BallHitWall("Left");
            CheckForAllFireActivation();
        }
        else if (col.gameObject.CompareTag("RightWall"))
        {
            AudioManager.HandlePaddleAudio("LeftRightWall");
            gameManager.BallHitWall("Right");
            CheckForAllFireActivation();
        }
        else
        {
            // Make paddle sound if ball hits a paddle (currently same for player/computer)
            if (col.gameObject.CompareTag("PlayerPaddle"))
            {
                AudioManager.HandlePaddleAudio("PlayerPaddle");
                HandlePaddleCollision(col);
            }
            if (col.gameObject.CompareTag("ComputerPaddle"))
            {
                AudioManager.HandlePaddleAudio("ComputerPaddle");
            }
            // Make top/bot wall sound if ball hits top/bot wall
            else if (col.gameObject.CompareTag("TopBotWall"))
            {
                AudioManager.HandlePaddleAudio("TopBotWall");
            }
            IncreaseVelocitySpeed();
            Debug.Log("rb.velocity.magnitude: " + rb.velocity.magnitude);
            CheckForAllFireActivation();
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
        float initialSpeed = rb.velocity.magnitude; // Save the initial speed (magnitude of velocity)
        Vector2 contactPoint = col.GetContact(0).point; // Get the collision point      
        Vector2 paddleCenter = col.collider.bounds.center; // Get the center point of the paddle       
        float paddleHeight = col.collider.bounds.size.y; // Get the height of the paddle       

        float relativePosition = (contactPoint.y - paddleCenter.y) / (paddleHeight / 2); // Calculate the relative position of the collision from the center of the paddle      
        float angle = relativePosition * 50; // Convert the relative position to an angle (from -50 to 50 degrees)      
        angle = Mathf.Clamp(angle, -50f, 50f); // Clamp the angle to be within the desired range just in case

        // Calculate the direction vector
        float radians = angle * Mathf.Deg2Rad; // Convert degrees to radians
        Vector2 direction = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));       
        rb.velocity = direction.normalized * initialSpeed; // New direction vectir with the same initial speed
    }


    private void IncreaseVelocitySpeed()
    {
        Vector2 direction = rb.velocity.normalized; // Normalize the current velocity
        float newSpeed = rb.velocity.magnitude + speedIncrease; // Calculate the new speed by adding speedIncrease to the current speed(magnitude)
        rb.velocity = direction * newSpeed; // Apply the new speed to the direction
    }


    public void ResetPaddlePosition()
    {
        transform.position = defaultPosition;
        SetInitialVelocity();
    }


    private void CheckForAllFireActivation()
    {
        ActivateFireBurningSound();
        CheckForFireballActivation();
        CheckForFireTrailActivation();
    }


    private void CheckForFireballActivation()
    {
        if (rb.velocity.magnitude > allFireActivationMagnitude)
        {
            if (activeFireballEffect == null) // This means fireball effect is not active
            {
                // Instantiate a new fireball effect from the prefab
                activeFireballEffect = Instantiate(fireballPrefab, rb.transform.position, Quaternion.identity);
                // Set the fireball effect as a child of the ball
                activeFireballEffect.transform.SetParent(rb.transform, false);
                // Set the local position of the fireball to zero, so it aligns with the ball's center
                activeFireballEffect.transform.localPosition = Vector3.zero;
            }
            UpdateFireballSize();
        }
        else
        {
            if (activeFireballEffect != null) // This means fireball effect is active
            {
                // Destroy the active fireball effect instance
                Destroy(activeFireballEffect);
                activeFireballEffect = null;
            }
        }
    }


    private void UpdateFireballSize()
    {
        if (activeFireballEffect != null)
        {
            // Get the VisualEffect component from the active fireball effect
            VisualEffect fireballVFX = activeFireballEffect.GetComponent<VisualEffect>();

            // Calculate the new size based on the ball's velocity magnitude
            float magnitude = rb.velocity.magnitude;
            float newSize = 3; // Default size

            if (magnitude >= 10) newSize = 7;
            else if (magnitude >= 9) newSize = 6;
            else if (magnitude >= 8) newSize = 5;
            else if (magnitude >= 7) newSize = 4;

            // Set the new size to the 'FireballSize' property
            if (fireballVFX != null)
            {
                fireballVFX.SetFloat("FireballSize", newSize);
            }
        }
    }


    private void CheckForFireTrailActivation()
    {

        if (rb.velocity.magnitude >= allFireActivationMagnitude)
        {
            if (activeFireTrail == null) // This means fire trail effect is not active
            {
                // Instantiate a new fire trail effect from the prefab
                activeFireTrail = Instantiate(fireTrailPrefab, rb.transform.position, Quaternion.identity);
                // Set the fire trail effect as a child of the ball
                activeFireTrail.transform.SetParent(rb.transform, false);
                // Set the local position of the fireball to zero, so it aligns with the ball's center
                activeFireTrail.transform.localPosition = Vector3.zero;
            }
            UpdateFireTrail_Scale(rb.velocity.magnitude); // Pass the magnitude for scaling
        }
        else
        {
            if (activeFireTrail != null) // This means fire trail effect is active
            {
                // Destroy the active fire trail effect instance
                Destroy(activeFireTrail);
                activeFireTrail = null;
            }
        }
    }

    private void UpdateFireTrail_Scale(float magnitude)
    {
        // Increase the scale based on the ball magnitude
        Vector3 newScale = Vector3.one;
        if (magnitude >= 9)
        {
            newScale = new Vector3(6, 12, 1);
        }
        else if (magnitude >= 9)
        {
            newScale = new Vector3(5, 10, 1);
        }
        else if (magnitude >= 8)
        {
            newScale = new Vector3(4, 8, 1);
        }
        else if (magnitude >= 7)
        {
            newScale = new Vector3(3, 6, 1);
        }
        else // if magnitude >= 6
        {
            newScale = new Vector3(1, 3, 1);
        }

        if (activeFireTrail != null)
        {
            activeFireTrail.transform.localScale = newScale;
        }
    }


    private void ActivateFireBurningSound()
    {
        if (rb.velocity.magnitude >= allFireActivationMagnitude)
        {
            AudioManager.HandleFireSound(rb.velocity.magnitude);
        }
        else
        {
            AudioManager.StopFireSound();
        }
    }
}
