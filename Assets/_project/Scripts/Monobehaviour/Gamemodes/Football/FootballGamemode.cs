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

public enum Football_Team
{
    Blue_Team = 0,
    Orange_Team = 1
}

[RequireComponent(typeof(RoundManager))]
//This will mostly be in the other scripts anyway
public class FootballGamemode : MonoBehaviour, IGamemode
{
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
    private WinScreenManager winScreenManager;
    [SerializeField]
    private FootballObjectContainer footballVariables;

    public List<CharacterManager> currentActivePlayers = new List<CharacterManager>();
    
    private List<CharacterManager> blueTeam = new List<CharacterManager>();
    private List<CharacterManager> orangeTeam = new List<CharacterManager>();

    //x magnitude is blue goals
    //y magnitude is orange goals
    private Vector2 score;

    private WinScreenManager wScreenManager;

    [SerializeField]
    private Transform[] goalVFXPositions;

    [SerializeField]
    private ScriptableParticles particlePlayer;
    private ScriptableParticles.Particle VFX = ScriptableParticles.Particle.GoalExplosion;

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

    //Knowing who won after the game
    [SerializeField]
    private bool blueTeamWon = false;

    #endregion

    #region Unity Methods

    //Getting the needed components
    private void OnEnable()
    {
        roundManager = roundManager ?? RoundManager.roundManager;
        arenaManager = arenaManager ?? GetComponent<ArenaManager>();
        winScreenManager = winScreenManager ?? WinScreenManager.instance;

        footballVariables = footballVariables ?? FootballObjectContainer.footballObjectContainer;

        countdownTimer = footballVariables.countdownTimer;
        scrollerText = footballVariables.scrollerText;
        scoreboard = footballVariables.scoreboard;
        potatoRB = footballVariables.potatoRB;
        goalPauseTimer = footballVariables.goalPauseTimer;
        goalVFXPositions = footballVariables.vfxPoints;
        particlePlayer = footballVariables.particleSpawner;
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

            //playing vfx in the orange goal's position
            if (particlePlayer && goalVFXPositions[(int)Football_Team.Blue_Team])
            {
                particlePlayer.CreateParticle(VFX, goalVFXPositions[1].position, Color.red);
            }
        }
        else
        {
            score.y++;

            if (scoreboard)
            {
                scoreboard.UpdateRedScoreText();
            }

            //Playing vfx in the blue goal's position
            if (particlePlayer && goalVFXPositions[(int)Football_Team.Orange_Team])
            {
                particlePlayer.CreateParticle(VFX, goalVFXPositions[0].position, Color.blue);
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
            //Seeing which team won
            ThisWinCondition();

            if (!winScreenManager)
            {
                if (Debug.isDebugBuild)
                {
                    Debug.Log("No win screen manager", this);
                }

                UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
            }
            else
            {
                //This goal was scored during extra time
                if (blueTeamWon)
                {
                    //Playing the win screen and passing through the blue team as the winners
                    winScreenManager.PlayWinScreen(Return_Mode(), currentActivePlayers, blueTeam);
                    enabled = false;
                }
                else
                {
                    //Playing the win screen and passing through the orange team as the winners
                    winScreenManager.PlayWinScreen(Return_Mode(), currentActivePlayers, orangeTeam);
                    enabled = false;
                }
            }
        }
        else
        {
            //Prepare to start the rest of the game
            //Having a few seconds before everything resets
            StartCoroutine(Co_GoalWait(5f));
        }
    }

    //Needed for win screen
    public bool ReturnWinners()
    {
        //only needed for after the game so it shouldn't matter
        return blueTeamWon;
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

        //Still need to add the active players for testing
        for (int i = 0; i < charArray.Length; ++i)
        {
            currentActivePlayers.Add(charArray[i]);
            charArray[i].LockPlayer();

            //Even numbers on blue team, odd on orange team (0 goes to blue)
            if (i % 2 == 0)
            {
                blueTeam.Add(charArray[i]);
            }
            else
            {
                orangeTeam.Add(charArray[i]);
            }
        }

        PutPlayersInSpawnPoints();

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

    //Someone leaves the game
    private void RemoveActivePlayer(CharacterManager characterLeft)
    {
        currentActivePlayers.Remove(characterLeft);
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
        //Having the potato fall
        if (potatoRB)
        {
            potatoRB.isKinematic = false;
        }
        else if (Debug.isDebugBuild)
        {          
            Debug.Log("There's no potato RB", this);
        }
    }

    private void RoundEnding()
    {
        //The game ends when the countdown ends
    }

    //This is what happens when this countdown starts
    private void CountdownStarting()
    {
        UnlockAllPlayers();
    }

    //When the countdown ends
    private void CountdownEnding()
    {
        //At the end on the countdown, seeing who has more goals
        if (ThisWinCondition())
        {
            if (!winScreenManager)
            {
                if (Debug.isDebugBuild)
                {
                    Debug.Log("No win screen manager", this);
                }

                UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
            }

            //A team won
            if (blueTeamWon)
            {
                winScreenManager.PlayWinScreen(Return_Mode(),currentActivePlayers, blueTeam);
            }
            else
            {
                winScreenManager.PlayWinScreen(Return_Mode(), currentActivePlayers, orangeTeam);
            }

            enabled = false;
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
            //This goal was scored during extra time
            if (score.x > score.y)
            {
                blueTeamWon = true;
            }
            else
            {
                blueTeamWon = false;
            }

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
                Transform spotTransform = arenaManager.GettingSpot((int)Football_Team.Orange_Team, spawnSpots[i]);

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
                Transform spotTransform = arenaManager.GettingSpot((int)Football_Team.Blue_Team, spawnSpots[i]);

                blueTeam[i].transform.position = spotTransform.position;
            }
        }

        //I would've preferred something like this
        //blueTeam[i].transform.rotation = spotTransform.rotation;
    }

    private IEnumerator Co_GoalWait(float duration)
    {


        yield return new WaitForSeconds(duration);

        LockAllPlayers();
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