////////////////////////////////////////////////////////////
// File: MenuMutatorUI
// Author: Charles Carter
// Date Created: 25/02/21
// Brief: The UI in the main menu to show what mutators the player can change based on the selected options
//////////////////////////////////////////////////////////// 

using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System;

//An enum value to be stored in each mutator to show how the value should be stored/used
public enum Mutator_Value_Type
{
    BOOL,
    FLOAT,
    INT,
    PERCENTAGE
}

//The mutators themselves are these
[System.Serializable]
public class MutatorUI
{
    public string name;

    //The text to change on the edit panel and display panel
    public TextMeshProUGUI TextToChange;

    //Mutator Identifier`s
    public int ID;

    //The mutator value type, for displaying
    public Mutator_Value_Type mType;

    //Knowing whether to save/send the value
    public bool isDefaultValue
    {
        get => default_value == value;
    }

    //Mutator values
    public float default_value;
    public float value;
}

public class MenuMutatorUI : MonoBehaviour
{
    #region Public Varialbes

    public MutatorPackager currentMutators;
    public GameCreationSettings creationSettings;

    #endregion

    #region Private Variables

    [Header("Variables Needed")]
    //Each gamemode has a stached list of texts
    [SerializeField]
    private List<GameObject> GamemodeMutatorsTexts;

    [Header("Mutator UIs")]

    //Each list of mutators is stored here
    [SerializeField]
    private List<MutatorUI> GeneralMutators = new List<MutatorUI>();

    //The specific mutator lists per gamemode
    [SerializeField]
    private List<MutatorUI> ClassicMutators = new List<MutatorUI>();
    [SerializeField]
    private List<MutatorUI> InfectedMutators = new List<MutatorUI>();
    [SerializeField]
    private List<MutatorUI> FootballMutators = new List<MutatorUI>();
    [SerializeField]
    private List<MutatorUI> SabotageMutators = new List<MutatorUI>();

    [SerializeField]
    private List<MutatorUI> MapMutators = new List<MutatorUI>();

    //The panel in which the player can edit the mutators
    [SerializeField]
    private GameObject MutatorsEditPanel;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        UpdateAllMutatorValues();

        creationSettings.SelectedGamemode = creationSettings.Current_GamemodesUI[0];

        UpdateAllMutatorTexts();
    }

    #endregion

    #region Public Methods

    //Updating all of the gamemode mutator texts
    public void UpdateAllGeneralMutatorTexts()
    {
        for (int i = 0; i < GeneralMutators.Count; ++i)
        {
            UpdateGeneralMutatorText(i);
        }
    }

    public void UpdateMapMutators(MAP_INDEX Map)
    {
        for (int i = 0; i < MapMutators.Count; ++i)
        {
            UpdateMapMutatorText(i);
        }
    }

    //Showing a different gamemode
    public void ShowGamemode(GAMEMODE_INDEX index)
    {
        if (GamemodeMutatorsTexts[(int)index])
        {
            GamemodeMutatorsTexts[(int)currentMutators.Gamemode].SetActive(false);
            currentMutators.Gamemode = index;
            GamemodeMutatorsTexts[(int)currentMutators.Gamemode].SetActive(true);
        }
    }

    //Setting mutator values to the respective lists
    public void SetMutatorValue(int mutatorID, float newValue)
    {
        if (creationSettings)
        {
            creationSettings.GeneralMutators[mutatorID].value = newValue;
            UpdateGeneralMutatorText(mutatorID);
        }
    }

    public void SetMutatorValue(int mutatorID, GAMEMODE_INDEX gamemode, float newValue)
    {
        if (creationSettings && currentMutators)
        {
            creationSettings.Current_GamemodesUI[(int)gamemode].GamemodeMutators[mutatorID].value = newValue;
            UpdateSpecificGamemodeMutatorText(mutatorID);
        }
    }

    public void SetMutatorValue(int mutatorID, MAP_INDEX Map, float newValue)
    {
        if (creationSettings)
        {
            creationSettings.MapMutators[mutatorID].value = newValue;
            UpdateSpecificGamemodeMutatorText(mutatorID);
        }
    }

    //Getting mutator values from the respective lists
    public MutatorUI GetMutatorValue(int mutatorID, GAMEMODE_INDEX gamemode)
    {
        return creationSettings.Current_GamemodesUI[(int)gamemode].GamemodeMutators[mutatorID];
    }

    public MutatorUI GetMutatorValue(int mutatorID)
    {
        return creationSettings.GeneralMutators[mutatorID];
    }
    public MutatorUI GetMutatorValue(int mutatorID, MAP_INDEX map)
    {
        return creationSettings.MapMutators[mutatorID];
    }

    //Opening the panel which should block off other options
    public void OpenEditMutatorsPanel()
    {
        MutatorsEditPanel.SetActive(true);
    }

    //Closing the panel which should update the options
    public void CloseEditMutatorsPanel()
    {
        MutatorsEditPanel.SetActive(false);
    }

    public void DefaultMutators()
    {

    }

    //A function to return a string based on the value of the inputted mutator
    public string processMutatorValueToText(MutatorUI mutatorToProcess)
    {
        //The mutator knows what type of value it is
        switch (mutatorToProcess.mType)
        {
            case Mutator_Value_Type.BOOL:
                if (mutatorToProcess.value == 0)
                {
                    return "false";
                }
                else
                {
                    return "true";
                }
            case Mutator_Value_Type.FLOAT:
                return mutatorToProcess.value.ToString();
            case Mutator_Value_Type.INT:
                int toReturn = (int)mutatorToProcess.value;
                return toReturn.ToString();
            case Mutator_Value_Type.PERCENTAGE:
                string toStringReturn = String.Format("{0:P0}", mutatorToProcess.value);
                return toStringReturn;
            default:
                return mutatorToProcess.value.ToString();
        }
    }

    #endregion

    #region Private Methods

    private void UpdateAllMutatorValues()
    {
        creationSettings.GeneralMutators = GeneralMutators;

        creationSettings.Current_GamemodesUI[(int)GAMEMODE_INDEX.CLASSIC].GamemodeMutators = ClassicMutators;
        creationSettings.Current_GamemodesUI[(int)GAMEMODE_INDEX.INFECTED].GamemodeMutators = InfectedMutators;
        creationSettings.Current_GamemodesUI[(int)GAMEMODE_INDEX.FOOTBALL].GamemodeMutators = FootballMutators;
        creationSettings.Current_GamemodesUI[(int)GAMEMODE_INDEX.SABOTAGE].GamemodeMutators = SabotageMutators;

        creationSettings.MapMutators = MapMutators;
    }

    private void UpdateAllMutatorTexts()
    {
        UpdateAllGeneralMutatorTexts();
        UpdateAllGamemodeMutatorTexts();
        UpdateMapMutators(MAP_INDEX.STUDIO);
    }

    //Updating all of the texts for all gamemodes
    private void UpdateAllGamemodeMutatorTexts()
    {
        //Going through all the gamemodes
        for (int j = 0; j < creationSettings.Current_GamemodesUI.Length; ++j)
        {
            //This one is temp selected
            creationSettings.SelectedGamemode = creationSettings.Current_GamemodesUI[j];

            //Going through the selected gamemode and updating the texts
            for (int i = 0; i < creationSettings.SelectedGamemode.GamemodeMutators.Count; ++i)
            {
                UpdateSpecificGamemodeMutatorText(i);
            }
        }
    }

    //Updating the text for each mutator (these 3 functions below can be merged into 1)
    private void UpdateGeneralMutatorText(int mutatorID)
    {
        //Set the text correctly if there is a text
        if (creationSettings.GeneralMutators[mutatorID].TextToChange)
        {
            creationSettings.GeneralMutators[mutatorID].TextToChange.text = (
            creationSettings.GeneralMutators[mutatorID].name + ": "
            + processMutatorValueToText(creationSettings.GeneralMutators[mutatorID]));
        }
    }

    private void UpdateSpecificGamemodeMutatorText(int mutatorID)
    {
        //Set the text correctly if there is a text
        if (creationSettings.SelectedGamemode.GamemodeMutators[mutatorID].TextToChange)
        {
            creationSettings.SelectedGamemode.GamemodeMutators[mutatorID].TextToChange.text = (
            creationSettings.SelectedGamemode.GamemodeMutators[mutatorID].name + ": "
            + processMutatorValueToText(creationSettings.SelectedGamemode.GamemodeMutators[mutatorID]));
        }
    }

    private void UpdateMapMutatorText(int mutatorID)
    {
        //Set the text correctly if there is a text
        if (creationSettings.MapMutators[mutatorID].TextToChange)
        {
            creationSettings.MapMutators[mutatorID].TextToChange.text = (
            creationSettings.MapMutators[mutatorID].name + ": "
            + processMutatorValueToText(creationSettings.MapMutators[mutatorID]));
        }
    }

    #endregion
}
