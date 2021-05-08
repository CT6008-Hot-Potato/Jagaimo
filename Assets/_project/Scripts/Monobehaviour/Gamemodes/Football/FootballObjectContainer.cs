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
    //There should only be 1 on scenes
    public static FootballObjectContainer footballObjectContainer;

    private RoundManager roundManager;
    private FootballGamemode gamemode;

    [Header("Variables needed from Gamemode")]
    [SerializeField]
    public CountdownTimer countdownTimer;

    [SerializeField]
    public ScrollerText scrollerText;
    [SerializeField]
    public ScoreboardText scoreboard;

    [SerializeField]
    public Rigidbody potatoRB;
    [SerializeField]
    public BasicTimerBehaviour goalPauseTimer;

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
