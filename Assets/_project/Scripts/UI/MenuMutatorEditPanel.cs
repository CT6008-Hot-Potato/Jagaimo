////////////////////////////////////////////////////////////
// File: MenuMutatorEditPanel
// Author: Charles Carter
// Date Created: 03/02/21
// Brief: This is the panel that the users use to edit the mutator values
//////////////////////////////////////////////////////////// 

using System.Collections.Generic;
using UnityEngine;

public class MenuMutatorEditPanel : MonoBehaviour
{
    #region Public Varialbes

    public MutatorManager currentMutators;
    public MenuMutatorUI mutatorUIs;

    #endregion

    #region Private Variables

    [SerializeField]
    private List<GameObject> GamemodePanels;

    #endregion

    #region Unity Methods

    //When the panel is activated
    private void OnEnable()
    {
        //Turn on the right mutator edit gameobjects
        if (currentMutators && currentMutators.Gamemode != GAMEMODE_INDEX.CLASSIC && GamemodePanels[(int)currentMutators.Gamemode])
        {
            GamemodePanels[(int)currentMutators.Gamemode].SetActive(true);
        }
    }

    //When the panel is unactived
    private void OnDisable()
    {
        //Turn off the right mutator edit gameobjects ready for next time
        if (currentMutators && currentMutators.Gamemode != GAMEMODE_INDEX.CLASSIC && GamemodePanels[(int)currentMutators.Gamemode])
        {
            GamemodePanels[(int)currentMutators.Gamemode].SetActive(false);
        }
    }

    #endregion

    #region Public Methods

    //Infected Edit Panel Function
    public void InfectedSpeed(int newSpeed)
    {
        mutatorUIs.UpdateMutatorValue(0, GAMEMODE_INDEX.INFECTED, newSpeed);
    }

    public void SurvivorSpeed(int newSpeed)
    {

    }

    public void InfectedCount(int newCount)
    {

    }

    #endregion
}
