/////////////////////////////////////////////////////////////
//
//  Script Name: CharacterManager.cs
//  Creator: Charles Carter
//  Description: A manager script for the characters in the scene player or otherwise
//  
/////////////////////////////////////////////////////////////

//This script uses these namespaces
using UnityEngine;

[RequireComponent(typeof(TaggedTracker))]
public class CharacterManager : MonoBehaviour
{
    #region Variables Needed

    //The tracker attached to the object
    public TaggedTracker _tracker;

    //A potentially useful bool for AI production
    public bool isPlayer { get; private set; }

    //A public variable for scripts to check if this player is locked
    public bool isPlayerLocked { private get; set; } = false;

    //Components already on this object
    [SerializeField]
    private PlayerController _movement;
    [SerializeField]
    private PlayerCamera _cam;
    [SerializeField]
    private PlayerAnimation _playerAnimation;

    //Other componenets needed
    [SerializeField]
    private SoundManager soundManager;
    [SerializeField]
    private GameSettingsContainer settings;

    //[SerializeField]
    //private bool bUsingConfettiVFX = false;
    [SerializeField]
    private ParticleSystem elimVFX;
    [SerializeField]
    private ParticleSystem confettiElimVFX;
    [SerializeField]
    private GameObject taggedDisplayObject;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        _tracker = _tracker ?? GetComponent<TaggedTracker>();
        _movement = _movement ?? GetComponent<PlayerController>();
        _cam = _cam ?? GetComponent<PlayerCamera>();
        _playerAnimation = _playerAnimation ?? GetComponent<PlayerAnimation>();

        soundManager = FindObjectOfType<SoundManager>();
        settings = GameSettingsContainer.instance;
    }

    private void Start()
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
    public CharacterManager CheckIfEliminated()
    {
        //If they arent tagged then do nothing
        if (!_tracker.isTagged) return null;

        //The player should do whatever the gamemode wants them to (base gamemode will want them to explode)
        if (Debug.isDebugBuild)
        {
            Debug.Log("Eliminated player shown", this);
        }

        //Send the player into "spectator" mode (No model, no colliders)
        if (_cam)
        {
            _cam.cameraState = PlayerCamera.cS.FREECAMUNCONSTRAINED;
        }

        //Play VFX + Sound
        if (soundManager)
        {
            soundManager.PlaySound(ScriptableSounds.Sounds.Explosion);
        }

        if (elimVFX)
        {
            elimVFX.Play();
        }

        //Turn all non-important scripts off (ones that allow the player to interact especially)

        return this;
    }

    //Functions to change the player when they're tagged or untagged
    public void ThisPlayerTagged()
    {
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

        //Animation for regaining potato
        if (_playerAnimation)
        {
            _playerAnimation.CheckToChangeState("FallingBackDeath", true);
        }
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

        //Animations switch back to being without potato (?)
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
        //_movement.StopMovement();

        //Stop camera player camera movement
        //_cam.StopCamera();

        //Note: option to have them switch to a different camera for cinematics
    }

    public void UnLockPlayer()
    {
        //Guard clause to make sure the components are correct
        if (!_cam || !_movement) return;

        isPlayerLocked = false;

        //Start player movement
        //_movement.StartMovement();

        //Restart camera movement
        //_cam.StartCamera();
    }

    #endregion
}
