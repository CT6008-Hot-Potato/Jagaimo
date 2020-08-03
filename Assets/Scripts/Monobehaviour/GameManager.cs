using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameMode
{
    NONE       =    0b00,
    QUICK_FFA  =    0b00000001,
    MEDIUM_FFA =    0b00000010,
    LONG_FFA   =    0b00000100,

    COUNT
}

public enum GameState
{
    MainMenu,
    Local_CoOp,
    Pause,
}

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
