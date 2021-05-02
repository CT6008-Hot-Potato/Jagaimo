////////////////////////////////////////////////////////////
// File: SabotageObject
// Author: Charles Carter
// Date Created: 21/04/21
// Brief: The script for the behaviour for the players to fix objects
//////////////////////////////////////////////////////////// 

using System.Collections.Generic;
using UnityEngine;

public class SabotageObject : MonoBehaviour
{
    #region Variables Needed

    [Header("Main Components Needed")]

    [SerializeField]
    private RoundManager rManager;

    [SerializeField]
    private SabotageGamemode gamemode;

    [SerializeField]
    private SoundManager soundManager;

    [Header("Variables for fixing the object")]

    //The timer before this object is finished
    private Timer sabotageTimer;
    [SerializeField]
    private float duration = 10;
    [SerializeField]
    private bool isBeingUsed = false;

    [SerializeField]
    private List<CharacterManager> charsInteracting;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        soundManager = soundManager ?? FindObjectOfType<SoundManager>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        if (rManager)
        {
            if (rManager._currentGamemode != null)
            {
                gamemode = gamemode ?? rManager.GetComponent<SabotageGamemode>();
            }
        }

        //These timers are available during the whole scene
        sabotageTimer = new Timer(duration);
    }

    // Update is called once per frame
    private void Update()
    {
        if (isBeingUsed)
        {
            sabotageTimer.Tick(Time.deltaTime);

            //The timer is over
            if (!sabotageTimer.isActive)
            {
                if (gamemode)
                {
                    gamemode.SabotageObjectFinished();
                }
            }
        }
    }

    #endregion

    #region Public Methods

    public void StartUsage(CharacterManager charInteracting)
    {
        isBeingUsed = true;
        charsInteracting.Add(charInteracting);

        if (soundManager)
        {
            //Start fixing sound
        }
    }

    public void StopUsage()
    {
        isBeingUsed = false;

        if (soundManager)
        {
            //Stop fixing sound
        }
    }

    public void SetGamemodeObject(SabotageGamemode sabotageGamemode)
    {
        gamemode = sabotageGamemode;
    }

    #endregion
}
