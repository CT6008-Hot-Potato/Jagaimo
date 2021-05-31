////////////////////////////////////////////////////////////
// File: WinScreen.cs
// Author: Charles Carter
// Date Created: 10/05/21
// Brief: The script that controls what happens on this win screen (inherited from per gamemode)
//////////////////////////////////////////////////////////// 

using System.Collections.Generic;
using UnityEngine;

public abstract class WinScreen : MonoBehaviour {
    #region Variables Needed

    [Header("General Win Screen Variables")]
    [SerializeField]
    protected RoundManager rManager;

    //Positions to put the players into
    [SerializeField]
    protected Transform[] winningSpots;

    [SerializeField]
    protected Camera viewCam;

    [SerializeField]
    protected List<ParticleSystem> VFXtoPlay = new List<ParticleSystem>();

    public float screenDuration = 10f;
    protected List<Camera> camerasInScene = new List<Camera>();

    protected List<CharacterManager> winningPlayers = new List<CharacterManager>();

    #endregion

    #region Unity Methods

    private void OnEnable() {
        rManager = RoundManager.roundManager;
    }

    #endregion

    #region Public Methods

    //The movement of the camera/anything, VFX to get played or anything else needed, also can be overriden if needed
    public virtual void StartWinSequence(List<CharacterManager> players, List<CharacterManager> winners) {
        winningPlayers = winners;

        //Going through the managers
        foreach (CharacterManager manager in players) {
            //Getting the players' cameras
            Camera[] camsToAdd = manager.ReturnCameras();

            //Adding them to the list to turn for the screen
            for (int i = 0; i < camsToAdd.Length; ++i) {
                camerasInScene.Add(camsToAdd[i]);
            }

            //Disabling player so they cant move, interact etc
            manager.DisablePlayer();
        }

        //Position in the players, making sure it uses the right camera, play the vfx
        CombineCameras();
        PlayCorrectVFX(VFXtoPlay);

        PositionPlayers(players);
    }

    #endregion

    #region Private Methods
    protected virtual void PositionPlayers(List<CharacterManager> objectsToPosition) {
        //should be overridden
    }

    protected virtual void PlayCorrectVFX(List<ParticleSystem> particlesToPlay) {
        //If there are particles systems to play
        if (particlesToPlay.Count > 0) {
            //Go through and play them
            foreach (ParticleSystem particles in particlesToPlay) {
                particles.Play();
            }
        }
    }

    protected virtual void CombineCameras() {
        viewCam.enabled = true;

        //Going through all the cameras and making the 
        for (int i = 0; i < camerasInScene.Count; ++i) {
            camerasInScene[i].enabled = false;
        }
    }

    #endregion
}
