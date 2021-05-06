////////////////////////////////////////////////////////////
// File: SabotageEscapeManager.cs
// Author: Charles Carter
// Date Created: 27/04/21
// Brief: A script for the escape process in the sabotage gamemode
//////////////////////////////////////////////////////////// 

//Namespaces to use
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SabotageEscapeManager : MonoBehaviour
{
    #region Variables Needed

    [SerializeField]
    private RoundManager roundManager;

    public SabotageGamemode gamemode { get; private set; }

    [SerializeField]
    private SoundManager soundManager;

    [SerializeField]
    private GameObject SaboatageObjectParent;

    //Things needed specifically for the sabotage gamemode
    //Generators on the map
    [SerializeField]
    private GameObject GParent;
    [SerializeField]
    private List<SabotageObject> Generators = new List<SabotageObject>();

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
            //Explicitly declaring which objects should be on/off at the start
            SaboatageObjectParent.SetActive(true);
            GParent.SetActive(true);
            ECParent.SetActive(false);
            BParent.SetActive(true);

            //Setting up the references between this script and the gamemode
            gamemode = roundManager.GetComponent<SabotageGamemode>();
            gamemode.SetEscapeManager(this);

            //Passing the reference onto the generator
            foreach (SabotageObject sabotageObject in Generators)
            {
                sabotageObject.SetGamemode(gamemode);
            }
        }
        else
        {
            //None of the sabotage objects need to be on (they should already be off but just in case)
            SaboatageObjectParent.SetActive(false);
            enabled = false;
        }
    }

    #endregion

    //Doing it here incase of any mutators
    public void GeneratorFinished(SabotageObject genFinished)
    {
        genFinished.enabled = false;
        gamemode.SabotageObjectFinished();
    }

    public void OpenEscapes()
    {
        ECParent.SetActive(true);

        //Play escape "siren" (?)
    }
}
