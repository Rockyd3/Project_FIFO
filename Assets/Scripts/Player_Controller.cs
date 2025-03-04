using UnityEngine;
using System.Collections;

public class Player_Controller : MonoBehaviour
{
    
    public float normalSpeed = 5f; // Default movement speed
    public float boostedSpeed = 7.5f; // 1.5x speed boost
    public float boostDuration = 30f; // 30 seconds duration
    private bool isBoosted = false;
    private Rigidbody rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Ensure the player has a Rigidbody
    }

    // Update is called once per frame
    void Update()
    {
        // Move player (Example: Change this if you use different movement logic)
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical) * (isBoosted ? boostedSpeed : normalSpeed) * Time.deltaTime;
        rb.MovePosition(rb.position + movement);

        // Trigger speed boost when 'R' is pressed
        if (Input.GetKeyDown(KeyCode.R) && !isBoosted)
        {
            StartCoroutine(SpeedBoost());
        }
    }
    IEnumerator SpeedBoost()
    {
        isBoosted = true;
        Debug.Log("Speed Boost Activated!");

        yield return new WaitForSeconds(boostDuration); // Wait for 30 seconds

        isBoosted = false;
        Debug.Log("Speed Boost Ended!");
    }
}
