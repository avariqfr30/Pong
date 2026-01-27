using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P2 : MonoBehaviour
{
    public float moveSpeed;
    private float minY = -4f;  // Adjust based on your court height
    private float maxY = 4f;   // Adjust based on your court height
    
    void Start()
    {
        
    }

    void Update()
    {
        bool isPressingUp = Input.GetKey(KeyCode.UpArrow);
        bool isPressingDown = Input.GetKey(KeyCode.DownArrow);

        if (isPressingUp)
        {
            transform.Translate(Vector2.up * moveSpeed * Time.deltaTime);
        }
        if (isPressingDown)
        {
            transform.Translate(Vector2.down * moveSpeed * Time.deltaTime);
        }
        
        // Clamp paddle position to court boundaries
        Vector3 pos = transform.position;
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        transform.position = pos;
    }
}
