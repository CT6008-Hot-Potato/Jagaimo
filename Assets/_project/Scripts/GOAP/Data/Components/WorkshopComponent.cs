////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///Name: WorkshopComponent.cs 
///Created by: Charlie Bullock based on GOAP example given in CT6024
///Description: This component contain a variable for the amount of components it contains
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WorkshopComponent : MonoBehaviour
{
    //Variables
    public int numComponents;
    private string startingText;
    private TextMeshPro text;
    private string currentText;
    
    //Start function gets the text component needed
    private void Start()
    {
        text = GetComponentInChildren<TextMeshPro>();
        startingText = text.text;
    }

    //Update function chcks if text needs to be updated
    private void Update()
    {
        if (text.text != (startingText + "\n" + "Amount of components: " + "\n" + numComponents))
        {
            text.text = startingText + "\n" + "Amount of components: " + "\n" + numComponents;
        }
    }
}
