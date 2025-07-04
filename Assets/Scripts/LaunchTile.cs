using UnityEngine;
using Unity.Netcode;

public class LaunchTile : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var launchable = other.GetComponentInParent<LaunchableCar>();
        if (launchable != null)
        {
            launchable.RequestLaunch(); // Ask the car to launch
        }
    }
}