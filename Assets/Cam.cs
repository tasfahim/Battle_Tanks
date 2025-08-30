using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target; // assign your Player Tank

    [Header("Camera Settings")]
    public Vector3 offset = new Vector3(0f, 33.6f, -25f); // tuned for your current height/angle
    public float followSpeed = 5f;
    public float rotationSpeed = 5f;
    public float tiltAngle = 25f; // camera tilt (X rotation)

    void LateUpdate()
    {
        if (target == null) return;

        // Desired position based on offset
        Vector3 desiredPosition = target.position + target.rotation * offset;

        // Smooth follow
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

        // Keep camera tilted down at tank
        Quaternion desiredRotation = Quaternion.Euler(tiltAngle, target.eulerAngles.y, 0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotationSpeed * Time.deltaTime);
    }
}
