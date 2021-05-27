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
    //The specified position moving by Charlie Bullock
    [SerializeField]
    private Vector3 position;

    //When something enters the bounds
    private void OnTriggerEnter(Collider other)
    {
        //Position isnt set to anything
        if (position == Vector3.zero)
        {
            if (Debug.isDebugBuild)
            {
                Debug.Log("Set position value" + other.name, this);
            }

            return;
        }

        if (other.tag != "PositionStay")
        {
            other.transform.position = position;
            if (other.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                if (Debug.isDebugBuild)
                {
                    Debug.Log("Something left the map: " + other.name, this);
                }
            }
        }
    }
}
