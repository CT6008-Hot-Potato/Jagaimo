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
public class DefaultGamemode : MonoBehaviour, IGamemode
{
    #region Interface Contract Expressions

    //Fulfilling the interfaces contracted functions
    GAMEMODE_INDEX IGamemode.Return_Mode() => Return_Mode();
    
    //These 3 functions will be the same on every gamemode I think
    void IGamemode.SetActivePlayers(CharacterManager[] charArray)        => SettingActivePlayers(charArray);
    void IGamemode.AddActivePlayer(CharacterManager charToAdd)          => AddActivePlayer(charToAdd);
    void IGamemode.RemoveActivePlayer(CharacterManager charToRemove)    => RemoveActivePlayer(charToRemove);

    void IGamemode.RoundStarted()      => RoundStarting();
    void IGamemode.RoundEnded()        => RoundEnding();
    void IGamemode.CountdownStarted()  => CountdownStarting();
    void IGamemode.CountdownEnded()    => CountdownEnding();
    void IGamemode.PlayerTagged(CharacterManager charTagged)      => PlayerTagged(charTagged);
    bool IGamemode.WinCondition()      => ThisWinCondition();

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

    #endregion

    #region Unity Methods

    //Getting the needed components
    private void OnEnable()
    {
        roundManager = roundManager ?? RoundManager.roundManager;
        arenaManager = arenaManager ?? GetComponent<ArenaManager>();
        winScreenManager = winScreenManager ?? WinScreenManager.instance;
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
            charArray[i].UnLockPlayer();
        }

        if (Debug.isDebugBuild)
        {
            Debug.Log("Active players set, Amount of Active players: " + currentActivePlayers.Count, this);
        }

        //Using this instead of the function here because of the scroller text
        CharacterManager manager = getRandomCharacter();
        if (manager)
        {
            roundManager.OnPlayerTagged(manager);
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
        //Debug.Log(currentActivePlayers.Count);
    }

    //This runs when the round is about to start/ during the initial timer
    private void RoundStarting()
    {

    }

    //A podium scene which ragdoll the players in order of elimination but doesnt go back to menu/lobby unless hit max round
    private void RoundEnding()
    {

    }

    //This is what happens when this countdown starts
    private void CountdownStarting()
    {
        if (iCountdownIndex > 0)
        {
            //Tags previously tagged character if there was one, if not choose a random character 
            if (previousTagged)
            {
                roundManager.OnPlayerTagged(previousTagged);
            }
            else
            {
                roundManager.OnPlayerTagged(getRandomCharacter());
            }
        }

        PutCharactersInStartPositions();
    }

    //When the countdown ends
    private void CountdownEnding()
    {
        //Exploding the tagged player and removing from active players
        foreach (CharacterManager cManager in currentActivePlayers)
        {
            if (cManager.CheckIfEliminated(currentActivePlayers.Count))
            {
                Debug.Log("Countdown End");
                RemoveActivePlayer(cManager);
                orderOfEliminations.Add(cManager);
                currentTagged = null;
                break;
            }
        }

        //Each countdown in this gamemode could be the end of this game
        if (ThisWinCondition())
        {
            roundManager.CallOnRoundEnd();

            if (winScreenManager)
            {
                //The making the eliminations in the order of first-last but every player including the winner is included
                orderOfEliminations.Add(currentActivePlayers[0]);
                orderOfEliminations.Reverse();

                winScreenManager.PlayWinScreen(Return_Mode(), orderOfEliminations, orderOfEliminations);

                enabled = false;
                return;
            }
            else
            {
                if (Debug.isDebugBuild)
                {
                    Debug.Log("No win screen manager", this);
                }

                UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
            }
        }
        //It's not the end of the game
        else
        {
            iCountdownIndex++;
            arenaManager.ClearUsageFromArena(0);
            roundManager.CallOnCountdownStart();
        }
    }

    //Doesnt really do anything in this gamemode apart from manage who might be tagged at the start of next round
    private void PlayerTagged(CharacterManager manager)
    {
        if (Debug.isDebugBuild)
        {
            Debug.Log(manager.name + " tagged");
        }

        //Getting the newly tagged player's tracker
        TaggedTracker tracker = manager._tracker;

        //If there is someone tagged when this is called
        if (currentTagged)
        {
            //They are now the previously tagged
            previousTagged = currentTagged;

            //If the previously tagged player isnt the one eliminated from another countdown
            if (currentActivePlayers.Contains(previousTagged) || iCountdownIndex == 0)
            {
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
    private bool ThisWinCondition()
    {
        //No one has loaded in/game hasnt started
        if (currentActivePlayers.Count == 0) return false;

        //1 player is left so someone has won this round
        if (currentActivePlayers.Count == 1)
        {
            return true;
        }

        //There's more than 1 person left active
        return false;
    }

    public GAMEMODE_INDEX Return_Mode()
    {
        return GAMEMODE_INDEX.CLASSIC;
    }

    #endregion

    #region Private Methods

    private CharacterManager getRandomCharacter()
    {
        if (currentActivePlayers.Count > 0)
        {
            int i = Random.Range(0, currentActivePlayers.Count);
            return currentActivePlayers[i];
        }
        else
        {
            return null;
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