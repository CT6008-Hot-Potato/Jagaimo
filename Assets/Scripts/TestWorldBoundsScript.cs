using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestWorldBoundsScript : MonoBehaviour
{
    //When something enters the bounds
    private void OnTriggerEnter(Collider other)
    {
        //Put it back at the start
        other.transform.position = new Vector3(0, 5, 0);
    }
}
