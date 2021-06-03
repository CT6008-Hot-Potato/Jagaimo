﻿////////////////////////////////////////////////////////////
// File: InfectedWinScreen.cs
// Author: Charles Carter
// Date Created: 10/05/21
// Brief: The script that controls the specific effects on the win screen for the infected gamemode
//////////////////////////////////////////////////////////// 

//This script uses these namespaces
using System.Collections.Generic;
using UnityEngine;

//A different thing happens if the infected win vs the survivors win
public class InfectedWinScreen : WinScreen {
    #region Variables Needed

    [Header("Variables Needed Specifically for the infected win screen")]
    [SerializeField]
    private InfectedGamemode gamemode;
    //Only 3 positions needed atm (this script inherits the infected positions)
    [SerializeField]
    private Transform[] survivorPositions;

    private bool bInfectedWon;

    #endregion

    #region Unity Methods

    // Start is called before the first frame update
    void Start() {
        rManager = RoundManager.roundManager;

        if (rManager) {
            gamemode = rManager.GetComponent<InfectedGamemode>();
        }

        if (!gamemode && Debug.isDebugBuild) {
            Debug.Log("This isnt the infected gamemode or something is set wrong in the win screen manager", this);
        }
    }

    #endregion

    #region Public Methods

    public override void StartWinSequence(List<CharacterManager> players, List<CharacterManager> winners) {
        gamemode = rManager.GetComponent<InfectedGamemode>();

        if (gamemode) {
            bInfectedWon = gamemode.ReturnWinners();
        }

        base.StartWinSequence(players, winners);
    }

    #endregion

    #region Protected Methods

    protected override void PositionPlayers(List<CharacterManager> objectsToPosition) {
        if (bInfectedWon) {
            //Position all of these objects in the infected spots
            for (int i = 0; i < objectsToPosition.Count; ++i) {
                PositionPerson(objectsToPosition[i], winningSpots[i]);
            }
        } else {
            //Position the infected in the infected spots and the survivors in the survivor spots
            for (int i = 0; i < gamemode.activeInfected.Count; ++i) {
                PositionPerson(objectsToPosition[i], winningSpots[i]);
            }

            for (int i = 0; i < gamemode.activeSurvivors.Count; ++i) {
                PositionPerson(objectsToPosition[i], survivorPositions[i]);
            }
        }
    }
    protected override void PlayCorrectVFX(List<ParticleSystem> particlesToPlay) {
        base.PlayCorrectVFX(particlesToPlay);
    }

    #endregion
}
