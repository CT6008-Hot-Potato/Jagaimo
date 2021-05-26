////////////////////////////////////////////////////////////
// File: SabotageWinScreen.cs
// Author: Charles Carter
// Date Created: 10/05/21
// Brief: The script that controls the specific effects on the win screen for the sabotage gamemode
//////////////////////////////////////////////////////////// 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SabotageWinScreen : WinScreen
{
    #region Variables Needed

    [Header("Variables Needed Specifically for the sabotage win screen")]

    [SerializeField]
    private SabotageGamemode gamemode;

    //The tagged player at the end sits opposite the alive players who won
    [SerializeField]
    private Transform taggedPosition;

    #endregion

    #region Unity Methods

    // Start is called before the first frame update
    void Start()
    {
        gamemode = rManager.GetComponent<SabotageGamemode>();

        if (!gamemode && Debug.isDebugBuild)
        {
            Debug.Log("This isnt the sabotage gamemode or something is set wrong in the win screen manager", this);
        }
    }

    #endregion

    #region Public Methods

    public override void StartWinSequence(List<CharacterManager> players, List<CharacterManager> winners)
    {
        base.StartWinSequence(players, winners);
    }

    #endregion

    #region Protected Methods

    protected override void PositionPlayers(List<CharacterManager> objectsToPosition)
    {
        //If there are things to place
        if (objectsToPosition.Count > 0)
        {
            //These are the survivors
            for (int i = 0; i < objectsToPosition.Count - 1; ++i)
            {
                if (winningSpots[i])
                {
                    objectsToPosition[i].transform.position = winningSpots[i].transform.position;
                    objectsToPosition[i].transform.rotation = winningSpots[i].transform.rotation;
                }
                else if (Debug.isDebugBuild)
                {
                    Debug.Log("No spot here, index: " + i, this);
                }
            }

            //If the tagged player is part of the winners
            if (gamemode.TaggedWin())
            {
                //The tagged player is in the last slot
                if (winningSpots[objectsToPosition.Count - 1])
                {
                    objectsToPosition[objectsToPosition.Count - 1].transform.position = winningSpots[objectsToPosition.Count - 1].transform.position;
                    objectsToPosition[objectsToPosition.Count - 1].transform.rotation = winningSpots[objectsToPosition.Count - 1].transform.rotation;
                }
            }
        }
    }

    protected override void PlayCorrectVFX(List<ParticleSystem> particlesToPlay)
    {
        base.PlayCorrectVFX(particlesToPlay);
    }

    #endregion
}
