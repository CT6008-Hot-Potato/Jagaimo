using System;
using Mirror;
using UnityEngine;

public class JagaimoTaggedTracker : NetworkBehaviour, IInteractable
{
    #region Variables Needed

    //The main bool for the tracker
    public bool isTagged { get; private set; }

    [SerializeField]
    private JagaimoRoundManager jagaimoRoundManager;
    [SerializeField]
    private JagaimoCharacterManager playerManager;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        playerManager = playerManager ? playerManager : GetComponent<JagaimoCharacterManager>();
        isTagged = false;
    }

    private void Start()
    {
        jagaimoRoundManager = jagaimoRoundManager ? jagaimoRoundManager : JagaimoRoundManager.jagaimoRoundManager;
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
        jagaimoRoundManager.OnPlayerTagged(playerManager);
        PlayerTagged();
    }

    #endregion

    #region Public Methods

    //This player isnt tagged anymore
    public void PlayerUnTagged()
    {
        isTagged = false;
        playerManager.ThisPlayerUnTagged();

        enabled = true;
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