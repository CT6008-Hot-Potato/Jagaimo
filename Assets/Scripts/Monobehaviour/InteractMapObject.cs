﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractMapObject : MonoBehaviour, IInteractable
{
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
        Destroy(gameObject);
    }
}
