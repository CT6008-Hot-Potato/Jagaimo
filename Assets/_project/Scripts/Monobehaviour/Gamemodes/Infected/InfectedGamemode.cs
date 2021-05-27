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
    #region Interfact Contract Expressions

    //Fulfilling the interfaces contracted functions
    GAMEMODE_INDEX IGamemode.Return_Mode() => Return_Mode();

    //These 3 functions will be the same on every gamemode I think
    void IGamemode.SetActivePlayers(CharacterManager[] charArray) => SettingActivePlayers(charArray);
    CharacterManager[] IGamemode.GetActivePlayers() => GetActivePlayers();
    void IGamemode.AddActivePlayer(CharacterManager charToAdd) => AddActivePlayer(charToAdd);
    void IGamemode.RemoveActivePlayer(CharacterManager charToRemove) => RemoveActivePlayer(charToRemove);

    void IGamemode.LockActivePlayers() => LockAllPlayers();
    void IGamemode.UnLockActivePlayers() => UnlockAllPlayers();

    //This gamemode is infected: when people are tagged, they join the tagged team, until the timer runs out or everyone is tagged
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

    [SerializeField]
    private WinScreenManager winScreenManager;

    [SerializeField]
    private GameSettingsContainer settings;

    //The players within the game
    public List<CharacterManager> currentActivePlayers = new List<CharacterManager>();

    //The survivors and infected 
    public List<CharacterManager> activeSurvivors = new List<CharacterManager>();
    public List<CharacterManager> activeInfected = new List<CharacterManager>();

    //Knowing whether the infected won or not
    private bool infectedWon = false;

    [SerializeField]
    private GameObject potatoPrefab;
    bool bAlreadyWon = false;

    #endregion

    #region Unity Methods

    //Getting the needed components
    private void OnEnable()
    {
        roundManager = roundManager ?? RoundManager.roundManager;
        arenaManager = arenaManager ?? GetComponent<ArenaManager>();
        winScreenManager = winScreenManager ?? WinScreenManager.instance;

        settings = GameSettingsContainer.instance;
        potatoPrefab = roundManager.potatoPrefab;
    }

    #endregion

    #region Interface Methods

    //A way for the round manager to set the active players at the start of the game
    private void SettingActivePlayers(CharacterManager[] charArray)
    {
        if (!winScreenManager)
        {
            winScreenManager = WinScreenManager.instance;
        }
        winScreenManager.SpawnWinScreen(Return_Mode());

        //Going through the give array and adding it to the list
        for (int i = 0; i < charArray.Length; ++i)
        {
            currentActivePlayers.Add(charArray[i]);
            PutSpecificCharacterInPosition(i);
            charArray[i].LockPlayer();
        }

        if (Debug.isDebugBuild)
        {
            Debug.Log("Active players set, Amount of Active players: " + currentActivePlayers.Count, this);
        }
    }
    private CharacterManager[] GetActivePlayers()
    {
        return currentActivePlayers.ToArray();
    }

    //Someone joins the game
    private void AddActivePlayer(CharacterManager newCharacter)
    {
        currentActivePlayers.Add(newCharacter);
    }

    private void RemoveActivePlayer(CharacterManager characterLeft)
    {
        //currentActivePlayers.Remove(characterLeft);
    }

    //Could potentially be something within the round manager which gets the active players from the gamemode (excluding null instances)
    public void LockAllPlayers()
    {
        //Go through the players
        for (int i = 0; i < currentActivePlayers.Count; ++i)
        {
            //If it's an actual player within the list
            if (currentActivePlayers[i])
            {
                //Use it's unlock function
                currentActivePlayers[i].LockPlayer();
            }
        }
    }

    //Could potentially be something within the round manager which gets the active players from the gamemode (excluding null instances)
    public void UnlockAllPlayers()
    {
        //Go through the players
        for (int i = 0; i < currentActivePlayers.Count; ++i)
        {
            //If it's an actual player within the list
            if (currentActivePlayers[i])
            {
                //Use it's unlock function
                currentActivePlayers[i].UnLockPlayer();
            }
        }
    }

    //This runs when the round is about to start/ during the initial timer
    private void RoundStarting()
    {

        //Make sure everything is in order... small cooldown before countdown to get everything

        activeSurvivors = currentActivePlayers;

        PutCharactersInStartPositions();
    }

    private void RoundEnding()
    {
        //The round has ended, it's win screen time... for the survivors
        WinScreen();
    }

    //This is what happens when this countdown starts
    private void CountdownStarting()
    {
        UnlockAllPlayers();

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
                    int iRandomerPlayer = Random.Range(0, currentActivePlayers.Count - 1);
                    roundManager.OnPlayerTagged(currentActivePlayers[iRandomerPlayer]);
                    return;
                }

                //If there are enough players to infected without the game just being over
                if (iInfectedCount < currentActivePlayers.Count)
                {
                    //Going through the active players, tagging them and moving them to the correct lists
                    for (int i = 0; i < iInfectedCount; ++i)
                    {
                        int iRandomerPlayer = Random.Range(0, chars.Count - 1);
                        chars.RemoveAt(iRandomerPlayer);
                        roundManager.OnPlayerTagged(chars[iRandomerPlayer]);
                    }

                    return;
                }
            }
        }

        //Only tag 1 player randomly
        int iRandomPlayer = Random.Range(0, currentActivePlayers.Count - 1);
        roundManager.OnPlayerTagged(currentActivePlayers[iRandomPlayer]);
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

    //Add them to the infected and take them away from the survivors
    private void PlayerTagged(CharacterManager charTagged)
    {
        //Removing them from the survivors and adding them to the infected
        activeSurvivors.Remove(charTagged);
        activeInfected.Add(charTagged);

        //Telling the character that they've been tagged
        charTagged.ThisPlayerTagged();

        //Put another potato on the map
        Instantiate(potatoPrefab);

        //Since someone is tagged, the infected could have won
        if (ThisWinCondition())
        {
            roundManager.CallOnRoundEnd();
        }
    }

    //When only 1 person is active in the game, return true
    private bool ThisWinCondition()
    {
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

    #endregion

    #region Public Methods

    public bool ReturnWinners()
    {
        return infectedWon;
    }

    #endregion

    #region Private Methods

    private void WinScreen()
    {
        //if infected won
        if (infectedWon)
        {
            winScreenManager.PlayWinScreen(Return_Mode(), currentActivePlayers, activeInfected);
        }
        //if survivors won
        else
        {
            winScreenManager.PlayWinScreen(Return_Mode(), currentActivePlayers, activeSurvivors);
        }

        enabled = false;
    }

    private void PutSpecificCharacterInPosition(int index)
    {
        Transform spot = arenaManager.GettingSpot(0, index);
        currentActivePlayers[index].gameObject.transform.position = spot.position;

        //This is the "solution" to not being able to turn the player based on the prefab object
        PlayerCamera camera = currentActivePlayers[index].GetComponent<PlayerCamera>();
        if (camera)
        {
            camera.ChangeYaw(spot.rotation.eulerAngles.y / Time.deltaTime);
            camera.flipSpin = !camera.flipSpin;
        }
    }

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