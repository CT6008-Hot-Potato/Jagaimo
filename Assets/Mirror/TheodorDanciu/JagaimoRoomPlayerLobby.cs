/////////////////////////////////////////////////////////////
//  File: JagaimoNRoomPlayerLobby.cs
//  Creator: Theodor Danciu
//  Brief: Logic that handles the updating state of the lobby including names and weather the players are ready uped 
/////////////////////////////////////////////////////////////

using System;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JagaimoRoomPlayerLobby : NetworkBehaviour
{
    [Header("UI")]
    [SerializeField] GameObject lobbyUI;
    [SerializeField] private TMP_Text[] playerNameTexts = new TMP_Text[4];
    [SerializeField] private TMP_Text[] playerReadyTexts = new TMP_Text[4];
    [SerializeField] private Button startGameButton = null;
    [SerializeField] private Button gameplaySettingsButton; //mutator ui
    
    [Tooltip("Showing the index of the local networked player")]
    [SyncVar]
    public int playerIndex;
    
    [SyncVar(hook = nameof(HandleDisplayNameChanged))]
    public string DisplayName = "Loading...";

    [SyncVar(hook = nameof(HandleReadyStatusChanged))]
    public bool IsReady = false;
    
    
    public GameObject LobbyUI
    {
        get => lobbyUI;
        set => lobbyUI = value;
    }
    private bool isLeader;

    /// <summary>
    /// This is to be used when the lobby is hosted and the player that hosts it becomes the leader
    /// </summary>
    public bool IsLeader
    {
        get => isLeader;
        set
        {
            isLeader = value;
            startGameButton.gameObject.SetActive(value);
        }
    }

    private JagaimoNetworkManager room;

    private JagaimoNetworkManager Room
    {
        get
        {
            if (room != null)
            {
                return room;
            }

            return room = NetworkManager.singleton as JagaimoNetworkManager;
        }
    }


    public override void OnStartAuthority()
    {
        CmdSetDisplayName(JagaimoPlayerNameInput.DisplayName);

        lobbyUI.SetActive(true);
    }

    /// <summary>
    /// When a player attempts to join the lobby and is successful, add the player in the lobby's list and update it's display - name and ready status
    /// </summary>
    public override void OnStartClient()
    {
        Room.RoomPlayers.Add(this);
        Room.RecalculatePlayerIndices();
        DontDestroyOnLoad(gameObject);

        UpdateDisplay();
    }

    /// <summary>
    /// When a player leaves the lobby, the player is removed from the lobby's list and the slot's that was used by the player updates its text
    /// </summary>
    public override void OnStopClient()
    {
        Room.RoomPlayers.Remove(this);

        UpdateDisplay();
    }

    /// <summary>
    /// Logic that handles the ready status when a player from the lobby has readied up
    /// </summary>
    /// <param name="oldValue"> Old state of the player's ready status in the previous frame </param>
    /// <param name="newValue"> Current state of the player's ready status updated </param>
    public void HandleReadyStatusChanged(bool oldValue, bool newValue) => UpdateDisplay();
    
    /// <summary>
    /// Logic that handles the display name that updates if a new player joins the lobby
    /// </summary>
    /// <param name="oldValue"> Old state of the player's name text field in the previous frame </param>
    /// <param name="newValue"> Current state of the player's name text field updated </param>
    public void HandleDisplayNameChanged(string oldValue, string newValue) => UpdateDisplay();

    /// <summary>
    /// Checks whether the player has authority over the object initially,
    /// if not check go through each player in the lobby and update its display - name and ready status.
    /// </summary>
    private void UpdateDisplay()
    {
        if (!hasAuthority)
        {
            foreach (var player in Room.RoomPlayers)
            {
                if (player.hasAuthority)
                {
                    player.UpdateDisplay();
                    break;
                }
            }

            return;
        }

        for (int i = 0; i < playerNameTexts.Length; i++)
        {
            playerNameTexts[i].text = "Waiting For Player...";
            playerReadyTexts[i].text = string.Empty;
        }

        for (int i = 0; i < Room.RoomPlayers.Count; i++)
        {
            playerNameTexts[i].text = Room.RoomPlayers[i].DisplayName;
            playerReadyTexts[i].text = Room.RoomPlayers[i].IsReady ? "<color=green>Ready</color>" : "<color=red>Not Ready</color>";
        }
    }

    /// <summary>
    /// Checks if everyone from the lobby has readied up by making the start game intractable 
    /// </summary>
    /// <param name="readyToStart"> Lobby's general ready status </param>
    public void HandleReadyToStart(bool readyToStart)
    {
        if (!isLeader)
        {
            return;
        }

        startGameButton.interactable = readyToStart;
    }

    /// <summary>
    /// Sends over the network a command to the server to display the player's name when a new player joins the lobby
    /// </summary>
    /// <param name="displayName"> player's display name in the lobby </param>
    [Command]
    private void CmdSetDisplayName(string displayName)
    {
        DisplayName = displayName;
    }

    /// <summary>
    /// Sends over the network a command to the server when a player has readied up 
    /// </summary>
    [Command]
    public void CmdReadyUp()
    {
        IsReady = !IsReady;

        Room.NotifyPlayersOfReadyState();
    }
    
    [Command]
    public void CmdStartGame()
    {
        if (Room.RoomPlayers[0].connectionToClient != connectionToClient)
        {
            return;
        }
        Room.StartGame();
    }
}