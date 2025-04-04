using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float lifetime = 5f;
    public float damage = 10f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Health health = collision.gameObject.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }
        }

        Destroy(gameObject);
    }
}

// public class Projectile : MonoBehaviour
// {
//     [Header("Projectile")]
//     [SerializeField]
//     private float speed;
//     [SerializeField]
//     private float lifetime;
//     [SerializeField]
//     private float damage;

//     // Start is called once before the first execution of Update after the MonoBehaviour is created
//     void Start()
//     {
//         Destroy(gameObject, lifetime);
//     }

//     // Note: projectiles use the layer collision matrix to calculate what to collide with.
//     // Thus we don't need to manually calculate what collides with what, meaning this function
//     // takes no parameters.
//     private void OnTriggerEnter()
//     {
//         Destroy(gameObject);
//     }

//     // Update is called once per frame
//     void Update()
//     {
//         // Very simple straight-line movement
//         // Post MVP: maybe implement different, complex moving patterns (homing, spiraling, etc.)
//         transform.position += speed * Time.deltaTime * transform.forward;
//     }
// }
