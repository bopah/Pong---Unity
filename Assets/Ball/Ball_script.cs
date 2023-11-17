using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.VFX;

public class Ball_script : MonoBehaviour
{
    public float horizontalSpeed = 5.0f; // Set the horizontal speed
    public float speedIncrease = 0.2f;
    public Rigidbody2D rb;
    [SerializeField] private GameManagerScript gameManager;
    [SerializeField] private AudioManagerScript AudioManager;

    // Fireball effect
    [SerializeField] private GameObject fireballPrefab; // Assign your fireball prefab GameObject in the inspector
    private GameObject activeFireballEffect; // the fireball gameobject that is intantiated and destroyed (we cant directly do stuff with a prefab)
    [SerializeField] private float fireballActivationMagnitude = 6f; // Set your desired velocity threshold

    // Fire trail effect
    [SerializeField] private GameObject fireTrailPrefab; // Assign your Fire_B prefab GameObject in the inspector
    private GameObject activeFireTrail; // The Fire_B GameObject that is instantiated and destroyed

    private Vector2 defaultPosition;

    void Start()
    {
        // Find the GameManager in the scene and cache it
        //gameManager = FindObjectOfType<GameManagerScript>();

        rb = GetComponent<Rigidbody2D>();
        CheckForFireballActivation();
        
  

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
            
            CheckForFireballActivation();
        }
        else if (col.gameObject.CompareTag("RightWall"))
        {
            AudioManager.HandlePaddleAudio("LeftRightWall");
            gameManager.BallHitWall("Right");
            
            CheckForFireballActivation();
        }
        else
        {
            if (col.gameObject.CompareTag("PlayerPaddle"))
            {
                //AudioManager.PlaySound(SoundType.PaddleHit);
                //Debug.Log("Ball script registers PlayerPaddle hit");
                AudioManager.HandlePaddleAudio("PlayerPaddle"); // make sound if ball hits player paddle
                HandlePaddleCollision(col);
            }

            // Make paddle sound if ball hits a computer paddle
            if (col.gameObject.CompareTag("ComputerPaddle"))
            {
                //AudioManager.PlaySound(SoundType.PaddleHit);
                //Debug.Log("Ball script registers all ComputerPaddle hit");
                AudioManager.HandlePaddleAudio("ComputerPaddle");
            }
            // Make top/bot wall sound if ball hits top/bot wall
            else if (col.gameObject.CompareTag("TopBotWall"))
            {
                //Debug.Log("Ball script registers top/bot wall hit");
                AudioManager.HandlePaddleAudio("TopBotWall");
            }

            Debug.Log("rb.velocity.magnitude: " + rb.velocity.magnitude);
            IncreaseVelocitySpeed();
            
            CheckForFireballActivation();

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

    public void ResetPaddlePosition()
    {
        transform.position = defaultPosition;
        SetInitialVelocity();
    }

    private void CheckForFireballActivation()
    {
        // Check if the velocity magnitude exceeds the threshold
        if (rb.velocity.magnitude > fireballActivationMagnitude)
        {
            if (activeFireballEffect == null) // This means fireball effect is not active
            {
                // Instantiate a new fireball effect from the prefab
                activeFireballEffect = Instantiate(fireballPrefab, rb.transform.position, Quaternion.identity);
                // Set the fireball effect as a child of the ball
                activeFireballEffect.transform.SetParent(rb.transform, false);
                // No need to setActive(true) because instantiation does that by default
                
                // Set the local position of the fireball to zero, so it aligns with the ball's center
                activeFireballEffect.transform.localPosition = Vector3.zero;
                Debug.Log("Fireball activated");
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
                Debug.Log("Fireball deactivated");
            }
        }

        // Now, check for the fire trail activation
        float magnitude = rb.velocity.magnitude;
        if (magnitude >= 6)
        {
            if (activeFireTrail == null) // This means fire trail effect is not active
            {
                // Instantiate a new fire trail effect from the prefab
                activeFireTrail = Instantiate(fireTrailPrefab, rb.transform.position, Quaternion.identity);
                // Set the fire trail effect as a child of the ball
                activeFireTrail.transform.SetParent(rb.transform, false);

                // Set the local position of the fireball to zero, so it aligns with the ball's center
                activeFireTrail.transform.localPosition = Vector3.zero;
                // No need to setActive(true) because instantiation does that by default
                Debug.Log("Fire trail activated");
            }
            UpdateFireTrail_Scale(magnitude); // Pass the magnitude for scaling
        }
        else
        {
            if (activeFireTrail != null) // This means fire trail effect is active
            {
                // Destroy the active fire trail effect instance
                Destroy(activeFireTrail);
                activeFireTrail = null;
                Debug.Log("Fire trail deactivated");
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


    private void UpdateFireTrail_Scale(float magnitude)
    {
        // The scale change logic remains the same, but apply it to the instantiated fireTrail
        // Increase the scale based on the magnitude
        Vector3 newScale = Vector3.one;
        if (magnitude >= 10)
        {
            newScale = new Vector3(5, 10, 1);
        }
        else if (magnitude >= 9)
        {
            newScale = new Vector3(4, 8, 1);
        }
        // No need for an else if for magnitude >= 8 since it's the same as for magnitude >= 9
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
}
