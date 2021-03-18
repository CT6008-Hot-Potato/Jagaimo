/////////////////////////////////////////////////////////////
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

//A small reference to the 2 main scripts of the characters
public class PlayerCharacter
{
    TaggedTracker pcTracker;
    CharacterManager pcManager;
}

//A class to hold the events that happen throughout the round, a round is a full game where the win condition of the gamemode is met
public class RoundManager : MonoBehaviour
{
    //There should only be 1 on scenes
    public static RoundManager roundManager;

    //The current gamemode
    IGamemode _currentGamemode;

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
    BasicTimerBehaviour startCountdown;
    [SerializeField]
    ScrollerText eventText;

    //Scripts specifically for local scenes
    [SerializeField]
    LocalMPScreenPartioning localMPIndexer;
    [SerializeField]
    GameSettingsContainer settings;

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

        //Getting the game settings saved over from the main menu
        GameSettingsContainer settingsContainer = GameSettingsContainer.instance;

        //There are settings to use
        if (settingsContainer)
        {
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
                    break;
            }
        }

        //If there is a gamemode already on the object
        if (!TryGetComponent<IGamemode>(out var gamemode))
        {
            _currentGamemode = gamemode;
        }

        //For some reason no gamemode was applied and none was on the objectwhat
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
    IEnumerator Co_WaitUntilPlayers()
    {
        if (!localMPIndexer && Debug.isDebugBuild)
        {
            Debug.LogError("Set An MP Screen Partitioner on this object", this);
        }

        while (localMPIndexer.playerIndex < 2)
        {
            yield return null;
        }

        //This will be done on round start and use non spectator characters in actual version
        _currentGamemode.SetActivePlayers(FindObjectsOfType<CharacterManager>());
        startCountdown.CallOnTimerStart();
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
        //Telling the gamemode script that this manager is tagged
        _currentGamemode.PlayerTagged(charManager);

        //Adding it to the scroller text
        if (eventText)
        {
            eventText.AddTaggedText();
        }
    }
}
