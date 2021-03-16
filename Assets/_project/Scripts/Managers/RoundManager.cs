/////////////////////////////////////////////////////////////
//
//  Script Name: RoundManager.cs
//  Creator: Charles Carter
//  Description: A manager script for the current round
//  
/////////////////////////////////////////////////////////////

//This script uses these namespaces
using System;
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

    //For multiplayer
    //public static event CountdownEvent CountdownPauseToggle;

    //The only trackers needed for mechanics in an overall round
    public TaggedTracker currentTagged  { get; private set; }
    public TaggedTracker previousTagged { get; private set; }

    //The person who starts as tagged each countdown
    [SerializeField]
    private TaggedTracker initialTagged;

    [SerializeField]
    BasicTimerBehaviour startCountdown;

    private void Awake()
    {
        //Adding the default gamemode if it doesnt have one
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
                    break;
                case GAMEMODE_INDEX.FOOTBALL:
                    break;
                case GAMEMODE_INDEX.SABOTAGE:
                    break;
                default:
                    break;
            }
        }
        //There are no settings to use
        else
        {
            //Must be a normal game if there's no script added
            _currentGamemode = _currentGamemode ?? gameObject.AddComponent<DefaultGamemode>();
        }

        //If there is a gamemode already on the object or is current gamemode is set but not on the object 
        if (!TryGetComponent<IGamemode>(out var gamemode))
        {
            _currentGamemode = gamemode;
            Type type = gamemode.GetType();
            gameObject.AddComponent(type);
        }

        //This will be done on round start and use non spectator characters in actual version
        _currentGamemode.SetActivePlayers(FindObjectsOfType<CharacterManager>());
        
        //This will be delegated to the gamemode at some point
        initialTagged = initialTagged ?? FindObjectOfType<TaggedTracker>();
        currentTagged = initialTagged;
        currentTagged.PlayerTagged();
    }

    private void Start()
    {
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
    public void OnPlayerTagged(TaggedTracker Tagged)
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
        _currentGamemode.PlayerTagged();
    }
}
