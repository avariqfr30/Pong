using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallBounce : MonoBehaviour
{
    private Rigidbody2D ballRb;
    private float lastBounceTime = -1f;
    private float bounceCooldown = 0.05f; // Prevent multiple bounces in short time

    void Start()
    {
        ballRb = GetComponent<Rigidbody2D>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Only bounce if it's not a paddle and enough time has passed since last bounce
        if (!collision.gameObject.GetComponent<PaddleCollision>() && 
            Time.time - lastBounceTime > bounceCooldown)
        {
            HandleWallBounce(collision);
        }
    }

    /// <summary>
    /// Handle ball collision with walls while maintaining speed
    /// Simple and robust bounce that prevents the ball from sticking
    /// </summary>
    private void HandleWallBounce(Collision2D collision)
    {
        lastBounceTime = Time.time;
        
        // Get current velocity
        Vector2 velocity = ballRb.linearVelocity;
        float speed = velocity.magnitude;

        // Get collision normal
        Vector2 normal = collision.GetContact(0).normal;

        // Simple bounce: reflect velocity across the normal
        Vector2 reflectedVelocity = Vector2.Reflect(velocity, normal);
        
        // Ensure we maintain speed
        reflectedVelocity = reflectedVelocity.normalized * speed;
        
        // Apply the new velocity
        ballRb.linearVelocity = reflectedVelocity;
        
        // Push ball slightly away from wall to prevent sticking
        transform.position += (Vector3)normal * 0.01f;
    }
}

