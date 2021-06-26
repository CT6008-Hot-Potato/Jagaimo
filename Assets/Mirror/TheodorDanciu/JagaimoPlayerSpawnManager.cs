using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

// Token: 0x02000010 RID: 16
public class JagaimoPlayerSpawnManager : NetworkBehaviour
{
    [SerializeField] private GameObject playerPrefab = null;

    private static List<Transform> spawnPoints = new List<Transform>();
    private int nextIndex = 0;

    public static void AddSpawnPoint(Transform t)
    {
        spawnPoints.Add(t);
        spawnPoints = (from x in spawnPoints orderby x.GetSiblingIndex() select x).ToList();
    }

    public static void RemoveSpawnPoint(Transform t)
    {
        spawnPoints.Remove(t);
    }

    public override void OnStartServer()
    {
        JagaimoNetworkManager.OnServerReadied += SpawnPlayer;
    }

    [ServerCallback]
    private void OnDestroy()
    {
        if (!NetworkServer.active)
        {
            return;
        }

        JagaimoNetworkManager.OnServerReadied -= this.SpawnPlayer;
    }

    [Server]
    public void SpawnPlayer(NetworkConnection conn)
    {
        if (!NetworkServer.active)
        {
            Debug.LogWarning(
                "[Server] function JagaimoPlayerSpawnManager::SpawnPlayer(Mirror.NetworkConnection)' called when server was not active");
            return;
        }

        Transform spawnPoint = spawnPoints.ElementAtOrDefault(nextIndex);

        if (spawnPoint == null)
        {
            Debug.LogError($"Missing spawn point for player {nextIndex}");
        }

        GameObject playerInstance =
            Instantiate(playerPrefab, spawnPoints[nextIndex].position, spawnPoints[nextIndex].rotation);

        NetworkServer.Spawn(playerInstance, conn);

        nextIndex++;
    }
}