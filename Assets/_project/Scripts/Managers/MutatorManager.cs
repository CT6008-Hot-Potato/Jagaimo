////////////////////////////////////////////////////////////
// File: MutatorManager
// Author: Charles Carter
// Date Created: 18/02/21
// Brief: This is going to keep track of everything going on with the mutators in the menu, and convert them to a packaged version
//////////////////////////////////////////////////////////// 

using System.Collections.Generic;
using UnityEngine;

#region Mutator Classes

//A more concise version of MutatorUI to be sent over scenes/ networks with less information
[System.Serializable]
public class PackagedMutator
{
    public PackagedMutator(string n, int id, object v)
    {
        name = n;
        ID = id;
        value = v;
    }

    public string name;
    public int ID;

    public object value;
}

#endregion

public class MutatorManager : MonoBehaviour
{
    #region Public Fields

    public static MutatorManager instance;

    //A reference to the current gamemode selected
    public GAMEMODE_INDEX Gamemode = GAMEMODE_INDEX.CLASSIC;

    public List<MutatorUI> currentGeneralMutatorList = new List<MutatorUI>();

    //The mutators being used currently
    public List<MutatorUI> currentGmodeMutatorList = new List<MutatorUI>();
    public List<MutatorUI> currentMapMutatorList = new List<MutatorUI>();

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
        PackagedMutator[] genMutators = packagedMutatorsForGen();
        PackagedMutator[] gmodeMutators = packagedMutatorsForGMode();
        PackagedMutator[] mapMutators = packagedMutatorsForMap();

        //If the game is just for local play
        if (bLocalPlay)
        {
            GameSettingsContainer.instance.StoreMutators(genMutators, gmodeMutators, mapMutators);
            GameSettingsContainer.instance.StoreGamemode(Gamemode);
        }
        //If the game is over a network
        else
        {
            Destroy(GameSettingsContainer.instance.gameObject);
        }
    }

    #endregion

    #region Unity Methods

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #endregion

    #region Private Methods

    //Getting an array of packaged mutators from each current mutatorUI list
    private PackagedMutator[] packagedMutatorsForGen()
    {
        //These 3 functions currently follow the same structure, could be optimized
        //Making a temp variable
        List<PackagedMutator> packagedGenMutators = new List<PackagedMutator>();

        //Going through the currently stored mutators
        foreach (MutatorUI mut in currentGeneralMutatorList)
        {
            //Only needs to convert it if it has been changed
            if (!mut.isDefaultValue)
            {
                //Converting it to a packaged version
                packagedGenMutators.Add(ConvertingMutatorUIToPackage(mut));
            }
        }

        //Returning the end list
        return packagedGenMutators.ToArray();
    }

    private PackagedMutator[] packagedMutatorsForGMode()
    {
        List<PackagedMutator> packagedGmodeMutators = new List<PackagedMutator>();

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
        List<PackagedMutator> packagedMapMutators = new List<PackagedMutator>();

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
        switch (mutatorToConvert.mType)
        {
            case Mutator_Value_Type.INT:
                return new PackagedMutator(mutatorToConvert.name, mutatorToConvert.ID, (int)mutatorToConvert.value);
            case Mutator_Value_Type.FLOAT:
                return new PackagedMutator(mutatorToConvert.name, mutatorToConvert.ID, mutatorToConvert.value);
            case Mutator_Value_Type.BOOL:
                if (mutatorToConvert.value == 0)
                {
                    return new PackagedMutator(mutatorToConvert.name, mutatorToConvert.ID, false);
                }
                else
                {
                    return new PackagedMutator(mutatorToConvert.name, mutatorToConvert.ID, true);
                }
            case Mutator_Value_Type.PERCENTAGE:
                return new PackagedMutator(mutatorToConvert.name, mutatorToConvert.ID, mutatorToConvert.value);
            default:
                return new PackagedMutator(mutatorToConvert.name, mutatorToConvert.ID, mutatorToConvert.value);
        }
    }

    #endregion
}
