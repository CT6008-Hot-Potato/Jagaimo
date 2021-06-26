using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DocumentComponent : MonoBehaviour
{
    //Variables
    public int numDocuments = 5;
    private string startingText;
    private TextMeshPro text;
    private string currentText;

    //Start function gets text component needed
    private void Start()
    {
        text = GetComponentInChildren<TextMeshPro>();
        startingText = text.text;
    }

    //Update function changes text when it needed to be updated
    private void Update()
    {
        if (text.text != (startingText + "\n" + "Amount of documents: " + "\n" + numDocuments))
        {
            text.text = startingText + "\n" + "Amount of documents: " + "\n" + numDocuments;
        }
    }
}
