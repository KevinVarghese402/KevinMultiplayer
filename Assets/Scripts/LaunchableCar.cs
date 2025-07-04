using UnityEngine;
using Unity.Netcode;

public class LaunchableCar : NetworkBehaviour
{
    public float launchForce = 8f;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Called by the tile when player touches it
    public void RequestLaunch()
    {
        if (!IsOwner) return; // Only the player who owns the car can request a launch

        ApplyLaunchServerRpc(); // Ask the server to apply it
    }

    [ServerRpc]
    void ApplyLaunchServerRpc()
    {
        ApplyLaunch(); // Server applies force
    }

    void ApplyLaunch()
    {
        if (rb != null)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, launchForce, rb.linearVelocity.z);
        }
    }

}