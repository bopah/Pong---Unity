using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerPaddle : Paddle
{
    protected override void Awake()
    {
        base.Awake();
        // Additional initialization for PlayerPaddle if needed
    }


    // Update is called once per frame
    void Update()
    {
        // Check for input in Update for more responsive controls
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            direction = Vector2.up;
        }
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            direction = Vector2.down;
        }
        else
        {
            direction = Vector2.zero;
        }
    }


    void FixedUpdate()
    {
        // Apply the movement in FixedUpdate
        rb.velocity = direction * this.speed;
    }
}
