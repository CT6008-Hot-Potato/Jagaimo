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

public enum eGameMode
{
    NONE       =    0b00,
    QUICK_FFA  =    0b00000001,
    MEDIUM_FFA =    0b00000010,
    LONG_FFA   =    0b00000100,

    COUNT
}

public enum eGameState
{
    MainMenu,
    Local_CoOp,
    Pause,
}

public class GameManager : MonoBehaviour
{

    private int MAX_ROUND_NUMBER;
    private int iCurrentRound;

    private int iPlayerCount;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
