using System.Collections;
using System.Collections.Generic;
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

    public delegate void TaggedEvent(TaggedTracker NewTagged, TaggedTracker previouslyTagged);
    public static event TaggedEvent PlayerTagged;

    [SerializeField]
    TaggedTracker initialTagged;

    private void Start()
    {
        CallOnRoundStart();
        currentTagged = initialTagged;
    }

    //Calling the OnRoundStart Delegate Event
    public void CallOnRoundStart()
    {
        //Null checking the delegate event
        if (RoundStarted != null)
        {
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
    public void CallOnPlayerTagged(TaggedTracker Tagged)
    {
        //Null checking the delegate event
        if (PlayerTagged != null)
        {
            //Variable management
            previousTagged = currentTagged;
            if (previousTagged && !previousTagged.enabled)
            {
                //Old tagged now should track if they're hit
                previousTagged.enabled = true;
            }

            currentTagged = Tagged;

            //Sending out the event
            PlayerTagged.Invoke(Tagged, previousTagged);
        }
    }
}
