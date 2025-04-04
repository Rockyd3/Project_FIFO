using UnityEngine;

public class SniperEnemy : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float fireRate = 2f;
    public float projectileSpeed = 50f;
    public float detectionRange = 30f;

    private Transform player;
    private float fireCooldown = 0f;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (player == null) return;

        fireCooldown -= Time.deltaTime;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (fireCooldown <= 0f && distanceToPlayer <= detectionRange)
        {
            ShootAtPlayer();
            fireCooldown = fireRate;
        }
    }

    void ShootAtPlayer()
    {
        Vector3 direction = (player.position - firePoint.position).normalized;
        GameObject bullet = Instantiate(projectilePrefab, firePoint.position, Quaternion.LookRotation(direction));
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.linearVelocity = direction * projectileSpeed;
    }
}
