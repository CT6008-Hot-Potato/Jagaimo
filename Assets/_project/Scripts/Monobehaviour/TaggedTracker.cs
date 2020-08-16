/////////////////////////////////////////////////////////////
//
//  Script Name: TaggedTracker.cs
//  Creator: Charles Carter
//  Description: A script for when an untagged player is hit by a potato
//  
/////////////////////////////////////////////////////////////

//This script uses these namespaces
using UnityEngine;

public class TaggedTracker : MonoBehaviour, IInteractable
{
    //The main bool for the tracker
    public bool isTagged { get; private set; }

    [SerializeField]
    private RoundManager roundManager;
    private PlayerManager playerManager;

    private void Awake()
    {
        roundManager = roundManager ?? FindObjectOfType<RoundManager>();
        playerManager = GetComponent<PlayerManager>();
    }

    //Triggering the tagged function if interact is called on the object
    void IInteractable.Interact() => Hit();

    //Runs when the player is hit by the potato and this component is active
    private void Hit()
    {
        //This player is already tagged, shouldnt ever happen since this component should be off when tagged
        if (isTagged) return;

        if (Debug.isDebugBuild)
        {
            Debug.Log("Non tagged player hit with potato", this);
        }

        //Telling the round manager that this was tagged
        roundManager.OnPlayerTagged(this);

        //Set the bool and turn off the component so the potato doesnt trigger it
        //Should change camera etc
        PlayerTagged();
    }

    //This player isnt tagged anymore
    public void PlayerUnTagged()
    {
        isTagged = false;

        if (Debug.isDebugBuild)
        {
            Debug.Log("This was untagged", this);
        }
    }

    //This player was just tagged
    public void PlayerTagged()
    {
        isTagged = true;
        enabled = false;
    }
}
