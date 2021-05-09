////////////////////////////////////////////////////////////
// File: GoalObject.cs
// Author: Charles Carter
// Date Created: 21/04/21
// Brief: The script for the behaviour when the potato collides with a goal collider
//////////////////////////////////////////////////////////// 

using UnityEngine;

public class GoalObject : MonoBehaviour, IInteractable
{
    #region Variables Needed

    [SerializeField]
    private RoundManager rManager;
    [SerializeField]
    private FootballGamemode gamemode;

    [SerializeField]
    private bool blueTeamGoal;
    private bool canScore;

    #endregion

    #region Unity Methods

    void Start()
    {
        rManager = rManager ?? RoundManager.roundManager;

        //It should always be this gamemode if this script is running
        if (rManager)
        {
            if (rManager._currentGamemode != null)
            {
                gamemode = rManager.GetComponent<FootballGamemode>();
                canScore = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Potato>(out var potato))
        {
            if (!canScore)
            {
                canScore = true;
            }
        }
    }

    #endregion

    #region Interface Methods

    //The potato hit the goal collider, meaning it crossed the goal line
    public void Interact()
    {
        //if there is a gamemode script
        if (gamemode)
        {
            //If the goal isnt "locked"
            if (canScore)
            {
                //Tell it a goal was scored in this goal (so the other team gets a point)
                gamemode.Goal(!blueTeamGoal);

                canScore = false;
            }
        }
        else
        {
            if (Debug.isDebugBuild)
            {
                Debug.Log("No football gamemode on the round manager", this);
            }
        }
    }

    #endregion
}
