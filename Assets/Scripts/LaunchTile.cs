using UnityEngine;
using Unity.Netcode;

public class LaunchTile : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return; 
        var launchable = other.GetComponentInParent<LaunchableCar>();
        if (launchable != null)
        {
            Vector3 launchAudioPosition = transform.position;
            
            launchable.TriggerLaunchRpc(launchAudioPosition);
        }
    }
}