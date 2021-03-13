////////////////////////////////////////////////////////////
// File: MutatorManager
// Author: Charles Carter
// Date Created: 18/02/21
// Brief: This is going to keep track of the current selected options for the game
//////////////////////////////////////////////////////////// 

using System.Collections.Generic;
using UnityEngine;

#region Mutator Classes

//A more concise version of MutatorUI to be sent over scenes/ networks with less information
[System.Serializable]
public class PackagedMutator
{
    public PackagedMutator(string n, int id, Mutator_Value_Type type, float v)
    {
        name = n;
        ID = id;
        value_type = type;
        value = v;
    }

    public string name;
    public int ID;

    public Mutator_Value_Type value_type;
    public float value;
}

#endregion

public class MutatorManager : MonoBehaviour
{
    #region Public Fields

    //A reference to the current gamemode selected
    public GAMEMODE_INDEX Gamemode = GAMEMODE_INDEX.CLASSIC;

    public List<MutatorUI> currentGeneralMutatorList = new List<MutatorUI>();

    //The mutators being used currently
    public List<MutatorUI> currentGmodeMutatorList = new List<MutatorUI>();
    public List<MutatorUI> currentMapMutatorList = new List<MutatorUI>();

    //The mutators list that will collate all the changed mutator options
    public List<PackagedMutator> packagedGenMutators { private set; get; }
    public List<PackagedMutator> packagedGmodeMutators { private set; get; }
    public List<PackagedMutator> packagedMapMutators { private set; get; }

    #endregion

    #region Private Variables


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

    //Saving And Loading the current mutators for the game to use
    public void MakeChangedMutatorArrays(bool bLocalPlay)
    {
        //If the game is just for local play
        if (bLocalPlay)
        {

        }
        //If the game is over a network
        else
        {

        }
    }

    #endregion

    #region Private Methods

    //Getting an array of packaged mutators from each current mutatorUI list
    private PackagedMutator[] packagedMutatorsForGen()
    {
        foreach (MutatorUI mut in currentGeneralMutatorList)
        {
            if (!mut.isDefaultValue)
            {
                packagedGenMutators.Add(ConvertingMutatorUIToPackage(mut));
            }
        }

        return packagedGenMutators.ToArray();
    }

    private PackagedMutator[] packagedMutatorsForGMode()
    {
        foreach (MutatorUI mut in currentGmodeMutatorList)
        {
            if (!mut.isDefaultValue)
            {
                packagedGmodeMutators.Add(ConvertingMutatorUIToPackage(mut));
            }
        }

        return packagedGmodeMutators.ToArray();
    }

    private PackagedMutator[] packagedMutatorsForMap()
    {
        foreach (MutatorUI mut in currentMapMutatorList)
        {
            if (!mut.isDefaultValue)
            {
                packagedMapMutators.Add(ConvertingMutatorUIToPackage(mut));
            }
        }

        return packagedMapMutators.ToArray();
    }

    //Making a packaged mutator from a mutator UI
    private PackagedMutator ConvertingMutatorUIToPackage(MutatorUI mutatorToConvert)
    {
        PackagedMutator returnMutator = new PackagedMutator(mutatorToConvert.name, mutatorToConvert.ID, mutatorToConvert.mType, mutatorToConvert.value);
        return returnMutator;
    }

    #endregion
}
