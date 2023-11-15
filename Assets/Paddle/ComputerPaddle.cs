using UnityEngine;

public class ComputerPaddle : Paddle
{
    private Ball_script ball;


    protected override void Awake()
    {
        base.Awake();
        // Additional initialization for PlayerPaddle if needed
    }


    protected override void Start()
    {
        base.Start();
        ball = FindObjectOfType<Ball_script>(); // This will find the Ball component in the scene
    }


    void Update()
    {
        
        //Debug.Log("transform.position.y: " + transform.position.y);
        float move;
        if (ball.rb.velocity.x > 0)
        {
            float targetY = ball.transform.position.y;
            move = Mathf.MoveTowards(transform.position.y, targetY, speed * Time.deltaTime);
        }
        else if (ball.rb.velocity.x < 0)
        {
            move = Mathf.MoveTowards(transform.position.y, 0, speed * Time.deltaTime);
        }
        else
        {
            move = transform.position.y; // If the ball isn't moving horizontally, don't move the paddle
        }
        // Making sure the paddle does not keep bumping into a top/bot wall
        
        // Get the Camera's viewable bounds in world space
        Camera mainCamera = Camera.main; // Reference to the main camera
        Vector2 minScreenBounds = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane)); // Bottom-left corner
        Vector2 maxScreenBounds = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, mainCamera.farClipPlane)); // Top-right corner

        // Finding min/max screen y value
        float minY = minScreenBounds.y; // This is the minimum screen Y value which should be -4.98
        float maxY = maxScreenBounds.y; // This is the maximum screen Y value which should be 5.02
        float paddleHeight = GetComponent<Collider2D>().bounds.size.y; // Paddle y-size

        // Clamp the paddle's y position to stay within the game field bounds
        float clampedY = Mathf.Clamp(move, minY + paddleHeight / 2, maxY - paddleHeight / 2);
        transform.position = new Vector2(transform.position.x, clampedY);
        
    }

}