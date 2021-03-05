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
public class PackagedMutator
{
    public string name;
    public int ID;
    public float value;
}

public class MutatorManager : MonoBehaviour
{
    #region Public Fields

    //A reference to the current gamemode selected
    public GAMEMODE_INDEX Gamemode;

    //The mutators being used currently
    public List<MutatorUI> currentGmodeMutatorList = new List<MutatorUI>();
    public List<MutatorUI> currentMapMutatorList = new List<MutatorUI>();

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

    //Setting the mutator list currently in use
    public void SetGModeMutatorList(List<MutatorUI> newMutatorList)
    {
        currentGmodeMutatorList = newMutatorList;
    }

    //Getting the current mutator list
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
