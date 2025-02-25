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
    [Header("Dash")]
    [SerializeField]
    private float dashSpeed;
    [SerializeField]
    private float dashDistance;
    private float dashCompleteTime = -1;
    // Projectile prefab for the FireProjectile action
    [Header("Projectile")]
    [SerializeField]
    private GameObject projectilePrefab;
    [SerializeField]
    private float ProjectilePlaneY;

    // From InputManager
    private float horizontalInput;
    private float verticalInput;
    private bool dashInput;
    private bool interactInput;
    private bool fireProjectileInput;

    // Other GameObject, set when the player attempts to interact with them
    private GameObject interactable;

    // Dynamically updated movement direction; Set to the zero vector when not moving
    private Vector3 moveDirection;
    // Effectively the same as the moveDirection, except when not moving, the orientation stores the last non-zero moveDirection to allow dashing while stationary.
    private Vector3 orientation = new Vector3(0, 0, 1);
    private Vector3 up = new Vector3(-1, 0, -1);
    private Vector3 right = new Vector3(-1, 0, 1);
    private Vector3 aimPoint = new Vector3(0, 0, 0);

    private Rigidbody rb;
    private Camera cam;


    // Start is called before the first frame update
    void Start()
    {

        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        cam = Camera.main;
      
        AimToMousePoint();
        aimPoint.y = ProjectilePlaneY;
    }

    
    void Update()
    {
        if (Time.time < dashCompleteTime)
        {
            rb.linearVelocity = orientation * dashSpeed;
        }
        else 
        {
            // Called once per frame. Reads input & updates vars
            GetInput();
            AimToMousePoint();
        }
        
        // If dash input pressed and the dash is not on cooldown, execute the dash
        if (dashInput && !Cooldown_manager.instance.IsDashOnCooldown)
        {
            StartDash(moveDirection.normalized);
        }

        // If interact input pressed and an interactable object has been set, execute interaction behavior with it.
        if (interactInput && interactable) 
        {
            Interact(interactable);
        }

        if (fireProjectileInput && !Cooldown_manager.instance.IsFireProjectileOnCooldown) 
        {
            FireProjectile();
        }

        Vector3 direction = new Vector3(aimPoint.x, transform.position.y, aimPoint.z);
        direction = direction - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 20f);
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
        fireProjectileInput = Input.GetButtonDown("Fire2");
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

    void StartDash(Vector3 vect)
    {
        // Dashes a with a speed specified by dashScalar and direction specified by orientation
        Cooldown_manager.instance.UpdateDashCooldown();
        rb.linearVelocity = orientation * dashSpeed;
        dashCompleteTime = Time.time + (dashDistance / dashSpeed);
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

    void FireProjectile()
    { 
        GameObject projectile = Instantiate(projectilePrefab, new Vector3(transform.position.x, ProjectilePlaneY, transform.position.z), transform.rotation);
        projectile.transform.LookAt(aimPoint);
        projectile.layer = 7; // Set to PlayerProjectiles layer, this makes it so it can only hit enemies and obstacles, but not the player.
        Cooldown_manager.instance.UpdateFireProjectileCooldown();
    }

    void AimToMousePoint()
    {
        float hit;
        Plane plane = new Plane(Vector3.up, -1 * ProjectilePlaneY);
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (plane.Raycast(ray, out hit))
        {
            aimPoint = ray.GetPoint(hit);
        }
    }

    // Getters, Setters
    public GameObject Interactable
    {
        get { return interactable; }
        set { interactable = value; }
    }

    public Vector3 AimPoint
    { 
        get { return aimPoint; } 
        set { aimPoint = value; } 
    }
}
