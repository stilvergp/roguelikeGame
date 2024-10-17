using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Movement
    public float moveSpeed;
    public Rigidbody2D rb;
    public Vector2 movement;

    public Animator animator;
    private Vector2 lastMoveDirection;

    public Transform Aim;
    bool isWalking = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Process inputs first
        ProcessInputs();

        // Animator updates
        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Speed", movement.sqrMagnitude);

        // Normalize movement vector
        movement.Normalize();
    }

    void FixedUpdate()
    {
        Move();

        if (isWalking)
        {
            Vector3 direction = new Vector3(movement.x, movement.y, 0);
            Aim.rotation = Quaternion.LookRotation(Vector3.forward, direction);
        }
    }

    public void Move()
    {
        rb.velocity = new Vector2(movement.x * moveSpeed, movement.y * moveSpeed);
    }

    void ProcessInputs()
    {
        // Get input first
        movement.x = Input.GetAxis("Horizontal");
        movement.y = Input.GetAxis("Vertical");

        if (movement.sqrMagnitude == 0)
        {
            // Player stopped moving
            isWalking = false;
            lastMoveDirection = movement; // Update with last direction
            Vector3 lastDirection = new Vector3(lastMoveDirection.x, lastMoveDirection.y, 0);
            Aim.rotation = Quaternion.LookRotation(Vector3.forward, lastDirection);
        }
        else
        {
            // Player is walking
            isWalking = true;
            lastMoveDirection = movement; // Update the last move direction during movement
        }
    }
}
