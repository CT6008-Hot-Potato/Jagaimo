/////////////////////////////////////////////////////////////
//
//  Script Name: GameManager.cs
//  Creator: Charles Carter
//  Description: A manager script for the game as a whole
//  
/////////////////////////////////////////////////////////////

//This script uses these namespaces
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//An enum to hold the current state of the overall game loop
public enum eGameState
{
    MainMenu,
    Local_CoOp,
    Pause,

    COUNT
}

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private eGameState _currentState;
    public IGamemode _chosenGamemode { get; private set; }

    private int MAX_ROUND_NUMBER;
    private int iCurrentRound;

    private int iPlayerCount;

    // Start is called before the first frame update
    void Awake()
    {
        _currentState = eGameState.MainMenu;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
