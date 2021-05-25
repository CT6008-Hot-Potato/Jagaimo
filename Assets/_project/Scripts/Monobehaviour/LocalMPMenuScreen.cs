﻿////////////////////////////////////////////////////////////
// File: LocalMPMenuScreen.cs
// Author: Charles Carter
// Date Created: 21/03/21
// Brief: Determing how many players will be in the local game in the menu
//////////////////////////////////////////////////////////// 

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class LocalMPMenuScreen : MonoBehaviour
{
    #region Variables Needed

    [SerializeField]
    private PlayerInputManager inputManager;
    private GameSettingsContainer game;

    //The reference to the texts
    [SerializeField]
    private GameObject[] joinedIcon;
    [SerializeField]
    private GameObject[] promptIcon;
    [SerializeField]
    private Button startButton;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        inputManager = inputManager ?? GetComponent<PlayerInputManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //The static is set in awake
        game = GameSettingsContainer.instance;

        if (startButton)
        {
            startButton.interactable = false;
        }
    }

    private void OnEnable()
    {
        inputManager.EnableJoining();
    }

    private void OnDisable()
    {
        inputManager.DisableJoining();
    }

    #endregion

    #region Public Methods

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        //if (Debug.isDebugBuild)
        //{
        //    Debug.Log("Player has joined in menu on device");
        //}

        //Storing the player object for the next scene
        playerInput.transform.SetParent(game.transform);

        //Updating the UI
        joinedIcon[game.iPlayercount].SetActive(true);
        promptIcon[game.iPlayercount].SetActive(false);

        game.LocalPlayerInputs[game.iPlayercount] = playerInput;
        game.iPlayercount++;

        if (game.iPlayercount == 1)
        {
            CheckIfCanStart();
        }
    }

    //When the back button is presseed, have to remove the currently stored players
    public void RemoveCharacters()
    {
        //Going through and resetting the icons
        for (int i = 0; i < promptIcon.Length; ++i)
        {
            if (joinedIcon[i] && promptIcon[i])
            {
                joinedIcon[i].SetActive(false);
                promptIcon[i].SetActive(true);
            }
        }

        game.ClearPlayers();
    }

    #endregion

    //Unlocking the start button
    private void CheckIfCanStart()
    {
        if (startButton)
        {
            startButton.interactable = true;
        }
    }
}
