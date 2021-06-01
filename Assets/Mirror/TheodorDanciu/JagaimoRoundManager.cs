﻿/////////////////////////////////////////////////////////////
//
//  Script Name: RoundManager.cs
//  Creator: Charles Carter
//  Description: A manager script for the current round
//  
/////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class JagaimoRoundManager : MonoBehaviour
{
     #region Delegate Events 

    //Defining Delegate
    public delegate void RoundEvent();
    public static event RoundEvent RoundStarted;
    public static event RoundEvent RoundEnded;

    //A Countdown is the timer before gamemode's effect
    public delegate void CountdownEvent();
    public static event CountdownEvent CountdownStarted;
    public static event CountdownEvent CountdownEnded;

    //For multiplayer ?
    //public static event CountdownEvent CountdownPauseToggle;

    #endregion

    #region Variables Needed

    //There should only be 1 on scenes
    public static JagaimoRoundManager jagaimoRoundManager;

    //The current gamemode
    public IGamemode _currentGamemode;

    //Necessary for infected and other utility based functionality
    public GameObject potatoPrefab;
    public GameObject initialPotatoObject;

    public WorldBounds worldBoundry;

    //Bits and pieces that will be in some of the game scenes
    [SerializeField]
    private BasicTimerBehaviour startCountdown;
    [SerializeField]
    private ScrollerText eventText;

    //Scripts specifically for local scenes
    [SerializeField]
    private PlayerInputManager playerJoinManager;
    [SerializeField]
    private GameSettingsContainer settings;

    //Starting when the players are in, false for using the trigger
    [SerializeField]
    private bool startWhenReady = true;

    [Header("This is an index so it starts from 0")]
    //The amount starts from 0, since it's compared to an index (this is for testing in scene)
    [SerializeField]
    private int iAmountOfExpectedPlayers = 1;

    //Change this to quickly test gamemodes in scenes
    [SerializeField]
    private GAMEMODE_INDEX testMode;

    
    #endregion

    #region Unity Methods

    private void Awake()
    {
        //Making sure there's only 1 round manager instance on the scene
        if (!jagaimoRoundManager)
        {
            jagaimoRoundManager = this;
        }
        else
        {
            Destroy(this);
        }

        //If there is a gamemode already on the object
        if (TryGetComponent<IGamemode>(out var gamemode))
        {
            _currentGamemode = gamemode;
        }

        //Getting the game settings saved over from the main menu
        GameSettingsContainer settingsContainer = GameSettingsContainer.instance;
        //There are settings to use
        if (settingsContainer)
        {
            if (Debug.isDebugBuild)
            {
                Debug.Log("Settings found", this);
            }

            //The player count here comes from the input manager, to try and meeting the game settings
            iAmountOfExpectedPlayers = settingsContainer.iPlayercount;

            //There is a current gamemode already
            if (_currentGamemode != null)
            {
                Destroy((MonoBehaviour)_currentGamemode);
                _currentGamemode = null;
            }

            //Adding the gamemode from the settings
            AddGamemode(settingsContainer.index);
        }

        //For some reason no gamemode was applied and none was on the object
        if (_currentGamemode == null)
        {
            AddGamemode(testMode);
        }

        if (Debug.isDebugBuild)
        {
            Debug.Log("Gamemode is: " + _currentGamemode.Return_Mode().ToString(), this);
        }
    }

    void Start()
    {
        playerJoinManager = playerJoinManager ? playerJoinManager : PlayerInputManager.instance;

        //Waiting for the players
        StartCoroutine(Co_WaitUntilPlayers());
    }

    #endregion

    #region Public Methods

    //Calling the RoundStarted Delegate Event
    public void CallOnRoundStart()
    {
        //Null checking the delegate event
        if (RoundStarted != null)
        {
            if (Debug.isDebugBuild)
            {
                Debug.Log("Round Started", this);
            }

            RoundStarted.Invoke();
        }

        _currentGamemode.RoundStarted();
    }

    //Calling the RoundEnded Delegate Event
    public void CallOnRoundEnd()
    {
        //Null checking the delegate event
        RoundEnded?.Invoke();

        _currentGamemode.RoundEnded();
    }

    //Calling the CountdownStarted Delegate Event
    public void CallOnCountdownStart()
    {
        //Null checking the delegate event
        CountdownStarted?.Invoke();

        _currentGamemode.CountdownStarted();
    }

    //Calling the CountdownEnded Delegate Event
    public void CallOnCountdownEnd()
    {
        //Null checking the delegate event
        CountdownEnded?.Invoke();

        _currentGamemode.CountdownEnded();
    }

    //A player has been tagged
    public void OnPlayerTagged(JagaimoCharacterManager charManager)
    {
        if (!(MonoBehaviour)_currentGamemode || !charManager)
        {
            return;
        }

        //Telling the gamemode script that this manager is tagged
        //_currentGamemode.PlayerTagged(charManager);

        //Adding it to the scroller text
        if (eventText)
        {
            eventText.AddTaggedText();
        }
    }

    #endregion

    #region Private Methods

    //The coroutine that waits for players before setting the active players
    private IEnumerator Co_WaitUntilPlayers()
    {
        if (!playerJoinManager && Debug.isDebugBuild)
        {
            Debug.Log("Set a player join manager here", this);
            StopCoroutine(Co_WaitUntilPlayers());
        }

        while (playerJoinManager.playerCount < iAmountOfExpectedPlayers)
        {
            yield return null;
        }

        if (_currentGamemode != null)
        {
            //Tell the gamemode to get everything ready
            _currentGamemode.SetActivePlayers(GameObject.FindObjectsOfType<CharacterManager>());
        }

        if (startWhenReady)
        {
            startCountdown.CallOnTimerStart();
            CallOnRoundStart();
        }
    }

    private void AddGamemode(GAMEMODE_INDEX index)
    {
        //add a different gamemode
        switch (index)
        {
            case GAMEMODE_INDEX.INFECTED:
                _currentGamemode = gameObject.AddComponent<InfectedGamemode>();
                break;
            case GAMEMODE_INDEX.SABOTAGE:
                _currentGamemode = gameObject.AddComponent<SabotageGamemode>();
                break;
            case GAMEMODE_INDEX.FOOTBALL:
                _currentGamemode = gameObject.AddComponent<FootballGamemode>();
                break;
            default:
                _currentGamemode = gameObject.AddComponent<DefaultGamemode>();
                break;
        }
    }

    #endregion
}