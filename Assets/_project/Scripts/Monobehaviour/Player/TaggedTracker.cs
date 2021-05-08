/////////////////////////////////////////////////////////////
//
//  Script Name: TaggedTracker.cs
//  Creator: Charles Carter
//  Description: A script for when an untagged player is hit by a potato
//  
/////////////////////////////////////////////////////////////

//This script uses these namespaces
using UnityEngine;

//The class needs these other components on the object (the CharacterManager script
// but dont add requires component because you cant remove them after)
public class TaggedTracker : MonoBehaviour, IInteractable
{
    #region Variables Needed

    //The main bool for the tracker
    public bool isTagged { get; private set; }

    [SerializeField]
    private RoundManager roundManager;
    [SerializeField]
    private CharacterManager playerManager;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        playerManager = playerManager ?? GetComponent<CharacterManager>();
        isTagged = false;
    }

    private void Start()
    {
        roundManager = roundManager ?? RoundManager.roundManager;
    }

    private void Update()
    {
        //Checking enabled/disabled in inspector
    }

    #endregion

    #region Interface Contract

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
        roundManager.OnPlayerTagged(playerManager);
        PlayerTagged();
    }

    #endregion

    #region Public Methods

    //This player isnt tagged anymore
    public void PlayerUnTagged()
    {
        Debug.Log(name + " Untagged");

        isTagged = false;
        playerManager.ThisPlayerUnTagged();

        if (Debug.isDebugBuild)
        {
            Debug.Log("This was untagged", this);
        }
    }

    //This player was just tagged
    public void PlayerTagged()
    {
        isTagged = true;
        playerManager.ThisPlayerTagged();

        enabled = false;
    }

    #endregion
}
