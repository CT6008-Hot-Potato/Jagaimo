////////////////////////////////////////////////////////////
// File: LocalPlayMutatorContainer
// Author: Charles Carter
// Date Created: 18/02/21
// Brief: The class which holds the mutators, gamemode and other data across local scenes
//////////////////////////////////////////////////////////// 

using UnityEngine;

public class GameSettingsContainer : MonoBehaviour
{
    #region Public Variables

    //Seperating it from normal instancing
    public static GameSettingsContainer instance;

    //The variables being stored across the scenes
    public GAMEMODE_INDEX index;
    public PackagedMutator[] generalMutators;
    public PackagedMutator[] gamemodeMutators;
    public PackagedMutator[] mapMutators;

    #endregion

    #region Public Methods

    public void StoreMutators(PackagedMutator[] genMutators, PackagedMutator[] gmodeMutators, PackagedMutator[] mMutators)
    {
        generalMutators = genMutators;
        gamemodeMutators = gmodeMutators;
        mapMutators = mMutators;
    }

    public void StoreGamemode(GAMEMODE_INDEX gamemode)
    {
        index = gamemode;
    }

    public object FindGeneralMutatorValue(int ID)
    {
        if (generalMutators.Length > 0)
        {
            for (int i = 0; i < generalMutators.Length; ++i)
            {
                if (ID == generalMutators[ID].ID)
                {
                    return generalMutators[ID].value;
                }
            }
        }

        return null;
    }

    public object FindGamemodeMutatorValue(int ID)
    {
        if (gamemodeMutators.Length > 0)
        {
            for (int i = 0; i < gamemodeMutators.Length; ++i)
            {
                if (ID == gamemodeMutators[ID].ID)
                {
                    return gamemodeMutators[ID].value;
                }
            }
        }

        return null;
    }

    #endregion

    #region Unity Methods

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this);
        }
    }

    #endregion 

}
