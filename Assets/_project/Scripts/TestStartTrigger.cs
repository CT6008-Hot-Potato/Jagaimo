using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A class so I can start the game whenever I want to in the test scene
public class TestStartTrigger : MonoBehaviour
{
    //The object that kicks the round off
    [SerializeField]
    BasicTimerBehaviour startCountdown;

    // Update is called once per frame
    void Update()
    {
        //P for play game
        if (Input.GetKeyDown(KeyCode.P))
        {
            startCountdown.CallOnTimerStart();
        }
    }
}
