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

        menuWaitBehaviour = menuWaitBehaviour ?? GetComponent<BasicTimerBehaviour>();
        worldBounds = worldBounds ?? FindObjectOfType<WorldBounds>();
    }



    #endregion

    #region Public Methods

    //Playing the win screen (and pass through the gamemode incase of different screens per gamemode)
    public void PlayWinScreen(GAMEMODE_INDEX gamemode, List<CharacterManager> everyPlayer, List<CharacterManager> winningChars)
    {
        if (worldBounds)
        {
            Destroy(worldBounds);
        }

        allPlayers = everyPlayer;
        charactersWhoWon = winningChars;

        //Going through all the manager 
        foreach (CharacterManager manager in everyPlayer)
        {
            //Turning off the revelant components;
            manager.DisablePlayer();
        }

        //There could be a tidier way to do this but for now this will work
        //This will determine what's shown on the screen (so podiums vs football spots vs infected/sabotage scene)
        //Spinning Podiums for classic gamemode
        //Use football arena for football
        //Using a generator scene for sabotage
        //Using a small barn scene for infected
        GameObject go = Instantiate(winScreenPrefabs[(int)gamemode], transform);
        winScreen = go.GetComponent<WinScreen>();

        winScreen.StartWinSequence(allPlayers, winningChars);

        //Setting a timer for going back to the menu
        menuWaitBehaviour.SetDuration(winScreen.screenDuration);
        menuWaitBehaviour.CallOnTimerStart();
    }

    //Going back to the main menu
    public void ReturnToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    #endregion
}
