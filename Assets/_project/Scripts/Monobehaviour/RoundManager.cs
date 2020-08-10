/////////////////////////////////////////////////////////////
//
//  Script Name: RoundManager.cs
//  Creator: Charles Carter
//  Description: A manager script for the current round
//  
/////////////////////////////////////////////////////////////

//This script uses these namespaces
using UnityEngine;

//A class to hold the events that happen throughout the round, a round is a full game where everyone is alive to the last player left
public class RoundManager : MonoBehaviour
{
    //Defining Delegate
    public delegate void RoundEvent();
    public static event RoundEvent RoundStarted;
    public static event RoundEvent RoundEnded;

    //A Countdown is the timer before the next player dying
    public delegate void CountdownEvent();
    public static event CountdownEvent CountdownStarted;
    public static event CountdownEvent CountdownEnded;

    //For multiplayer
    public static event CountdownEvent CountdownPauseToggle;

    //The only trackers needed for mechanics in an overall round
    public TaggedTracker currentTagged  { get; private set; }
    public TaggedTracker previousTagged { get; private set; }

    //The person who starts as tagged each countdown
    [SerializeField]
    private TaggedTracker initialTagged;

    private void Awake()
    {
        initialTagged = initialTagged ?? FindObjectOfType<TaggedTracker>();
        currentTagged = initialTagged;
    }

    //Calling the RoundStarted Delegate Event
    public void CallOnRoundStart()
    {
        //Null checking the delegate event
        if (RoundStarted != null)
        {
            Debug.Log("Round Started", this);
            RoundStarted.Invoke();
        }
    }

    //Calling the RoundEnded Delegate Event
    public void CallOnRoundEnd()
    {
        //Null checking the delegate event
        if (RoundEnded != null)
        {
            RoundEnded.Invoke();
        }
    }

    //Calling the CountdownStarted Delegate Event
    public void CallOnCountdownStart()
    {
        //Null checking the delegate event
        if (CountdownStarted != null)
        {
            CountdownStarted.Invoke();
        }
    }

    //Calling the CountdownEnded Delegate Event
    public void CallOnCountdownEnd()
    {
        //Null checking the delegate event
        if (CountdownEnded != null)
        {
            CountdownEnded.Invoke();
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
    }
}
