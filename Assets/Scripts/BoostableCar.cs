using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using Unity.Netcode;

public class BoostableCar : NetworkBehaviour
{
    public float boostForce = 10f;
    public float hearRadius = 15f;
    private Rigidbody rb;

    public AudioSource audioSource;
    public AudioClip boostClip;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
        
    }

    public void RequestBoost()
    {
        if (!IsServer) return; // Only the server should apply boost force

        if (rb != null)
        {
            Vector3 boostDirection = transform.forward;
            rb.AddForce(boostDirection * boostForce, ForceMode.VelocityChange);
        }

        // Tell all clients to play the boost sound
        
        PlayBoostSFXRpc(transform.position);
    }

    [Rpc(SendTo.Everyone, RequireOwnership = false)]
    private void PlayBoostSFXRpc(Vector3 boostPosition)
    {
        
        var localPlayer = NetworkManager.Singleton.LocalClient.PlayerObject;
        if (localPlayer == null) return;
        
        if (Vector3.Distance(boostPosition, localPlayer.transform.position) <= hearRadius)
        {
            AudioSource.PlayClipAtPoint(boostClip, boostPosition);
        }
        
        
        
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, hearRadius);
    }
    
    

}
