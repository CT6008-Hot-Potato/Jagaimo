/////////////////////////////////////////////////////////////
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
public class SabotageGamemode : MonoBehaviour, IGamemode {
    #region Interfact Contract Expressions

    //Fulfilling the interfaces contracted functions
    GAMEMODE_INDEX IGamemode.Return_Mode() => Return_Mode();
    CharacterManager[] IGamemode.GetActivePlayers() => GetActivePlayers();

    //These 3 functions will be the same on every gamemode I think
    void IGamemode.SetActivePlayers(CharacterManager[] charArray) => SettingActivePlayers(charArray);
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

    public List<CharacterManager> currentActivePlayers = new List<CharacterManager>();

    //The only trackers needed for mechanics in an overall round
    public CharacterManager currentTagged { get; private set; }
    public CharacterManager previousTagged { get; private set; }

    //Whoever escaped without the potato... or potentially the person with the potato if the time ran out
    private List<CharacterManager> playersWhoWon = new List<CharacterManager>();

    [SerializeField]
    private SabotageEscapeManager escapeManager;

    //Variables for tracking complete generators
    private int iCurrentGeneratorsComplete = 0;
    private int iGeneratorsNeeded = 3;

    [SerializeField]
    private WinScreenManager wScreenManager;
    private bool TaggedPlayerWon = false;

    #endregion

    #region Unity Methods

    //Getting the needed components
    private void OnEnable() {
        wScreenManager = wScreenManager ?? WinScreenManager.instance;
        roundManager = roundManager ?? RoundManager.roundManager;
        arenaManager = arenaManager ?? GetComponent<ArenaManager>();
    }

    #endregion

    #region Interface Methods

    //A way for the round manager to set the active players at the start of the game
    private void SettingActivePlayers(CharacterManager[] charArray) {
        int arenaIndex = arenaManager.GetRandomArenaNumber();

        roundManager.initialPotatoObject.transform.position = arenaManager.GettingPositionFromArena(arenaIndex, 0);

        //Going through the give array and adding it to the list
        for (int i = 0; i < charArray.Length; ++i) {
            currentActivePlayers.Add(charArray[i]);
            PutSpecificCharacterInPosition(i, arenaIndex);
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

    public void EliminatePlayer(CharacterManager charToEliminate) {
        //This is the end of the game
        if (currentActivePlayers.Count <= 2) {
            if (!charToEliminate._tracker.isTagged) {
                TaggedPlayerWon = true;
            }

            roundManager.CallOnRoundEnd();

            if (wScreenManager) {
                wScreenManager.PlayWinScreen(GAMEMODE_INDEX.SABOTAGE, currentActivePlayers, playersWhoWon);
            }

            return;
        }

        if (charToEliminate._tracker.isTagged) {
            CharacterManager character = getRandomCharacter();
            roundManager.OnPlayerTagged(character);
        }
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

        //Tagging a random character
        roundManager.OnPlayerTagged(getRandomCharacter());
    }

    //When the countdown ends
    private void CountdownEnding() {
        //Someone apart from the tagged player didnt escape
        if (playersWhoWon.Count < currentActivePlayers.Count - 1) {
            TaggedPlayerWon = true;
            playersWhoWon.Add(currentTagged);
            playersWhoWon.Reverse();
        }

        if (wScreenManager) {
            wScreenManager.PlayWinScreen(GAMEMODE_INDEX.SABOTAGE, currentActivePlayers, playersWhoWon);
        }
    }

    //Should stop the person from interacting with the generators, and kick them out of any generator they're currently fixing
    private void PlayerTagged(CharacterManager charTagged) {
        //Getting the newly tagged player's tracker
        TaggedTracker tracker = charTagged._tracker;

        //If there is someone tagged when this is called
        if (currentTagged) {
            //They are now the previously tagged
            previousTagged = currentTagged;

            //Turn their tracker back on
            previousTagged._tracker.enabled = true;
            previousTagged._tracker.PlayerUnTagged();
        }

        //The current tagged is now the person tagged
        currentTagged = charTagged;
        currentTagged.ThisPlayerTagged();
    }

    //When everyone has escaped or the time ran out with people still in the arena
    private bool ThisWinCondition() {
        return true;
    }

    public GAMEMODE_INDEX Return_Mode() {
        return GAMEMODE_INDEX.SABOTAGE;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// The general functions specific to this gamemode
    /// </summary>

    public void SetEscapeManager(SabotageEscapeManager newManager) {
        escapeManager = newManager;
    }

    //One of the sabotage points have been completed
    public void SabotageObjectFinished() {
        iCurrentGeneratorsComplete++;

        if (iCurrentGeneratorsComplete >= iGeneratorsNeeded) {
            escapeManager.OpenEscapes();
        }
    }

    //Someone escaped
    public void CharacterEscapes(CharacterManager charWhoEscaped) {
        //That player won
        playersWhoWon.Add(charWhoEscaped);
    }

    public bool TaggedWin() {
        return TaggedPlayerWon;
    }

    #endregion

    #region Private Methods

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

    private CharacterManager getRandomCharacter() {
        if (currentActivePlayers.Count > 0) {
            int i = Random.Range(0, currentActivePlayers.Count);
            return currentActivePlayers[i];
        } else {
            return null;
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
                camera.ChangeYaw(spot.spotTransform.rotation.eulerAngles.y / Time.deltaTime);
                camera.flipSpin = !camera.flipSpin;

                spot.isUsed = true;
            }
        }
    }

    #endregion
}