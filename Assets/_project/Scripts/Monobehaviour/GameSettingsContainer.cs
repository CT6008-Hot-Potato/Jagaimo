////////////////////////////////////////////////////////////
// File: GameSettingsContainer
// Author: Charles Carter
// Date Created: 18/02/21
// Brief: The class which holds the mutators, gamemode and other data across local scenes
//////////////////////////////////////////////////////////// 

using UnityEngine;
using UnityEngine.InputSystem;

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

    public int iPlayercount = 0;
    public PlayerInput[] LocalPlayerInputs = new PlayerInput[4];

    #endregion

    #region Public Methods

    //Resetting these variables
    public void ClearPlayers()
    {
        //Resetting the player count
        iPlayercount = 0;

        //going through the objects and destroying them
        foreach (PlayerInput input in LocalPlayerInputs)
        {
            if (input)
            {
                Destroy(input.gameObject);
            }
        }

        //Resetting the array
        LocalPlayerInputs = new PlayerInput[4];
    }

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
                if (ID == generalMutators[i].ID)
                {
                    return generalMutators[i].value;
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
                if (ID == gamemodeMutators[i].ID)
                {
                    return gamemodeMutators[i].value;
                }
            }
        }

        return null;
    }

    public bool HasGenMutator(int ID)
    {
        if (generalMutators.Length > 0)
        {
            for (int i = 0; i < generalMutators.Length; ++i)
            {
                if (ID == generalMutators[i].ID)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public bool HasGamMutator(int ID)
    {
        if (gamemodeMutators.Length > 0)
        {
            for (int i = 0; i < gamemodeMutators.Length; ++i)
            {
                if (ID == gamemodeMutators[i].ID)
                {
                    return true;
                }
            }
        }

        return false;
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
