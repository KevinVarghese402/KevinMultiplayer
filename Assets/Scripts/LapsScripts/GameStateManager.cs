using UnityEngine;
using Unity.Netcode;

public class GameStateManager : NetworkBehaviour
{
    public static GameStateManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    [Rpc(SendTo.Server)]
    public void PlayerFinishedRaceServerRpc(ulong clientId)
    {
        Debug.Log($"Player {clientId} has finished the race!");
        // You can add logic here for leaderboard, end game screen, etc.
    }
}