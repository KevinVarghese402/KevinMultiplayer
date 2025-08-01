using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RelayUIManager : MonoBehaviour
{
    public InputField joinCodeInput;
    public Button hostButton;
    public Button joinButton;

    private void Start()
    {
        hostButton.onClick.AddListener(OnHostClicked);
        joinButton.onClick.AddListener(OnJoinClicked);
    }

    private void OnHostClicked()
    {
        StartCoroutine(HostRoutine());
    }

    private IEnumerator HostRoutine()
    {
        var relayTask = Relay.Instance.CreateRelayAndReturnJoinCode(); // This method must return a Task<string>
        while (!relayTask.IsCompleted) yield return null;

        if (!string.IsNullOrEmpty(relayTask.Result))
        {
            LobbyManager.Instance.CreateLobbyWithRelayCode(relayTask.Result);
        }
        else
        {
            Debug.LogError("Relay join code was null or empty.");
        }
    }

    private void OnJoinClicked()
    {
        // If input field has a code, join directly. Otherwise, use public lobby system.
        string code = joinCodeInput.text.Trim();

        if (!string.IsNullOrEmpty(code))
        {
            Relay.Instance.JoinRelay(code); // Manual join by code
        }
        else
        {
            LobbyManager.Instance.JoinFirstLobbyAndRelay(); // Join via public lobby
        }
    }
}