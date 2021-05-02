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
    public TaggedTracker _tracker;

    private PlayerController _movement;
    private PlayerCamera _cam;
    private PlayerAnimation _playerAnimation;

    private SoundManager sm;
    private ParticleSystem elimVFX;

    public bool isPlayer { get; private set; }

    [SerializeField]
    GameObject taggedDisplayObject;

    private void Awake()
    {
        _tracker = _tracker ?? GetComponent<TaggedTracker>();
        _movement = _movement ?? GetComponent<PlayerController>();
        _cam = _cam ?? GetComponent<PlayerCamera>();
        _playerAnimation = _playerAnimation ?? GetComponent<PlayerAnimation>();

        sm = FindObjectOfType<SoundManager>();
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
    }

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
        if (sm)
        {
            sm.PlaySound(ScriptableSounds.Sounds.Explosion);
        }

        if (elimVFX)
        {
            elimVFX.Play();
        }

        //Turn all non-important scripts off (ones that allow the player to interact especially)

        return this;
    }

    //Function to have the player locked in a position (so they cant move or rotate the camera)
    private void LockPlayer()
    {
        //Stop movement
        //Stop camera player camera movement

        //Note: option to have them switch to a different camera for cinematics
    }

    private void UnLockPlayer()
    {
        //Restart camera movement
        //Start player movement
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

        if (sm)
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
}
