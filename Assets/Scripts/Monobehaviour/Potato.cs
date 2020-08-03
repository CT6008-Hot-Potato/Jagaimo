using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potato : MonoBehaviour
{

    //The Potato actives a trigger
    private void OnTriggerEnter(Collider other)
    {
        //If the trigger does not have an interactable script
        if (!other.TryGetComponent<IInteractable>(out var interactable))
        {
            //Exit the function
            return;
        }

        //Run it's interact function if the script is enabled
        if (((MonoBehaviour)interactable).enabled)
        {
            interactable.Interact();
        }
    }
}
