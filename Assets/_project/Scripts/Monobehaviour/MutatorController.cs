////////////////////////////////////////////////////////////
// File: MutatorController
// Author: Charles Carter
// Date Created: 11/03/21
// Brief: This script is to control the mutators not covered by other scripts
//////////////////////////////////////////////////////////// 

using UnityEngine;

public class MutatorController : MonoBehaviour
{
    #region Variables

    IGamemode current_gamemode;

    [SerializeField]
    private GameSettingsContainer settings;

    #endregion

    #region Public Methods

   

    #endregion

    #region Unity Functions

    private void Awake()
    {
        settings = GameSettingsContainer.instance;
    }

    private void Start()
    {
        
    }

    #endregion
}
