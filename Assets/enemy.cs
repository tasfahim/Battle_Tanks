using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyTank : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float turnSpeed = 40f;

    [Header("Target Settings")]
    public Transform player;
    public float attackRange = 30f;
    public float fireCooldown = 3f;

    [Header("Turret Settings")]
    public Transform turret;
    public Transform muzzle;

    [Header("Shooting Settings")]
    public GameObject projectilePrefab;
    public float projectileForce = 500f;

    private Rigidbody rb;
    private float fireTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void Update()
    {
        if (player == null) return;

        HandleTurret();

        // Fire at player if in range
        fireTimer -= Time.deltaTime;
        if (Vector3.Distance(transform.position, player.position) <= attackRange && fireTimer <= 0f)
        {
            Fire();
            fireTimer = fireCooldown;
        }
    }

    void FixedUpdate()
    {
        if (player != null)
            HandleMovementPhysics();
    }

    void HandleMovementPhysics()
    {
        // Direction to player (ignore height)
        Vector3 targetDir = player.position - transform.position;
        targetDir.y = 0;

        // Smooth body rotation towards player
        Quaternion lookRotation = Quaternion.LookRotation(targetDir);
        rb.MoveRotation(Quaternion.RotateTowards(rb.rotation, lookRotation, turnSpeed * Time.fixedDeltaTime));

        // Always move forward
        Vector3 moveVector = transform.forward * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + moveVector);
    }

    void HandleTurret()
    {
        if (turret == null) return;

        Vector3 dir = player.position - turret.position;
        dir.y = 0; // only horizontal turret rotation
        Quaternion turretRot = Quaternion.LookRotation(dir);
        turret.rotation = Quaternion.RotateTowards(turret.rotation, turretRot, turnSpeed * Time.deltaTime);
    }

    void Fire()
    {
        if (projectilePrefab != null && muzzle != null)
        {
            GameObject shell = Instantiate(projectilePrefab, muzzle.position, muzzle.rotation);
            Rigidbody rbShell = shell.GetComponent<Rigidbody>();
            if (rbShell != null)
                rbShell.AddForce(muzzle.forward * projectileForce);

            Destroy(shell, 5f);
        }
    }
}
