/////////////////////////////////////////////////////////////
//
//  Script Name: RoundManager.cs
//  Creator: Charles Carter
//  Description: A manager script for the current round
//  
/////////////////////////////////////////////////////////////

using UnityEngine;

//A class to hold the events that happen throughout the round
public class RoundManager : MonoBehaviour
{
    //Defining Delegate
    public delegate void RoundEvent();
    public static event RoundEvent RoundStarted;
    public static event RoundEvent RoundEnded;
    public static event RoundEvent RoundPauseToggle;

    public TaggedTracker currentTagged  { get; private set; }
    public TaggedTracker previousTagged { get; private set; }

    [SerializeField]
    private TaggedTracker initialTagged;

    private void Start()
    {
        //CallOnRoundStart();
        currentTagged = initialTagged;
    }

    //Calling the OnRoundStart Delegate Event
    public void CallOnRoundStart()
    {
        //Null checking the delegate event
        if (RoundStarted != null)
        {
            Debug.Log("Round Started", this);
            RoundStarted.Invoke();
        }
    }

    //Calling the OnRoundEnd Delegate Event
    public void CallOnRoundEnd()
    {
        //Null checking the delegate event
        if (RoundEnded != null)
        {
            RoundEnded.Invoke();
        }
    }

    //Calling the OnPlayerTagged Delegate Event
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
