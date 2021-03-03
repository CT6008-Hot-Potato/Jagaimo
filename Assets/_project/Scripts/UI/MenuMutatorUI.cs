////////////////////////////////////////////////////////////
// File: MenuMutatorUI
// Author: Charles Carter
// Date Created: 25/02/21
// Brief: The UI in the main menu to show what mutators the player can change based on the selected options
//////////////////////////////////////////////////////////// 

using UnityEngine;
using TMPro;
using System.Collections.Generic;

//An enum value to be stored in each mutator to show how the value should be stored/used
public enum Mutator_Value_Type
{
    BOOL,
    FLOAT,
    INT
}

//The mutators themselves are these
[System.Serializable]
public struct MutatorUI
{
    public TextMeshProUGUI TextToChange;
    public string name;
    public int ID;

    public Mutator_Value_Type mType;

    public bool isDefaultValue
    {
        get => default_value == value;
    }

    public float default_value;
    public float value;
}

public class MenuMutatorUI : MonoBehaviour
{
    #region Public Varialbes

    public MutatorManager currentMutators;

    #endregion

    #region Private Variables

    [SerializeField]
    private GameObject GModeMutatorTextParent;

    //Each gamemode has a stached list of texts
    [SerializeField]
    private List<GameObject> GamemodeMutatorsTexts;

    //Each Map also has a stached list of texts
    [SerializeField]
    private List<GameObject> MapMutatorsTexts;

    //Determining whether the text for gamemode mutators needs to be shown
    [SerializeField]
    private bool isClassicGamemode = true;

    //All of the mutators in this class
    private List<List<MutatorUI>> AllGamemodeMutators;

    //The specific mutator lists per gamemode
    [SerializeField]
    private List<MutatorUI> InfectedMutators;
    [SerializeField]
    private List<MutatorUI> FootballMutators;
    [SerializeField]
    private List<MutatorUI> SabotageMutators;

    //A reference of the current gamemode
    GAMEMODE_INDEX current_gamemode;

    //The panel in which the player can edit the mutators
    [SerializeField]
    private GameObject MutatorsEditPanel;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        AllGamemodeMutators.Add(InfectedMutators);
        AllGamemodeMutators.Add(FootballMutators);
        AllGamemodeMutators.Add(SabotageMutators);
    }

    void Start()
    {
	
    }
 
    void Update()
    {
	
    }

    #endregion

    #region Public Methods

    //Changing what mutators are shown to be able to edit 
    public void UpdateGamemodeMutators(GAMEMODE_INDEX Gamemode)
    {
        GamemodeMutatorsTexts[(int)current_gamemode].SetActive(false);

        currentMutators.SetGModeMutatorList(AllGamemodeMutators[(int)Gamemode]);
        current_gamemode = Gamemode;

        GamemodeMutatorsTexts[(int)current_gamemode].SetActive(true);
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

    //Opening the panel which should block off other options
    public void OpenEditMutatorsPanel()
    {
        MutatorsEditPanel.SetActive(true);
    }

    #endregion

    #region Private Methods

    //A function to return a string based on the value of the inputted mutator
    private string processMutatorValueToText(MutatorUI mutatorToProcess)
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

            default:
                return mutatorToProcess.value.ToString();
        }
    }

    #endregion
}
