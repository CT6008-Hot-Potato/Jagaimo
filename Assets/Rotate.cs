using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    [SerializeField]
    private float rotateSpeed;
    private bool playerEntered;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Rigidbody rB) && other.name != "CarryPosition")
        {
            other.transform.parent = transform;
            if (other.tag == "Player")
            {
                playerEntered = true;
            }
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.TryGetComponent(out Rigidbody rB) && collider.transform.parent == transform && collider.name != "CarryPosition")
        {
            if (collider.tag == "Player")
            {
                playerEntered = false;
                transform.GetChild(0).transform.eulerAngles = new Vector3(0, 0, 0);
            }
            collider.transform.parent = null;
        }
    }
    void Update()
    {
        transform.RotateAround(transform.position, transform.up, rotateSpeed);

        if (playerEntered)
        {
            if (transform.GetChild(0).tag == "Player")
            {
                transform.GetChild(0).transform.eulerAngles = new Vector3 (0,0,0);
            }
        }
    }
}
