////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///Name: ReportingComponent.cs 
///Created by: Charlie Bullock based on GOAP example given in CT6024
///Description: This component can be tampered with or detected if tampered 
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ReportingComponent : MonoBehaviour
{
    //Variables
    public bool tamperedWith = false;
    private string startingText;
    private TextMeshPro text;
    private string currentText;

    //Start function gets text component needed
    private void Start()
    {
        text = GetComponentInChildren<TextMeshPro>();
        startingText = text.text;
    }

    //Update function updates text if it has changes
    private void Update()
    {
        if (text.text != (startingText + "\n" + "Tampered with: " + "\n" + tamperedWith))
        {
            text.text = startingText + "\n" + "Tampered with: " + "\n" + tamperedWith;
        }
    }
}
