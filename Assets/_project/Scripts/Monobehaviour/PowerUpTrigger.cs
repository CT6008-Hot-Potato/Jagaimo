////////////////////////////////////////////////////////////
// File: PowerUp.cs
// Author: Charles Carter
// Date Created: 5/05/21
// Brief: The base script for powerups to use
//////////////////////////////////////////////////////////// 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpTrigger : MonoBehaviour
{
    //Need a way of identifying what this powerup does
    [SerializeField]
    private int PowerUp_ID;

    //If something enters the trigger collider
    private void OnTriggerEnter(Collider other)
    {
        //If it's a player
        if (other.tag.Equals("Player"))
        {
            //Tell their manager that they've triggered this powerup
            //other.GetComponent<PlayerPowerUpManager>().TriggerPowerUp(PowerUp_ID);

            gameObject.SetActive(false);
        }
    }
}
