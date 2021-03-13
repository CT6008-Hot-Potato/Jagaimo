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
[Serializable]
public class MutatorUI
{
    //The text to change on the edit panel and display panel
    public TextMeshProUGUI TextToChange;

    //Mutator Identifiers
    public string name;
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

    public MutatorManager currentMutators;
    public GameCreationSettings creationSettings;

    #endregion

    #region Private Variables

    [Header("Variables Needed")]
    [SerializeField]
    private GameObject GModeMutatorTextParent;

    //Each gamemode has a stached list of texts
    [SerializeField]
    private List<GameObject> GamemodeMutatorsTexts;

    //Each Map also has a stached list of texts
    [SerializeField]
    private List<GameObject> MapMutatorsTexts;

    //The specific mutator lists per gamemode
    [SerializeField]
    private List<MutatorUI> ClassicMutators = new List<MutatorUI>();
    [SerializeField]
    private List<MutatorUI> InfectedMutators = new List<MutatorUI>();
    [SerializeField]
    private List<MutatorUI> FootballMutators = new List<MutatorUI>();
    [SerializeField]
    private List<MutatorUI> SabotageMutators = new List<MutatorUI>();

    //The panel in which the player can edit the mutators
    [SerializeField]
    private GameObject MutatorsEditPanel;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        creationSettings.Current_GamemodesUI[(int)GAMEMODE_INDEX.CLASSIC].GamemodeMutators = ClassicMutators;
        creationSettings.Current_GamemodesUI[(int)GAMEMODE_INDEX.INFECTED].GamemodeMutators = InfectedMutators;
        creationSettings.Current_GamemodesUI[(int)GAMEMODE_INDEX.FOOTBALL].GamemodeMutators = FootballMutators;
        creationSettings.Current_GamemodesUI[(int)GAMEMODE_INDEX.SABOTAGE].GamemodeMutators = SabotageMutators;

        for (int i = 0; i < (int)GAMEMODE_INDEX.COUNT; ++i)
        {
            UpdateGamemodeMutators((GAMEMODE_INDEX)i);
            UpdateGamemodeMutatorsText();
        }
    }

    #endregion

    #region Public Methods

    //Changing what mutators are shown to be able to edit 
    public void UpdateGamemodeMutators(GAMEMODE_INDEX Gamemode)
    {
        GamemodeMutatorsTexts[(int)currentMutators.Gamemode].SetActive(false);

        currentMutators.SetGModeMutatorList(creationSettings.Current_GamemodesUI[(int)Gamemode].GamemodeMutators);
        currentMutators.Gamemode = Gamemode;

        GamemodeMutatorsTexts[(int)Gamemode].SetActive(true);
    }

    public void UpdateMapMutators(MAP_INDEX Map)
    {

    }

    public void UpdateGamemodeMutatorsText()
    {
        //Go through the gmode mutator list to set all of the values
        for (int i = 0; i < currentMutators.currentGmodeMutatorList.Count; ++i)
        {
            //Set the text correctly if there is a text
            if (currentMutators.currentGmodeMutatorList[i].TextToChange)
            {
                currentMutators.currentGmodeMutatorList[i].TextToChange.text = currentMutators.currentGmodeMutatorList[i].name.ToString() + ":" + processMutatorValueToText(currentMutators.currentGmodeMutatorList[i]);
            }
        }
    }

    public void UpdateMutatorValue(int mutatorID, GAMEMODE_INDEX gamemode, float newValue)
    {
        creationSettings.Current_GamemodesUI[(int)gamemode].GamemodeMutators[mutatorID].value = newValue;
        UpdateSpecificGamemodeMutatorText(mutatorID);
    }

    public MutatorUI GetMutatorValue(int mutatorID, GAMEMODE_INDEX gamemode)
    {
        return creationSettings.Current_GamemodesUI[(int)gamemode].GamemodeMutators[mutatorID];
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
                string toStringReturn = String.Format("Value: {0:P2}.", mutatorToProcess.value);
                return toStringReturn;
            default:
                return mutatorToProcess.value.ToString();
        }
    }

    #endregion

    #region Private Methods

    //Updating the text for each mutator
    private void UpdateSpecificGamemodeMutatorText(int mutatorID)
    {
        //Set the text correctly if there is a text
        if (currentMutators.currentGmodeMutatorList[mutatorID].TextToChange)
        {
            currentMutators.currentGmodeMutatorList[mutatorID].TextToChange.text = (
            currentMutators.currentGmodeMutatorList[mutatorID].name + ": "
            + processMutatorValueToText(currentMutators.currentGmodeMutatorList[mutatorID]));
        }
    }

    #endregion
}
