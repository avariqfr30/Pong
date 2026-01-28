using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaddleCollision : MonoBehaviour
{
    public float bounceForce = 15f;
    public float maxBounceAngle = 75f; // Maximum angle the ball can bounce (in degrees)

    private Rigidbody2D ballRb;
    private Ball ballScript;

    void Start()
    {
        ballRb = FindAnyObjectByType<Ball>().GetComponent<Rigidbody2D>();
        ballScript = FindAnyObjectByType<Ball>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            HandlePaddleHit(collision);
        }
    }

    /// <summary>
    /// Handle ball collision with paddle
    /// Implements realistic ping-pong bounce based on paddle hit position
    /// Different zones of the paddle create different reflection angles
    /// </summary>
    private void HandlePaddleHit(Collision2D collision)
    {
        Ball ball = collision.gameObject.GetComponent<Ball>();
        if (ball == null) return;

        // Get the contact point
        Vector2 hitPoint = collision.GetContact(0).point;
        Vector2 paddleCenter = transform.position;

        // Calculate where on the paddle the ball hit (normalized to -1 to 1)
        // -1 = bottom, 0 = center, 1 = top
        float paddleHeight = GetComponent<Collider2D>().bounds.size.y;
        float hitPosition = (hitPoint.y - paddleCenter.y) / (paddleHeight * 0.5f);
        hitPosition = Mathf.Clamp(hitPosition, -1f, 1f);

        // Calculate bounce angle based on hit position
        // Top of paddle: upward angle
        // Center of paddle: straight horizontal
        // Bottom of paddle: downward angle
        float bounceAngle = hitPosition * maxBounceAngle * Mathf.Deg2Rad;

        // Determine direction (left paddle bounces right, right paddle bounces left)
        float direction = transform.position.x < 0 ? 1f : -1f;

        // Preserve current ball speed instead of using fixed bounceForce
        float currentSpeed = ballRb.linearVelocity.magnitude;
        
        // Calculate new velocity maintaining speed
        float vx = Mathf.Cos(bounceAngle) * currentSpeed * direction;
        float vy = Mathf.Sin(bounceAngle) * currentSpeed;

        ballRb.linearVelocity = new Vector2(vx, vy);
    }
}
