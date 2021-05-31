////////////////////////////////////////////////////////////
// File: ClassicWinScreen.cs
// Author: Charles Carter
// Date Created: 10/05/21
// Brief: The script that controls the specific effects on the win screen for the classic gamemode
//////////////////////////////////////////////////////////// 

using System.Collections.Generic;
using UnityEngine;

//This will be really close to the win screen script, but I'm keeping the formatting and still using the template to keep consistently
public class ClassicWinScreen : WinScreen {
    //This takes all of the players in order of 1st to last and positions them, not just the winners
    protected override void PositionPlayers(List<CharacterManager> objectsToPosition) {
        //if there's no winning spots, just return
        if (winningSpots.Length == 0) {
            if (Debug.isDebugBuild) {
                Debug.Log("Set winning end positions", this);
            }

            return;
        }

        if (objectsToPosition.Count > 0) {
            //Putting the players who won in the winning spots
            for (int i = 0; i < objectsToPosition.Count; ++i) {
                if (winningSpots[i] != null && objectsToPosition[i] != null) {
                    //Debug.Log(objectsToPosition[i].transform.position + " is going to: " + winningSpots[i].transform.position, this);
                    objectsToPosition[i].GetComponent<Rigidbody>().velocity = Vector3.zero;
                    objectsToPosition[i].transform.position = winningSpots[i].position;
                    objectsToPosition[i].transform.rotation = winningSpots[i].rotation;
                } else {
                    if (Debug.isDebugBuild) {
                        Debug.Log("This spot or player isnt set", this);
                    }
                }
            }
        }
    }
}
