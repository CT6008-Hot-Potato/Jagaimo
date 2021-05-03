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

    //This gamemode is infected: when people are tagged, they join the tagged team, until the timer runs out or everyone is tagged
    void IGamemode.RoundStarted() => RoundStarting();
    void IGamemode.RoundEnded() => RoundEnding();
    void IGamemode.CountdownStarted() => CountdownStarting();
    void IGamemode.CountdownEnded() => CountdownEnding();
    void IGamemode.PlayerTagged(CharacterManager charTagged) => PlayerTagged(charTagged);
    bool IGamemode.WinCondition() => ThisWinCondition();

    //Variables needed for the gamemode
    [SerializeField]
    private RoundManager roundManager;

    [SerializeField]
    private GameSettingsContainer settings;

    //The players within the game
    public List<CharacterManager> currentActivePlayers = new List<CharacterManager>();

    //The survivors and infected 
    public List<CharacterManager> activeSurvivors = new List<CharacterManager>();
    public List<CharacterManager> activeInfected = new List<CharacterManager>();

    //Knowing whether the infected won or not
    bool infectedWon = false;

    //Getting the needed components
    private void OnEnable()
    {
        roundManager = roundManager ?? GetComponent<RoundManager>();
        settings = GameSettingsContainer.instance;
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

        activeSurvivors = currentActivePlayers;
    }

    private void RoundEnding()
    {

    }

    //This is what happens when this countdown starts
    private void CountdownStarting()
    {
        //Choosing a random person(s) to be infected

        //If there are settings
        if (settings)
        {
            //If the settings include a change to the base infected count
            if (settings.HasGamMutator(2))
            {
                Debug.Log("Based Infected Count Raised To: " + ((int)settings.FindGamemodeMutatorValue(2)).ToString());

                List<CharacterManager> chars = currentActivePlayers;
                int iInfectedCount = (int)settings.FindGamemodeMutatorValue(2);

                if (iInfectedCount == 0)
                {
                    int iRandomPlayer = Random.Range(0, currentActivePlayers.Count - 1);
                    roundManager.OnPlayerTagged(currentActivePlayers[iRandomPlayer]);
                    return;
                }

                //If there are enough players to infected without the game just being over
                if (iInfectedCount < currentActivePlayers.Count)
                {
                    //Going through the active players, tagging them and moving them to the correct lists
                    for (int i = 0; i < iInfectedCount; ++i)
                    {
                        int iRandomPlayer = Random.Range(0, chars.Count - 1);
                        chars.RemoveAt(iRandomPlayer);
                        roundManager.OnPlayerTagged(chars[iRandomPlayer]);
                    }
                }
                else
                {
                    //Only tag 1 player randomly
                    int iRandomPlayer = Random.Range(0, currentActivePlayers.Count - 1);
                    roundManager.OnPlayerTagged(currentActivePlayers[iRandomPlayer]);
                }
            }
        }
        else
        {
            //Only tag 1 player randomly
            int iRandomPlayer = Random.Range(0, currentActivePlayers.Count - 1);
            roundManager.OnPlayerTagged(currentActivePlayers[iRandomPlayer]);
        }
    }

    //When the countdown ends
    private void CountdownEnding()
    {      
        //At the end of the countdown, doing a last check if survivors won
        if (ThisWinCondition())
        {
            //Infected Won
            infectedWon = true;
        }
        else
        {
            //Survivors Won
            infectedWon = false;
        }

        roundManager.CallOnRoundEnd();
    }

    //Doesnt really do anything in this gamemode
    private void PlayerTagged(CharacterManager charTagged)
    {
        //Removing them from the survivors and adding them to the infected
        activeSurvivors.Remove(charTagged);
        activeInfected.Add(charTagged);

        //Telling the character that they've been tagged
        charTagged.ThisPlayerTagged();

        //TODO: Make 2 types of tagged "Forceably" (which multiplies the potatoes in the game) and "Softly" (which retains the potato it was tagged by)
        //TODO NOTE: This might be on the tagged tracker? and this gamemode will switch the one used

        //Since someone is tagged, the infected could have won
        if (ThisWinCondition())
        {
            infectedWon = true;
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
        return GAMEMODE_INDEX.INFECTED;
    }

    private void WinScreen(bool infectedwin)
    {
        //if infected won
        if (infectedwin)
        {

        }
        //if survivors won
        else
        {

        }
    }
}
