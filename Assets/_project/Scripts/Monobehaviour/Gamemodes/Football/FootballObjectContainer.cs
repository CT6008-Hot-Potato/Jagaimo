////////////////////////////////////////////////////////////
// File: FootballObjectContainer.cs
// Author: Charles Carter
// Date Created: 08/05/21
// Brief: The script that holds relevant references for the football gamemode to then use
//////////////////////////////////////////////////////////// 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootballObjectContainer : MonoBehaviour
{
    #region Variables for the gamemode script

    //There should only be 1 on scenes
    public static FootballObjectContainer footballObjectContainer;

    private RoundManager roundManager;
    private FootballGamemode gamemode;

    [Header("Variables needed for Gamemode")]

    public CountdownTimer countdownTimer;
    public ScrollerText scrollerText;
    public ScoreboardText scoreboard;
    public Rigidbody potatoRB;
    public BasicTimerBehaviour goalPauseTimer;
    public ScriptableParticles particleSpawner;
    public Transform[] vfxPoints;

    #endregion

    #region Variables for this script

    //Potential variables for capture the potato mutator
    [SerializeField]
    Transform[] flagpoles;
    [SerializeField]
    Transform[] goalObjects;

    #endregion

    private void Awake()
    {
        if (!footballObjectContainer)
        {
            footballObjectContainer = this;
        }
        else
        {
            Destroy(this);
        }

        roundManager = RoundManager.roundManager;
    }
}
