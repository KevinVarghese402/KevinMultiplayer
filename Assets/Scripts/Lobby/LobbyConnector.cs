using UnityEngine;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Core;
using Unity.Services.Authentication;

public class LobbyConnector : MonoBehaviour
{
    private async void Start()
    {
        if (!UnityServices.State.Equals(ServicesInitializationState.Initialized))
        {
            await UnityServices.InitializeAsync();
        }

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    public async void JoinFirstLobbyAndRelay()
    {
        try
        {
            var response = await LobbyService.Instance.QueryLobbiesAsync();

            if (response.Results.Count > 0)
            {
                Lobby lobby = response.Results[0];

                if (lobby.Data.ContainsKey("relayCode"))
                {
                    string relayCode = lobby.Data["relayCode"].Value;

                    Debug.Log("Joining Relay with code from lobby: " + relayCode);

                    // Use your existing Relay script to join
                    Relay.Instance.JoinRelay(relayCode);
                }
                else
                {
                    Debug.LogWarning("Lobby doesn't contain a relayCode.");
                }
            }
            else
            {
                Debug.Log("No public lobbies found.");
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError("Failed to join lobby or relay: " + e);
        }
    }
}