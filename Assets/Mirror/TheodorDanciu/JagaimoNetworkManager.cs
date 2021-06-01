/////////////////////////////////////////////////////////////
//  File: JagaimoNetworkManagerLobby.cs
//  Creator: Theodor Danciu
//  Brief: Logic that handles the network connections and the objects that need to sent over the network once the lobby is initialised
/////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JagaimoNetworkManager : Mirror.NetworkManager
{
    [SerializeField] private int minPlayers = 2;
    
    [Header("Project Scenes")]
    [Scene] 
    [SerializeField] private string menuScene = string.Empty;
    [Scene]
    [SerializeField] private string gameplayScene = string.Empty;

    [Header("Room")] 
    [SerializeField] private JagaimoRoomPlayerLobby roomPlayerPrefab = null;
    [SerializeField] private GameObject mpHostConnectMenu;
    [SerializeField] private GameObject lobbyBackButton;

    [Header("Game")] 
    [SerializeField] private JagaimoGamePlayer gamePlayer;
    [SerializeField] private GameObject playerSpawnSystem;

    public static event Action                    OnClientConnected;
    public static event Action                    OnClientDisconnected;
    public static event Action<NetworkConnection> OnServerReadied;
    public static event Action                    OnServerStopped; 
    public static event Action                    OnSceneLoaded;
    
    public List<JagaimoRoomPlayerLobby> RoomPlayers { get; } = new List<JagaimoRoomPlayerLobby>();
    public List<JagaimoGamePlayer>      GamePlayers { get; } = new List<JagaimoGamePlayer>();

    private static List<Transform> spawnPoints = new List<Transform>(); 
    private        int             nextIndex   = 0;

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
        
        lobbyBackButton.gameObject.SetActive(true);
        Debug.Log("Client connected!");
    }

    /// <summary>
    /// Server connection handler when a current player is leaving the lobby
    /// </summary>
    /// <param name="conn">Client's connection attempt</param>
    public override void OnClientDisconnect(Mirror.NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        OnClientDisconnected?.Invoke();
        Debug.Log("Lost connection to the server!");
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

        if (SceneManager.GetActiveScene().path != menuScene)
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
        if (SceneManager.GetActiveScene().path == menuScene)
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
            
            RecalculatePlayerIndices();
            NotifyPlayersOfReadyState();
        }

        if (lobbyBackButton != null)
        {
            lobbyBackButton.gameObject.SetActive(false);
        }

        if (mpHostConnectMenu != null)
        {
            mpHostConnectMenu.gameObject.SetActive(true);
        }

        Debug.Log("Client left the server!");
        base.OnServerDisconnect(conn);
    }
    
    /// <summary>
    /// When the server is stopped, clear the list that had the players' details
    /// </summary>
    public override void OnStopServer()
    {
        OnServerStopped?.Invoke();

        RoomPlayers.Clear();
        GamePlayers.Clear();
    }

    /// <summary>
    /// Notifies the other players from the lobby if a player has readied up
    /// </summary>
    public void NotifyPlayersOfReadyState()
    {
        foreach (JagaimoRoomPlayerLobby jagaimoRoomPlayerLobby in RoomPlayers)
        {
            jagaimoRoomPlayerLobby.HandleReadyToStart(IsReadyToStart());
        }
    }
    
    /// <summary>
    /// Checks if any player has readied player
    /// </summary>
    /// <returns> Player is ready </returns>
    private bool IsReadyToStart()
    {
        if (numPlayers < minPlayers) { return false; }

        foreach (JagaimoRoomPlayerLobby player in RoomPlayers)
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
    
    public void StartGame()
    {
        if (SceneManager.GetActiveScene().path == menuScene)
        {
            if (!IsReadyToStart())
            {
                return;
            }
            ServerChangeScene(gameplayScene);
        }
    }
    
    public override void ServerChangeScene(string newSceneName)
    {
        if (SceneManager.GetActiveScene().path == menuScene)
        {
            // for (int i = RoomPlayers.Count - 1; i >= 0; i--)
            // { 
            //     //for(int i = 0;)
            //     
            //     Mirror.NetworkConnection connectionToClient = RoomPlayers[i].connectionToClient;
            //
            //     JagaimoGamePlayer jagaimoGamePlayer = Instantiate(gamePlayer);
            //
            //     jagaimoGamePlayer.SetDisplayName(RoomPlayers[i].DisplayName);
            //     jagaimoGamePlayer.SetPlayerIndex(RoomPlayers[i].playerIndex);
            //     
            //     Mirror.NetworkServer.Destroy(connectionToClient.identity.gameObject); 
            //     
            //     Mirror.NetworkServer.ReplacePlayerForConnection(connectionToClient, jagaimoGamePlayer.gameObject, true);
            // }

            // foreach (var roomPlayer in RoomPlayers)
            // {
            //     JagaimoGamePlayer tempGamePlayer = new JagaimoGamePlayer();
            //     Mirror.NetworkConnection connectionToClient = roomPlayer.connectionToClient;
            //     
            //     tempGamePlayer.SetDisplayName(roomPlayer.DisplayName);
            //     tempGamePlayer.SetPlayerIndex(roomPlayer.playerIndex);
            //     
            //     Mirror.NetworkServer.Destroy(connectionToClient.identity.gameObject);
            //     
            //     tempGamePlayers.Add(tempGamePlayer);
            //     Debug.Log(tempGamePlayer);
            // }
            base.ServerChangeScene(newSceneName);
        }
    }
    
    /// <summary>
    /// Logic that handles player spawn when the gameplay scene is loaded
    /// </summary>
    /// <param name="sceneName"> Gameplay scene </param>
    public override void OnServerSceneChanged(string sceneName)
    {
        // if (SceneManager.GetActiveScene().path == gameplayScene)
        // {
        //     GameObject playerSpawn = Instantiate(playerSpawnSystem);
        //     NetworkServer.Spawn(playerSpawn);
        // }
        
        Transform spawnPoint = spawnPoints.ElementAtOrDefault(nextIndex);
        if (spawnPoint == null)
        {
            Debug.LogError($"Missing spawn point for player {nextIndex}");
            return;
        }
        //
        // //todo de facut aici cu playeruls spawn
        // if (SceneManager.GetActiveScene().path == gameplayScene)
        // {
        //     foreach (JagaimoGamePlayer t in GamePlayers)
        //     {
        //         GameObject go = t.gameObject;
        //         go.transform.position = spawnPoints[nextIndex].position;
        //         go.transform.rotation = spawnPoints[nextIndex].rotation;
        //         nextIndex++;
        //         Debug.Log(t + " has been moved to " + go.transform);
        //     }
        // }

         if (SceneManager.GetActiveScene().path == gameplayScene)
         {
             for (int i = RoomPlayers.Count - 1; i >= 0; i--)
             { 
                 Mirror.NetworkConnection connectionToClient = RoomPlayers[i].connectionToClient;
        
                 JagaimoGamePlayer jagaimoGamePlayer = Instantiate(gamePlayer);
                 GameObject go = jagaimoGamePlayer.gameObject;
                 go.transform.position = spawnPoints[nextIndex].position;
                 go.transform.rotation = spawnPoints[nextIndex].rotation;
                 nextIndex++;
                 Debug.Log(go.name + " has been spawned and will be be moved!");
                 Debug.Log(go + " has been moved to " + go.transform.position + go.transform.rotation);
                 
                 jagaimoGamePlayer.SetDisplayName(RoomPlayers[i].DisplayName);
                 jagaimoGamePlayer.SetPlayerIndex(RoomPlayers[i].playerIndex);
                 
                 Mirror.NetworkServer.Destroy(connectionToClient.identity.gameObject);

                 Mirror.NetworkServer.ReplacePlayerForConnection(connectionToClient, jagaimoGamePlayer.gameObject, true);
             }
         }
    }
    
    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        if (IsSceneActive(gameplayScene))
        {
            if (NetworkClient.isConnected)
            {
                 foreach (var roomPlayer in RoomPlayers)
                 {
                     roomPlayer.LobbyUI.SetActive(false);
                 }
            }
            base.OnClientSceneChanged(conn);
        }
    }

    private new void OnStopClient()
    {
        StopClient();
        lobbyBackButton.gameObject.SetActive(false);
        mpHostConnectMenu.gameObject.SetActive(true);
        OnClientDisconnected?.Invoke();
    }

    private new void OnStopHost()
    {
        RoomPlayers.Clear();
        GamePlayers.Clear();
        StopHost();
    }

    /// <summary>
    /// Logic that handles lobby behaviour based on the client's status when it disconnects from the lobby
    /// </summary>
    public void ClientServerLeaveButton()
    {
        if (RoomPlayers.Count.Equals(0))
        {
            Debug.LogWarning("RoomPlayers list is empty! Check the instantiating function.");
            return;
        }
        List<JagaimoRoomPlayerLobby> roomPlayers = this.RoomPlayers;
        for (int i = 0; i < roomPlayers.Count; i++)
        {
            if (roomPlayers[i].isLocalPlayer)
            {
                if (roomPlayers[i].IsLeader)
                {
                    Debug.Log("server closed");
                    OnStopHost();
                }
                else
                {
                    Debug.Log("client left");
                    OnStopClient();
                }
            }
        }
    }
    
    public static void AddSpawnPoint(Transform t)
    {
        spawnPoints.Add(t);
        spawnPoints = (from x in spawnPoints orderby x.GetSiblingIndex() select x).ToList();
    }
    
    public static void RemoveSpawnPoint(Transform t)
    {
        spawnPoints.Remove(t);
    }

    public void RecalculatePlayerIndices()
    {
        if (RoomPlayers.Count > 0)
        {
            for (int i = 0; i < RoomPlayers.Count; i++)
            {
                RoomPlayers[i].playerIndex = i;
            }
        }
    }
}
