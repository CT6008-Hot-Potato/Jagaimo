/////////////////////////////////////////////////////////////
//
//  Script Name: Potato.cs
//  Creator: Charles Carter
//  Description: The script for the potato
//  
/////////////////////////////////////////////////////////////

//This script uses these namespaces
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The never changes itself, only triggers other things
public class Potato : MonoBehaviour
{
    //The Potato activates a trigger
    private void OnTriggerEnter(Collider other)
    {
        //Guard clause for using the interactable interface
        if (!other.TryGetComponent(out IInteractable interactable)) return;

        //Run it's interact function if the script is enabled
        if (((MonoBehaviour)interactable).enabled)
        {
            other.gameObject.GetComponent<PlayerAnimation>().CheckToChangeState("FallingBackDeath", true);
            interactable.Interact();
        }
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.gameObject.tag == "Player")
    //    {
    //        collision.gameObject.SetActive(false);
    //    }
    //}
}
