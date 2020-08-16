using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultGamemode : MonoBehaviour, IGamemode
{
    bool IGamemode.WinCondition() => ThisWinCondition();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //When only 1 person is active in the game, return true
    bool ThisWinCondition()
    {
        return false;
    }
}
