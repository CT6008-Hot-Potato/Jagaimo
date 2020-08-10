/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///Rotate.cs
///Developed by Charlie Bullock
///This class is rotates an object constantly at a set speed.
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    //Variables
    [SerializeField]
    private float rotateSpeed;
    private bool playerEntered;
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
    //When player enters parent them
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.transform.parent = transform;
            playerEntered = true;
        }
    }
    //When player exits unparent them
    private void OnTriggerExit(Collider collider)
    {
        if (collider.transform.parent == transform && collider.tag == "Player")
        {
            playerEntered = false;
            transform.GetChild(0).transform.eulerAngles = new Vector3(0, 0, 0);
            collider.transform.parent = null;
        }
    }
    //Rotate around in update
    void Update()
    {
        //Rotate object around
        switch (dir)
        {
            //Forward
            case directions.FORWARDS:
                transform.RotateAround(transform.position, transform.forward, rotateSpeed * Time.deltaTime);
                break;
            //Backward
            case directions.BACKWARDS:
                transform.RotateAround(transform.position, -transform.forward, rotateSpeed * Time.deltaTime);
                break;
            //Left
            case directions.LEFT:
                transform.RotateAround(transform.position, -transform.right, rotateSpeed * Time.deltaTime);
                break;
            //Right
            case directions.RIGHT:
                transform.RotateAround(transform.position, transform.right, rotateSpeed * Time.deltaTime);
                break;
            //Up
            case directions.UP:
                transform.RotateAround(transform.position, transform.up, rotateSpeed * Time.deltaTime);
                break;
            //Down
            case directions.DOWN:
                transform.RotateAround(transform.position, -transform.up, rotateSpeed * Time.deltaTime);
                break;
            default:
                Debug.Log("Incorrect direction value recieved.");
                break;
        }

        //Stabalise player rotation
        if (playerEntered)
        {
            if (transform.GetChild(0).tag == "Player")
            {
                transform.GetChild(0).transform.eulerAngles = new Vector3(0, 0, 0);
            }
        }
    }
}
