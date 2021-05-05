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
    //Noting down where this power up spawned
    [SerializeField]
    private int SpawningSpotID;

    //Need a way of identifying what this powerup does
    [SerializeField]
    private int PowerUp_ID;

    [SerializeField]
    private ArenaManager spawningManager;

    //If something enters the trigger collider
    private void OnTriggerEnter(Collider other)
    {
        //If it's a player
        if (other.tag.Equals("Player"))
        {
            //Tell their manager that they've triggered this powerup
            //other.GetComponent<PlayerPowerUpManager>().TriggerPowerUp(PowerUp_ID);

            //If there is an arena manager associated with this power up
            if (spawningManager)
            {
                //Tell it that this spot has cleared up
                spawningManager.PowerUpPickedUp(SpawningSpotID);
            }

            gameObject.SetActive(false);
        }
    }

    //Passing a reference to the arena manager to the power up
    public void SetArenaManager(ArenaManager aManager)
    {
        spawningManager = aManager;
    }
}
