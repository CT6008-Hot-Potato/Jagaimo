////////////////////////////////////////////////////////////
// File: ScoreboardText.cs
// Author: Charles Carter
// Date Created: 23/04/21
// Brief: The text that shows the current score in the football gamemode
//////////////////////////////////////////////////////////// 

using TMPro;
using UnityEngine;

public class ScoreboardText : MonoBehaviour
{
    #region Variables Needed

    //The score texts
    [SerializeField]
    private TextMeshProUGUI redScoreText;
    [SerializeField]
    private TextMeshProUGUI blueScoreText;

    //Instead of passing over the score, just having an internal reference doesnt take up too much space
    int BlueScore = 0;
    int RedScore = 0;

    #endregion

    #region Public Methods

    public void UpdateBlueScoreText()
    {
        BlueScore++;
        blueScoreText.text = BlueScore.ToString();
    }

    public void UpdateRedScoreText()
    {
        RedScore++;
        redScoreText.text = RedScore.ToString();
    }

    #endregion
}
