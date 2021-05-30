/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///ScrollText.cs
///Developed by James Bradbury
/// Moves text elements across a screen once created
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollText : MonoBehaviour
{
    RectTransform MyTransform; // Reference to the elements position


    [SerializeField] // Desired change in position over time, accessaible in the inspector
    Vector3 Change;

    void Start() // Grabs the rect transform attached to the gameobject 
    {
        TryGetComponent(out MyTransform);   
    }

    void LateUpdate() // Adjusts position over time 
    {
        if (MyTransform == null)
            return;

        MyTransform.localPosition += Change * Time.deltaTime;
    }
}
