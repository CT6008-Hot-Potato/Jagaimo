/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///RebindPanelManager.cs
///Developed by James Bradbury
///
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RebindPanelManager : MonoBehaviour
{
    [SerializeField] GameObject rebindObject;
    public InputActionAsset ControlScheme;  // put new Input manager here

    public string MapName; // Name of the list of inputs being changed. (player 1, player 2, etc)

    
    // Start is called before the first frame update
    void Start()
    {
        foreach (InputAction i in ControlScheme.FindActionMap(MapName).actions)
        {
            if (i.type == InputActionType.Button)
            {
                GameObject j = Instantiate(rebindObject, transform);

                if (j.TryGetComponent(out RebindButton button))
                {

                    button.UpdateDisplay(i.name, i.GetBindingDisplayString(InputBinding.DisplayStringOptions.DontIncludeInteractions), i);

                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
