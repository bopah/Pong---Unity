using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paddle : MonoBehaviour
{
    protected Rigidbody2D rb;

    protected float speed = 4f;
    protected Vector2 direction;

    protected Vector2 defaultPosition;

    protected virtual void Awake()
    {
        // Set the default starting position
        defaultPosition = transform.position;
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        // Get the Rigidbody2D component from the GameObject
        rb = GetComponent<Rigidbody2D>();
    }

    public void resetPaddlePosition()
    {
        transform.position = defaultPosition;
    }

}
