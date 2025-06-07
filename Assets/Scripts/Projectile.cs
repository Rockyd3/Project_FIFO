using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile")]
    [SerializeField] private float speed;
    [SerializeField] private float lifetime;
    [SerializeField] private float damage;
    [SerializeField] private bool Indestructable = false;

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
        Debug.Log($"HIT! {gameObject.name} hit {other.gameObject.name} with Health: {health != null}");
        if (health != null)
        {
            health.TakeDamage(damage);
        }

        if(!Indestructable){ Destroy(gameObject); }
    }
    
    public void SetLifetime(float new_lifetime){
        lifetime = new_lifetime;
    }

    public float GetSpeed(){
        return speed;
    }

    public void SetAsIndestructable(bool indestructable){
        Indestructable = indestructable;
    }
}
