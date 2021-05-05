////////////////////////////////////////////////////////////
// File: PowerUpSpawningManager.cs
// Author: Charles Carter
// Date Created: 5/05/21
// Brief: A manager to keep track of placing power ups at regular intervals, or stopping spawning them completely
//////////////////////////////////////////////////////////// 

//The namespaces used
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpSpawningManager : MonoBehaviour
{
    #region Variables Needed

    [Header("Necessary Variables")]

    public GameObject[] powerUpPrefabs;

    [SerializeField]
    private GameSettingsContainer settings;

    [SerializeField]
    private BasicTimerBehaviour waitTimer;

    //This will contain the transforms for the potential power spots, and utility for getting them
    [SerializeField]
    private ArenaManager spawningManager;

    [Header("Internal tracking variables")]

    //Will another power up get put down?
    [SerializeField]
    private bool spawnNextPowerUp = false;
    //When a power up wants to get put down but all the spots are filled
    [SerializeField]
    private bool waitingForPowerUpSpot = false;
    //The duration before the next power up is spawned
    [SerializeField]
    private float spawningDuration = 20;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        settings = GameSettingsContainer.instance;
        waitTimer = waitTimer ?? gameObject.AddComponent<BasicTimerBehaviour>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //If there are local settings
        if (settings)
        {
            //Get the power ups mutator
            if (settings.HasGenMutator(14))
            {
                //Since there's a value, it must be true
                spawnNextPowerUp = true;
                waitingForPowerUpSpot = false;
            }
            //It doesnt have a value
            else
            {
                //So this script is redundant
                enabled = false;
            }
        }
    }

    #endregion

    #region Public Methods

    //The basic timer behaviour has run out, so it's time to see if another spawn is possible
    public void WaitTimerRunOut()
    {
        if (spawningManager)
        {
            //If there's a spot
            if (spawningManager.canPowerUpSpawn())
            {
                //Finding the a spot and trying to spawn a random power up there
                if (SpawnPowerUp(spawningManager.ReturnFreePowerUpSpot()))
                {
                    //The timer resets within this function
                    waitTimer.CallOnTimerStart();
                }
            }
        }
    }

    #endregion

    #region Private Methods

    //The spawning function, which might need some overrides/overloads at some point
    private bool SpawnPowerUp(Vector3 positionToSpawn)
    {
        if (spawnNextPowerUp && powerUpPrefabs.Length > 0)
        {
            int iRandPowerUp = Random.Range(0, powerUpPrefabs.Length);

            Instantiate(powerUpPrefabs[iRandPowerUp], transform);

            return true;
        }

        return false;
    }

    #endregion
}
