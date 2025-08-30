using UnityEngine;

public class DamageForwarder : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        SendMessageUpwards("OnCollisionEnter", collision, SendMessageOptions.DontRequireReceiver);
    }
}
