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
public class Potato : MonoBehaviour {
    #region Variables Needed

    [SerializeField]
    private RoundManager roundManager;
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
    [SerializeField]
    private float fMagnetismStr;
    [SerializeField]
    private float fMagnetismDur;

    //A way to force it from the inspector
    [SerializeField]
    private bool isMagnetised = false;

    //The hit chance for the infected gamemode mutator
    private float hitChance = 1.0f;

    #endregion

    #region Unity Methods

    private void Awake() {
        gameSettings = GameSettingsContainer.instance;
    }

    private void Start() {
        roundManager = roundManager ?? RoundManager.roundManager;

        //If there are game settings to take from
        if (gameSettings) {
            //If the mutator to make the potato bouncy is there then it's true
            if (gameSettings.HasGenMutator(5) && bouncyMaterial) {
                //Making the potato bouncy
                _coll.material = bouncyMaterial;
            }

            //Magnetised mutator
            if (gameSettings.HasGenMutator(0) || isMagnetised) {
                magnetism.gameObject.SetActive(true);
                isMagnetised = true;

                if (magnetism) {
                    magnetism.enabled = true;
                    magnetism.SetMagnetismStr(fMagnetismStr);
                    magnetism.SetMagnetismDur(fMagnetismDur);
                }
            }

            if (roundManager._currentGamemode.Return_Mode() == GAMEMODE_INDEX.INFECTED) {
                if (gameSettings.HasGamMutator(3)) {
                    hitChance = (float)gameSettings.FindGamemodeMutatorValue(3);
                }
            }
        }

        //If there is a player set and a position to put it
        if (particlePlayer && fusePoint) {
            //Instantiating the correct prefab under the right transform
            Instantiate(particlePlayer.CreateParticle(ScriptableParticles.Particle.Fuse, Vector3.zero), fusePoint);
        }
    }

    //The Potato activates a trigger
    private void OnTriggerEnter(Collider other) {
        //Guard clause for using the interactable interface
        if (!other.TryGetComponent(out IInteractable interactable)) return;

        if (hitChance < 1.0f && other.CompareTag("Player")) {
            if (Random.Range(0, 1.0f) < hitChance) {
                HitRegister(interactable);
            }
        } else {
            HitRegister(interactable);
        }
    }

    private void HitRegister(IInteractable interactable) {
        //Run it's interact function if the script is enabled
        if (((MonoBehaviour)interactable).enabled) {
            interactable.Interact();
        }
    }

    #endregion
}
