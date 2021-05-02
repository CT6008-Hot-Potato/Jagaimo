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
    #region Variables Needed

    RoundManager roundManager;
    private SabotageGamemode gamemode;

    [SerializeField]
    private SoundManager soundManager;

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

    #endregion

    #region Unity Methods

    private void Awake()
    {
        roundManager = roundManager ?? RoundManager.roundManager;
        soundManager = soundManager ?? FindObjectOfType<SoundManager>();
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

    #endregion

    public void OpenEscapes()
    {
        ECParent.SetActive(true);

        //Play escape "siren" (?)
    }
}
