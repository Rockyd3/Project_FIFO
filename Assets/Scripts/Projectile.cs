using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile")]
    [SerializeField] private float speed;
    [SerializeField] private float lifetime;
    [SerializeField] private float damage;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Moves the projectile forward every frame
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Optional: use layers to restrict collisions via physics matrix
        Health health = other.GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
