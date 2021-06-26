/////////////////////////////////////////////////////////////
//
//  Script Name: FPScounter.cs
//  Creator: Charles Carter
//  Description: The script for the frames per second counter in game
//  
/////////////////////////////////////////////////////////////

//This script uses these namespaces
using UnityEngine;
using TMPro;

//Our own FPS Display at some point
public class FPScounter : MonoBehaviour {
    #region Variables Needed

    [SerializeField]
    int iFPSLimit = 60;

    float current_fps;
    int highest_fps;
    int lowest_fps;

    [SerializeField]
    TextMeshProUGUI fpsText;
    [SerializeField]
    bool showFPS;

    #endregion

    #region Unity Methods

    void Start() {
        if (iFPSLimit == 0) {
            Application.targetFrameRate = 999;
        } else {
            Application.targetFrameRate = iFPSLimit;
        }
    }

    //Update is called once per frame
    void Update() {
        current_fps = 1 / Time.unscaledDeltaTime;

        if (fpsText && showFPS) {
            fpsText.text = "" + (int)current_fps;
        }
    }

    #endregion
}
