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
    [SerializeField]
    private ScriptableParticles particlePlayer;
    [SerializeField]
    private Transform fusePoint;

    [SerializeField]
    private PotatoMagnetism magnetism;
    private float fMagnetismStr = 0.5f;
    private bool isMagnetised = false;

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

            if (gameSettings.HasGenMutator(0) && magnetism)
            {
                isMagnetised = true;
                magnetism.enabled = true;
                magnetism.SetMagnetismStr(fMagnetismStr);
            }
        }

        //If there is a player set and a position to put it
        if (particlePlayer && fusePoint)
        {
            //Instantiating the correct prefab under the right transform
            Instantiate(particlePlayer.CreateParticle(ScriptableParticles.Particle.Fuse, Vector3.zero), fusePoint);
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
