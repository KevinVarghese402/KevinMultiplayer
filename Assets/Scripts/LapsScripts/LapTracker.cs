using UnityEngine;
using Unity.Netcode;

public class LapTracker : NetworkBehaviour
{
    public int currentLap = 0;
    public int maxLaps = 3;

    public void PassFinishLine()
    {
        if (!IsOwner) return;

        currentLap++;
        Debug.Log($"Player {OwnerClientId} completed lap {currentLap}");

        if (currentLap >= maxLaps)
        {
            GameStateManager.Instance.PlayerFinishedRaceServerRpc(OwnerClientId);
        }
    }
}