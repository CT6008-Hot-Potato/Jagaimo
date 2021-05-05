////////////////////////////////////////////////////////////
// File: MutatorController
// Author: Charles Carter
// Date Created: 11/03/21
// Brief: The script that ends up storing the mutators in game, to be grabbed by other scripts
//////////////////////////////////////////////////////////// 

using UnityEngine;

public class MutatorController : MonoBehaviour
{
    #region Variables

    IGamemode current_gamemode;
    PackagedMutator mut;
    #endregion

    #region Public Methods

    //Getting the value of a general mutator
    public void FindGeneralMutatorValue(int index)
    {

    }

    //Getting the value of a gamemode mutator
    public void FindGamemodeMutatorValue(int index)
    {
        if (current_gamemode != null)
        {

        }
    }

    #endregion

    #region Unity Functions

    private void Awake()
    {
        
    }

    #endregion
}
