using System;
using Unity.Netcode;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode.Transports.UTP;

public class Relay : MonoBehaviour
{
    public static Relay Instance;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this; //  Assign the static instance
        }
        else
        {
            Destroy(gameObject); 
        }
    }
    private async void Start()
    {
        // Initialize Unity Services
        await UnityServices.InitializeAsync();
        
        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed In As " + AuthenticationService.Instance.PlayerId);
        };

        // Sign in anonymously
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        
      
   
    }

    public async void CreateRelay()
    {
        try
        {
            // Create a relay allocation for up to 4 players
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(4);
            
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log("Join Code: " + joinCode);

            // Set the relay server data for the host
            UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetHostRelayData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData
            );

            // Start hosting
            NetworkManager.Singleton.StartHost();
            Debug.Log("Host started with Relay.");
        }
        catch (RelayServiceException e)
        {
            Debug.LogError("Relay Create Error: " + e);
        }
    }

    // Call this function with a join code to connect as a client
    public async void JoinRelay(string joinCode)
    {
        try
        {
            Debug.Log("Trying to join Relay with code: " + joinCode);

            
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            
            UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetClientRelayData(
                joinAllocation.RelayServer.IpV4,
                (ushort)joinAllocation.RelayServer.Port,
                joinAllocation.AllocationIdBytes,
                joinAllocation.Key,
                joinAllocation.ConnectionData,
                joinAllocation.HostConnectionData
            );

            // Start the client connection
            NetworkManager.Singleton.StartClient();
            Debug.Log("Client joined Relay.");
        }
        catch (RelayServiceException e)
        {
            Debug.LogError("Error: " + e);
        }
    }
}
