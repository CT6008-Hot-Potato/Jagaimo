/////////////////////////////////////////////////////////////
//
//  Script Name: InteractMapObject.cs
//  Creator: Charles Carter
//  Description: A script for when the potato hits an interactable map object
//  
/////////////////////////////////////////////////////////////

//This script uses these namespaces
using UnityEngine;

public class InteractMapObject : MonoBehaviour, IInteractable
{
    //Does it get destroyed on hit
    [SerializeField]
    private bool bDestroyOnHit;
    [SerializeField]
    private bool bDropOnHit;

    //Playing an animation
    [SerializeField]
    private Animation obj_anim;
    [SerializeField]
    private Rigidbody _rb;

    void IInteractable.Interact() => Interact();
    private void Interact()
    {
        //if (Debug.isDebugBuild)
        //{
        //    Debug.Log("Hit Interactable Map Object", this);
        //}

        if (obj_anim)
        {
            obj_anim.Play();
        }

        //Destroying the object if needed
        //Note: Objects shouldnt really have this and an animation unless it plays an animation for the parent
        if (bDestroyOnHit)
        {
            Destroy(gameObject);
        }
        else if (bDropOnHit)
        {
            Debug.Log("Here");
            Destroy(obj_anim);
            
            if (_rb)
            {
                _rb.isKinematic = false;
            }
        }
    }
}
