////////////////////////////////////////////////////////////
// File: WinScreen.cs
// Author: Charles Carter
// Date Created: 10/05/21
// Brief: The script that controls what happens on this win screen (inherited from per gamemode)
//////////////////////////////////////////////////////////// 

using System.Collections.Generic;
using UnityEngine;

public class WinScreen : MonoBehaviour
{
    #region Variables Needed

    //Positions to put the players into
    [SerializeField]
    private Transform[] winningSpots;

    [SerializeField]
    private Camera viewCam;

    [SerializeField]
    private List<ParticleSystem> VFXtoPlay = new List<ParticleSystem>();

    public float screenDuration { get; private set; } = 10f;
    private Camera[] camerasInScene;

    #endregion

    #region Unity Methods

    private void Start()
    {
        //Probably the only time I'll be using this...
        camerasInScene = FindObjectsOfType<Camera>();
    }

    #endregion

    #region Public Methods

    //The movement of the camera/anything, VFX to get played or anything else needed
    public void StartWinSequence(List<CharacterManager> objectsToPosition)
    {
        PositionPlayers(objectsToPosition);
        CombineCameras();

        //If there are particles systems to play
        if (VFXtoPlay.Count > 0)
        {
            //Go through and play them
            foreach (ParticleSystem particles in VFXtoPlay)
            {
                particles.Play();
            }
        }
    }

    #endregion

    #region Private Methods
    private void PositionPlayers(List<CharacterManager> objectsToPosition)
    {
        //if there's no winning spots, just return
        if (winningSpots.Length == 0)
        {
            if (Debug.isDebugBuild)
            {
                Debug.Log("Set winning end positions", this);
            }

            return;
        }

        //Putting the players who won in the winning spots
        for (int i = 0; i < objectsToPosition.Count; ++i)
        {
            if (winningSpots[i] != null)
            {
                objectsToPosition[i].transform.position = winningSpots[i].transform.position;
            }
            else
            {
                if (Debug.isDebugBuild)
                {
                    Debug.Log("This spot isnt set", this);
                }
            }
        }
    }

    private void CombineCameras()
    {
        //Going through all the cameras and making the 
        for (int i = 0; i < camerasInScene.Length; ++i)
        {
            //If it's not the camera needed turn it off, and if it is, turn it on
            if (camerasInScene[i] != viewCam)
            {
                camerasInScene[i].enabled = false;
            }
            else
            {
                viewCam.enabled = true;
            }
        }
    }

    #endregion
}
