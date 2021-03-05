////////////////////////////////////////////////////////////
// File: GameCreationSettings
// Author: Charles Carter
// Date Created: 16/02/21
// Brief: The settings for creating a game in the practice menu
//////////////////////////////////////////////////////////// 

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//A container class for the gamemodes
[System.Serializable]
public class GamemodeUI
{
    //Constructor
    public GamemodeUI(string gname, GAMEMODE_INDEX gindex, List<MutatorUI> gmutators)
    {
        name = gname;
        index = gindex;
        GamemodeMutators = gmutators;
    }

    //Reference Values
    public string name;
    public GAMEMODE_INDEX index;

    //Is set by menu mutator UI
    [HideInInspector]
    public List<MutatorUI> GamemodeMutators;

    //Getting the correct maps for each gamemode
    public GameObject potentialMaps;
    public ToggleGroup MapsGroup;
    public Toggle firstMaps;
}

public enum GAMEMODE_INDEX
{
    CLASSIC = 0,
    INFECTED = 1,
    FOOTBALL = 2,
    SABOTAGE = 3,

    COUNT
}

public enum MAP_INDEX
{
    STUDIO = 0,
    STADIUM = 1,

    COUNT
}

public class GameCreationSettings : MonoBehaviour
{
    #region Public Variables

    public GamemodeUI[] Current_GamemodesUI;
    public GamemodeUI SelectedGamemode;

    #endregion

    #region Inspector Fields

    [Header("Needed Variables")]
    [SerializeField]
    private MenuMutatorUI MutatorUI;
    static GamemodeUI[] Static_GamemodesUI;
    [SerializeField]
    private GAMEMODE_INDEX iCurrentGamemodeSelection = GAMEMODE_INDEX.CLASSIC;
    [SerializeField]
    private MAP_INDEX iCurrentMapSelection = MAP_INDEX.STUDIO;


    [SerializeField]
    private List<MutatorUI> GeneralMutators;

    [SerializeField]
    private EventSystem eventSystem;

    #endregion

    #region Unity Methods

    void Awake()
    {
        SelectedGamemode = Current_GamemodesUI[0];
        UpdateMapGroup(0);
    }

    #endregion

    #region Public Methods

    //A different gamemode was selected
    public void GamemodeChanged(int iGamemodeChange)
    {
        UpdateMapGroup(iGamemodeChange);
    }

    //A different map was selected
    public void MapChanged(int iMapChange)
    {
        iCurrentMapSelection = (MAP_INDEX)iMapChange;
    }

    public void StartButton()
    {
        if (Debug.isDebugBuild)
        {
            Debug.Log("Map Chosen is: " + iCurrentMapSelection.ToString());
        }
    }

    #endregion

    #region Private Methods

    //Changing the maps viewed
    private void UpdateMapGroup(int newMapGroup)
    {
        //If there is an object for the new gamemode
        if (Current_GamemodesUI[newMapGroup].potentialMaps && Current_GamemodesUI[(int)iCurrentGamemodeSelection].firstMaps && Current_GamemodesUI[(int)iCurrentGamemodeSelection].MapsGroup)
        {
            if (Debug.isDebugBuild)
            {
                Debug.Log("New Gamemode: " + newMapGroup);
                Debug.Log("Current Selection: " + iCurrentGamemodeSelection);
            }

            //Current_GamemodesUI[(int)iCurrentGamemodeSelection].MapsGroup.NotifyToggleOn(Current_GamemodesUI[(int)iCurrentGamemodeSelection].firstMaps);

            Current_GamemodesUI[(int)iCurrentGamemodeSelection].potentialMaps.SetActive(false);
            Current_GamemodesUI[newMapGroup].potentialMaps.SetActive(true);

            iCurrentGamemodeSelection = (GAMEMODE_INDEX)newMapGroup;
            SelectedGamemode = Current_GamemodesUI[(int)iCurrentGamemodeSelection];

            //Resetting the maps for this group
            BaseEventData baseEvent = new BaseEventData(eventSystem);
            Current_GamemodesUI[(int)iCurrentGamemodeSelection].firstMaps.OnSubmit(baseEvent);

            GenerateMutators();
        }
        else if (Debug.isDebugBuild)
        {
            Debug.Log("This map isnt set: " + ((MAP_INDEX)newMapGroup).ToString(), this);
        }
    }

    private void GenerateMutators()
    {
        //Change the gamemode mutators section
        MutatorUI.UpdateGamemodeMutators(iCurrentGamemodeSelection);

        //Change the map mutators section
        MutatorUI.UpdateMapMutators(iCurrentMapSelection);
    }

    #endregion
}
