////////////////////////////////////////////////////////////
// File: MutatorEditUI
// Author: Charles Carter
// Date Created: 04/03/21
// Brief: A script to hold the functionlity in the UI for changing the value of mutators
//////////////////////////////////////////////////////////// 

using TMPro;
using UnityEngine;

internal enum MutatorList
{
    GEN,
    GAM,
    MAP
}

public class MutatorEditUI : MonoBehaviour
{
    #region Public Variables

    //These are necessary to be correct
    [Header("Needed Variables")]
    [SerializeField]
    private MutatorList ListToGoInto;
    public GAMEMODE_INDEX gamemode;
    public MAP_INDEX map;
    public int mutatorID;

    #endregion

    #region Private Variables
    
    [SerializeField]
    private MenuMutatorUI MenuMutatorUI;
    private MutatorUI thisMutator;

    //Displaying on the stored value when it changes
    [SerializeField]
    TextMeshProUGUI valueText;

    [Header("Button UI Variables")]
    [SerializeField]
    private float StoredValue;
    [SerializeField]
    private float fMinimumVal;
    [SerializeField]
    private float fMaximumVal;

    #endregion

    #region Unity Methods

    private void Start()
    {
        //Relies on Awake in this script to run first
        if (MenuMutatorUI)
        {
            switch (ListToGoInto)
            {
                case MutatorList.GEN:
                    thisMutator = MenuMutatorUI.GetMutatorValue(mutatorID);
                    break;
                case MutatorList.GAM:
                    thisMutator = MenuMutatorUI.GetMutatorValue(mutatorID, gamemode);
                    break;
                case MutatorList.MAP:
                    thisMutator = MenuMutatorUI.GetMutatorValue(mutatorID, map);
                    break;
            }

            StoredValue = thisMutator.value;

            if (valueText)
            {
                MutatorEditPanelDisplayUpdate();
            }
        }
        else
        {
            if (Debug.isDebugBuild)
            {
                Debug.Log("This mutator edit doesnt have a reference to MenuMutatorUI", this);
            }
        }
    }

    #endregion

    #region Public Methods

    //This is a toggle based mutator
    public void MutatorBoolChange(bool newValue)
    {
        float val = 0f;

        if (newValue)
        {
            val = 1f;
        }

        switch (ListToGoInto)
        {
            case MutatorList.GEN:
                MenuMutatorUI.SetMutatorValue(mutatorID, val);
                break;
            case MutatorList.GAM:
                MenuMutatorUI.SetMutatorValue(mutatorID, gamemode, val);
                break;
            case MutatorList.MAP:
                MenuMutatorUI.SetMutatorValue(mutatorID, map, val);
                break;
        }

        MutatorEditPanelDisplayUpdate();
    }

    //This is a slider based mutator
    public void MutatorSliderChange(float newValue)
    {
        switch (ListToGoInto)
        {
            case MutatorList.GEN:
                MenuMutatorUI.SetMutatorValue(mutatorID, newValue);
                break;
            case MutatorList.GAM:
                MenuMutatorUI.SetMutatorValue(mutatorID, gamemode, newValue);
                break;
            case MutatorList.MAP:
                MenuMutatorUI.SetMutatorValue(mutatorID, map, newValue);
                break;
        }

        MutatorEditPanelDisplayUpdate();
    }

    //The buttons are slightly more difficult, one is to increase and one is to decrease
    public void MutatorIncreaseButton(float valueToIncreaseBy)
    {
        if (StoredValue < fMaximumVal)
        {
            StoredValue += valueToIncreaseBy;
            MutatorEditPanelDisplayUpdate();
        }
    }

    public void MutatorDecreaseButton(float valueToDecreaseBy)
    {
        if (StoredValue > fMinimumVal)
        {
            StoredValue -= valueToDecreaseBy;
            MutatorEditPanelDisplayUpdate();
        }
    }

    #endregion

    #region Private Methods

    private void MutatorEditPanelDisplayUpdate()
    {
        if (valueText)
        {
            valueText.text = GetStringBasedOnMutator();
        }
    }

    private string GetStringBasedOnMutator()
    {
        return MenuMutatorUI.processMutatorValueToText(thisMutator);
    }

    #endregion
}
