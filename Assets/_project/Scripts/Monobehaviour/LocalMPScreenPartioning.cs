////////////////////////////////////////////////////////////
// File: LocalMPScreenPartioning.cs
// Author: Charlie Bullock (Edited by Charles Carter)
// Date Created: 17/02/21
// Brief: This script determines the camera's view of the screen in relation to each player
//////////////////////////////////////////////////////////// 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalMPScreenPartioning : MonoBehaviour
{
    public int playerIndex;
    private int playerIndexPrior;
    [SerializeField]
    private PlayerCamera[] playerCameras;
    [SerializeField]
    private GameObject playerPrefab;
    [SerializeField]
    private GameObject playerManager;
    [SerializeField]
    private bool singleLocalPlayer;
    public bool isActive { get; private set; }

    private void Awake()
    {
        if (singleLocalPlayer)
        {
            playerManager.SetActive(false);
            Instantiate(playerPrefab, new Vector3(0, 1, 0), playerPrefab.transform.rotation);
        }

        playerCameras = FindObjectsOfType<PlayerCamera>();
        playerIndex = (playerCameras.Length - 1) / 2;
        for (int i = 0; i > playerIndex; i++)
        {
            playerCameras[i].playerIndex = i;
        }
        playerIndexPrior = playerIndex;
    }

    // Update is called once per frame
    void Update()
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
                cameras[i].SetPlayerColor();
                //Debug.Log(playerIndex + "PLAYER INDEX");
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
}
