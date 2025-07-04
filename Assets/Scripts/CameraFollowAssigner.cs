using UnityEngine;
using Unity.Netcode;
using Unity.Cinemachine;

public class CameraFollowAssigner : MonoBehaviour
{
    private CinemachineVirtualCamera vcam;

    void Start()
    {
        vcam = GetComponent<CinemachineVirtualCamera>();

        Debug.Log("📦 Current GameObject: " + gameObject.name);

        if (vcam == null)
        {
            Debug.LogError("CinemachineVirtualCamera not found on: " + gameObject.name);
            return;
        }
        else
        {
            Debug.Log("CinemachineVirtualCamera FOUND on: " + gameObject.name);
        }

        foreach (var netObject in FindObjectsOfType<NetworkObject>())
        {
            if (netObject.IsOwner)
            {
                Debug.Log("Found local player's object: " + netObject.name);
                vcam.Follow = netObject.transform;
                vcam.LookAt = netObject.transform;
                break;
            }
        }
    }
}