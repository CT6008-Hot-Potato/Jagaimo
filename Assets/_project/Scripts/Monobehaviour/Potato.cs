/////////////////////////////////////////////////////////////
//
//  Script Name: Potato.cs
//  Creator: Charles Carter
//  Description: The script for the potato
//  
/////////////////////////////////////////////////////////////

//This script uses these namespaces
using UnityEngine;

//The never changes itself, only triggers other things
public class Potato : MonoBehaviour
{
    #region Variables Needed

    private GameSettingsContainer gameSettings;

    [Header("Variables Needed")]

    [SerializeField]
    private Collider _coll;
    [SerializeField]
    private PhysicMaterial bouncyMaterial;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        gameSettings = GameSettingsContainer.instance;
    }

    private void Start()
    {
        //If there are game settings to take from
        if (gameSettings)
        {
            //If the mutator to make the potato bouncy is there then it's true
            if (gameSettings.HasGenMutator(5) && bouncyMaterial)
            {
                //Making the potato bouncy
                _coll.material = bouncyMaterial;
            }
        }
    }

    //The Potato activates a trigger
    private void OnTriggerEnter(Collider other)
    {
        //Guard clause for using the interactable interface
        if (!other.TryGetComponent(out IInteractable interactable)) return;

        //Run it's interact function if the script is enabled
        if (((MonoBehaviour)interactable).enabled)
        {
            interactable.Interact();
        }
    }

    #endregion
}
