using UnityEngine;
using UnityEngine.UI;

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
        Relay.Instance.CreateRelay(); // Static access
    }

    private void OnJoinClicked()
    {
        string code = joinCodeInput.text;
        Relay.Instance.JoinRelay(code); //  Static access
    }
}