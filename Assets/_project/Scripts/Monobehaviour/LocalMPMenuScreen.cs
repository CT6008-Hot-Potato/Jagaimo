////////////////////////////////////////////////////////////
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

    [SerializeField]
    private List<PlayerInput> joinedPlayers = new List<PlayerInput>();

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
        if (Debug.isDebugBuild)
        {
            Debug.Log("Player has joined in menu on device");
        }

        //Storing the player object for the next scene
        playerInput.transform.SetParent(game.transform);

        //Updating the game settings
        game.LocalPlayerInputs[joinedPlayers.Count] = playerInput;
        game.iPlayercount++;

        //Updating the UI
        joinedIcon[joinedPlayers.Count].SetActive(true);
        promptIcon[joinedPlayers.Count].SetActive(false);

        joinedPlayers.Add(playerInput);

        if (joinedPlayers.Count == 1)
        {
            CheckIfCanStart();
        }
    }

    public void StoringPlayerCount()
    {
        game.iPlayercount = inputManager.playerCount;
    }

    //When the back button is presseed, have to remove the currently stored players
    public void RemoveCharacters()
    {
        //Destroying all the joined players
        for (int i = 0; i < joinedPlayers.Count; ++i)
        {
            Destroy(joinedPlayers[i].gameObject);           
        }

        //Clearing the list
        joinedPlayers.Clear();

        game.ClearPlayers();

        for (int i = 0; i < promptIcon.Length; ++i)
        {
            joinedIcon[i].SetActive(false);
            promptIcon[i].SetActive(true);
        }
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
