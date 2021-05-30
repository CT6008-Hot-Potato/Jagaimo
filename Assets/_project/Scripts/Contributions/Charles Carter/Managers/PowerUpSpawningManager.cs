////////////////////////////////////////////////////////////
// File: PowerUpSpawningManager.cs
// Author: Charles Carter
// Date Created: 5/05/21
// Brief: A manager to keep track of placing power ups at regular intervals, or stopping spawning them completely
//////////////////////////////////////////////////////////// 

//The namespaces used
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpSpawningManager : MonoBehaviour
{
    #region Variables Needed

    [Header("Necessary Variables")]

    public GameObject[] powerUpPrefabs;

    [SerializeField]
    private RoundManager rManager;

    [SerializeField]
    private GameSettingsContainer settings;

    [SerializeField]
    private BasicTimerBehaviour waitTimer;

    [SerializeField]
    PowerUpTrigger[] powerUpObjects;

    [SerializeField]
    List<PowerUpTrigger> availableTriggers = new List<PowerUpTrigger>();

    [Header("Internal tracking variables")]

    //Will another power up get put down?
    [SerializeField]
    private bool spawnNextPowerUp = false;
    //When a power up wants to get put down but all the spots are filled
    [SerializeField]
    private bool waitingForPowerUpSpot = false;
    //The duration before the next power up is spawned
    [SerializeField]
    private float spawningDuration = 5;

    //Incase the scroller text wants to say that there's a power up taken/spawned
    [SerializeField]
    private ScrollerText scroller;

    //This is to use in the inspector to force the mutator to be on
    [SerializeField]
    private bool bSpawnPowerUps;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        settings = GameSettingsContainer.instance;
        waitTimer = waitTimer ?? gameObject.AddComponent<BasicTimerBehaviour>();

        //Every power up could potentially "spawn"
        for (int i = 0; i < powerUpObjects.Length; ++i)
        {
            availableTriggers.Add(powerUpObjects[i]);
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        rManager = RoundManager.roundManager;

        //If there are local settings
        if (settings)
        {
            //Get the power ups mutator
            if (settings.HasGenMutator(7))
            {
                //Since there's a value, it must be true
                spawnNextPowerUp = true;
                waitingForPowerUpSpot = false;

                waitTimer.SetDuration(spawningDuration);
            }
            //It doesnt have a value
            else
            {
                spawnNextPowerUp = false;
                //So this script is redundant (which stops the timer from starting when the countdown starts)
                enabled = false;
            }
        }
        else if (bSpawnPowerUps)
        {
            spawnNextPowerUp = true;
            waitingForPowerUpSpot = false;

            waitTimer.SetDuration(spawningDuration);
        }
    }

    private void OnEnable()
    {
        RoundManager.CountdownStarted += StartPowerUpTimer;
    }

    private void OnDisable()
    {
        RoundManager.CountdownStarted -= StartPowerUpTimer;
    }

    #endregion

    #region Public Methods

    //The basic timer behaviour has run out, so it's time to see if another spawn is possible
    public void WaitTimerRunOut()
    {
        //If there's a spot
        if (canSpawnPowerUp())
        {
            //Finding the a spot and trying to spawn a random power up there
            if (SpawnPowerUp(RandomFreePowerUp()))
            {
                scroller.AddPowerUpSpawnText();

                //The timer resets within this function
                waitTimer.CallOnTimerStart();
            }
        }
        else
        {
            //Start waiting for a spot to clear up
            waitingForPowerUpSpot = true;

            StartCoroutine(Co_waitForPowerUpSpot());
        }
    }

    #endregion

    #region Private Methods

    private void StartPowerUpTimer()
    {
        if (Debug.isDebugBuild)
        {
            Debug.Log("Waiting for first power up", this);
        }

        //The timer resets within this function
        waitTimer.CallOnTimerStart();
    }

    public void PowerUpPickedUp(PowerUpTrigger triggerActivated)
    {
        waitingForPowerUpSpot = false;

        availableTriggers.Remove(triggerActivated);
    }

    //The spawning function, which might need some overrides/overloads at some point
    private bool SpawnPowerUp(PowerUpTrigger PowerUpToSetActive)
    {
        if (spawnNextPowerUp && powerUpPrefabs.Length > 0)
        {
            int iRandPowerUp = UnityEngine.Random.Range(0, powerUpPrefabs.Length);

            //There is a free spot check before it gets to this part
            PowerUpToSetActive.SetPowerUpID(iRandPowerUp);
            PowerUpToSetActive.gameObject.SetActive(true);

            availableTriggers.Remove(PowerUpToSetActive);

            if (Debug.isDebugBuild)
            {
                Debug.Log("Power Up: " + iRandPowerUp + " spawned", this);
            }

            return true;
        }

        return false;
    }

    private IEnumerator Co_waitForPowerUpSpot()
    {
        //Keeping an eye on whether it's done
        while (waitingForPowerUpSpot)
        {
            yield return null;
        }

        //Start the timer again to wait for a power up
        StartPowerUpTimer();
    }

    //Make sure there is a free one before running this
    private PowerUpTrigger RandomFreePowerUp()
    {
        int iRandPowerUp = UnityEngine.Random.Range(0, availableTriggers.Count);

       if (availableTriggers.Count == 0)
        {
            //Should never reach here
            Debug.LogError("No free spot and forgot to check", this);
            enabled = false;
        }

        return availableTriggers[iRandPowerUp];
    }

    //Looping through to see if any of the power ups are off
    private bool canSpawnPowerUp()
    {
        for (int i = 0; i < powerUpObjects.Length; ++i)
        {
            if (!powerUpObjects[i].gameObject.activeSelf)
            {
                return true;
            }
        }

        return false;
    }


    #endregion
}
