/////////////////////////////////////////////////////////////
//
//  Script Name: FootballGamemode.cs
//  Creator: Charles Carter
//  Description: A script for the football gamemode
//  
/////////////////////////////////////////////////////////////

//This script uses these namespaces
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RoundManager))]
//This will mostly be in the other scripts anyway
public class FootballGamemode : MonoBehaviour, IGamemode
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
    void IGamemode.PlayerTagged(CharacterManager charTagged) => PlayerTagged();
    bool IGamemode.WinCondition() => ThisWinCondition();

    #endregion

    #region Variables Needed

    [Header("Core Game Elements")]

    //Variables needed for the gamemode
    [SerializeField]
    private RoundManager roundManager;
    public List<CharacterManager> currentActivePlayers = new List<CharacterManager>();
    
    private List<CharacterManager> blueTeam = new List<CharacterManager>();
    private List<CharacterManager> orangeTeam = new List<CharacterManager>();

    //x magnitude is blue goals
    //y magnitude is orange goals
    private Vector2 score;

    [Header("UI Elements")]

    [SerializeField]
    private CountdownTimer countdownTimer;

    [SerializeField]
    private ScrollerText scrollerText;
    [SerializeField]
    private ScoreboardText scoreboard;

    [Header("Spawning Variables")]

    [SerializeField]
    private ArenaManager arenaManager;

    [SerializeField]
    private List<int> spawnSpots = new List<int>();

    [Header("Misc Variables")]

    [SerializeField]
    private Rigidbody potatoRB;
    [SerializeField]
    private BasicTimerBehaviour goalPauseTimer;

    #endregion

    #region Unity Methods

    //Getting the needed components
    private void OnEnable()
    {
        roundManager = roundManager ?? GetComponent<RoundManager>();
        arenaManager = arenaManager ?? GetComponent<ArenaManager>();
    }

    #endregion

    #region Public Methods

    public GAMEMODE_INDEX Return_Mode()
    {
        return GAMEMODE_INDEX.FOOTBALL;
    }

    //Either team scores a goal
    public void Goal(bool blueTeamScore)
    {
        //Updating the score
        if (blueTeamScore)
        {
            score.x++;

            if (scoreboard)
            {
                scoreboard.UpdateBlueScoreText();
            }
        }
        else
        {
            score.y++;

            if (scoreboard)
            {
                scoreboard.UpdateRedScoreText();
            }
        }

        //Updating the event texts
        if (scrollerText)
        {
            scrollerText.AddGoalText(blueTeamScore);
        }

        //Stopping the countdown, putting the players and potato back, starting the countdown up again
        if (countdownTimer)
        {
            countdownTimer.LockTimer(true);
        }

        //Having a few seconds before everything resets
        StartCoroutine(Co_GoalWait(5f));
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

    #endregion

    #region Interface Methods

    //A way for the round manager to set the active players at the start of the game
    private void SettingActivePlayers(CharacterManager[] charArray)
    {
        //If there's no arenamanager, there's nothing more to do
        if (!arenaManager)
        {
            Debug.Log("There's no arena manager", this);

            //Still need to add the active players for testing
            for (int i = 0; i < charArray.Length; ++i)
            {
                currentActivePlayers.Add(charArray[i]);

                //Even numbers on orange team, odd on blue team
                if (i % 2 == 0)
                {
                    orangeTeam.Add(charArray[i]);
                }
                else
                {
                    blueTeam.Add(charArray[i]);
                }
            }

            return;
        }

        spawnSpots = arenaManager.ReturnFootballSpawnIndexers(charArray.Length);
        Transform spotTransform;
        int spotUsed = 0;

        //Going through the give array and adding it to the list
        for (int i = 0; i < charArray.Length; ++i)
        {
            currentActivePlayers.Add(charArray[i]);

            //Even numbers on orange team, odd on blue team
            if (i % 2 == 0)
            {
                spotTransform = arenaManager.GettingSpot(1, spawnSpots[spotUsed]);
                orangeTeam.Add(charArray[i]);
            }
            else
            {
                spotTransform = arenaManager.GettingSpot(0, spawnSpots[spotUsed]);
                blueTeam.Add(charArray[i]);

                //Only increment it at the end of odd numbers so both teams get the same spot before it moves to the next
                spotUsed++;
            }

            charArray[i].transform.position = spotTransform.position;
            charArray[i].transform.rotation = spotTransform.rotation;
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

    //Someone leaves the game
    private void RemoveActivePlayer(CharacterManager characterLeft)
    {
        currentActivePlayers.Remove(characterLeft);
    }

    //This runs when the round is about to start/ during the initial timer
    private void RoundStarting()
    {
        //Having the potato fall
        if (potatoRB)
        {
            potatoRB.isKinematic = false;
        }
    }

    private void RoundEnding()
    {

    }

    //This is what happens when this countdown starts
    private void CountdownStarting()
    {
        //Spawns players on their respective teams' sides
    }

    //When the countdown ends
    private void CountdownEnding()
    {
        //At the end on the countdown, seeing who has more goals
        if (ThisWinCondition())
        {
            //Blue team won
            return;
        }

        //Orange team won

    }

    //Doesnt really do anything in this gamemode
    private void PlayerTagged()
    {

    }

    //Which teams has more goals - blue = return true and orange = return false
    private bool ThisWinCondition()
    {
        //Blue team wins
        if (score.x > score.y)
        {
            return true;
        }

        //Orange team wins
        return false;
    }

    #endregion

    #region Private Methods

    private void PutPlayersInSpawnPoints()
    {
        //Guard clause, nothing should happen without the manager
        if (!arenaManager) return;

        spawnSpots = arenaManager.ReturnFootballSpawnIndexers(currentActivePlayers.Count);

        if (orangeTeam != null)
        {
            //Doing the loops based on the team amounts not the spot amounts
            for (int i = 0; i < orangeTeam.Count; ++i)
            {
                Transform spotTransform = arenaManager.GettingSpot(1, spawnSpots[i]);

                orangeTeam[i].transform.position = spotTransform.position;
                orangeTeam[i].transform.rotation = spotTransform.rotation;
            }
        }

        if (blueTeam != null)
        {
            for (int i = 0; i < blueTeam.Count; ++i)
            {
                Transform spotTransform = arenaManager.GettingSpot(0, spawnSpots[i]);

                blueTeam[i].transform.position = spotTransform.position;
                blueTeam[i].transform.rotation = spotTransform.rotation;
            }
        }
    }

    private IEnumerator Co_GoalWait(float duration)
    {
        yield return new WaitForSeconds(duration);

        //Locking both players for the pause timer to unlock
        for (int i = 0; i < currentActivePlayers.Count; ++i)
        {
            if (currentActivePlayers[i])
            {
                currentActivePlayers[i].LockPlayer();
            }
        }

        //Putting them back in starting points
        PutPlayersInSpawnPoints();

        //Moving the potato back to the start if this has a reference to it (which it should)
        if (potatoRB)
        {
            potatoRB.transform.position = Vector3.zero;
        }

        //The reset timer before the play starts up again
        goalPauseTimer.CallOnTimerStart();
    }

    #endregion
}