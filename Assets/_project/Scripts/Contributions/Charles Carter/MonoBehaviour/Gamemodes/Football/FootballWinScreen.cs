////////////////////////////////////////////////////////////
// File: FootballWinScreen.cs
// Author: Charles Carter
// Date Created: 10/05/21
// Brief: The script that controls the specific effects on the win screen for the football gamemode
//////////////////////////////////////////////////////////// 

using System.Collections.Generic;
using UnityEngine;

//Each team will reference different spots to move to, a different rotation for the camera, and a different goal explosion
public class FootballWinScreen : WinScreen {
    #region Variables Needed

    [Header("Variables Needed Specifically for the football win screen")]

    [SerializeField]
    private FootballGamemode gamemode;

    //This one is for the orange team and the one in the parent's variables inherited is for the blue team
    [SerializeField]
    private Transform[] orangeTeamPos;

    [SerializeField]
    private List<ParticleSystem> VFXtoPlayAsOrangeTeam = new List<ParticleSystem>();

    // 0 - BlueTeam
    // 1 - OrangeTeam
    [SerializeField]
    private Transform[] cameraPoints;

    bool bBlueWin;

    #endregion

    #region Unity Methods

    // Start is called before the first frame update
    void Start() {
        gamemode = rManager.GetComponent<FootballGamemode>();

        if (!gamemode && Debug.isDebugBuild) {
            Debug.Log("This isnt the football gamemode or something is set wrong in the win screen manager", this);
        }
    }

    #endregion

    #region Public Methods

    public override void StartWinSequence(List<CharacterManager> players, List<CharacterManager> winners) {
        gamemode = rManager.GetComponent<FootballGamemode>();

        //Getting which team won here
        bBlueWin = gamemode.ReturnWinners();

        //Looks the same as the normal parent one but this gamemode only positions the winners
        winningPlayers = winners;

        CombineCameras();

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

            if (!winners.Contains(manager)) {
                Destroy(manager.gameObject);
            }
        }

        //Position in the players, making sure it uses the right camera, play the vfx
        PositionPlayers(winners);

        PlayCorrectVFX(VFXtoPlay);
    }


    #endregion

    #region Protected Methods

    protected override void PositionPlayers(List<CharacterManager> objectsToPosition) {
        if (bBlueWin) {
            //Position them infont of the blue team goal
            if (objectsToPosition.Count > 0) {
                for (int i = 0; i < objectsToPosition.Count; ++i) {
                    if (winningSpots[i] && objectsToPosition[i]) {
                        objectsToPosition[i].transform.position = winningSpots[i].position;
                        objectsToPosition[i].transform.rotation = winningSpots[i].rotation;
                    }
                }
            }
        } else {
            //Position them infront of the orange team goal

            if (objectsToPosition.Count > 0) {
                for (int i = 0; i < objectsToPosition.Count; ++i) {
                    if (orangeTeamPos[i] && objectsToPosition[i]) {
                        objectsToPosition[i].transform.position = orangeTeamPos[i].position;
                        objectsToPosition[i].transform.rotation = orangeTeamPos[i].rotation;
                    }
                }
            }
        }
    }

    protected override void CombineCameras() {
        //Running the parents' version
        base.CombineCameras();

        //Guard clause to make sure there's points to use and a camera
        if (cameraPoints.Length < 1 || !viewCam) return;

        //Move the camera based on the team who won
        if (bBlueWin) {
            //Position it looking at the blue team goal
            viewCam.transform.position = cameraPoints[0].position;
            viewCam.transform.rotation = cameraPoints[0].rotation;
        } else {
            //Position it looking at the orange team goal
            viewCam.transform.position = cameraPoints[1].position;
            viewCam.transform.rotation = cameraPoints[1].rotation;
        }
    }

    protected override void PlayCorrectVFX(List<ParticleSystem> vfxToPlay) {
        if (bBlueWin) {
            //Play blue goal explosion
            base.PlayCorrectVFX(VFXtoPlay);
        } else {
            //Play orange goal explosion
            base.PlayCorrectVFX(VFXtoPlayAsOrangeTeam);
        }
    }

    #endregion
}
