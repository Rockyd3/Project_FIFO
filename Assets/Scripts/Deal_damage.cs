using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deal_damage : MonoBehaviour
{
    [SerializeField]
    private float damage = 15;

    private void OnTriggerEnter(Collider c)
    {
        if (c.GetComponent<Health>() != null)
        {
            Health health = c.gameObject.GetComponent<Health>();
            health.TakeDamage(damage);
        }
    }
}