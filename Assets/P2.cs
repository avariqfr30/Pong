using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P2 : MonoBehaviour
{
    public float moveSpeed;
    private float minY = -4f;  // Adjust based on your court height
    private float maxY = 4f;   // Adjust based on your court height
    private Ball ball;
    
    void Start()
    {
        ball = FindAnyObjectByType<Ball>();
    }

    void Update()
    {
        // AI follows the ball smoothly
        if (ball != null)
        {
            Vector3 ballPos = ball.transform.position;
            Vector3 currentPos = transform.position;
            
            // Calculate the direction to the ball with a small dead zone
            float yDifference = ballPos.y - currentPos.y;
            float deadZone = 0.2f; // Small dead zone to prevent jitter
            
            if (Mathf.Abs(yDifference) > deadZone)
            {
                // Move toward the ball's Y position
                float moveDirection = Mathf.Sign(yDifference);
                transform.Translate(Vector2.up * moveDirection * moveSpeed * Time.deltaTime);
            }
        }
        
        // Clamp paddle position to court boundaries
        Vector3 pos = transform.position;
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        transform.position = pos;
    }
}
