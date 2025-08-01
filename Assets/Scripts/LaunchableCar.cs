using UnityEngine;
using Unity.Netcode;

public class LaunchableCar : NetworkBehaviour
{
    public float launchForce = 8f;
    private Rigidbody rb;
    
    public float hearRadius = 15f;
    public AudioSource audioSource;
    public AudioClip launchClip;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }
    
    [Rpc(SendTo.Everyone, RequireOwnership = false)] 
    public void TriggerLaunchRpc(Vector3 launchAudioPosition)
    {
        if (rb != null)
        { 
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, launchForce, rb.linearVelocity.z);
        }
        
        var localPlayer = NetworkManager.Singleton.LocalClient.PlayerObject;
        if (localPlayer == null) return;
        
        if (Vector3.Distance(launchAudioPosition, localPlayer.transform.position) <= hearRadius)
        {
            AudioSource.PlayClipAtPoint(launchClip, launchAudioPosition);
        }
        
    }
    
}