using UnityEngine;
using Unity.Netcode;
using Unity.Cinemachine;

public class CameraFollowAssigner : NetworkBehaviour
{
    void Start()
    {
        if (!IsOwner) return; // Only the local player activates their camera

        var vcam = GetComponentInChildren<CinemachineVirtualCamera>(true); // 'true' finds inactive objects
        if (vcam == null)
        {
            Debug.LogWarning("CinemachineVirtualCamera not found.");
            return;
        }

        // Enable this camera only for the local player
        vcam.gameObject.SetActive(true);

        // Optional: follow a specific camera target under the car
        var cameraTarget = transform.Find("CameraTarget");
        if (cameraTarget != null)
        {
            vcam.Follow = cameraTarget;
            vcam.LookAt = cameraTarget;
        }
    }
}