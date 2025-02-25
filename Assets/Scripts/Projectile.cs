using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile")]
    [SerializeField]
    private float speed;
    [SerializeField]
    private float lifetime;
    [SerializeField]
    private float damage;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    // Note: projectiles use the layer collision matrix to calculate what to collide with.
    // Thus we don't need to manually calculate what collides with what, meaning this function
    // takes no parameters.
    private void OnTriggerEnter()
    {
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        // Very simple straight-line movement
        // Post MVP: maybe implement different, complex moving patterns (homing, spiraling, etc.)
        transform.position += transform.forward * speed * Time.deltaTime;
    }
}
