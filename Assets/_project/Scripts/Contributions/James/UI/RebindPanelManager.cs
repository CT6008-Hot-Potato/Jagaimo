﻿/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///RebindPanelManager.cs
///Developed by James Bradbury
///A component to set rebind buttons ready for rebinding
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class RebindPanelManager : MonoBehaviour
{
    [SerializeField] GameObject rebindObject;
    public InputActionAsset ControlScheme;  // put new Input manager here
    private PlayerInput inputAction;
    public string MapName; // Name of the list of inputs being changed. (player 1, player 2, etc)

    

    void Start() // for each type of button in the input manager, create a rebind button for it and ready it for rebinding   
    {
        inputAction = transform.root.GetComponent<PlayerInput>();
        foreach (InputAction i in ControlScheme.FindActionMap(MapName).actions)
        {
            if (i.type == InputActionType.Button)
            {
                GameObject j = Instantiate(rebindObject, transform);

                if (j.TryGetComponent(out RebindButton button))
                {
                    j.GetComponentInChildren<Button>().onClick.AddListener(button.ButtonPressedStartRebind);
                    button.pI = inputAction;
                    button.UpdateDisplay(i.name, i.GetBindingDisplayString(InputBinding.DisplayStringOptions.DontIncludeInteractions), i);
                }
            }
        }
    }

}