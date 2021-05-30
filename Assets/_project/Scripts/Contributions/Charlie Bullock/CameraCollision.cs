/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///CameraCollision.cs
///Developed by Charlie Bullock
///This class is responsible for ensuring the collision on the freecam works correctly and on a collision it will send the camer back to zero.
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//This class is using:
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCollision : MonoBehaviour {

    //Function for on collision stay sending the camera back to position zero and setting velocity back to zero
    private void OnCollisionStay(Collision collision) {
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        transform.position = Vector3.zero;
    }
}
