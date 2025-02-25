using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player_input_manager : MonoBehaviour
{
    public static Player_input_manager instance;

    private void Awake() // makes Player_input_manager callable in any script: Player_input_manager.instance.[]
    {
        instance = this;
    }

    [Header("Movement")]
    // Player's movespeed
    [SerializeField]
    private float moveSpeed;
    // The amount of force that is multiplied to the orientation vector when dashing
    [SerializeField]
    private float dashScalar;

    // From InputManager
    private float horizontalInput;
    private float verticalInput;
    private bool dashInput;
    private bool interactInput;

    // Other GameObject, set when the player attempts to interact with them
    private GameObject interactable;
    
    // Dynamically updated movement direction; Set to the zero vector when not moving
    private Vector3 moveDirection;

    // Effectively the same as the moveDirection, except when not moving, the orientation stores the last non-zero moveDirection to allow dashing while stationary.
    private Vector3 orientation = new Vector3(0, 0, 1);

    private Rigidbody rb;

    private Vector3 up = new Vector3(-1, 0, -1);
    private Vector3 right = new Vector3(-1, 0, 1);

    // Start is called before the first frame update
    void Start()
    {
        // Disable and hide Cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    
    void Update()
    {
        // Called once per frame. Reads input & updates vars
        GetInput();

        // If dash input pressed and the dash is not on cooldown, execute the dash
        if (dashInput && !Cooldown_manager.instance.IsDashOnCooldown)
        {
            Dash(moveDirection.normalized);
        }
        dashInput = false;

        // If interact input pressed and an interactable object has been set, execute interaction behavior with it.
        if (interactInput && interactable) 
        {
            Interact(interactable);
        }
    }

    void FixedUpdate()
    {
        // Called once per set amount of time. Moves Player
        MovePlayer();
    }

    void GetInput()
    {
        // Sets all input variables to their respective keybinds (as defined in Project Settings --> Input Manager)
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        dashInput = Input.GetButtonDown("Dash");
        interactInput = Input.GetButtonDown("Interact");
    }

    void MovePlayer()
    {
        // Calculate moveDirection
        moveDirection = up * verticalInput + right * horizontalInput;

        // Store the last non-zero moveDirection as the orientation
        if (!moveDirection.Equals(new Vector3(0, 0, 0))) 
        {
            orientation = moveDirection.normalized;
        }

        // Add the forces in direction specified by moveDirection and speed specified by moveSpeed
        rb.AddForce(moveDirection.normalized * moveSpeed, ForceMode.Force);
        
    }

    void Dash(Vector3 vect)
    {   
        // Dashes a with a speed specified by dashScalar and direction specified by orientation
        rb.AddForce(orientation * dashScalar, ForceMode.VelocityChange);
        Cooldown_manager.instance.UpdateDashCooldown();
    }

    void Interact(GameObject interactable)
    {
        Debug.Log("Interaction attempted with " + interactable.name);

        // If the other GameObject is a ShopItem and the shop is active, buy the item.
        if (interactable.tag == "ShopItem" && interactable.GetComponent<Shop_interaction_manager>().IsShopActive) 
        {
            interactable.GetComponent<Shop_interaction_manager>().buy();
        }
    }

    // Getters, Setters
    public GameObject Interactable
    {
        get { return interactable; }
        set { interactable = value; }
    }
}
