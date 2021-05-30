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
using UnityEngine.InputSystem;

[RequireComponent(typeof(TaggedTracker))]
public class CharacterManager : MonoBehaviour
{
    #region Variables Needed

    [Header("Public Variables")]
    //The tracker attached to the object
    public TaggedTracker _tracker;

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
    [SerializeField]
    private PlayerInput _input;
    [SerializeField]
    private PlayerPowerUpHandler _powerUpHandler;

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

    [SerializeField]
    private ScriptableSounds.Sounds Bloodboom, Confettiboom;

    //Where the particles are played from when the player is eliminated
    [SerializeField]
    private Transform headTransform;
    [SerializeField]
    private ParticleSystem taggedDisplayObject;
    [SerializeField]
    private ParticleSystem elimDisplayObject;
    private float taggedAnimduration = 2f;

    private bool bEliminated = false;

    //Infected mutator variables
    private bool bApplyInfectedSpeed = false;
    private bool bRemoveSurvivorSpeed = false;
    private float survivorSpeedAddition = 0.0f;
    private float infectedSpeedAddition = 0.0f;

    //Player reticle variables
    [SerializeField]
    private GameObject reticle;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        //A big chunck of scripts that manage the player
        _tracker = _tracker ?? GetComponent<TaggedTracker>();
        _movement = _movement ?? GetComponent<PlayerController>();
        _cam = _cam ?? GetComponent<PlayerCamera>();
        _playerAnimation = _playerAnimation ?? GetComponent<PlayerAnimation>();
        _playerInteraction = _playerInteraction ?? GetComponent<PlayerInteraction>();
        _rb = _rb ?? GetComponent<Rigidbody>();
        _input = _input ?? GetComponent<PlayerInput>();
        _powerUpHandler = _powerUpHandler ?? GetComponent<PlayerPowerUpHandler>();

        soundManager = FindObjectOfType<SoundManager>();
        settings = GameSettingsContainer.instance;

        //A bit messy and could be changed
        taggedDisplayObject = taggedDisplayObject ?? transform.GetChild(3).GetComponent<ParticleSystem>();
        elimDisplayObject = elimDisplayObject ?? transform.GetChild(4).GetComponent<ParticleSystem>();
    }

    private void Start()
    {
        rManager = rManager ?? RoundManager.roundManager;

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

                Bloodboom = Confettiboom;
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
            //Shouldn't untag in the win screen or generally when eliminated
            StopCoroutine(Co_TaggedEffect(taggedAnimduration));

            return this;
        }

        EliminationEffect();

        bEliminated = true;

        //Turn all non-important scripts off (ones that allow the player to interact especially)
        return this;
    }

    public void ForceElimination()
    {
        //Make sure that it doesnt trigger multiple times
        if (bEliminated) return;

        IGamemode gamemode = rManager._currentGamemode;

        //The infected mode doesnt need to do the effects etc
        if (gamemode.GetActivePlayers().Length > 2 && gamemode.Return_Mode() != GAMEMODE_INDEX.INFECTED)
        {
            //Make sure stuff can move
            UnLockPlayer();

            //Play all the sounds and effects etc
            EliminationEffect();

            if (_playerAnimation)
            {
                _playerAnimation.CheckToChangeState("FallingBackDeath", true);
                _playerAnimation.timer.isLocked = true;
                _playerAnimation.enabled = false;
            }

            bEliminated = true;
        }

        //Telling the gamemode that this isnt an active player anymore
        rManager._currentGamemode.ForceEliminatePlayer(this);
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

        //change reticle
        if(reticle.TryGetComponent(out ReticleSwitcher i))
        {
            i.ChangeReticle(1);
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
        if (_cam  && taggedDisplayObject && _playerAnimation)
        {
            _cam.SetCameraView(false);
            taggedDisplayObject.Stop();
            _playerAnimation.CheckToChangeState("Idle", false); ;
        }
        else if (Debug.isDebugBuild)
        {
            Debug.Log("Something isnt set here", this);
        }

        //change reticle
        if (reticle.TryGetComponent(out ReticleSwitcher i))
        {
            i.ChangeReticle(0);
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

        isPlayerLocked = true;

        //Stop movement in the movement script, dont disable or deactive player input (they couldn't pause then)
        _movement.SetMovement(3);
        _input.DeactivateInput();

        //Stop camera player camera movement
        _cam.cameraRotationLock = true;

        //Note: option to have them switch to a different camera for cinematics
        if (Debug.isDebugBuild)
        {
            Debug.Log("Player locked!", this);
        }
    }

    public void UnLockPlayer()
    {
        //Guard clause to make sure the components are correct
        if (!_cam || !_movement) return;

        isPlayerLocked = false;

        //Start player movement
        _movement.SetMovement(2);
        _input.ActivateInput();

        //Restart camera movement
        _cam.cameraRotationLock = false;

        if (Debug.isDebugBuild)
        {
            Debug.Log("Player unlocked!", this);
        }
    }

    /// <summary>
    /// BE CAREFUL WHEN USING THIS, ONLY FOR WIN SCREEN
    /// </summary>
    public void DisablePlayer()
    {
        //Shouldn't untag in the win screen or generally when eliminated
        StopCoroutine(Co_TaggedEffect(taggedAnimduration));

        if (_playerAnimation)
        {
            _playerAnimation.CheckToChangeState("Idle");
        }

        _input.DeactivateInput();
        _movement.enabled = false;
        _playerInteraction.enabled = false;
        _cam.enabled = false;

        if (taggedDisplayObject && taggedDisplayObject.isPlaying)
        {
            taggedDisplayObject.Stop();
        }
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

        if (_cam  && taggedDisplayObject)
        {
            //Play VFX + Sound
            //Lerp into first person camera mode#
            if (_cam.enabled)
            {
                _cam.SetCameraView(true);
            }

            taggedDisplayObject.Play();
        }
        else if (Debug.isDebugBuild)
        {
            Debug.Log("Something isnt set here", this);
        }


        if (soundManager)
        {
            //Play the tagged sound
            //sm.PlaySound();
        }
    }

    private void EliminationEffect()
    {
        //Send the player into "spectator" mode (No model, no colliders)
        if (_cam)
        {
            //Forcing it into third person
            _cam.EnableThirdPerson();

            //Moving it to the correct state
            _cam.cameraState = PlayerCamera.cS.FREECAMUNCONSTRAINED;
            
        }

        //Play Sound
        if (soundManager)
        {
            soundManager.PlaySound(Bloodboom);
        }

        //Making sure the elimination icon is showing
        if (taggedDisplayObject != null)
        {
            taggedDisplayObject.Stop();
        }
        if (elimDisplayObject != null)
        {
            elimDisplayObject.Play();
        }

        //Play VFX
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
    }

    #endregion
}
