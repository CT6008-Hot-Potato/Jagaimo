////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///Name: RadioComponent.cs 
///Created by: Charlie Bullock based on GOAP example given in CT6024
///Description: This component can be tampered with or detected when tampered with
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class RadioComponent : MonoBehaviour
{
    //Variables
    public bool tamperedWith = false;
    private string startingText;
    private TextMeshPro text;
    private string currentText;

    //Start function gets component for text mesh pro
    private void Start()
    {
        text = GetComponentInChildren<TextMeshPro>();
        startingText = text.text;
    }

    //Update function will check if text needs updating
    private void Update()
    {
        if (text.text != (startingText + "\n" + "Tampered with: " + "\n" + tamperedWith))
        {
            text.text = startingText + "\n" + "Tampered with: " + "\n" + tamperedWith;
        }
    }
}
