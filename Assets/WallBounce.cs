using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallBounce : MonoBehaviour
{
    private Rigidbody2D ballRb;

    void Start()
    {
        ballRb = GetComponent<Rigidbody2D>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if ball hit a wall (anything that isn't a paddle)
        if (!collision.gameObject.CompareTag("Ball") && !collision.gameObject.GetComponent<PaddleCollision>())
        {
            HandleWallBounce(collision);
        }
    }

    /// <summary>
    /// Handle ball collision with walls while maintaining speed
    /// Implements realistic ping-pong wall physics
    /// </summary>
    private void HandleWallBounce(Collision2D collision)
    {
        // Get the current velocity and speed
        Vector2 velocity = ballRb.linearVelocity;
        float speed = velocity.magnitude;

        // Get collision normal to determine bounce direction
        Vector2 normal = collision.relativeVelocity.normalized;
        
        // Reflect velocity based on surface normal
        Vector2 reflectedVelocity = Vector2.Reflect(velocity, collision.GetContact(0).normal);
        
        // Maintain consistent speed after bounce (realistic ping-pong physics)
        reflectedVelocity = reflectedVelocity.normalized * speed;
        
        ballRb.linearVelocity = reflectedVelocity;
    }
}
