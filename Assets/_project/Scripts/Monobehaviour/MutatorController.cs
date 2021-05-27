////////////////////////////////////////////////////////////
// File: MutatorController
// Author: Charles Carter
// Date Created: 11/03/21
// Brief: This script is to control the mutators not covered in other scripts
//////////////////////////////////////////////////////////// 

using UnityEngine;

public class MutatorController : MonoBehaviour
{
    #region Variables

    [SerializeField]
    private RoundManager rManager;
    [SerializeField]
    private GameSettingsContainer settings;

    #endregion

    #region Unity Functions

    private void Awake()
    {
        settings = GameSettingsContainer.instance;
    }

    private void Start()
    {
        rManager = RoundManager.roundManager;

        if (settings)
        {

        }
    }

    #endregion
}
