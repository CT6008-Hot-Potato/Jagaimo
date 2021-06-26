/////////////////////////////////////////////////////////////
//  File: JagaimoNetworkJoinLobby.cs
//  Creator: Theodor Danciu
//  Brief: Logic that handles the connection to the host's lobby
/////////////////////////////////////////////////////////////
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JagaimoNetworkJoinLobby : MonoBehaviour
{
    [SerializeField] private JagaimoNetworkManager networkManager = null;

    [Header("UI")] 
    [SerializeField] private GameObject landingPagePanel = null;
    [SerializeField] private TMP_InputField ipAddressInputField = null;
    [SerializeField] private Button         joinButton          = null;

    private void OnEnable()
    {
        JagaimoNetworkManager.OnClientConnected += HandleClientConnected;
        JagaimoNetworkManager.OnClientDisconnected += HandleClientDisconnected;
    }

    private void OnDisable()
    {
        JagaimoNetworkManager.OnClientConnected -= HandleClientConnected;
        JagaimoNetworkManager.OnClientDisconnected -= HandleClientDisconnected;
    }

    /// <summary>
    /// Logic that handles the join connection to the server
    /// </summary>
    public void JoinLobby()
    {
        string ipAddress = ipAddressInputField.text;
        networkManager.networkAddress = ipAddress;
        networkManager.StartClient();

        joinButton.interactable = false;
    }

    private void HandleClientConnected()
    {
        joinButton.interactable = true;
        landingPagePanel.SetActive(false);
    }

    private void HandleClientDisconnected()
    {
        joinButton.interactable = true;
    }
}
