/////////////////////////////////////////////////////////////
//
//  Script Name: TestWorldBoundsScript.cs
//  Creator: Charles Carter
//  Description: A script for when something leaves the map
//  
/////////////////////////////////////////////////////////////

//This script uses these namespaces
using UnityEngine;

//A trigger placed below the test world
public class TestWorldBoundsScript : MonoBehaviour
{
    //When something enters the bounds
    private void OnTriggerEnter(Collider other)
    {
        //Put it back at the start
        other.transform.position = new Vector3(0, 5, 0);
        if (Debug.isDebugBuild)
        {
            Debug.Log("Something left the map", this);
        }
    }
}
