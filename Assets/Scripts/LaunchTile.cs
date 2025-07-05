using UnityEngine;
using Unity.Netcode;

public class LaunchTile : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return; // Ensure only server handles this

        var launchable = other.GetComponentInParent<LaunchableCar>();
        if (launchable != null)
        {
            launchable.TriggerLaunch(); // Call directly on server
        }
    }
}