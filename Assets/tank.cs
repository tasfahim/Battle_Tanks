using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 10f;
    public float turnSpeed = 60f;

    [Header("Turret Settings")]
    public Transform turret;       // Drag turret transform here
    public float turretTurnSpeed = 50f;

    [Header("Muzzle Settings")]
    public Transform muzzle;       // Drag muzzle (firePoint) here
    public float muzzleTurnSpeed = 30f;
    public float minMuzzleAngle = 10f;   // now between 10° and 45°
    public float maxMuzzleAngle = 45f;

    [Header("Shooting Settings")]
    public GameObject projectilePrefab;
    public float projectileForce = 500f;

    private float muzzleAngle = 10f; // start at lowest angle

    void Update()
    {
        // Tank body movement
        float move = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        float turn = Input.GetAxis("Horizontal") * turnSpeed * Time.deltaTime;

        transform.Translate(Vector3.forward * move);
        transform.Rotate(Vector3.up * turn);

        // Turret rotation with mouse X
        if (turret != null)
        {
            float turretTurn = Input.GetAxis("Mouse X") * turretTurnSpeed * Time.deltaTime;
            turret.Rotate(Vector3.up * turretTurn);
        }

        // Muzzle up/down with mouse Y
        if (muzzle != null)
        {
            float muzzleTurn = -Input.GetAxis("Mouse Y") * muzzleTurnSpeed * Time.deltaTime;
            muzzleAngle = Mathf.Clamp(muzzleAngle + muzzleTurn, minMuzzleAngle, maxMuzzleAngle);
            muzzle.localRotation = Quaternion.Euler(muzzleAngle, 0f, 0f);
        }

        // Fire projectile (Spacebar only)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Fire();
        }
    }

    void Fire()
    {
        if (projectilePrefab != null && muzzle != null)
        {
            GameObject shell = Instantiate(projectilePrefab, muzzle.position, muzzle.rotation);
            Rigidbody rb = shell.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(muzzle.forward * projectileForce);
            }

            Destroy(shell, 5f); // cleanup
        }
    }

}


