////////////////////////////////////////////////////////////
// File: FootballObjectContainer.cs
// Author: Charles Carter
// Date Created: 08/05/21
// Brief: The script that holds relevant references for the football gamemode to then use
//////////////////////////////////////////////////////////// 

//This file uses these namespaces
using System.Collections.Generic;
using UnityEngine;

public class FootballObjectContainer : MonoBehaviour {
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
    public GameObject[] MapObjects;

    #endregion

    #region Variables for this script

    //Potential variables for capture the potato mutator
    [SerializeField]
    Transform[] flagpoles;
    List<Vector3> mapObjPositions = new List<Vector3>();

    #endregion

    private void Awake() {
        if (!footballObjectContainer) {
            footballObjectContainer = this;
        } else {
            Destroy(this);
        }
    }

    private void Start() {
        roundManager = RoundManager.roundManager;
        gamemode = roundManager.GetComponent<FootballGamemode>();

        foreach (GameObject objtransform in MapObjects) {
            mapObjPositions.Add(objtransform.transform.position);
        }
    }

    public void UnlockPlayersAfterGoal() {
        CharacterManager[] managers = gamemode.currentActivePlayers.ToArray();

        //This cuts out any forces applied to all the players
        for (int i = 0; i < managers.Length; ++i) {
            managers[i].GetComponent<Rigidbody>().isKinematic = false;
        }

        gamemode.UnlockAllPlayers();
    }

    public void PutObjectsBack() {
        for (int i = 0; i < MapObjects.Length;  ++i) {
            Rigidbody mapRb = MapObjects[i].GetComponent<Rigidbody>();
            mapRb.isKinematic = true;
            mapRb.velocity = Vector3.zero;
            mapRb.angularVelocity = Vector3.zero;

            MapObjects[i].transform.position = mapObjPositions[i];

            mapRb.isKinematic = false;
        }
    }
}
