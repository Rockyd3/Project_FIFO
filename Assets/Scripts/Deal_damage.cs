using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deal_damage : MonoBehaviour
{
    // Fixed base damage (treat as const, its only not const to allow unity to serialize it)
    public float baseDamage = 15.0f;

    // How much damage is actually dealt by the attack
    private float damage = 0;

    void Start()
    {
        // Decrease enemy damage based on fortified upgrade
        if (gameObject.tag == "Enemy")
        {
            damage = baseDamage * Level_manager.instance.fortifiedUpgradeModifier;
        }
    }

    private void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.GetComponent<Health>() != null)
        {
            damage = baseDamage + Level_manager.instance.precisionUpgradeModifier; // Recalculate damage on every hit, based on upgrade/pattern modifiers
            Health health = c.gameObject.GetComponent<Health>();
            health.TakeDamage(damage);
        }
    }

    // Getters, Setters
    public float Damage {
        get { return damage; }
        set { damage = value; }
    }
}