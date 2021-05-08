﻿/////////////////////////////////////////////////////////////
//
//  Script Name: SabotageGamemode.cs
//  Creator: Charles Carter
//  Description: A script for the Sabotage gamemode
//  
/////////////////////////////////////////////////////////////

//This script uses these namespaces
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RoundManager))]
//This will mostly be in the other scripts anyway
public class SabotageGamemode : MonoBehaviour, IGamemode
{
    #region Interfact Contract Expressions

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

    #endregion

    #region Variables Needed

    //Variables needed for the gamemode
    [SerializeField]
    private RoundManager roundManager;
    [SerializeField]
    private ArenaManager arenaManager;

    public List<CharacterManager> currentActivePlayers = new List<CharacterManager>();
    //Whoever escaped without the potato... or potentially the person with the potato if the time ran out
    private List<CharacterManager> playersWhoWon = new List<CharacterManager>();

    [SerializeField]
    private SabotageEscapeManager escapeManager;

    //Variables for tracking complete generators
    private int iCurrentGeneratorsComplete = 0;
    private int iGeneratorsNeeded = 3;

    #endregion

    #region Unity Methods

    //Getting the needed components
    private void OnEnable()
    {
        roundManager = roundManager ?? RoundManager.roundManager;
        arenaManager = arenaManager ?? GetComponent<ArenaManager>();
    }

    #endregion

    #region Interface Methods

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
        //Putting people in the correct positions
        PutCharactersInStartPositions();
    }

    //A podium scene which ragdoll the players in order of elimination but doesnt go back to menu/lobby unless hit max round
    private void RoundEnding()
    {

    }

    //This is what happens when this countdown starts
    private void CountdownStarting()
    {

    }

    //When the countdown ends
    private void CountdownEnding()
    {       
        //This countdown ending determines that someone must have won
        if (ThisWinCondition())
        {
            //Moving to podium screen with a lobby countdown
            if (Debug.isDebugBuild)
            {
               
            }
        }
    }

    //Should stop the person from interacting with the generators, and kick them out of any generator they're currently fixing
    private void PlayerTagged(CharacterManager charTagged)
    {
        //Getting the script which allows them to interact with the generator
        //PogChamp champ = charTagged.GetComponent<PogChamp>();
        //if (champ)
        //{
            //Having the character stop interacting with the generator forcefully
            //champ.StopInteracting();
            //champ.SetAbilityToInteract(false);
        //}
    }

    //When everyone has escaped or the time ran out with people still in the arena
    private bool ThisWinCondition()
    {
        return false;
    }

    public GAMEMODE_INDEX Return_Mode()
    {
        return GAMEMODE_INDEX.SABOTAGE;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// The general functions specific to this gamemode
    /// </summary>

    public void SetEscapeManager(SabotageEscapeManager newManager)
    {
        escapeManager = newManager;
    }

    //One of the sabotage points have been completed
    public void SabotageObjectFinished()
    {
        iCurrentGeneratorsComplete++;

        if (iCurrentGeneratorsComplete >= iGeneratorsNeeded)
        {
            escapeManager.OpenEscapes();
        }
    }

    //Someone escaped
    public void CharacterEscapes(CharacterManager charWhoEscaped)
    {
        //That player won
        playersWhoWon.Add(charWhoEscaped);
    }

    #endregion

    #region Private Methods

    private void PutCharactersInStartPositions()
    {
        //Putting the characters in random spots of the 0th arena
        for (int i = 0; i < currentActivePlayers.Count; ++i)
        {
            //If there is a spot (may not be due to inspector not being filled out)
            if (arenaManager.isPossibleToSpawnIn(0))
            {
                SpawningSpot spot = arenaManager.ReturnRandomSpotForArena(0);
                currentActivePlayers[i].gameObject.transform.position = spot.spotTransform.position;

                //This is the "solution" to not being able to turn the player based on the prefab object
                PlayerCamera camera = currentActivePlayers[i].GetComponent<PlayerCamera>();
                camera.ChangeYaw(spot.spotTransform.rotation.eulerAngles.y / Time.deltaTime);
                camera.flipSpin = !camera.flipSpin;

                spot.isUsed = true;
            }
        }
    }

    #endregion
}