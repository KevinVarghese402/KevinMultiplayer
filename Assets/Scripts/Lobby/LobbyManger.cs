using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Authentication;
using Unity.Services.Core;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance;
    private Lobby currentLobby;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private async void Start()
    {
        if (!UnityServices.State.Equals(ServicesInitializationState.Initialized))
        {
            await UnityServices.InitializeAsync();
        }

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("Signed into Lobby as: " + AuthenticationService.Instance.PlayerId);
        }
    }

    // Called AFTER Relay.Instance.CreateRelay() returns a join code
    public async void CreateLobbyWithRelayCode(string relayJoinCode)
    {
        try
        {
            var data = new Dictionary<string, DataObject>()
            {
                { "relayCode", new DataObject(visibility: DataObject.VisibilityOptions.Public, value: relayJoinCode) }
            };

            CreateLobbyOptions options = new CreateLobbyOptions
            {
                IsPrivate = false,
                Data = data
            };

            currentLobby = await LobbyService.Instance.CreateLobbyAsync("My Lobby", 4, options);
            Debug.Log("Lobby created with Relay Code: " + relayJoinCode);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError("Lobby creation failed: " + e);
        }
    }

    // Call this to find the first public lobby and join its Relay session
    public async void JoinFirstLobbyAndRelay()
    {
        try
        {
            var response = await LobbyService.Instance.QueryLobbiesAsync();

            if (response.Results.Count > 0)
            {
                var lobby = response.Results[0];
                string relayCode = lobby.Data["relayCode"].Value;

                Debug.Log("Found Lobby with Relay Code: " + relayCode);

                // Now join Relay
                Relay.Instance.JoinRelay(relayCode);
            }
            else
            {
                Debug.Log("No public lobbies found.");
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError("Lobby join failed: " + e);
        }
    }
}
