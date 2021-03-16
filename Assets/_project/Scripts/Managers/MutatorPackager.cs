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

    [SerializeField]
    public object value;
}

#endregion

public class MutatorPackager : MonoBehaviour
{
    #region Public Fields

    public static MutatorPackager instance;

    //A reference to the current gamemode selected
    public GAMEMODE_INDEX Gamemode = GAMEMODE_INDEX.CLASSIC;

    #endregion

    #region Private Variables


    #endregion

    #region Public Methods

    //Saving And Loading the current mutators for the game to use
    public void MakeChangedMutatorArrays(bool bLocalPlay, List<MutatorUI> GenMut, List<MutatorUI> GamMut, List<MutatorUI> MapMut)
    {
        PackagedMutator[] genMutators = packagedMutatorsForGen(GenMut);
        PackagedMutator[] gmodeMutators = packagedMutatorsForGMode(GamMut);
        PackagedMutator[] mapMutators = packagedMutatorsForMap(MapMut);

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
    private PackagedMutator[] packagedMutatorsForGen(List<MutatorUI> currentGeneralMutatorList)
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

    private PackagedMutator[] packagedMutatorsForGMode(List<MutatorUI> currentGmodeMutatorList)
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

    private PackagedMutator[] packagedMutatorsForMap(List<MutatorUI> currentMapMutatorList)
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
