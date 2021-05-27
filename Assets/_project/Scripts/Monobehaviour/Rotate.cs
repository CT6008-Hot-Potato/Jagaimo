/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///Rotate.cs
///Developed by Charlie Bullock
///This class is rotates an object constantly at a set speed.
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//This class is using:
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    //Variables
    [SerializeField]
    private float rotateSpeed;
    private bool playerEntered;
    private List<GameObject> players = new List<GameObject>();
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

                players.Add(other.gameObject);
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
            players.Remove(collider.gameObject);
        }
    }
    //Rotate around in update
    void Update()
    {
        if (players != null )
        {
            for (int i = 0; i < players.Count;i++)
            {
                players[i].transform.rotation = Quaternion.Euler(0,0, 0);
            }
        }
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
        //if (playerEntered)
        //{
        //    if (transform.GetChild(0).tag == "Player")
        //    {
        //        transform.GetChild(0).transform.eulerAngles = new Vector3(0, 0, 0);
        //    }
        //}
    }
}
