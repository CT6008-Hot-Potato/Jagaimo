using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractMapObject : MonoBehaviour, IInteractable
{
    //Does it get destroyed on hit
    [SerializeField]
    private bool bDestroyOnHit;

    //Playing an animation
    [SerializeField]
    private Animation obj_anim;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void IInteractable.Interact() => Interact();

    void Interact()
    {
        Debug.Log("Hit Interactable Map Object");

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
    }
}
