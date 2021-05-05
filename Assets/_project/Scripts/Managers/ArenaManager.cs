////////////////////////////////////////////////////////////
// File: ArenaManager.cs
// Author: Charles Carter
// Date Created: 21/04/21
// Brief: The script that determines and store positions in which the players can spawn in
//////////////////////////////////////////////////////////// 

using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
//Each potential spot
class SpawningSpot
{
    public Transform spotTransform;
    public bool isUsed;
}

[System.Serializable]
//Each "arena" of spots
class Arena
{
    public SpawningSpot[] spots;
}

public class ArenaManager : MonoBehaviour
{
    #region Variables Needed

    //This will likely be the way to spawn in deathmatch or something similar
    [SerializeField]
    private Arena allSpots;
    //0 will likely be the main circle of spawning spots etc
    [SerializeField]
    private Arena[] arenaSpots;

    #endregion

    #region Public Methods

    public Transform GettingSpot(int arenaIndex, int SpotIndex)
    {
        return arenaSpots[arenaIndex].spots[SpotIndex].spotTransform;
    }

    public Vector3 GettingPositionFromArena(int arenaIndex, int SpotIndex)
    {
        return arenaSpots[arenaIndex].spots[SpotIndex].spotTransform.position;
    }

    //This is for consistent spawn points for the football gamemode, the other team will use the same points from arena 1
    public List<int> ReturnFootballSpawnIndexers()
    {
        List<int> points = new List<int>();

        int randSpot = Random.Range(0, arenaSpots[0].spots.Length);
        int randSpotTwo = Random.Range(0, arenaSpots[0].spots.Length);

        while (randSpot == randSpotTwo)
        {
            randSpotTwo = Random.Range(0, arenaSpots[0].spots.Length);
        }

        points.Add(randSpot);
        points.Add(randSpotTwo);

        return points;
    }

    /// <summary>
    /// Random Positions and checks
    /// </summary>

    //Is there a place to spawn in, in a given arena
    public bool isPossibleToSpawnIn(int arenaIndex)
    {
        foreach (SpawningSpot sSpot in arenaSpots[arenaIndex].spots)
        {
            if (!sSpot.isUsed)
            {
                return true;
            }
        }

        return false;
    }

    //Just a random position from any arena
    public Vector3 ReturnRandomPositionFromAllArenas()
    {
        int rand = Random.Range(0, allSpots.spots.Length);

        while (allSpots.spots[rand].isUsed)
        {
            rand = Random.Range(0, allSpots.spots.Length);
        }

        return allSpots.spots[rand].spotTransform.position;
    } 

    //An empty spot from a given arena
    public Vector3 ReturnRandomEmptySpotFromArena(int ArenaIndex)
    {
        int randSpot = Random.Range(0, arenaSpots[ArenaIndex].spots.Length);

        while (arenaSpots[ArenaIndex].spots[randSpot].isUsed)
        {
            randSpot = Random.Range(0, arenaSpots[ArenaIndex].spots.Length);
        }

        return arenaSpots[ArenaIndex].spots[randSpot].spotTransform.position;
    }

    //Clearing known usage from a given arena
    public void ClearUsageFromArena(int ArenaIndex)
    {
        for (int i = 0; i < arenaSpots[ArenaIndex].spots.Length; ++i)
        {
            arenaSpots[ArenaIndex].spots[i].isUsed = false;
        }
    }

    #endregion
}
