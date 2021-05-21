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
    PlayerInteraction playerInteraction;
    private bool canCollide = false;
    public float distanceOffset;
    //Start function gettiing moveobject

    //Corouting for waiting before collider can be dropped called and playerinteraction component found
    private void Awake()
    {
        playerInteraction = gameObject.transform.parent.parent.GetComponent<PlayerInteraction>();
        StartCoroutine("WaitFirst");        
    }

    //Coroutine giving brief window of pickup from position without collision
    IEnumerator WaitFirst()
    {
        yield return new WaitForSeconds(1f);
        Debug.Log(distanceOffset);
        canCollide = true;
    }
    //Drop the carried object when it something is within it's collision
    private void OnTriggerStay(Collider collider)
    {
        if (canCollide)
        {
            if (transform.GetChild(0).name == "Potato")
            {

            }
            else if (Vector3.Distance(collider.ClosestPoint(collider.transform.position), transform.position) < distanceOffset)
            {
                if (collider.gameObject != playerInteraction.gameObject && collider.gameObject != gameObject && collider.gameObject.GetComponent<Rotate>() == null && collider.gameObject.GetComponent<Bounce>() == null)
                {
                    playerInteraction.Drop(false);
                }

            }

        }
    }
}
