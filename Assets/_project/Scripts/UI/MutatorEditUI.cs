////////////////////////////////////////////////////////////
// File: MutatorEditUI
// Author: Charles Carter
// Date Created: 04/03/21
// Brief: A script to hold the functionlity in the UI for changing the value of mutators
//////////////////////////////////////////////////////////// 

using TMPro;
using UnityEngine;

public class MutatorEditUI : MonoBehaviour
{
    #region Public Variables

    //These are necessary to be correct
    [Header("Needed Variables")]
    public GAMEMODE_INDEX gamemode;
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

    private void Awake()
    {
        if (MenuMutatorUI)
        {
            thisMutator = MenuMutatorUI.GetMutatorValue(mutatorID, gamemode);
        }
    }

    private void Start()
    {
        if (MenuMutatorUI)
        {
            StoredValue = MenuMutatorUI.GetMutatorValue(mutatorID, gamemode).value;
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
        if (newValue)
        {
            MenuMutatorUI.UpdateMutatorValue(mutatorID, gamemode, 1f);
        }
        else
        {
            MenuMutatorUI.UpdateMutatorValue(mutatorID, gamemode, 0f);
        }

        MutatorEditPanelDisplayUpdate();
    }

    //This is a slider based mutator
    public void MutatorSliderChange(float newValue)
    {
        MenuMutatorUI.UpdateMutatorValue(mutatorID, gamemode, newValue);
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
