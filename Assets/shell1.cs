using UnityEngine;

public class ProjectileDamage : MonoBehaviour
{
    [Header("Damage Settings")]
    public int damage = 20;
    public float lifeTime = 5f;

    [Header("Explosion Settings")]
    public GameObject explosionEffect;
    public float explosionForce = 700f;
    public float explosionRadius = 5f;

    [Header("Audio Settings")]
    public AudioClip fireSound;        // sound when shell is created
    public AudioClip explosionSound;   // sound on impact
    public float fireVolume = 1f;
    public float explosionVolume = 1f;

    void Start()
    {
        // 🔊 Play fire sound when spawned
        if (fireSound != null)
            AudioSource.PlayClipAtPoint(fireSound, transform.position, fireVolume);

        Destroy(gameObject, lifeTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        HandleHit(collision);
    }

    // In case child colliders forward collision (optional helper)
    void OnForwardedCollision(Collision collision)
    {
        HandleHit(collision);
    }

    private void HandleHit(Collision collision)
    {
        // 💥 Spawn explosion effect
        if (explosionEffect != null)
        {
            GameObject explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            Destroy(explosion, 3f);
        }

        // 🔊 Play explosion sound
        if (explosionSound != null)
            AudioSource.PlayClipAtPoint(explosionSound, transform.position, explosionVolume);

        // 💥 Apply explosion force
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider nearby in colliders)
        {
            Rigidbody rb = nearby.GetComponent<Rigidbody>();
            if (rb != null)
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
        }

        // 🎯 Deal damage (check parent objects, not just hit collider)
        PlayerHealth player = collision.gameObject.GetComponentInParent<PlayerHealth>();
        if (player != null)
        {
            Debug.Log("Hit Player! -" + damage + " HP");
            player.TakeDamage(damage);
        }

        EnemyHealth enemy = collision.gameObject.GetComponentInParent<EnemyHealth>();
        if (enemy != null)
        {
            Debug.Log("Hit Enemy! -" + damage + " HP");
            enemy.TakeDamage(damage);
        }

        // 🗑️ Destroy projectile
        Destroy(gameObject);
    }
}
