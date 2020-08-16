/////////////////////////////////////////////////////////////
//
//  Script Name: DefaultGamemode.cs
//  Creator: Charles Carter
//  Description: A script for the default gamemode
//  
/////////////////////////////////////////////////////////////

//This script uses these namespaces
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This will mostly be in the other scripts anyway
public class DefaultGamemode : MonoBehaviour, IGamemode
{
    //Fulfilling the interfaces contracted functions
    void IGamemode.RoundStarting()      => RoundStarting();
    void IGamemode.RoundEnding()        => RoundEnding();
    void IGamemode.CountdownStarting()  => CountdownStarting();
    void IGamemode.CountdownEnding()    => CountdownEnding();
    void IGamemode.PlayerTagged()       => PlayerTagged();
    bool IGamemode.WinCondition()       => ThisWinCondition();

    //Getting the needed components
    private void Awake()
    {
        
    }

    //Chooses a tagged player, spawns all players in random points on an arena section
    private void RoundStarting()
    {

    }

    //A podium scene which ragdoll the players in order of elimination
    private void RoundEnding()
    {

    }

    //Doesnt really do anything in this gamemode
    private void CountdownStarting()
    {
        
    }

    //Explodes the tagged player in this gamemode
    private void CountdownEnding()
    {

    }

    //Doesnt really do anything in this gamemode
    private void PlayerTagged()
    {

    }

    //When only 1 person is active in the game, return true
    private bool ThisWinCondition()
    {
        return false;
    }
}
