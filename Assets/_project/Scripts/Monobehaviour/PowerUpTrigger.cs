////////////////////////////////////////////////////////////
// File: PowerUp.cs
// Author: Charles Carter
// Date Created: 5/05/21
// Brief: The base script for powerups to use
//////////////////////////////////////////////////////////// 

//The namespaces used
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpTrigger : MonoBehaviour
{
    #region Variables Needed

    //Need a way of identifying what this powerup does
    [SerializeField]
    private int PowerUp_ID;

    [SerializeField]
    private PowerUpSpawningManager spawningManager;

    #endregion

    #region Unity Methods

    //If something enters the trigger collider
    private void OnTriggerEnter(Collider other)
    {
        //If it's a player
        if (other.tag.Equals("Player"))
        {
            PlayerPowerUpHandler handler = other.GetComponent<PlayerPowerUpHandler>();
            if (handler)
            {
                //Tell their manager that they've triggered this powerup
                handler.PowerUpTriggered(PowerUp_ID);
            }

            //If there is an arena manager associated with this power up
            if (spawningManager)
            {
                //Tell it that this spot has cleared up
                spawningManager.PowerUpPickedUp(this);
            }

            gameObject.SetActive(false);
        }
    }

    #endregion

    #region Public Methods

    //Passing a reference to the arena manager to the power up
    public void SetSpawningManager(PowerUpSpawningManager aManager)
    {
        spawningManager = aManager;
    }

    public void SetPowerUpID(int newID)
    {
        PowerUp_ID = newID;
    }

    #endregion
}
