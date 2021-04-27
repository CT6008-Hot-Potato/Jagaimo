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
    [SerializeField]
    private RoundManager rManager;

    [SerializeField]
    private SabotageGamemode gamemode;

    //The timer before this object is finished
    private Timer sabotageTimer;
    [SerializeField]
    private float duration = 10;
    [SerializeField]
    private bool isBeingUsed = false;

    [SerializeField]
    private List<CharacterManager> charsInteracting;

    // Start is called before the first frame update
    void Start()
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
    void Update()
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

    public void StartUsage(CharacterManager charInteracting)
    {
        isBeingUsed = true;
        charsInteracting.Add(charInteracting);
    }

    public void StopUsage()
    {
        isBeingUsed = false;
    }

    public void SetGamemodeObject(SabotageGamemode sabotageGamemode)
    {
        gamemode = sabotageGamemode;
    }
}
