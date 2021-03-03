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

public enum GAMEMODE_INDEX
{
    CLASSIC = 0,
    INFECTED = 1,
    FOOTBALL = 2,
    SABOTAGE = 3
}

public enum MAP_INDEX
{
    STUDIO = 0,
    STADIUM = 1
}

public class GameCreationSettings : MonoBehaviour
{
    #region Inspector Fields

    [Header("Needed Variables")]
    [SerializeField]
    MenuMutatorUI MutatorUI;

    [SerializeField]
    private GAMEMODE_INDEX iCurrentGamemodeSelection = 0;
    [SerializeField]
    private MAP_INDEX iCurrentMapSelection = 0;

    [SerializeField]
    private ToggleGroup GamemodeGroup;

    //All of the parents of the maps
    [SerializeField]
    private List<GameObject> potentialMaps;

    //All of the parents of the maps
    [SerializeField]
    private List<ToggleGroup> MapsGroup;

    //First map of each toggle group
    [SerializeField]
    private List<Toggle> firstMaps;

    [SerializeField]
    private EventSystem eventSystem;

    #endregion

    #region Unity Methods

    void Start()
    {
	    
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
        if (potentialMaps[newMapGroup] && firstMaps[(int)iCurrentGamemodeSelection] && MapsGroup[(int)iCurrentGamemodeSelection])
        {
            MapsGroup[(int)iCurrentGamemodeSelection].NotifyToggleOn(firstMaps[(int)iCurrentGamemodeSelection]);

            potentialMaps[(int)iCurrentGamemodeSelection].SetActive(false);
            potentialMaps[newMapGroup].SetActive(true);

            iCurrentGamemodeSelection = (GAMEMODE_INDEX)newMapGroup;

            //Resetting the maps for this group
            BaseEventData baseEvent = new BaseEventData(eventSystem);
            firstMaps[(int)iCurrentGamemodeSelection].OnSubmit(baseEvent);
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
