using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bounce : MonoBehaviour
{
    [SerializeField]
    private float force;
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Rigidbody rB) && other.name != "CarryPosition")
        {
            rB.AddForce(transform.up * force,ForceMode.Impulse);
        }
    }
}
