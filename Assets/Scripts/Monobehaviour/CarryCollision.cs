/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///CarryCollision.cs
///Developed by Charlie Bullock
///This class simply ensures rigidbody objects drop when collide without doing at start.
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarryCollision : MonoBehaviour
{
    MoveObject moveObject;
    private bool canCollide = false;
    private void Start()
    {
        moveObject = FindObjectOfType<MoveObject>();
        StartCoroutine("WaitFirst");
    }

    IEnumerator WaitFirst()
    {
        yield return new WaitForSeconds(0.1f);
        canCollide = true;
    }

    private void OnTriggerStay(Collider collider)
    {
        if (canCollide && collider.gameObject != gameObject)
        {
            moveObject.Drop(false);
        }
    }
}
