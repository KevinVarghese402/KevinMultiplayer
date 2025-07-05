using UnityEngine;
using Unity.Netcode;

public class LaunchableCar : NetworkBehaviour
{
    public float launchForce = 8f;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Called directly by server (LaunchTile)
    public void TriggerLaunch()
    {
        if (rb != null)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, launchForce, rb.linearVelocity.z);
        }
    }
}