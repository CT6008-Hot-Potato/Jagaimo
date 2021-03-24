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
    private List<GameObject> joinedPlayers = new List<GameObject>();

    [SerializeField]
    private GameObject[] joinedIcon;
    [SerializeField]
    private GameObject[] promptIcon;
    [SerializeField]
    Button startButton;

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

    }

    private void OnEnable()
    {
        inputManager.EnableJoining();
    }

    #endregion

    #region Public Methods

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        if (Debug.isDebugBuild)
        {
            Debug.Log("Player has joined in menu");
        }

        //Storing the player object for the next scene
        DontDestroyOnLoad(playerInput.gameObject);
        joinedPlayers.Add(playerInput.gameObject);

        //Updating the game settings
        game.LocalPlayerInputs[joinedPlayers.Count - 1] = playerInput.gameObject.GetComponent<PlayerInput>();
        game.iPlayercount++;

        //Updating the UI
        joinedIcon[joinedPlayers.Count - 1].SetActive(true);
        promptIcon[joinedPlayers.Count - 1].SetActive(false);

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
        for (int i = 1; i < joinedPlayers.Count; ++i)
        {
            Destroy(joinedPlayers[i]);           
        }

        //Clearing the list
        joinedPlayers.Clear();

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
