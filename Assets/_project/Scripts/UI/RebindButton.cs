/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///RebindButton.cs
///Developed by James Bradbury
///
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class RebindButton : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI actionName, buttonName;

    public InputAction currentBindingInput;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    
    public void UpdateDisplay(string action, string button, InputAction BindingInput)
    {
        actionName.text = action;
        buttonName.text = button;

        currentBindingInput = BindingInput;
    }
}
