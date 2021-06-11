////////////////////////////////////////////////////////////
// File: SabotageWinScreen.cs
// Author: Charles Carter
// Date Created: 10/05/21
// Brief: The script that controls the specific effects on the win screen for the sabotage gamemode
//////////////////////////////////////////////////////////// 

//This script uses these namespaces
using System.Collections.Generic;
using UnityEngine;

public class SabotageWinScreen : WinScreen {
    #region Variables Needed

    [Header("Variables Needed Specifically for the sabotage win screen")]

    [SerializeField]
    private SabotageGamemode gamemode;

    //The tagged player at the end sits opposite the alive players who won
    [SerializeField]
    private Transform taggedPosition;

    #endregion

    #region Unity Methods

    void OnEnable() {
        rManager = RoundManager.roundManager;

        if (rManager) {
            gamemode = rManager.GetComponent<SabotageGamemode>();
        } else {
            enabled = false;
        }

        if (!gamemode && Debug.isDebugBuild) {
            Debug.Log("This isnt the sabotage gamemode or something is set wrong in the win screen manager", this);
        }
    }

    #endregion

    #region Public Methods

    public override void StartWinSequence(List<CharacterManager> players, List<CharacterManager> winners) {
        base.StartWinSequence(players, winners);
    }

    #endregion

    #region Protected Methods

    protected override void PositionPlayers(List<CharacterManager> objectsToPosition) {
        //If there are things to place
        if (objectsToPosition.Count > 0) {
            //These are the survivors
            for (int i = 1; i < objectsToPosition.Count - 1; ++i) {
                if (winningSpots[i] && objectsToPosition[i]) {
                    PositionPerson(objectsToPosition[i], winningSpots[i]);
                } else if (Debug.isDebugBuild) {
                    Debug.Log("No spot here, index: " + i, this);
                }
            }

            if (gamemode) {
                //If the tagged player is part of the winners
                if (gamemode.TaggedWin()) {
                    //The tagged player is in the first slot
                    if (winningSpots[0]) {
                        PositionPerson(objectsToPosition[0], taggedPosition);
                    }
                }
            }
        }
    }

    protected override void PlayCorrectVFX(List<ParticleSystem> particlesToPlay) {
        base.PlayCorrectVFX(particlesToPlay);
    }

    #endregion
}
