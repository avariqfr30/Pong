using System.Collections;
using System.Collections.Generic;   
using UnityEngine;

public class P1 : MonoBehaviour
{
    public float moveSpeed = 10f; // Default move speed if not set
    private float minY = -4f;  // Adjust based on your court height
    private float maxY = 4f;   // Adjust based on your court height
    
    void Start()
    {
        
    }

    void Update()
    {
        Vector3 currentPos = transform.position;
        float moveInput = 0f;
        
        // Keyboard input: W and S
        if (Input.GetKey(KeyCode.W))
        {
            moveInput = 1f;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            moveInput = -1f;
        }
        
        // Mouse/trackpad movement
        if (moveInput == 0f) // Only use mouse if not pressing keys
        {
            // Get mouse position in screen space
            Vector3 mouseScreenPos = Input.mousePosition;
            // Convert to viewport space (0 to 1)
            Vector3 mouseViewportPos = Camera.main.ScreenToViewportPoint(mouseScreenPos);
            // Get camera's world position and size for 2D
            Camera cam = Camera.main;
            float camHeight = cam.orthographicSize * 2f;
            float mouseWorldY = cam.transform.position.y + (mouseViewportPos.y - 0.5f) * camHeight;
            
            float yDifference = mouseWorldY - currentPos.y;
            float deadZone = 0.1f;
            
            if (Mathf.Abs(yDifference) > deadZone)
            {
                moveInput = Mathf.Sign(yDifference);
            }
        }
        
        // Apply movement
        if (moveInput != 0f)
        {
            currentPos.y += moveInput * moveSpeed * Time.deltaTime;
        }
        
        // Clamp paddle position to court boundaries
        currentPos.y = Mathf.Clamp(currentPos.y, minY, maxY);
        transform.position = currentPos;
    }
}
