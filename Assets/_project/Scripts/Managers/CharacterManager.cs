/////////////////////////////////////////////////////////////
//
//  Script Name: CharacterManager.cs
//  Creator: Charles Carter
//  Description: A manager script for the characters in the scene player or otherwise
//  
/////////////////////////////////////////////////////////////

//This script uses these namespaces
using UnityEngine;

//The class needs these other components on the object
[RequireComponent(typeof(TaggedTracker))]
public class CharacterManager : MonoBehaviour
{  
    private TaggedTracker _tracker;
    private PlayerController _movement;
    private Renderer _rend;

    //Just for testing
    [SerializeField]
    private Material eliminatedMat;

    public bool isActive { get; private set; }
    public bool isPlayer { get; private set; }

    private void Awake()
    {
        _tracker = GetComponent<TaggedTracker>();
        _movement = GetComponent<PlayerController>();
        _rend = GetComponent<Renderer>();
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

    private void OnEnable()
    {

    }

    private void OnDisable()
    {

    }

    //Some Gamemodes will have elimination, some wont
    public void CheckIfEliminated()
    {
        //If they arent tagged then do nothing
        if (!_tracker.isTagged) return;

        //The player should do whatever the gamemode wants them to (base gamemode will want them to explode)
        if (eliminatedMat != null)
        {
            if (Debug.isDebugBuild)
            {
                Debug.Log("Eliminated player shown", this);
            }

            _rend.material = eliminatedMat;
        }
        else
        {
            if (Debug.isDebugBuild)
            {
                Debug.Log("Set an eliminated material in the renderer", this);
            }
        }

        //Play VFX + Sound
        //Turn all non-important scripts off (ones that allow the player to interact especially)
        //Make them in spectator camera
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
        //Animation for regaining potato
    }

    public void ThisPlayerUnTagged()
    {
        //Play VFX + Sound
        //Lerp into thrid person camera mode Note: this should be quicker than the lerp when you're tagged
    }
}
