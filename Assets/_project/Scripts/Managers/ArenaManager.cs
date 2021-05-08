////////////////////////////////////////////////////////////
// File: ArenaManager.cs
// Author: Charles Carter
// Date Created: 21/04/21
// Brief: The script that determines and store positions in which the players can spawn in
//////////////////////////////////////////////////////////// 

//Namespaces required
using System.Collections.Generic;
using UnityEngine;

#region Internal Classes

[System.Serializable]
//Each potential spot
public class SpawningSpot
{
    public Transform spotTransform;
    public bool isUsed;
}

[System.Serializable]
//Each "arena" of spots
public class Arena
{
    public SpawningSpot[] spots;
}

#endregion

public class ArenaManager : MonoBehaviour
{
    #region Variables Needed

    //This will likely be the way to spawn in deathmatch or something similar
    [SerializeField]
    private Arena allSpots;
    //0 will likely be the main circle of spawning spots etc
    [SerializeField]
    private Arena[] arenaSpots;
    //Spots specifically for the spawning of power ups
    [SerializeField]
    private Arena powerUpSpots;

    #endregion

    #region Public Methods

    #region Utility

    //General Methods for utility
    public Transform GettingSpot(int arenaIndex, int SpotIndex)
    {
        return arenaSpots[arenaIndex].spots[SpotIndex].spotTransform;
    }

    public Vector3 GettingPositionFromArena(int arenaIndex, int SpotIndex)
    {
        return arenaSpots[arenaIndex].spots[SpotIndex].spotTransform.position;
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

    //If I need the transform and rotation
    public SpawningSpot ReturnRandomSpotForArena(int ArenaIndex)
    {
        int randSpot = Random.Range(0, arenaSpots[ArenaIndex].spots.Length);

        while (arenaSpots[ArenaIndex].spots[randSpot].isUsed)
        {
            randSpot = Random.Range(0, arenaSpots[ArenaIndex].spots.Length);
        }

        return arenaSpots[ArenaIndex].spots[randSpot];
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

    #region Gamemode Specific

    //This is for consistent spawn points for the football gamemode, the other team will use the same points from arena 1
    public List<int> ReturnFootballSpawnIndexers(int iPlayerCount)
    {
        List<int> points = new List<int>();
        points.Clear();

        //Making sure there's no errors when it comes to moving the players
        if (arenaSpots[0].spots.Length != arenaSpots[1].spots.Length && Debug.isDebugBuild)
        {
            Debug.LogWarning("There should be the same amount of spots in the arenas in this gamemode");
        }

        //Rand can only be 0, 1 or 2
        //Get 1 spot for upto 2 players
        if (iPlayerCount < 3)
        {
            points.Add(Random.Range(0, arenaSpots[0].spots.Length));
        }
        //Get 2 spots for upto 4 players
        else if (iPlayerCount < 5)
        {
            int spotOne = Random.Range(0, arenaSpots[0].spots.Length);
            int spotTwo = Random.Range(0, arenaSpots[0].spots.Length);

            //This should never lead to infinite recursion
            if (arenaSpots[0].spots.Length > 1)
            {
                while (spotOne == spotTwo)
                {
                    spotTwo = Random.Range(0, arenaSpots[0].spots.Length);
                }
            }

            points.Add(spotOne);
            points.Add(spotTwo);
        }
        //Add all 3 spots
        else
        {
            points.Add(0);
            points.Add(1);
            points.Add(2);
        }

        return points;
    }

    #endregion

    #region Power Ups Specific

    //Self explanatory
    public bool canPowerUpSpawn()
    {
        foreach (SpawningSpot sSpot in powerUpSpots.spots)
        {
            if (!sSpot.isUsed)
            {
                return true;
            }
        }

        return false;
    }

    //Just a random position for the power up (inefficent with only 1 spot left)
    public Vector3 ReturnFreePowerUpSpot()
    {
        //The temp list for the free spots
        List<SpawningSpot> freeSpots = new List<SpawningSpot>(); 

        //Going through the spots
        foreach (SpawningSpot spot in powerUpSpots.spots)
        {
            //If they are free
            if (!spot.isUsed)
            {
                //Add them to the list
                freeSpots.Add(spot);
            }
        }

        //Getting one of the free spots and returning it's position
        int rand = Random.Range(0, powerUpSpots.spots.Length);
        return powerUpSpots.spots[rand].spotTransform.position;
    }

    //A power up in a spot was picked up
    public void PowerUpPickedUp(int SpotID)
    {
        powerUpSpots.spots[SpotID].isUsed = false;
    }

    #endregion

    #region All Arena Checks

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

    #endregion

    #endregion
}
