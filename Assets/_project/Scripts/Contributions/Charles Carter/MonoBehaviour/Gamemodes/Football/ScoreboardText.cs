////////////////////////////////////////////////////////////
// File: ScoreboardText.cs
// Author: Charles Carter
// Date Created: 23/04/21
// Brief: The text that shows the current score in the football gamemode
//////////////////////////////////////////////////////////// 

//This script uses these namespaces
using TMPro;
using UnityEngine;

public class ScoreboardText : MonoBehaviour {
    #region Variables Needed

    [Header("Text variables")]
    //The score texts
    [SerializeField]
    private TextMeshProUGUI redScoreText;
    [SerializeField]
    private TextMeshProUGUI blueScoreText;

    //Instead of passing over the score, just having an internal reference doesnt take up too much space
    private int BlueScore = 0;
    private int RedScore = 0;

    #endregion

    #region Public Methods

    public void UpdateBlueScoreText() {
        BlueScore++;
        blueScoreText.text = BlueScore.ToString();
    }

    public void UpdateRedScoreText() {
        RedScore++;
        redScoreText.text = RedScore.ToString();
    }

    #endregion
}
