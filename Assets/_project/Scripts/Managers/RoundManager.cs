﻿/////////////////////////////////////////////////////////////
//
//  Script Name: RoundManager.cs
//  Creator: Charles Carter
//  Description: A manager script for the current round
//  
/////////////////////////////////////////////////////////////

//This script uses these namespaces
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//A class to hold the events that happen throughout the round, a round is a full game where the win condition of the gamemode is met
public class RoundManager : MonoBehaviour
{
    //There should only be 1 on scenes
    public static RoundManager roundManager;

    //The current gamemode
    public IGamemode _currentGamemode;

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

    //Bits and pieces that will be in some of the game scenes
    [SerializeField]
    private BasicTimerBehaviour startCountdown;
    [SerializeField]
    private ScrollerText eventText;

    //Scripts specifically for local scenes
    [SerializeField]
    private LocalMPScreenPartioning localMPIndexer;
    [SerializeField]
    private GameSettingsContainer settings;

    //Starting when the players are in, false for using the trigger
    [SerializeField]
    bool startWhenReady = true;

    private void Awake()
    {
        //Making sure there's only 1 round manager instance on the scene
        if (!roundManager)
        {
            roundManager = this;
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
            //There is a current gamemode already
            if (_currentGamemode != null)
            {
                Destroy((MonoBehaviour)_currentGamemode);
                _currentGamemode = null; 
            }

            //Depending on which gamemode, a different script is added
            switch (settingsContainer.index)
            {
                case GAMEMODE_INDEX.CLASSIC:
                    _currentGamemode = gameObject.AddComponent<DefaultGamemode>();
                    break;
                case GAMEMODE_INDEX.INFECTED:
                    _currentGamemode = gameObject.AddComponent<InfectedGamemode>();
                    break;
                case GAMEMODE_INDEX.FOOTBALL:
                    _currentGamemode = gameObject.AddComponent<FootballGamemode>();
                    break;
                case GAMEMODE_INDEX.SABOTAGE:
                    _currentGamemode = gameObject.AddComponent<SabotageGamemode>();
                    break;
                default:
                    _currentGamemode = gameObject.AddComponent<DefaultGamemode>();
                    break;
            }
        }

        //For some reason no gamemode was applied and none was on the object
        if (_currentGamemode == null)
        {
            _currentGamemode = gameObject.AddComponent<DefaultGamemode>();
        }
    }

    private void Start()
    {
        //Waiting for the players
        StartCoroutine(Co_WaitUntilPlayers());
    }

    //The coroutine that waits for players before setting the active players
    private IEnumerator Co_WaitUntilPlayers()
    {
        if (!localMPIndexer && Debug.isDebugBuild)
        {
            Debug.Log("Set An MP Screen Partitioner on this object", this);
            StopCoroutine(Co_WaitUntilPlayers());
        }

        while (localMPIndexer.playerIndex == 0)
        {
            yield return null;
        }

        //This will be done on round start and use non spectator characters in actual version
        _currentGamemode.SetActivePlayers(FindObjectsOfType<CharacterManager>());

        if (startWhenReady)
        {
            startCountdown.CallOnTimerStart();
            CallOnRoundStart();
        }
    }

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
            _currentGamemode.RoundStarted();
        }
    }

    //Calling the RoundEnded Delegate Event
    public void CallOnRoundEnd()
    {
        //Null checking the delegate event
        if (RoundEnded != null)
        {
            RoundEnded.Invoke();
            _currentGamemode.RoundEnded();
        }
    }

    //Calling the CountdownStarted Delegate Event
    public void CallOnCountdownStart()
    {
        //Null checking the delegate event
        if (CountdownStarted != null)
        {
            CountdownStarted.Invoke();
            _currentGamemode.CountdownStarted();
        }
    }

    //Calling the CountdownEnded Delegate Event
    public void CallOnCountdownEnd()
    {
        //Null checking the delegate event
        if (CountdownEnded != null)
        {
            CountdownEnded.Invoke();
            _currentGamemode.CountdownEnded();
        }
    }

    //A player has been tagged
    public void OnPlayerTagged(CharacterManager charManager)
    {
        if (!(MonoBehaviour)_currentGamemode  || !charManager)
        {
            if (Debug.isDebugBuild)
            {
                Debug.Log("Gamemode: " + _currentGamemode.ToString() + " or no char manager: " + charManager.gameObject.name);
            }
        }

        //Telling the gamemode script that this manager is tagged
        _currentGamemode.PlayerTagged(charManager);

        //Adding it to the scroller text
        if (eventText)
        {
            eventText.AddTaggedText();
        }
    }
}
