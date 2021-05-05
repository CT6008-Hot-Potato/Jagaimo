using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpSpawningManager : MonoBehaviour
{
    #region Variables Needed

    [SerializeField]
    private GameSettingsContainer settings;

    //This will contain the transforms for the potential power spots, and utility for getting them
    [SerializeField]
    private ArenaManager spawningManager;

    //Will another power up get put down?
    [SerializeField]
    private bool spawnNextPowerUp = true;
    //When a power up wants to get put down but all the spots are filled
    [SerializeField]
    private bool waitingForPowerUpSpot = false;
    //The duration before the next power up is spawned
    private float spawningDuration = 20;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #endregion
}
