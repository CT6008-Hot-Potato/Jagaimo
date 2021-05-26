/////////////////////////////////////////////////////////////
//  File: JagaimoNetworkManagerLobby.cs
//  Creator: Theodor Danciu
//  Brief: Logic that handles the network connections and the objects that need to sent over the network once the lobby is initialised
/////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using Mirror.FizzySteam;
using UnityEngine;
using UnityEngine.UI;

public class JagaimoNetworkManagerLobby : Mirror.NetworkManager
{
    [SerializeField] private int minPlayers = 2;
    
    [Scene] 
    [SerializeField] private string menuScene = String.Empty;
    [Header("Room")] 
    [SerializeField] private JagaimoRoomPlayerLobby roomPlayerPrefab = null;

    [Header("UI Lobby")] 
    [SerializeField] private Button steamLobbyButton;

    public static event Action OnClientConnected;
    public static event Action OnClientDisconnected;
    public static event Action<NetworkConnection> OnServerReadied;
    public static event Action OnServerStopped;
    
    public List<JagaimoRoomPlayerLobby> RoomPlayers { get; } = new List<JagaimoRoomPlayerLobby>();
   public List<JagaimoRoomPlayerLobby> GamePlayers { get; } = new List<JagaimoRoomPlayerLobby>();

   /// <summary>
   /// When server is hosted, make sure the list of the prefabs that are to be spawned is emptied first,
   /// then add the lobby prefabs into the list
   /// </summary>
   public override void OnStartServer()
   {
       spawnPrefabs.Clear();
       spawnPrefabs = Resources.LoadAll<GameObject>("Spawnables").ToList();
   }

   /// <summary>
   /// When a client joins the lobby, the list of the prefabs that are to be registered and spawned is emptied first,
   /// load the lobby prefabs into the list and register each prefab so it can be used over the network
   /// </summary>
public override void OnStartClient()
   {
       spawnPrefabs.Clear();
       spawnPrefabs = Resources.LoadAll<GameObject>("Spawnables").ToList();

       NetworkClient.ClearSpawners();

       foreach (GameObject prefab in spawnPrefabs)
       {
           NetworkClient.RegisterPrefab(prefab);
       }
   }

    /// <summary>
    /// Server connection handler when a client is attempting to join a lobby
    /// </summary>
    /// <param name="conn">Client's connection attempt</param>
    public override void OnClientConnect(Mirror.NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        OnClientConnected?.Invoke();
    }

    /// <summary>
    /// Server connection handler when a current player is leaving the lobby
    /// </summary>
    /// <param name="conn">Client's connection attempt</param>
    public override void OnClientDisconnect(Mirror.NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        OnClientDisconnected?.Invoke();
    }

    /// <summary>
    /// Server connection handler if the room that the client is trying to connect is full or when the client's current scene is different thant the server's
    /// </summary>
    /// <param name="conn">Client's connection attempt</param>
    public override void OnServerConnect(Mirror.NetworkConnection conn)
    {
        if (numPlayers >= maxConnections)
        {
            conn.Disconnect();
            return;
        }

        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().path != menuScene)
        {
            conn.Disconnect();
            return;
        }
    }

    /// <summary>
    /// Server connection handler to instantiate a new player prefab when the client's connection attempt is a success
    /// </summary>
    /// <param name="conn">Client's connection attempt</param>
    public override void OnServerAddPlayer(Mirror.NetworkConnection conn)
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().path == menuScene)
        {
            bool isLeader = RoomPlayers.Count == 0;

            JagaimoRoomPlayerLobby roomPlayerInstance = Instantiate(roomPlayerPrefab);

            roomPlayerInstance.IsLeader = isLeader;
            
            Mirror.NetworkServer.AddPlayerForConnection(conn, roomPlayerInstance.gameObject);
        }
    }
    
    /// <summary>
    /// When the player host closes the lobby, make sure other players that have joined are removed from the server
    /// </summary>
    /// <param name="conn">Client's connection attempt</param>
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        if (conn.identity != null)
        {
            JagaimoRoomPlayerLobby player = conn.identity.GetComponent<JagaimoRoomPlayerLobby>();

            RoomPlayers.Remove(player);

            NotifyPlayersOfReadyState();
        }

        base.OnServerDisconnect(conn);
    }
    
    /// <summary>
    /// When the server is stopped, clear the list that had the players' details
    /// </summary>
    public override void OnStopServer()
    {
        OnServerStopped?.Invoke();

        RoomPlayers.Clear();
        //GamePlayers.Clear();
    }

    /// <summary>
    /// Notifies the other players from the lobby if a player has readied up
    /// </summary>
    public  void NotifyPlayersOfReadyState()
    {
        foreach (JagaimoRoomPlayerLobby player in RoomPlayers)
        {
            player.HandleReadyToStart(IsReadyToStart());
        }
    }
    
    /// <summary>
    /// Checks if any player has readied player
    /// </summary>
    /// <returns> Player is ready </returns>
    private bool IsReadyToStart()
    {
        if (numPlayers < minPlayers) { return false; }

        foreach (var player in RoomPlayers)
        {
            if (!player.IsReady) { return false; }
        }

        return true;
    }

    public override void OnServerReady(NetworkConnection conn)
    {
        base.OnServerReady(conn);

        OnServerReadied?.Invoke(conn);
    }
    
    // public void StartGame()
    // {
    //     if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == menuScene)
    //     {
    //         if (!IsReadyToStart()) { return; }
    //
    //         mapHandler = new MapHandler(mapSet, numberOfRounds);
    //
    //         ServerChangeScene(mapHandler.NextMap);
    //     }
    // }

    public void SteamLobbyInit()
    {
        transport = gameObject.GetComponent<FizzySteamworks>();
        Debug.Log("SteamSDK loaded for Mirror!");
    }
}
