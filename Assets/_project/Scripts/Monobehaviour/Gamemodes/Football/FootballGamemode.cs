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
    [SerializeField]
    private FootballObjectContainer footballVariables;

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

    //Dont edit in inspector, it's to view whether the values are correct or not
    [SerializeField]
    private List<int> spawnSpots = new List<int>();

    [Header("Misc Variables")]

    [SerializeField]
    private Rigidbody potatoRB;
    [SerializeField]
    private BasicTimerBehaviour goalPauseTimer;

    private bool bExtraTime = false;

    #endregion

    #region Unity Methods

    //Getting the needed components
    private void OnEnable()
    {
        roundManager = roundManager ?? GetComponent<RoundManager>();
        arenaManager = arenaManager ?? GetComponent<ArenaManager>();

        footballVariables = footballVariables ?? FootballObjectContainer.footballObjectContainer;

        countdownTimer = footballVariables.countdownTimer;
        scrollerText = footballVariables.scrollerText;
        scoreboard = footballVariables.scoreboard;
        potatoRB = footballVariables.potatoRB;
        goalPauseTimer = footballVariables.goalPauseTimer;
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

        if (bExtraTime)
        {
            //This goal was scored during extra time
        }
        else
        {
            //Prepare to start the rest of the game
            //Having a few seconds before everything resets
            StartCoroutine(Co_GoalWait(5f));
        }
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

    #endregion

    #region Interface Methods

    //A way for the round manager to set the active players at the start of the game
    private void SettingActivePlayers(CharacterManager[] charArray)
    {
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

        PutPlayersInSpawnPoints();

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
        else if (Debug.isDebugBuild)
        {          
            Debug.Log("There's no potato RB", this);
        }

        LockAllPlayers();
    }

    private void RoundEnding()
    {
        //The game ends when the countdown ends
    }

    //This is what happens when this countdown starts
    private void CountdownStarting()
    {
        //Players are ready to go
        UnlockAllPlayers();
    }

    //When the countdown ends
    private void CountdownEnding()
    {
        //At the end on the countdown, seeing who has more goals
        if (ThisWinCondition())
        {
            //A team won
            return;
        }

        //A draw
        StartExtraTime();
    }

    //Doesnt really do anything in this gamemode
    private void PlayerTagged()
    {

    }

    //Which teams has more goals - blue = return true and orange = return false
    private bool ThisWinCondition()
    {
        //Blue or Orange team wins
        if (score.x != score.y)
        {
            return true;
        }

        //Draw
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

                //This is the "solution" to not being able to turn the player based on the prefab object
                PlayerCamera camera = orangeTeam[i].GetComponent<PlayerCamera>();
                camera.ChangeYaw(180 / Time.deltaTime);
            }
        }

        if (blueTeam != null)
        {
            for (int i = 0; i < blueTeam.Count; ++i)
            {
                Transform spotTransform = arenaManager.GettingSpot(0, spawnSpots[i]);

                blueTeam[i].transform.position = spotTransform.position;
            }
        }

        //I would've preferred something like this
        //blueTeam[i].transform.rotation = spotTransform.rotation;
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

    private void StartExtraTime()
    {
        //The game ended in a draw so have the clock tick upwards until a goal
        //Putting them back in starting points
        PutPlayersInSpawnPoints();

        //Moving the potato back to the start if this has a reference to it (which it should)
        if (potatoRB)
        {
            potatoRB.transform.position = Vector3.zero;
        }

        //Having the time go to zero and go upwards
        countdownTimer.CountUpwards();

        bExtraTime = true;
    }

    #endregion
}