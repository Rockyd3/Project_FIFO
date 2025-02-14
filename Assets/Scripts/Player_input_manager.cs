using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player_input_manager : MonoBehaviour
{
    [Header("Movement")]
    // Player's movespeed
    [SerializeField]
    private float MoveSpeed;
    // The amount of force that is multiplied to the orientation vector when dashing
    [SerializeField]
    private float DashScalar;

    // From InputManager
    float HorizontalInput;
    float VerticalInput;
    bool DashInput;
    
    // Dynamically updated movement direction; Set to the zero vector when not moving
    Vector3 MoveDirection;
    // Effectively the same as the MoveDirection, except when not moving, the orientation remembers the last stored direction to allow dashing while stationary.
    Vector3 Orientation = new Vector3(0, 0, 1);

    Rigidbody rb;

    Vector3 Up = new Vector3(-1, 0, -1);
    Vector3 Right = new Vector3(-1, 0, 1);

    // Start is called before the first frame update
    void Start()
    {
        //Disable and hide Cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        DashScalar = 8;
    }

    
    void Update()
    {
        //Called once per frame. Reads input & updates vars
        GetInput();
        if (DashInput)
        {
            Dash(MoveDirection.normalized);
        }

        DashInput = false;
    }

    void FixedUpdate()
    {
        //Called once per set amount of time. Moves Player
        MovePlayer();
    }

    void GetInput()
    {
        //Set HorizontalInput & VerticalInput variables
        HorizontalInput = Input.GetAxisRaw("Horizontal");
        VerticalInput = Input.GetAxisRaw("Vertical");
        DashInput = Input.GetButtonDown("Dash");
        
    }

    void MovePlayer()
    {
        //Moves the player rigidBody
        MoveDirection = Up * VerticalInput + Right * HorizontalInput;
        // If the player becomes stationary, store the last moved direction as the orientation
        if (!MoveDirection.Equals(new Vector3(0, 0, 0))) {
            Orientation = MoveDirection.normalized;
        }
        rb.AddForce(MoveDirection.normalized * MoveSpeed, ForceMode.Force);
        

    }

    void Dash(Vector3 vect)
    {   
        // Dashes a with a speed specified by DashScalar and direction specified by Orientation
        rb.AddForce(Orientation * DashScalar, ForceMode.VelocityChange);
    }
}
