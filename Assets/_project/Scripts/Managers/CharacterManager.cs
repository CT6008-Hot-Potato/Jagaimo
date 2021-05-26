/////////////////////////////////////////////////////////////
//
//  Script Name: CharacterManager.cs
//  Creator: Charles Carter
//  Description: A manager script for the characters in the scene player or otherwise
//  
/////////////////////////////////////////////////////////////

//This script uses these namespaces
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TaggedTracker))]
public class CharacterManager : MonoBehaviour
{
    #region Variables Needed

    [Header("Public Variables")]
    //The tracker attached to the object
    public TaggedTracker _tracker;

    //A potentially useful bool for AI production
    public bool isPlayer { get; private set; }

    //A public variable for scripts to check if this player is locked
    public bool isPlayerLocked { private get; set; } = false;

    [Header("Components Needed")]
    //Components already on this object
    [SerializeField]
    private PlayerController _movement;
    [SerializeField]
    private PlayerCamera _cam;
    [SerializeField]
    private PlayerAnimation _playerAnimation;
    [SerializeField]
    private PlayerInteraction _playerInteraction;
    [SerializeField]
    private Rigidbody _rb;

    //Other components needed
    [SerializeField]
    private RoundManager rManager;
    [SerializeField]
    private SoundManager soundManager;
    [SerializeField]
    private GameSettingsContainer settings;

    // 0 - First Person
    // 1 - Third Person
    [SerializeField]
    private Camera[] playerCameras;

    [Header("Customization Variables")]
    //[SerializeField]
    //private bool bUsingConfettiVFX = false;
    [SerializeField]
    private ScriptableParticles particlePlayer;
    [SerializeField]
    private ScriptableParticles.Particle elimVFX = ScriptableParticles.Particle.BloodBurst;
    [SerializeField]
    private ScriptableParticles.Particle confettiElimVFX = ScriptableParticles.Particle.ConfettiBurst;
    //Where the particles are played from when the player is eliminated
    [SerializeField]
    private Transform headTransform;
    [SerializeField]
    private GameObject taggedDisplayObject;
    [SerializeField]
    private GameObject elimDisplayObject;
    private float taggedAnimduration = 2f;

    //Infected mutator variables
    private bool bApplyInfectedSpeed = false;
    private bool bRemoveSurvivorSpeed = false;
    private float survivorSpeedAddition = 0.0f;
    private float infectedSpeedAddition = 0.0f;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        _tracker = _tracker ?? GetComponent<TaggedTracker>();
        _movement = _movement ?? GetComponent<PlayerController>();
        _cam = _cam ?? GetComponent<PlayerCamera>();
        _playerAnimation = _playerAnimation ?? GetComponent<PlayerAnimation>();
        _playerInteraction = _playerInteraction ?? GetComponent<PlayerInteraction>();
        _rb = _rb ?? GetComponent<Rigidbody>();

        soundManager = FindObjectOfType<SoundManager>();
        settings = GameSettingsContainer.instance;
    }

    private void Start()
    {
        rManager = rManager ?? RoundManager.roundManager;

        //This can be done better
        if (_movement)
        {
            isPlayer = true;
        }
        else
        {
            isPlayer = false;
        }

        if (settings)
        {
            float valueToAdd = 0.05f;

            if (settings.HasGenMutator(8))
            {

                int multiplier = (int)settings.FindGeneralMutatorValue(8);

                valueToAdd *= multiplier;
                _movement.speedMultiplier += valueToAdd;

                //It's the infected gamemode
                if (rManager._currentGamemode.Return_Mode() == GAMEMODE_INDEX.INFECTED)
                {
                    //The infected speed's mutator
                    if (settings.HasGamMutator(0))
                    {
                        int inf_multiplier = (int)settings.FindGamemodeMutatorValue(0);
                        infectedSpeedAddition = valueToAdd * inf_multiplier;

                        bApplyInfectedSpeed = true;
                    }

                    //The survivor's speed's mutator
                    if (settings.HasGamMutator(1))
                    {
                        int surv_multiplier = (int)settings.FindGamemodeMutatorValue(1);
                        survivorSpeedAddition = valueToAdd * surv_multiplier;

                        _movement.speedMultiplier += survivorSpeedAddition;

                        bRemoveSurvivorSpeed = true;
                    }
                }
            }

            //The mutator for using confetti is true, so swap out the elim vfx for the confetti one
            if (settings.HasGenMutator(14))
            {
                //bUsingConfettiVFX = true;
                //Not sure if this works tbh (test this)
                elimVFX = confettiElimVFX;
            }
        }
    }

    #endregion

    #region Public Methods

    //Some Gamemodes will have elimination, some wont
    public CharacterManager CheckIfEliminated(int playersLeft)
    {
        //If they arent tagged then do nothing
        if (!_tracker.isTagged) return null;

        //The win screen is about to happen, dont change the camera or play the VFX etc
        if (playersLeft <= 2)
        {
            return this;
        }

        //Send the player into "spectator" mode (No model, no colliders)
        if (_cam)
        {
            _cam.cameraState = PlayerCamera.cS.FREECAMUNCONSTRAINED;
        }

        //Play Sound
        if (soundManager)
        {
            //soundManager.PlaySound(ScriptableSounds.Sounds.Explosion);
        }

        //Making sure the elimination icon is showing
        if (taggedDisplayObject)
        {
            taggedDisplayObject.SetActive(false);
        }
        if (elimDisplayObject)
        {
            elimDisplayObject.SetActive(true);
        }

        //Play vfx
        if (particlePlayer)
        {
            //Play it on the head spot
            if (headTransform)
            {
                Instantiate(particlePlayer.CreateParticle(elimVFX, Vector3.zero), headTransform);
            }
            else
            {
                //Play it from the feet?
                Instantiate(particlePlayer.CreateParticle(elimVFX, Vector3.zero), transform);

                if (Debug.isDebugBuild)
                {
                    Debug.Log("No head transform given", this);
                }
            }
        }

        //Turn all non-important scripts off (ones that allow the player to interact especially)
        return this;
    }

    //Functions to change the player when they're tagged or untagged
    public void ThisPlayerTagged()
    {
        //Animation for regaining potato
        if (_playerAnimation)
        {
            _playerAnimation.CheckToChangeState("FallingBackDeath", true);
        }

        //This is only applied during the infected gamemode
        if (bRemoveSurvivorSpeed)
        {
            _movement.speedMultiplier -= survivorSpeedAddition;
        }

        if (bApplyInfectedSpeed)
        {
            _movement.speedMultiplier += infectedSpeedAddition;
        }

        StartCoroutine(Co_TaggedEffect(taggedAnimduration));
    }

    public void ThisPlayerUnTagged()
    {
        //Play VFX + Sound
        if (soundManager)
        {
            //Maybe an untagged sound, maybe it would become too chaotic
        }

        //Lerp into third person camera mode Note: this should be quicker than the lerp when you're tagged
        if (_cam)
        {
            _cam.SetCameraView(false);
        }

        if (taggedDisplayObject)
        {
            taggedDisplayObject.SetActive(false);
        }

        //Animations switch back to being idle
        if (_playerAnimation)
        {
            _playerAnimation.CheckToChangeState("Idle", false); ;
        }
    }

    /// <summary>
    /// BE CAREFUL WHEN USING THESE
    /// </summary>

    //Function to have the player locked in a position (so they cant move or rotate the camera)
    public void LockPlayer()
    {
        //Guard clause to make sure the components are correct
        if (!_cam || !_movement) return;

        if (Debug.isDebugBuild)
        {
            Debug.Log("Player locked!", this);
        }

        isPlayerLocked = true;

        //Stop movement in the movement script, dont disable or deactive player input (they couldn't pause then)
        _movement.SetMovement(3);

        //Stop camera player camera movement
        _cam.cameraRotationLock = true;

        //Note: option to have them switch to a different camera for cinematics
    }

    public void UnLockPlayer()
    {
        //Guard clause to make sure the components are correct
        if (!_cam || !_movement) return;

        if (Debug.isDebugBuild)
        {
            Debug.Log("Player unlocked!", this);
        }

        isPlayerLocked = false;

        //Start player movement
        _movement.SetMovement(2);

        //Restart camera movement
        _cam.cameraRotationLock = false;
    }

    /// <summary>
    /// BE CAREFUL WHEN USING THESE
    /// </summary>
    /// 
    public void DisablePlayer()
    {
        StopCoroutine(Co_TaggedEffect(taggedAnimduration));

        _playerAnimation.CheckToChangeState("Idle");

        _cam.enabled = false;
        _movement.enabled = false;
        _tracker.enabled = false;
        _playerInteraction.enabled = false;

        taggedDisplayObject.SetActive(false);
    }

    public void EnablePlayer()
    {
        _cam.enabled = true;
        _movement.enabled = true;
        _tracker.enabled = true;
        _playerInteraction.enabled = true;
    }

    public Camera[] ReturnCameras()
    {
        return playerCameras;
    }

    #endregion

    #region Private Methods

    private IEnumerator Co_TaggedEffect(float animDuration)
    {
        LockPlayer();

        yield return new WaitForSeconds(animDuration);

        UnLockPlayer();

        //Play VFX + Sound
        //Lerp into first person camera mode
        if (_cam)
        {
            _cam.SetCameraView(true);
        }

        if (taggedDisplayObject)
        {
            taggedDisplayObject.SetActive(true);
        }

        if (soundManager)
        {
            //Play the tagged sound
            //sm.PlaySound();
        }
    }

    #endregion
}
