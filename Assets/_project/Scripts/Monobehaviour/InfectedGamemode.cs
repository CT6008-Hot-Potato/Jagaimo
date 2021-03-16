/////////////////////////////////////////////////////////////
//
//  Script Name: InfectedGamemode.cs
//  Creator: Charles Carter
//  Description: A script for the infected gamemode
//  
/////////////////////////////////////////////////////////////

//This script uses these namespaces
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RoundManager))]
//This will mostly be in the other scripts anyway
public class InfectedGamemode : MonoBehaviour, IGamemode
{
    //Fulfilling the interfaces contracted functions
    GAMEMODE_INDEX IGamemode.Return_Mode() => Return_Mode();

    //These 3 functions will be the same on every gamemode I think
    void IGamemode.SetActivePlayers(CharacterManager[] charArray) => SettingActivePlayers(charArray);
    void IGamemode.AddActivePlayer(CharacterManager charToAdd) => AddActivePlayer(charToAdd);
    void IGamemode.RemoveActivePlayer(CharacterManager charToRemove) => RemoveActivePlayer(charToRemove);

    void IGamemode.RoundStarted() => RoundStarting();
    void IGamemode.RoundEnded() => RoundEnding();
    void IGamemode.CountdownStarted() => CountdownStarting();
    void IGamemode.CountdownEnded() => CountdownEnding();
    void IGamemode.PlayerTagged(CharacterManager charTagged) => PlayerTagged(charTagged);
    bool IGamemode.WinCondition() => ThisWinCondition();

    //Variables needed for the gamemode
    [SerializeField]
    private RoundManager roundManager;
    public List<CharacterManager> currentActivePlayers = new List<CharacterManager>();

    public List<CharacterManager> activeSurvivors = new List<CharacterManager>();
    public List<CharacterManager> activeInfected = new List<CharacterManager>();

    bool infectedWon = false;

    //Getting the needed components
    private void OnEnable()
    {
        roundManager = roundManager ?? GetComponent<RoundManager>();
    }

    //A way for the round manager to set the active players at the start of the game
    private void SettingActivePlayers(CharacterManager[] charArray)
    {
        //Going through the give array and adding it to the list
        for (int i = 0; i < charArray.Length; ++i)
        {
            currentActivePlayers.Add(charArray[i]);
        }

        if (Debug.isDebugBuild)
        {
            Debug.Log("Active players set, Amount of Active players: " + currentActivePlayers.Count, this);
        }
    }

    //Someone joins the game
    private void AddActivePlayer(CharacterManager newCharacter)
    {
        currentActivePlayers.Add(newCharacter);
    }

    //Someone dies or leaves the game
    private void RemoveActivePlayer(CharacterManager characterLeft)
    {
        currentActivePlayers.Remove(characterLeft);
    }

    //This runs when the round is about to start/ during the initial timer
    private void RoundStarting()
    {
        //Make sure everything is in order... small cooldown before countdown to get everything
    }

    //A podium scene which ragdoll the players in order of elimination but doesnt go back to menu/lobby unless hit max round
    private void RoundEnding()
    {

    }

    //This is what happens when this countdown starts
    private void CountdownStarting()
    {
        //Tags previously tagged character if there was one, if not choose a random character 
        //Spawns all character on random points (out of a set number of points) in an arena section
    }

    //When the countdown ends
    private void CountdownEnding()
    {
        //Exploding the tagged player and removing from active players
        foreach (CharacterManager cManager in currentActivePlayers)
        {
            if (cManager.CheckIfEliminated())
            {
                RemoveActivePlayer(cManager);
                break;
            }
        }

        //At the end of the first countdown, doing a last check if survivors won
        if (ThisWinCondition())
        {

        }
    }

    //Doesnt really do anything in this gamemode
    private void PlayerTagged(CharacterManager charTagged)
    {
        //Removing them from the survivors and adding them to the infected
        activeSurvivors.Remove(charTagged);
        activeInfected.Add(charTagged);

        //Since someone is tagged, the infected could have won
        if (ThisWinCondition())
        {
            //The infected has tagged the last player
        }
    }

    //When only 1 person is active in the game, return true
    private bool ThisWinCondition()
    {
        //No one has loaded in/game hasnt started
        if (currentActivePlayers.Count == 0) return false;

        //1 player is left so someone has won this round
        if (activeSurvivors.Count == 0)
        {
            infectedWon = true;
            return true;
        }

        //There's more than 1 person left active
        return false;
    }

    public GAMEMODE_INDEX Return_Mode()
    {
        return GAMEMODE_INDEX.CLASSIC;
    }
}
