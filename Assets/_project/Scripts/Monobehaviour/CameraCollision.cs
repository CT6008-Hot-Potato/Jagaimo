using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCollision : MonoBehaviour
{
    private void OnCollisionStay(Collision collision)
    {
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        transform.position = Vector3.zero;
    }
}
