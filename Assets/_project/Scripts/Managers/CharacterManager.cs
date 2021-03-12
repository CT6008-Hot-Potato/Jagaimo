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
    public int playerIndex;
    private int playerIndexPrior;
    //Just for testing
    [SerializeField]
    private Material eliminatedMat;
    [SerializeField]
    private PlayerCamera[] playerCameras;
    [SerializeField]
    private GameObject playerPrefab;
    [SerializeField]
    private GameObject playerManager;
    [SerializeField]
    private bool singleLocalPlayer;
    public bool isActive { get; private set; }
    public bool isPlayer { get; private set; }

    private void Awake()
    {
        if (singleLocalPlayer)
        {
            playerManager.SetActive(false);
            Instantiate(playerPrefab, new Vector3(0, 1, 0),playerPrefab.transform.rotation);
        }
        _tracker = GetComponent<TaggedTracker>();
        _movement = GetComponent<PlayerController>();
        _rend = GetComponent<Renderer>();
        playerCameras = FindObjectsOfType<PlayerCamera>();
        playerIndex = (playerCameras.Length - 1) / 2;
        for (int i = 0; i > playerIndex;i++)
        {
            playerCameras[i].playerIndex = i;
        }
        playerIndexPrior = playerIndex;
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

    private void Update()
    {
        playerCameras = FindObjectsOfType<PlayerCamera>();
        playerIndex = (playerCameras.Length - 1);
        if (playerIndex != playerIndexPrior)
        {
            playerIndexPrior = playerIndex;
            PlayerCamera[] cameras = FindObjectsOfType<PlayerCamera>();
            for (int i = 0; i < cameras.Length; i++)
            {
                cameras[i].playerIndex = i;
                Debug.Log(playerIndex + "PLAYER INDEX");
                Debug.Log(i + "Current player");
                switch (playerIndex)
                {
                    case 0:
                        if (i == 0)
                        {
                            cameras[0].firstPersonCamera.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
                            cameras[0].thirdPersonCamera.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
                        }
                        else
                        {
                            Debug.Log("Value too high");
                        }
                        break;
                    case 1:
                        switch (i)
                        {
                            case 0:
                                cameras[0].firstPersonCamera.rect = new Rect(0.0f, 0.5f, 1.0f, 0.5f);
                                cameras[0].thirdPersonCamera.rect = new Rect(0.0f, 0.5f, 1.0f, 0.5f);
                                break;
                            case 1:
                                cameras[1].firstPersonCamera.rect = new Rect(0.0f, 0.0f, 1.0f, 0.5f);
                                cameras[1].thirdPersonCamera.rect = new Rect(0.0f, 0.0f, 1.0f, 0.5f);
                                break;
                            default:
                                Debug.Log("Value too high");
                                break;
                        }
                        break;
                    case 2:
                        switch (i)
                        {
                            case 0:
                                cameras[0].firstPersonCamera.rect = new Rect(0.0f, 0.5f, 1.0f, 0.5f);
                                cameras[0].thirdPersonCamera.rect = new Rect(0.0f, 0.5f, 1.0f, 0.5f);
                                break;
                            case 1:
                                cameras[1].firstPersonCamera.rect = new Rect(0.0f, 0.0f, 0.5f, 0.5f);
                                cameras[1].thirdPersonCamera.rect = new Rect(0.0f, 0.0f, 0.5f, 0.5f);
                                break;
                            case 2:
                                cameras[2].firstPersonCamera.rect = new Rect(0.5f, 0.0f, 0.5f, 0.5f);
                                cameras[2].thirdPersonCamera.rect = new Rect(0.5f, 0.0f, 0.5f, 0.5f);
                                break;
                            default:
                                Debug.Log("Value too high");
                                break;
                        }
                        break;
                    case 3:
                        switch (i)
                        {
                            case 0:
                                cameras[0].firstPersonCamera.rect = new Rect(0.0f, 0.5f, 0.5f, 0.5f);
                                cameras[0].thirdPersonCamera.rect = new Rect(0.0f, 0.5f, 0.5f, 0.5f);
                                break;
                            case 1:
                                cameras[1].firstPersonCamera.rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
                                cameras[1].thirdPersonCamera.rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
                                break;
                            case 2:
                                cameras[2].firstPersonCamera.rect = new Rect(0.0f, 0.0f, 0.5f, 0.5f);
                                cameras[2].thirdPersonCamera.rect = new Rect(0.0f, 0.0f, 0.5f, 0.5f);
                                break;
                            case 3:
                                cameras[3].firstPersonCamera.rect = new Rect(0.5f, 0.0f, 0.5f, 0.5f);
                                cameras[3].thirdPersonCamera.rect = new Rect(0.5f, 0.0f, 0.5f, 0.5f);
                                break;
                            default:
                                Debug.Log("Value too high");
                                break;
                        }
                        break;
                }
            }
        }
    }

    private void OnEnable()
    {

    }

    private void OnDisable()
    {

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
        //Animation for regaining potato
    }

    public void ThisPlayerUnTagged()
    {
        //Play VFX + Sound
        //Lerp into thrid person camera mode Note: this should be quicker than the lerp when you're tagged
    }
}
