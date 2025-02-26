using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    private float maxHealth = 100;
    private float health = 100;
    private bool isDead = false;

    void Update()
    {
        isDead = (health < 0) ? true : false;
    }

    public void SetMaxHealth(float health)
    {
        maxHealth = health;
    }

    public void Heal(float heal)
    {
        if (health < maxHealth)
        {
            health += heal;
        }
    }

    public void TakeDamage(float damage)
    {
        if (health > 0)
        {
            health -= damage;
        }
    }
}
