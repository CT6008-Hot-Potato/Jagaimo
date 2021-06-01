/////////////////////////////////////////////////////////////
//
//  Script Name: CharacterManager.cs
//  Creator: Charles Carter
//  Editor for Multiplayer Networking: Theodor Danciu 
//  Description: A manager script for the characters in the scene player or otherwise
//  
/////////////////////////////////////////////////////////////

//This script uses these namespaces
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

[RequireComponent(typeof(JagaimoTaggedTracker))]
public class JagaimoCharacterManager : NetworkBehaviour
{
    #region Variables Needed

    [Header("Public Variables")]
    //The tracker attached to the object
    public JagaimoTaggedTracker _tracker;

    //A potentially useful bool for AI production
    public bool isPlayer { get; private set; }

    //A public variable for scripts to check if this player is locked
    public bool isPlayerLocked { private get; set; } = false;


    [Header("Componenets Needed")]
    //Components already on this object
    [SerializeField]
    private JagaimoPlayerController _movement;
    [SerializeField]
    private JagaimoPlayerCamera _cam;
    [SerializeField]
    private JagaimoPlayerAnimation _playerAnimation;
    [SerializeField]
    private JagaimoPlayerInteraction _playerInteraction;

    //Other componenets needed
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


    private JagaimoNetworkManager networkManager;
    #endregion

    #region Unity Methods

    
    
    private void Awake()
    {
        _tracker = _tracker ? _tracker : GetComponent<JagaimoTaggedTracker>();
        _movement = _movement ? _movement : GetComponent<JagaimoPlayerController>();
        _cam = _cam ? _cam : GetComponent<JagaimoPlayerCamera>();
        _playerAnimation = _playerAnimation ? _playerAnimation : GetComponent<JagaimoPlayerAnimation>();
        _playerInteraction = _playerInteraction ? _playerInteraction : GetComponent<JagaimoPlayerInteraction>();
        
        soundManager = FindObjectOfType<SoundManager>();
        settings = GameSettingsContainer.instance;

        networkManager = networkManager
            ? networkManager
            : gameObject.GetComponent<JagaimoNetworkManager>();

        //To be unlocked when the game is ready
        LockPlayer();
    }

    public override void OnStartLocalPlayer()
    {

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
    public JagaimoCharacterManager CheckIfEliminated(int playersLeft)
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
            _cam.cameraState = JagaimoPlayerCamera.cS.FREECAMUNCONSTRAINED;
        }

        //Play Sound
        if (soundManager)
        {
            //soundManager.PlaySound(ScriptableSounds.Sounds.Explosion);
        }

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

        LockPlayer();

        StartCoroutine(Co_TaggedEffect(2));
        //Wait 2s for animation to complete      
    }

    public void ThisPlayerUnTagged()
    {
        //Play VFX + Sound
        if (soundManager)
        {
            //Maybe an untagged sound, maybe it would become too chaotic
        }

        //Lerp into thrid person camera mode Note: this should be quicker than the lerp when you're tagged
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