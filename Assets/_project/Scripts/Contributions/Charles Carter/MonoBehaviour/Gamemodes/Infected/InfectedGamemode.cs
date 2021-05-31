/////////////////////////////////////////////////////////////
//
//  Script Name: InfectedGamemode.cs
//  Creator: Charles Carter
//  Description: A script for the infected gamemode
//  
/////////////////////////////////////////////////////////////

//This script uses these namespaces
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RoundManager))]
//This will mostly be in the other scripts anyway
public class InfectedGamemode : MonoBehaviour, IGamemode {
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
    void IGamemode.ForceEliminatePlayer(CharacterManager charEliminated) => EliminatePlayer(charEliminated);


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
    //private bool bGameStarted = false;

    #endregion

    #region Unity Methods

    //Getting the needed components
    private void OnEnable() {
        roundManager = roundManager ?? RoundManager.roundManager;
        arenaManager = arenaManager ?? GetComponent<ArenaManager>();
        winScreenManager = winScreenManager ?? WinScreenManager.instance;

        settings = GameSettingsContainer.instance;
        potatoPrefab = roundManager.potatoPrefab;
    }

    #endregion

    #region Interface Methods

    //A way for the round manager to set the active players at the start of the game
    private void SettingActivePlayers(CharacterManager[] charArray) {
        //This gamemode uses a random arena
        int arena = arenaManager.GetRandomArenaNumber();

        //Putting the potato in the arena
        roundManager.initialPotatoObject.transform.position = arenaManager.GettingPositionFromArena(arena, 0);

        //Going through the give array and adding it to the list
        for (int i = 0; i < charArray.Length; ++i) {
            activeSurvivors.Add(charArray[i]);
            currentActivePlayers.Add(charArray[i]);
            PutSpecificCharacterInPosition(i, arena);
            charArray[i].LockPlayer();
        }

        if (Debug.isDebugBuild) {
            Debug.Log("Active players set, Amount of Active players: " + currentActivePlayers.Count, this);
        }
    }
    private CharacterManager[] GetActivePlayers() {
        return currentActivePlayers.ToArray();
    }

    //Someone joins the game
    private void AddActivePlayer(CharacterManager newCharacter) {
        currentActivePlayers.Add(newCharacter);
    }

    private void RemoveActivePlayer(CharacterManager characterLeft) {
        Debug.Log("Player Left");
        currentActivePlayers.Remove(characterLeft);
    }

    //Could potentially be something within the round manager which gets the active players from the gamemode (excluding null instances)
    public void LockAllPlayers() {
        //Go through the players
        for (int i = 0; i < currentActivePlayers.Count; ++i) {
            //If it's an actual player within the list
            if (currentActivePlayers[i]) {
                //Use it's unlock function
                currentActivePlayers[i].LockPlayer();
            }
        }
    }

    //Could potentially be something within the round manager which gets the active players from the gamemode (excluding null instances)
    public void UnlockAllPlayers() {
        //Go through the players
        for (int i = 0; i < currentActivePlayers.Count; ++i) {
            //If it's an actual player within the list
            if (currentActivePlayers[i]) {
                //Use it's unlock function
                currentActivePlayers[i].UnLockPlayer();
            }
        }
    }

    public void EliminatePlayer(CharacterManager charEliminated) {
        //Getting a random spot to put the infected onto
        int iRandArena = arenaManager.GetRandomArenaNumber();

        //Putting them back
        PutSpecificCharacterInPosition(charEliminated, iRandArena);

        if (activeSurvivors.Contains(charEliminated)) {
            roundManager.OnPlayerTagged(charEliminated);
        }
    }

    //This runs when the round is about to start/ during the initial timer
    private void RoundStarting() {

        //Choosing a random person(s) to be infected
        //If there are settings
        if (settings) {
            //If the settings include a change to the base infected count
            if (settings.HasGamMutator(2)) {
                Debug.Log("Based Infected Count Raised To: " + ((int)settings.FindGamemodeMutatorValue(2)).ToString());

                List<CharacterManager> chars = currentActivePlayers;
                int iInfectedCount = (int)settings.FindGamemodeMutatorValue(2);

                //If there are enough players to infected without the game just being over
                if (iInfectedCount < currentActivePlayers.Count) {
                    //Going through the active players, tagging them and moving them to the correct lists
                    for (int i = 0; i < iInfectedCount - 1; ++i) {
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

    private void RoundEnding() {
        //The round has ended, it's win screen time... for the survivors
        WinScreen();
    }

    //This is what happens when this countdown starts
    private void CountdownStarting() {
        UnlockAllPlayers();
    }

    //When the countdown ends
    private void CountdownEnding() {
        //At the end of the countdown, doing a last check if survivors won
        if (ThisWinCondition()) {
            //Infected Won
            infectedWon = true;
        } else {
            //Survivors Won
            infectedWon = false;
        }
    }

    //Add them to the infected and take them away from the survivors
    private void PlayerTagged(CharacterManager charTagged) {
        if (activeSurvivors.Contains(charTagged)) {
            //Removing them from the survivors and adding them to the infected
            activeSurvivors.Remove(charTagged);
            activeInfected.Add(charTagged);

            //Telling the character that they've been tagged
            charTagged.ThisPlayerTagged();

            //Getting a random spot to put the infected onto
            int iRandArena = arenaManager.GetRandomArenaNumber();
            Vector3 newPotatoPos = arenaManager.GettingSpot(iRandArena, 0).position;

            //Put another potato on the map
            Instantiate(potatoPrefab, newPotatoPos, Quaternion.identity);

            //Since someone is tagged, the infected could have won
            if (ThisWinCondition()) {
                roundManager.CallOnRoundEnd();
            }
        }
    }

    //When only 1 person is active in the game, return true
    private bool ThisWinCondition() {
        //1 player is left so someone has won this round
        if (activeSurvivors.Count == 0) {
            infectedWon = true;
            return true;
        }

        //There's more than 1 person left active
        return false;
    }

    public GAMEMODE_INDEX Return_Mode() {
        return GAMEMODE_INDEX.INFECTED;
    }

    #endregion

    #region Public Methods

    public bool ReturnWinners() {
        return infectedWon;
    }

    #endregion

    #region Private Methods

    private void WinScreen() {
        //if infected won
        if (infectedWon) {
            winScreenManager.PlayWinScreen(Return_Mode(), currentActivePlayers, activeInfected);
        }
        //if survivors won
        else {
            winScreenManager.PlayWinScreen(Return_Mode(), currentActivePlayers, activeSurvivors);
        }

        enabled = false;
    }

    private void PutSpecificCharacterInPosition(int index, int arenaIndex) {
        Transform spot = arenaManager.GettingSpot(arenaIndex, index + 1);
        currentActivePlayers[index].gameObject.transform.position = spot.position;

        //This is the "solution" to not being able to turn the player based on the prefab object
        PlayerCamera camera = currentActivePlayers[index].GetComponent<PlayerCamera>();
        if (camera) {
            camera.ChangeYaw(spot.rotation.eulerAngles.y / Time.deltaTime);
            camera.flipSpin = !camera.flipSpin;
        }
    }

    //Used for the forced elimination
    private void PutSpecificCharacterInPosition(CharacterManager manager, int arenaIndex) {
        //Doesnt matter if they hit a potato
        Transform spot = arenaManager.GettingSpot(arenaIndex, 0);
        manager.GetComponent<Rigidbody>().velocity = Vector3.zero;
        manager.gameObject.transform.position = spot.position;

        //This is the "solution" to not being able to turn the player based on the prefab object
        PlayerCamera camera = manager.GetComponent<PlayerCamera>();
        if (camera) {
            camera.ChangeYaw(spot.rotation.eulerAngles.y / Time.deltaTime);
            camera.flipSpin = !camera.flipSpin;
        }
    }

    #endregion
}