////////////////////////////////////////////////////////////
// File: WinScreenManager.cs
// Author: Charles Carter
// Date Created: 10/05/21
// Brief: The script that controls what happens when the game is over and players have won
//////////////////////////////////////////////////////////// 

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class WinScreenManager : MonoBehaviour
{
    #region Variables Needed

    public static WinScreenManager instance;

    //The timer before we go back to the main menu
    [SerializeField]
    private BasicTimerBehaviour menuWaitBehaviour;

    [SerializeField]
    private GameObject[] winScreenPrefabs;

    [SerializeField]
    private WorldBounds worldBounds;

    //The list is in order of 1st-Last (or just the winners depending on the gamemode)
    [SerializeField]
    private List<CharacterManager> charactersWhoWon = new List<CharacterManager>();
    [SerializeField]
    private List<CharacterManager> allPlayers = new List<CharacterManager>();

    //The win screen itself (which is on the prefab object)
    private WinScreen winScreen;

    [SerializeField]
    private bool bReturnToMenu = true;

    [SerializeField]
    AudioClip WinMusic;

    [SerializeField]
    private GameSettingsContainer settings;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        if (instance)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }

        settings = settings ?? GameSettingsContainer.instance;
        menuWaitBehaviour = menuWaitBehaviour ?? GetComponent<BasicTimerBehaviour>();
        worldBounds = worldBounds ?? FindObjectOfType<WorldBounds>();
    }

    #endregion

    #region Public Methods

    public void SpawnWinScreen(GAMEMODE_INDEX gamemode)
    {
        GameObject go = Instantiate(winScreenPrefabs[(int)gamemode], transform);
        winScreen = go.GetComponent<WinScreen>();
    }

    //Playing the win screen (and pass through the gamemode incase of different screens per gamemode)
    public void PlayWinScreen(GAMEMODE_INDEX gamemode, List<CharacterManager> everyPlayer, List<CharacterManager> winningChars)
    {
        if (WinMusic != null)
        {
          if(TryGetComponent(out AudioSource changeMe))
            {
                changeMe.Stop(); ;
                changeMe.clip = WinMusic;
                changeMe.Play();
            }
        }

        //Destroying the world bounds so it doesnt interfere with the win screen
        if (worldBounds)
        {
            Destroy(worldBounds);
        }

        //Destroying the settings so it doesnt interfere with the next game
        if (settings)
        {
            Destroy(settings.gameObject);
        }

        allPlayers = everyPlayer;
        charactersWhoWon = winningChars;

        //Going through all the manager 
        foreach (CharacterManager manager in everyPlayer)
        {
            if (manager)
            {
                manager.LockPlayer();
                //Turning off the revelant components;
                manager.DisablePlayer();
            }
        }

        //This will determine what's shown on the screen (so podiums vs football spots vs infected/sabotage scene)
        //Spinning Podiums for classic gamemode
        //Use football arena for football
        //Using a generator scene for sabotage
        //Using a small barn scene for infected

        if (winScreen)
        {
            winScreen.StartWinSequence(allPlayers, winningChars);
        }

        //Setting a timer for going back to the menu
        if (bReturnToMenu)
        {
            menuWaitBehaviour.SetDuration(winScreen.screenDuration);
            menuWaitBehaviour.CallOnTimerStart();
        }
    }

    //Going back to the main menu
    public void ReturnToMenu()
    {
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("MainMenu");
    }

    #endregion
}
