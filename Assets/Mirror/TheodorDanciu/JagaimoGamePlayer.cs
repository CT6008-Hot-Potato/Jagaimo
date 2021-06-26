using System;
using Mirror;
using UnityEngine;

public class JagaimoGamePlayer : NetworkBehaviour
{
    [SyncVar] 
    private string displayName = "Loading...";

    [Tooltip("Showing the index of the local networked player")]
    [SyncVar]
    public int playerIndex;

    private JagaimoNetworkManager room;

    private JagaimoNetworkManager Room
    {
        get
        {
            if (room != null)
            {
                return room;
            }

            return room = (NetworkManager.singleton as JagaimoNetworkManager);
        }
    }

    public override void OnStartClient()
    {
        DontDestroyOnLoad(gameObject);
        
        this.Room.GamePlayers.Add(this);
    }

    public override void OnStopClient()
    {
        this.Room.GamePlayers.Remove(this);
    }

    private void OnEnable()
    {
        
    }

    [Server]
    public void SetDisplayName(string displayName)
    {
        if (!NetworkServer.active)
        {
            Debug.LogWarning(
                "[Server] function 'System.Void JagaimoGamePlayerLobby::SetDisplayName(System.String)' called when server was not active");
            return;
        }

        this.displayName = displayName;
    }

    [Server]
    public void SetPlayerIndex(int oldIndex)
    {
        this.playerIndex = oldIndex;
    }
}