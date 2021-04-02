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
    //Variables
    MoveObject moveObject;
    private bool canCollide = false;
    //Start function gettiing moveobject
    private void Start()
    {
        moveObject = gameObject.transform.parent.parent.GetComponent<MoveObject>();
        StartCoroutine("WaitFirst");
    }
    //Coroutine giving brief window of pickup from position without collision
    IEnumerator WaitFirst()
    {
        yield return new WaitForSeconds(0.1f);
        canCollide = true;
    }
    //Drop the carried object when it something is within it's collision
    private void OnTriggerStay(Collider collider)
    {
        if (canCollide && collider.gameObject != moveObject.gameObject && collider.gameObject != gameObject && collider.gameObject.GetComponent<Rotate>() == null && collider.gameObject.GetComponent<Bounce>() == null)
        {
            moveObject.Drop(false);
        }
    }
}
