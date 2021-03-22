////////////////////////////////////////////////////////////
// File: GameCreationSettings
// Author: Charles Carter
// Date Created: 16/02/21
// Brief: The settings for creating a game in the practice menu on the UI
//////////////////////////////////////////////////////////// 

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

    public List<MutatorUI> GeneralMutators;
    public List<MutatorUI> MapMutators;

    #endregion

    #region Inspector Fields

    [Header("Needed Variables")]
    [SerializeField]
    private MenuMutatorUI MutatorUI;
    [SerializeField]
    private GAMEMODE_INDEX iCurrentGamemodeSelection = GAMEMODE_INDEX.CLASSIC;
    [SerializeField]
    private MAP_INDEX iCurrentMapSelection = MAP_INDEX.STUDIO;
    private bool bLocalPlayerSettings;
    [SerializeField]
    private EventSystem eventSystem;
    [SerializeField]
    GameObject GamemodeMutatorsTextParent;

    #endregion

    #region Public Methods

    public void SetLocalPlay(bool bLocalPlayButtonPressed)
    {
        bLocalPlayerSettings = bLocalPlayButtonPressed;
        UpdateMapGroup(0);

        //Change the map mutators section
        MutatorUI.UpdateMapMutators(iCurrentMapSelection);
    }

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
            //Debug.Log("Map Chosen is: " + iCurrentMapSelection.ToString());
            //Debug.Log("Gamemode Chosen is: " + iCurrentGamemodeSelection.ToString());
            
            foreach (MutatorUI mutator in SelectedGamemode.GamemodeMutators)
            {
                if (!mutator.isDefaultValue)
                {
                    Debug.Log(mutator.name + " value changed to: " + mutator.value.ToString());
                }
            }
        }
     
        //Telling the mutator packager to package all the current mutators
        MutatorPackager.instance.MakeChangedMutatorArrays(bLocalPlayerSettings, GeneralMutators, SelectedGamemode.GamemodeMutators, MapMutators);

        //Loading the right map, other scripts in scene will make the relevant changes based on gamemode then mutators
        SceneManager.LoadScene("Studio");
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

            //Changing the maps shown
            Current_GamemodesUI[(int)iCurrentGamemodeSelection].potentialMaps.SetActive(false);
            Current_GamemodesUI[newMapGroup].potentialMaps.SetActive(true);

            //Updating the gamemode values
            iCurrentGamemodeSelection = (GAMEMODE_INDEX)newMapGroup;
            SelectedGamemode = Current_GamemodesUI[(int)iCurrentGamemodeSelection];

            //Resetting the toggles for this group
            BaseEventData baseEvent = new BaseEventData(eventSystem);
            Current_GamemodesUI[(int)iCurrentGamemodeSelection].firstMaps.OnSubmit(baseEvent);

            //Showing the mutators
            GenerateMutators();
        }
        else if (Debug.isDebugBuild)
        {
            Debug.Log("This map isnt set: " + ((MAP_INDEX)newMapGroup).ToString(), this);
        }

        //Classic doesnt have gamemode mutators
        if (iCurrentGamemodeSelection == GAMEMODE_INDEX.CLASSIC && GamemodeMutatorsTextParent)
        {
            GamemodeMutatorsTextParent.SetActive(false);
        }
        //Where as every other gamemode does
        else if (iCurrentGamemodeSelection != GAMEMODE_INDEX.CLASSIC && GamemodeMutatorsTextParent)
        {
            GamemodeMutatorsTextParent.SetActive(true);
        }
    }

    private void GenerateMutators()
    {
        //Change the gamemode mutators section
        MutatorUI.ShowGamemode(iCurrentGamemodeSelection);
    }

    #endregion
}
