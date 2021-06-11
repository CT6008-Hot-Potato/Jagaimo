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

[RequireComponent(typeof(RoundManager))]
//This will mostly be in the other scripts anyway
public class DefaultGamemode : MonoBehaviour, IGamemode {
    #region Interface Contract Expressions

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

    //The players in the game 
    public List<CharacterManager> currentActivePlayers = new List<CharacterManager>();

    //A way of tracking who was eliminated in what order
    private List<CharacterManager> orderOfEliminations = new List<CharacterManager>();

    //The only trackers needed for mechanics in an overall round
    public CharacterManager currentTagged { get; private set; }
    public CharacterManager previousTagged { get; private set; }

    //How many countdowns have happened so far
    private int iCountdownIndex = 0;

    [SerializeField]
    private GameObject Potato;
    [SerializeField]
    private CharacterManager backUpCharacter;

    #endregion

    #region Unity Methods

    //Getting the needed components
    private void OnEnable() {
        roundManager = roundManager ?? RoundManager.roundManager;
        arenaManager = arenaManager ?? GetComponent<ArenaManager>();
        winScreenManager = winScreenManager ?? WinScreenManager.instance;

        Potato = roundManager.initialPotatoObject;
    }

    #endregion

    #region Interface Methods

    //A way for the round manager to set the active players at the start of the game
    private void SettingActivePlayers(CharacterManager[] charArray) {
        //This gamemode uses a random arena
        int arena = arenaManager.GetRandomArenaNumber();

        //Putting the potato in the arena
        Potato.transform.position = arenaManager.GettingPositionFromArena(arena, 0);

        //Getting a backup character just in case
        backUpCharacter = charArray[0];

        //Going through the give array and adding it to the list
        for (int i = 0; i < charArray.Length; ++i) {
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

    //Someone dies or leaves the game
    private void RemoveActivePlayer(CharacterManager characterLeft) {
        currentActivePlayers.Remove(characterLeft);
        //orderOfEliminations.Add(characterLeft);
        //Debug.Log(currentActivePlayers.Count);
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
        //This is the end of the game
        if (currentActivePlayers.Count <= 2) {
            roundManager.CallOnRoundEnd();

            if (winScreenManager) {
                //A player cant be eliminated more than once
                if (!orderOfEliminations.Contains(charEliminated)) {
                    currentActivePlayers.Remove(charEliminated);

                    if (currentActivePlayers.Count > 0) {
                        orderOfEliminations.Add(currentActivePlayers[0]);
                    }

                    orderOfEliminations.Add(charEliminated);
                    orderOfEliminations.Reverse();
                }

                winScreenManager.PlayWinScreen(Return_Mode(), orderOfEliminations, orderOfEliminations);
                enabled = false;
            }

            return;
        }

        if (charEliminated._tracker.isTagged) {
            CharacterManager character = getRandomCharacter();
            roundManager.OnPlayerTagged(character);
        }

        RemoveActivePlayer(charEliminated);
        orderOfEliminations.Add(charEliminated);
    }

    //This runs when the round is about to start/ during the initial timer
    private void RoundStarting() {

    }

    //A podium scene which ragdoll the players in order of elimination but doesnt go back to menu/lobby unless hit max round
    private void RoundEnding() {

    }

    //This is what happens when this countdown starts
    private void CountdownStarting() {
        UnlockAllPlayers();

        //Tags previously tagged character if there was one, if not choose a random character 
        if (previousTagged) {
            roundManager.OnPlayerTagged(previousTagged);
        } else {
            CharacterManager nextTagged = getRandomCharacter();

            if (nextTagged != null) {
                roundManager.OnPlayerTagged(nextTagged);
            } else if (currentActivePlayers.Count <= 1) {
                winScreenManager.PlayWinScreen(Return_Mode(), orderOfEliminations, orderOfEliminations);
            }
        }

        if (iCountdownIndex > 0) {
            PutCharactersInStartPositions();
        }
    }

    //When the countdown ends
    private void CountdownEnding() {
        //Exploding the tagged player and removing from active players
        foreach (CharacterManager cManager in currentActivePlayers) {
            if (cManager.CheckIfEliminated(currentActivePlayers.Count)) {
                Debug.Log("Countdown End");
                RemoveActivePlayer(cManager);

                if (!orderOfEliminations.Contains(cManager)) {
                    orderOfEliminations.Add(cManager);
                }

                currentTagged = null;
                break;
            }
        }

        //Each countdown in this gamemode could be the end of this game
        if (ThisWinCondition()) {
            roundManager.CallOnRoundEnd();

            if (winScreenManager) {
                //The making the eliminations in the order of first-last but every player including the winner is included
                orderOfEliminations.Add(currentActivePlayers[0]);
                orderOfEliminations.Reverse();
                winScreenManager.PlayWinScreen(Return_Mode(), orderOfEliminations, orderOfEliminations);

                enabled = false;
                return;
            } else {
                if (Debug.isDebugBuild) {
                    Debug.Log("No win screen manager", this);
                }

                UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
            }
        }
        //It's not the end of the game
        else {
            iCountdownIndex++;
            arenaManager.ClearUsageFromArena(0);
            roundManager.CallOnCountdownStart();
        }
    }

    //Doesnt really do anything in this gamemode apart from manage who might be tagged at the start of next round
    private void PlayerTagged(CharacterManager manager) {
        if (Debug.isDebugBuild) {
            Debug.Log(manager.name + " tagged");
        }

        //Getting the newly tagged player's tracker
        TaggedTracker tracker = manager._tracker;

        //If there is someone tagged when this is called
        if (currentTagged) {
            //They are now the previously tagged
            previousTagged = currentTagged;

            //If the previously tagged player isnt the one eliminated from another countdown
            if (currentActivePlayers.Contains(previousTagged) || iCountdownIndex == 0) {
                //Turn their tracker back on
                previousTagged._tracker.enabled = true;
                //Old tagged now should track if they're hit
                previousTagged._tracker.PlayerUnTagged();
            }
        }

        //The current tagged is now the person tagged
        currentTagged = manager;

        //Telling the player that they should run their tagged function
        tracker.PlayerTagged();
    }

    //When only 1 person is active in the game, return true
    private bool ThisWinCondition() {
        //No one has loaded in/game hasnt started
        if (currentActivePlayers.Count == 0 && !backUpCharacter) return false;

        //1 player is left so someone has won this round
        if (currentActivePlayers.Count == 1) {
            return true;
        }

        //There's more than 1 person left active
        return false;
    }

    public GAMEMODE_INDEX Return_Mode() {
        return GAMEMODE_INDEX.CLASSIC;
    }

    #endregion

    #region Private Methods

    private CharacterManager getRandomCharacter() {
        if (currentActivePlayers.Count > 0) {
            int i = Random.Range(0, currentActivePlayers.Count);
            return currentActivePlayers[i];
        } else {
            if (currentActivePlayers[0]) {
                return currentActivePlayers[0];
            } else {
                if (Debug.isDebugBuild) {
                    Debug.Log("No characters to get from", this);
                }

                if (backUpCharacter) {
                    return backUpCharacter;
                } else {
                    return null;
                }
            }
        }
    }

    private void PutSpecificCharacterInPosition(int index, int arenaIndex) {
        Transform spot = arenaManager.GettingSpot(arenaIndex, index + 1);
        currentActivePlayers[index].gameObject.transform.position = spot.position;

        //This is the "solution" to not being able to turn the player based on the prefab object
        PlayerCamera camera = currentActivePlayers[index].GetComponent<PlayerCamera>();
        if (camera) {
            camera.ChangeYaw(spot.rotation.eulerAngles.y, true);
        }
    }

    private void PutCharactersInStartPositions() {
        //Putting the characters in random spots of the 0th arena
        for (int i = 0; i < currentActivePlayers.Count; ++i) {
            //If there is a spot (may not be due to inspector not being filled out)
            if (arenaManager.isPossibleToSpawnIn(0)) {
                SpawningSpot spot = arenaManager.ReturnRandomSpotForArena(0);
                currentActivePlayers[i].gameObject.transform.position = spot.spotTransform.position;

                //This is the "solution" to not being able to turn the player based on the prefab object
                PlayerCamera camera = currentActivePlayers[i].GetComponent<PlayerCamera>();
                camera.ChangeYaw(spot.spotTransform.rotation.eulerAngles.y, true);

                spot.isUsed = true;
            }
        }
    }

    #endregion
}