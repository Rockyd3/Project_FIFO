using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deal_damage : MonoBehaviour
{
    // Fixed base damage (treat as const, its only not const to allow unity to serialize it)
    public float baseDamage = 15.0f;
    public bool isDealtByPlayer = false;

    // How much damage is actually dealt by the attack
    private float damage = 0;

    private void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.GetComponent<Health>() != null)
        {
            damage = baseDamage + Level_manager.instance.precisionUpgradeModifier; // Recalculate damage on every hit, based on upgrade/pattern modifiers

            // Check if this damage is dealt by a player
            if (isDealtByPlayer && !c.CompareTag("Player"))
            {
                // Spice of life upgrade - multiply damage depending on unique patterns found
                int uniquePatterns = Level_manager.instance.uniquePatterns.Count;

                float originalDamage = damage;
                damage = damage * uniquePatterns * Level_manager.instance.spiceLifeUpgradeModifier;

                Debug.Log("Damage collided, unique pattern multiplier: x" + uniquePatterns + ", original damage: " + originalDamage);
            }

            Health health = c.gameObject.GetComponent<Health>();
            health.TakeDamage(damage);
        }
    }

    // Getters, Setters
    public float Damage
    {
        get { return damage; }
        set { damage = value; }
    }
}