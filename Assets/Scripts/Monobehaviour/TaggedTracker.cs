using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaggedTracker : MonoBehaviour, IInteractable
{
    //The main bool for the tracker
    [SerializeField]
    private bool isTagged;

    [SerializeField]
    private RoundManager roundManager;

    //Runs when Object is enabled
    private void OnEnable()
    {
        RoundManager.RoundEnded += ExplodedCheck;
        RoundManager.PlayerTagged += PlayerTaggedCheck;
    }

    //Runs when Object is disabled
    private void OnDisable()
    {
        RoundManager.RoundEnded -= ExplodedCheck;
        RoundManager.PlayerTagged -= PlayerTaggedCheck;
    }

    //Triggering the tagged function if interact is called on the object
    void IInteractable.Interact() => Hit();

    //Runs when the player is hit by the potato and this component is active
    void Hit()
    {
        //This player is already tagged
        if (isTagged)
        {
            return;
        }

        Debug.Log("Non tagged player hit with potato");

        if (roundManager.previousTagged)
        {
            //Turning on the previously tagged player's tracker
            roundManager.previousTagged.enabled = true;
        }

        //Call the on playertagged delegate event
        roundManager.CallOnPlayerTagged(this);
    }

    //Setting the variable
    void PlayerTaggedCheck(TaggedTracker taggedPlayer, TaggedTracker untaggedPlayer)
    {
        //If this instance of the script is the tagged player
        if (this.Equals(taggedPlayer))
        {
            //Set the bool and turn off the component so the potato doesnt trigger it (?)
            //Change camera etc on player
            isTagged = true;
            Debug.Log("Tagged Me: " + isTagged);
            enabled = false;
            return;
        }
        //If it's the player who was tagged last
        else if (this.Equals(untaggedPlayer))
        {
            enabled = true;
            isTagged = false;
            //Change camera etc on player
        }
    }

    //Just a test function for now
    void ExplodedCheck()
    {
        //This player isnt tagged
        if (!isTagged)
        {
            return;
        }

        //Player should explode
    }
}
