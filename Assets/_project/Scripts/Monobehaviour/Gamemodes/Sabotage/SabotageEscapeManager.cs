////////////////////////////////////////////////////////////
// File: SabotageEscapeManager.cs
// Author: Charles Carter
// Date Created: 27/04/21
// Brief: A script for the escape process in the sabotage gamemode
//////////////////////////////////////////////////////////// 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SabotageEscapeManager : MonoBehaviour
{
    RoundManager roundManager;
    private SabotageGamemode gamemode;

    //Things needed specifically for the sabotage gamemode
    //Generators on the map
    [SerializeField]
    private GameObject GParent;
    // "Escape points" on the map
    [SerializeField]
    private GameObject ECParent;
    //Barracades for the escape points on the map
    [SerializeField]
    private GameObject BParent;
    
    private void Awake()
    {
        roundManager = roundManager ?? RoundManager.roundManager;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (roundManager._currentGamemode.Return_Mode() == GAMEMODE_INDEX.SABOTAGE)
        {
            GParent.SetActive(true);
            gamemode = roundManager.GetComponent<SabotageGamemode>();
            gamemode.SetEscapeManager(this);
        }
    }

    public void OpenEscapes()
    {
        ECParent.SetActive(true);
    }
}
