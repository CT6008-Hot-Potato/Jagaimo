////////////////////////////////////////////////////////////
// File: LocalMPScreenPartioning.cs
// Author: Charlie Bullock and Charles Carter
// Date Created: 17/02/21
// Brief: This script determines the camera's view of the screen in relation to each player
//////////////////////////////////////////////////////////// 

//This class is using:
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LocalMPScreenPartioning : MonoBehaviour
{
    #region Variables Needed

    public int playerIndex;
    [SerializeField]
    private PlayerCamera[] playerCameras;
    [SerializeField]
    private GameObject playerPrefab;
    [SerializeField]
    private GameObject playerManager;
    [SerializeField]
    private bool singleLocalPlayer;
    public bool isActive { get; private set; }

    //The settings of the game
    private GameSettingsContainer settings;
    [SerializeField]
    private PlayerInputManager manager;

    private GameObject singlePlayer;
    private List<PlayerCamera> newPlayerCameras = new List<PlayerCamera>();
    private int ExpectedPlayerCount = 1;

    private InputDevice latestDeviceJoined;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        settings = GameSettingsContainer.instance;
        manager = manager ?? GetComponent<PlayerInputManager>();

        //If there are settings to take values from
        if (settings)
        {
            if (settings.iPlayercount > 1)
            {
                ExpectedPlayerCount = settings.iPlayercount;

                singleLocalPlayer = false;

                for (int i = 0; i < settings.iPlayercount; ++i)
                {
                    int tempIndex = settings.LocalPlayerInputs[i].playerIndex;
                    int tempSplitscreenIndex = settings.LocalPlayerInputs[i].splitScreenIndex;
                    string tempControlScheme = settings.LocalPlayerInputs[i].currentControlScheme;
                    latestDeviceJoined = settings.LocalPlayerInputs[i].devices[0];

                    Destroy(settings.LocalPlayerInputs[i].gameObject);

                    manager.JoinPlayer(tempIndex, tempSplitscreenIndex, tempControlScheme);
                }
            }
            else if (settings.iPlayercount == 1)
            {
                singleLocalPlayer = true;

                int tempIndex = settings.LocalPlayerInputs[0].playerIndex;
                int tempSplitscreenIndex = settings.LocalPlayerInputs[0].splitScreenIndex;
                string tempControlScheme = settings.LocalPlayerInputs[0].currentControlScheme;
                latestDeviceJoined = settings.LocalPlayerInputs[0].devices[0];

                Destroy(settings.LocalPlayerInputs[0].gameObject);

                manager.JoinPlayer(tempIndex, tempSplitscreenIndex, tempControlScheme);
            }
            else
            {
                //No players joined, so it was single player from the main menu
                singleLocalPlayer = true;
                singlePlayer = Instantiate(playerPrefab, new Vector3(0, 25, 0), playerPrefab.transform.rotation);
                singlePlayer.GetComponent<CharacterManager>().UnLockPlayer();
            }
        }
        //Was played from the scene, anyone can join
        else if (singleLocalPlayer)
        {
            //playerManager.SetActive(false);
            singlePlayer = Instantiate(playerPrefab, new Vector3(0, 25, 0), playerPrefab.transform.rotation);
            singlePlayer.GetComponent<CharacterManager>().UnLockPlayer();
        }
        else
        {
            manager.EnableJoining();
        }
    }

    #endregion

    #region Public Methods

    public void OnPlayerJoined(PlayerInput playerInput)
    {     
        //Getting the camera from the player that joined
        PlayerCamera camera = playerInput.GetComponent<PlayerCamera>();

        //Making sure it has a camera
        if (camera)
        {
            //Using the relevant functions
            camera.playerIndex = playerInput.playerIndex;
            camera.SetPlayerMask();

            //Checking player device
            if (latestDeviceJoined != null)
            {
                if (latestDeviceJoined.name != "Keyboard" && latestDeviceJoined.name != "Mouse")
                {
                    //Gamepad used
                    camera.useControllerSensitivity = true;
                }
                else
                {
                    //Keyboard used
                    camera.useControllerSensitivity = false;
                }
            }

            newPlayerCameras.Add(camera);
        }

        //This is the last player to join
        if (newPlayerCameras.Count >= ExpectedPlayerCount && !singleLocalPlayer)
        {
            SetScreenResolutions();
        }
    }
    #endregion

    #region Private Methods

    //Setting the resolutions of the cameras based on the amount of players
    private void SetScreenResolutions()
    {
        switch (newPlayerCameras.Count)
        {
            case 1:
                newPlayerCameras[0].firstPersonCamera.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
                newPlayerCameras[0].thirdPersonCamera.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
                break;
            case 2:
                newPlayerCameras[0].firstPersonCamera.rect = new Rect(0.0f, 0.5f, 1.0f, 0.5f);
                newPlayerCameras[0].thirdPersonCamera.rect = new Rect(0.0f, 0.5f, 1.0f, 0.5f);

                newPlayerCameras[1].firstPersonCamera.rect = new Rect(0.0f, 0.0f, 1.0f, 0.5f);
                newPlayerCameras[1].thirdPersonCamera.rect = new Rect(0.0f, 0.0f, 1.0f, 0.5f);
                break;
            case 3:
                newPlayerCameras[0].firstPersonCamera.rect = new Rect(0.0f, 0.5f, 1.0f, 0.5f);
                newPlayerCameras[0].thirdPersonCamera.rect = new Rect(0.0f, 0.5f, 1.0f, 0.5f);
                newPlayerCameras[1].firstPersonCamera.rect = new Rect(0.0f, 0.0f, 0.5f, 0.5f);
                newPlayerCameras[1].thirdPersonCamera.rect = new Rect(0.0f, 0.0f, 0.5f, 0.5f);
                newPlayerCameras[2].firstPersonCamera.rect = new Rect(0.5f, 0.0f, 0.5f, 0.5f);
                newPlayerCameras[2].thirdPersonCamera.rect = new Rect(0.5f, 0.0f, 0.5f, 0.5f);
                break;
            case 4:
                newPlayerCameras[0].firstPersonCamera.rect = new Rect(0.0f, 0.5f, 0.5f, 0.5f);
                newPlayerCameras[0].thirdPersonCamera.rect = new Rect(0.0f, 0.5f, 0.5f, 0.5f);
                newPlayerCameras[1].firstPersonCamera.rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
                newPlayerCameras[1].thirdPersonCamera.rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
                newPlayerCameras[2].firstPersonCamera.rect = new Rect(0.0f, 0.0f, 0.5f, 0.5f);
                newPlayerCameras[2].thirdPersonCamera.rect = new Rect(0.0f, 0.0f, 0.5f, 0.5f);
                newPlayerCameras[3].firstPersonCamera.rect = new Rect(0.5f, 0.0f, 0.5f, 0.5f);
                newPlayerCameras[3].thirdPersonCamera.rect = new Rect(0.5f, 0.0f, 0.5f, 0.5f);
                break;
            default:
                if (Debug.isDebugBuild)
                {
                    Debug.Log("There are no cameras", this);
                }
                break;
        }
    }

    #endregion
}
