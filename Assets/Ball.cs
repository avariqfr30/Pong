using System.Collections;
using System.Collections.Generic;       
using UnityEngine;

public class Ball : MonoBehaviour
{
    public Rigidbody2D rb;
    public float startingSpeed;
    
    // Ball boundary for out of bounds detection
    private float leftBoundary = -12f;  // Adjust based on your scene width
    private float rightBoundary = 12f;  // Adjust based on your scene width
    
    private GameManager gameManager;
    private Vector3 startingPosition;
    private bool pointAlreadyAwarded = false;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        startingPosition = transform.position;
        
        // Configure Rigidbody2D for realistic ping-pong physics
        ConfigurePhysics();
        
        // Don't launch ball initially - wait for game to start
        if (gameManager != null && gameManager.IsGameActive())
        {
            LaunchBall();
        }
    }

    /// <summary>
    /// Configure Rigidbody2D settings for realistic ping-pong physics
    /// </summary>
    private void ConfigurePhysics()
    {
        // Disable drag to prevent slowdown
        rb.linearDamping = 0f;
        rb.angularDamping = 0f;
        
        // Minimize gravity for consistent horizontal movement
        rb.gravityScale = 0f;
        
        // Enable continuous collision detection to prevent missing collisions at high speed
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        
        // Freeze rotation to prevent unwanted spinning
        rb.freezeRotation = true;
    }

    void Update()
    {
        // Check if ball goes out of bounds (left side = Player 2 loses, right side = Player 1 loses)
        if (transform.position.x < leftBoundary && !pointAlreadyAwarded)
        {
            gameManager.AwardPoint(1); // Player 1 scores
            pointAlreadyAwarded = true;
            return;
        }
        
        if (transform.position.x > rightBoundary && !pointAlreadyAwarded)
        {
            gameManager.AwardPoint(2); // Player 2 scores
            pointAlreadyAwarded = true;
            return;
        }
    }

    /// <summary>
    /// Launch the ball with random direction and vertical velocity
    /// </summary>
    private void LaunchBall()
    {
        bool isRight = UnityEngine.Random.value >= 0.5f;
        float xVelocity = isRight ? 1f : -1f;
        float yVelocity = UnityEngine.Random.Range(-1f, 1f);

        rb.linearVelocity = new Vector2(xVelocity * startingSpeed, yVelocity * startingSpeed);
    }

    /// <summary>
    /// Reset ball to starting position and launch it again
    /// Called by GameManager after each point
    /// </summary>
    public void ResetBall()
    {
        pointAlreadyAwarded = false; // Reset the flag
        transform.position = startingPosition;
        rb.linearVelocity = Vector2.zero;
        LaunchBall();
    }
}
