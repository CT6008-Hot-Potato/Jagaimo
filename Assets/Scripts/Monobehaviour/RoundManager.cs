using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    //Defining Delegate
    public delegate void RoundEvent();
    public static event RoundEvent RoundStarted;
    public static event RoundEvent RoundEnded;

    public TaggedTracker currentTagged  { get; private set; }
    public TaggedTracker previousTagged { get; private set; }

    public delegate void TaggedEvent(TaggedTracker NewTagged, TaggedTracker previouslyTagged);
    public static event TaggedEvent PlayerTagged;

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
            previousTagged = currentTagged;
            PlayerTagged.Invoke(Tagged, previousTagged);
            currentTagged = Tagged;
        }
    }
}
