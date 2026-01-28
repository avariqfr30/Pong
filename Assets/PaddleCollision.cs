using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaddleCollision : MonoBehaviour
{
    public float maxBounceAngle = 85f; // Maximum angle the ball can bounce (in degrees) - closer to 90 for sharper edges
    public float centerDeadZone = 0.15f; // Zone where the ball bounces perfectly horizontal

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
    /// Center zone: horizontal bounce (perfect)
    /// Edge zones: progressively steeper angles (up to 85 degrees)
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
        // Center zone: perfectly horizontal (0 degrees)
        // Edge zones: progressively sharper angles
        float bounceAngle;
        
        if (Mathf.Abs(hitPosition) < centerDeadZone)
        {
            // Center hit: perfectly horizontal
            bounceAngle = 0f;
        }
        else
        {
            // Map the hit position outside dead zone to full angle range
            float normalizedPos = (Mathf.Abs(hitPosition) - centerDeadZone) / (1f - centerDeadZone);
            bounceAngle = normalizedPos * maxBounceAngle * Mathf.Sign(hitPosition) * Mathf.Deg2Rad;
        }

        // Determine direction (left paddle bounces right, right paddle bounces left)
        float direction = transform.position.x < 0 ? 1f : -1f;

        // Preserve current ball speed
        float currentSpeed = ballRb.linearVelocity.magnitude;
        
        // Calculate new velocity maintaining speed
        float vx = Mathf.Cos(bounceAngle) * currentSpeed * direction;
        float vy = Mathf.Sin(bounceAngle) * currentSpeed;

        ballRb.linearVelocity = new Vector2(vx, vy);
    }
}
