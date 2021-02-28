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
    private int iCurrentGamemodeSelection = 0;
    [SerializeField]
    private MAP_INDEX iCurrentMapSelection = 0;

    [SerializeField]
    private ToggleGroup GamemodeGroup;

    // 0 - Classic
    // 1 - Infected
    // 2 - Football
    // 3 - Sabotage

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
        if (potentialMaps[newMapGroup] && firstMaps[iCurrentGamemodeSelection] && MapsGroup[iCurrentGamemodeSelection])
        {
            MapsGroup[iCurrentGamemodeSelection].NotifyToggleOn(firstMaps[iCurrentGamemodeSelection]);

            potentialMaps[iCurrentGamemodeSelection].SetActive(false);
            potentialMaps[newMapGroup].SetActive(true);

            iCurrentGamemodeSelection = newMapGroup;

            BaseEventData baseEvent = new BaseEventData(eventSystem);
            firstMaps[iCurrentGamemodeSelection].OnSubmit(baseEvent);
        }
        else if (Debug.isDebugBuild)
        {
            Debug.Log("This map isnt set: " + newMapGroup, this);
        }
    }

    private void GenerateMutators()
    {
        //Change the gamemode mutators section

        //Change the map mutators section
    }

    #endregion
}
