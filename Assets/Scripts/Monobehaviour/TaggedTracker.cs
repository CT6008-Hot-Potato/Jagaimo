/////////////////////////////////////////////////////////////
//
//  Script Name: TaggedTracker.cs
//  Creator: Charles Carter
//  Description: A script for when an untagged player is hit by a potato
//  
/////////////////////////////////////////////////////////////

using UnityEngine;

public class TaggedTracker : MonoBehaviour, IInteractable
{
    //The main bool for the tracker
    [SerializeField]
    private bool isTagged;

    [SerializeField]
    private RoundManager roundManager;

    private void Awake()
    {
        roundManager = roundManager ?? FindObjectOfType<RoundManager>();
    }

    //Triggering the tagged function if interact is called on the object
    void IInteractable.Interact() => Hit();

    //Runs when the player is hit by the potato and this component is active
    private void Hit()
    {
        //This player is already tagged, shouldnt ever happen since this component should be off when tagged
        if (isTagged) return;
        
        Debug.Log("Non tagged player hit with potato", this);

        //Telling the round manager that this was tagged
        roundManager.OnPlayerTagged(this);

        //Set the bool and turn off the component so the potato doesnt trigger it
        //Should change camera etc
        isTagged = true;
        enabled = false;
    }

    //This player isnt tagged anymore
    public void PlayerUnTagged()
    {
        isTagged = false;
        Debug.Log("This was untagged", this);
        //Change camera etc on player
    }
}
