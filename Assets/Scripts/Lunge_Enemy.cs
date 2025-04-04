using System.Collections;
using UnityEngine;

public class LungeEnemy : MonoBehaviour
{
    public float detectionRadius = 5f;
    public float lungeForce = 10f; // Was 20f before — now shorter lunge
    public float lungeDelay = 0.5f;
    public float recoveryTime = 1.5f;
    public float movementSpeed = 2f;
    public int damage = 10;

    private Transform player;
    private Rigidbody rb;
    private bool isLunging = false;
    private bool isRecovering = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (isLunging || isRecovering) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= detectionRadius)
        {
            StartCoroutine(Lunge());
        }
        else
        {
            // Slowly move towards the player
            Vector3 direction = (player.position - transform.position).normalized;
            rb.MovePosition(transform.position + direction * movementSpeed * Time.fixedDeltaTime);
        }
    }

    IEnumerator Lunge()
    {
        isLunging = true;

        Vector3 direction = (player.position - transform.position).normalized;
        yield return new WaitForSeconds(lungeDelay); // Wind-up

        rb.AddForce(direction * lungeForce, ForceMode.VelocityChange); // Quick burst forward

        yield return new WaitForSeconds(recoveryTime); // After-lunge pause
        isLunging = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isLunging && collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player hit!");

            Health playerHealth = collision.gameObject.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
        }
    }
}

