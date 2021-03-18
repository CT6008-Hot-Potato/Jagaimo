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
    private Renderer _rend;
    private PlayerCamera _cam;

    //Just for testing
    [SerializeField]
    private Material eliminatedMat;

    public bool isPlayer { get; private set; }

    [SerializeField]
    TestStartTrigger trigger;

    private void Awake()
    {
        trigger = TestStartTrigger.instance;
        _tracker = _tracker ?? GetComponent<TaggedTracker>();
        _movement = _movement ?? GetComponent<PlayerController>();
        _cam = _cam ?? GetComponent<PlayerCamera>();
        _rend = GetComponent<Renderer>();
    }


    //public void RedoIndex()
    //{
    //    playerCameras = FindObjectsOfType<PlayerCamera>();
    //    playerIndex = (playerCameras.Length - 1) / 2;
    //    for (int i = 0; i > playerIndex; i++)
    //    {
    //        playerCameras[i].playerIndex = i;
    //    }
    //}

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
        if (eliminatedMat != null)
        {
            if (Debug.isDebugBuild)
            {
                Debug.Log("Eliminated player shown", this);
            }

            transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            gameObject.SetActive(false);

            if (_rend)
            {
                _rend.material = eliminatedMat;
            }
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

        return this;
    }

    //public void EliminatePlayer()
    //{
    //
    //}

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
            GetComponent<PlayerCamera>().SetCameraView(false);
        }

        //Animation for regaining potato
    }

    public void ThisPlayerUnTagged()
    {
        //Play VFX + Sound
        //Lerp into thrid person camera mode Note: this should be quicker than the lerp when you're tagged
        if (_cam)
        {
            GetComponent<PlayerCamera>().SetCameraView(false);
        }
    }
}
