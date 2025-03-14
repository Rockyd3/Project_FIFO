using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField]
    private float maxHealth = 100;

    [SerializeField]
    private float health = 100;

    [SerializeField]
    private bool isDead = false;

    void Start()
    {
        if (gameObject.tag == "Player"){
            maxHealth = Level_manager.instance.PlayerMaxHealth;
            health = Level_manager.instance.PlayerHealth;
            if (health == -1){
                health = Level_manager.instance.PlayerMaxHealth;
                UpdateHealth();
            }
        }
    }

    void Update()
    {
        isDead = (health <= 0) ? true : false;
    }

    public void SetMaxHealth(float health)
    {
        maxHealth = health;
        UpdateMax();
    }

    public void Heal(float heal)
    {
        if (health < maxHealth)
        {
            health += heal;
            UpdateHealth();
        }
    }

    public void TakeDamage(float damage)
    {
        if (health > 0)
        {
            health -= damage;
            UpdateHealth();
        }
    }

    //Functions used to update the UI Healthbar
    private void UpdateMax(){
        if (gameObject.tag == "Player"){
            Level_manager.instance.SetMaxHealth(maxHealth);
        }
    }

    private void UpdateHealth(){
        if (gameObject.tag == "Player"){
            Level_manager.instance.SetHealth(health);
        }
    }
}
