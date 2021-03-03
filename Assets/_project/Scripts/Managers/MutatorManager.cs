////////////////////////////////////////////////////////////
// File: MutatorManager
// Author: Charles Carter
// Date Created: 18/02/21
// Brief: This is going to keep track of the current selected options for the game
//////////////////////////////////////////////////////////// 

using System.Collections.Generic;
using UnityEngine;

//A more concise version of MutatorUI to be sent over scenes/ networks with less information
[System.Serializable]
public struct PackagedMutator
{
    string name;
    int ID;

    Mutator_Value_Type mutatorValue;
    float value;
}

public class MutatorManager : MonoBehaviour
{
    #region Public Fields

    public List<MutatorUI> currentGmodeMutatorList;
    public List<MutatorUI> currentMapMutatorList;

    #endregion

    #region Unity Methods

    void Start()
    {
	
    }
 
    void Update()
    {
	
    }

    #endregion

    #region Public Methods

    public void SetGModeMutatorList(List<MutatorUI> newMutatorList)
    {
        currentGmodeMutatorList = newMutatorList;
    }

    public List<MutatorUI> GetGModeMutatorList()
    {
        return currentGmodeMutatorList;
    }

    public void SetMapMutatorList(List<MutatorUI> newMutatorList)
    {
        currentMapMutatorList = newMutatorList;
    }

    public List<MutatorUI> GetMapMutatorList()
    {
        return currentMapMutatorList;
    }

    #endregion

    #region Private Methods
    #endregion
}
