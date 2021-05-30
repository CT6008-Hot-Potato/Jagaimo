////////////////////////////////////////////////////////////
// File: MutatorController
// Author: Charles Carter
// Date Created: 11/03/21
// Brief: This script is to control the mutators not covered in other scripts
//////////////////////////////////////////////////////////// 

using System.Collections.Generic;
using UnityEngine;

public class MutatorController : MonoBehaviour
{
    #region Variables

    [SerializeField]
    private RoundManager rManager;
    [SerializeField]
    private GameSettingsContainer settings;

    //Have 16 spots to fill out for studio
    [SerializeField]
    private List<Transform> FodderSpots = new List<Transform>();
    private int iFodderCount;

    [SerializeField]
    private GameObject fodderPrefab;

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
            //If it has a value for the cannon fodder and isnt on a football map
            if (settings.HasGenMutator(0) && rManager._currentGamemode.Return_Mode() != GAMEMODE_INDEX.FOOTBALL)
            {
                iFodderCount = (int)settings.FindGeneralMutatorValue(0);

                //Going through and placing an AI down in random spots
                for (int i = 0; i < iFodderCount; ++i)
                {
                    int thisSpot = GetRandomFodderSpot();
                    Vector3 fodderPoint = FodderSpots[thisSpot].position;
                    GameObject fodder = Instantiate(fodderPrefab, fodderPoint, Quaternion.identity);

                    //Making sure the same spots arent used again
                    FodderSpots.Remove(FodderSpots[thisSpot]);
                }
            }
        }
    }

    #endregion

    #region Private Methods

    private int GetRandomFodderSpot() {
        int i = Random.Range(0, FodderSpots.Count);
        return i;
    }

    #endregion
}
