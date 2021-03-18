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

//A class to hold the events that happen throughout the round, a round is a full game where everyone is alive to the last player left
public class RoundManager : MonoBehaviour
{
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

    //The only trackers needed for mechanics in an overall round
    public TaggedTracker currentTagged  { get; private set; }
    public TaggedTracker previousTagged { get; private set; }

    //The person who starts as tagged each countdown
    [SerializeField]
    private TaggedTracker initialTagged;

    [SerializeField]
    BasicTimerBehaviour startCountdown;
    [SerializeField]
    ScrollerText eventText;
    [SerializeField]
    LocalMPScreenPartioning localMPIndexer;

    private void Awake()
    {

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

        //List<CharacterManager> managers = FindObjectsOfType<CharacterManager>().ToList();
        //managers.Remove(characterManager);

        ////This will be done on round start and use non spectator characters in actual version
        //_currentGamemode.SetActivePlayers(managers.ToArray());

        ////This will be delegated to the gamemode at some point
        //initialTagged = initialTagged ?? managers[0]._tracker;
        //currentTagged = initialTagged;

        //currentTagged.PlayerTagged();
    }

    private void Start()
    {
        StartCoroutine(Co_WaitUntilPlayers());
    }

    IEnumerator Co_WaitUntilPlayers()
    {
        if (!localMPIndexer)
        {
            Debug.LogError("Set An MP Screen Partitioner on this object", this);
        }

        while (localMPIndexer.playerIndex < 1)
        {
            yield return null;
        }

        List<CharacterManager> managers = FindObjectsOfType<CharacterManager>().ToList();

        //This will be done on round start and use non spectator characters in actual version
        _currentGamemode.SetActivePlayers(managers.ToArray());

        //This will be delegated to the gamemode at some point
        initialTagged = initialTagged ?? managers[0]._tracker;
        currentTagged = initialTagged;

        currentTagged.PlayerTagged();

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
    public void OnPlayerTagged(TaggedTracker Tagged, CharacterManager manager)
    {
        //Variable management
        previousTagged = currentTagged;
        if (previousTagged && !previousTagged.enabled)
        {
            //Old tagged now should track if they're hit
            previousTagged.enabled = true;
            previousTagged.PlayerUnTagged();
        }

        currentTagged = Tagged;

        currentTagged.PlayerTagged();
        _currentGamemode.PlayerTagged(manager);

        if (eventText)
        {
            eventText.AddTaggedText();
        }
    }
}
