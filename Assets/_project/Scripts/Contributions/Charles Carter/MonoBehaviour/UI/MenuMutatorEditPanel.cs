////////////////////////////////////////////////////////////
// File: MenuMutatorEditPanel
// Author: Charles Carter
// Date Created: 03/02/21
// Brief: This is the panel that the users use to edit the mutator values
//////////////////////////////////////////////////////////// 

//This script uses these namespaces
using System.Collections.Generic;
using UnityEngine;

public class MenuMutatorEditPanel : MonoBehaviour {
    #region Public Variables

    public MutatorPackager currentMutators;

    #endregion

    #region Private Variables

    [SerializeField]
    private List<GameObject> GamemodePanels;
    [SerializeField]
    private GameObject gamemodeTitle;

    #endregion

    #region Unity Methods

    //When the panel is activated
    private void OnEnable() {
        //Turn on the right mutator edit gameobjects
        if (currentMutators && GamemodePanels[(int)currentMutators.Gamemode]) {
            if (currentMutators.Gamemode != GAMEMODE_INDEX.CLASSIC && gamemodeTitle) {
                gamemodeTitle.SetActive(true);
            }

            GamemodePanels[(int)currentMutators.Gamemode].SetActive(true);
        }
    }

    //When the panel is unactived
    private void OnDisable() {
        //Turn off the right mutator edit gameobjects ready for next time
        if (currentMutators && GamemodePanels[(int)currentMutators.Gamemode]) {
            if (gamemodeTitle) {
                gamemodeTitle.SetActive(false);
            }

            GamemodePanels[(int)currentMutators.Gamemode].SetActive(false);
        }
    }

    #endregion
}
