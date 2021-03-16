/////////////////////////////////////////////////////////////
//
//  Script Name: TaggedTracker.cs
//  Creator: Charles Carter
//  Description: A script for when an untagged player is hit by a potato
//  
/////////////////////////////////////////////////////////////

//This script uses these namespaces
using UnityEngine;

[RequireComponent(typeof(CharacterManager))]
public class TaggedTracker : MonoBehaviour, IInteractable
{
    //The main bool for the tracker
    public bool isTagged { get; private set; }

    [SerializeField]
    private RoundManager roundManager;
    private CharacterManager playerManager;

    private void Awake()
    {
        roundManager = roundManager ?? FindObjectOfType<RoundManager>();
        playerManager = GetComponent<CharacterManager>();
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

        //Telling the round manager that this was tagged and the relevant manager
        roundManager.OnPlayerTagged(this, playerManager);
        PlayerTagged();
    }

    //This player isnt tagged anymore
    public void PlayerUnTagged()
    {
        isTagged = false;
        GetComponent<PlayerCamera>().SetCameraView(false);
        if (Debug.isDebugBuild)
        {
            Debug.Log("This was untagged", this);
        }
    }

    //This player was just tagged
    public void PlayerTagged()
    {
        GetComponent<PlayerCamera>().SetCameraView(true);

        isTagged = true;
        enabled = false;
    }
}
