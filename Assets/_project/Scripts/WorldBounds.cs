/////////////////////////////////////////////////////////////
//
//  Script Name: WorldBounds.cs
//  Creator: Charles Carter
//  Description: A script for when something leaves the map
//  
/////////////////////////////////////////////////////////////

//This script uses these namespaces
using UnityEngine;

//A trigger placed below the test world
public class WorldBounds : MonoBehaviour
{
    [SerializeField]
    private Vector3 position;
    //When something enters the bounds
    private void OnTriggerEnter(Collider other)
    {
        //Put it back at the start
        if (position == Vector3.zero)
        {
            other.transform.position = new Vector3(0, 5, 0);
        }
        else
        {
            other.transform.position = position;
        }
        if (Debug.isDebugBuild)
        {
            Debug.Log("Something left the map: " + other.name, this);
        }
    }
}
