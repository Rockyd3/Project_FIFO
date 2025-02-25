using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cooldown_manager : MonoBehaviour
{
    public static Cooldown_manager instance;

    private void Awake() // makes Cooldown_manager callable in any script: Cooldown_manager.instance.[]
    {
        instance = this;
    }

    [Header("Cooldowns")]
    // Units are seconds
    [SerializeField]
    private float dashCooldownLength;

    private float dashCooldownCompleteTime;
    private bool isDashOnCooldown;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // If dash is on cooldown and the cooldown timer is over, take the dash off cooldown
        if (isDashOnCooldown && Time.time >= dashCooldownCompleteTime) {
            isDashOnCooldown = false;
        }
    }

    // Called when dash executed
    public void UpdateDashCooldown()
    {   
        // Put the dash on cooldown and set the time for when it should come off cooldown
        isDashOnCooldown = true;
        dashCooldownCompleteTime = Time.time + dashCooldownLength;
    }

    // Getters, setters
    public float DashCooldownLength
    {
        get { return dashCooldownLength; }
        set { dashCooldownLength = value; }
    }

    public float DashCooldownCompleteTime
    {
        get { return dashCooldownCompleteTime; }
        set { dashCooldownCompleteTime = value; }
    }

    public bool IsDashOnCooldown
    {
        get { return isDashOnCooldown; }
        set { isDashOnCooldown = value; }
    }
}
