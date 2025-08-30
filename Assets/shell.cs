using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage = 20;
    public float lifeTime = 5f;
    public string targetTag = "Enemy"; // set per prefab

    public GameObject explosionEffect;
    public float explosionForce = 700f;
    public float explosionRadius = 5f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (explosionEffect != null)
        {
            GameObject explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            Destroy(explosion, 3f);
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider nearby in colliders)
        {
            Rigidbody rb = nearby.GetComponent<Rigidbody>();
            if (rb != null)
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
        }

        BaseHealth target = collision.gameObject.GetComponentInParent<BaseHealth>();
        if (target != null && target.CompareTag(targetTag))
        {
            target.TakeDamage(damage);
            Debug.Log($"Hit {targetTag}! -{damage} HP");
        }

        Destroy(gameObject);
    }
}
