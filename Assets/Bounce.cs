/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///Bounce.cs
///Developed by Charlie Bullock
///This class simply gives the player a bounce force each time they touch it's trigger.
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bounce : MonoBehaviour
{
    //Variables
    [SerializeField]
    private float force;
    private enum directions
    {
        FORWARDS,
        BACKWARDS,
        LEFT,
        RIGHT,
        UP,
        DOWN
    }
    [SerializeField]
    private directions dir;
    //On trigger stay will apply force to player in the direction selected
    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent(out Rigidbody rB) && other.tag == "Player")
        {
            switch (dir)
            {
                //Forward
                case directions.FORWARDS:
                    rB.AddForce(transform.forward * force, ForceMode.Impulse);
                    break;
                //Backward
                case directions.BACKWARDS:
                    rB.AddForce(-transform.forward * force, ForceMode.Impulse);
                    break;
                //Left
                case directions.LEFT:
                    rB.AddForce(-transform.right * force, ForceMode.Impulse);
                    break;
                //Right
                case directions.RIGHT:
                    rB.AddForce(transform.right * force, ForceMode.Impulse);
                    break;
                //Up
                case directions.UP:
                    rB.AddForce(transform.up * force, ForceMode.Impulse);
                    break;
                //Down
                case directions.DOWN:
                    rB.AddForce(-transform.up * force, ForceMode.Impulse);
                    break;
                default:
                    Debug.Log("Incorrect direction value recieved.");
                    break;
            }
        }
    }
}
