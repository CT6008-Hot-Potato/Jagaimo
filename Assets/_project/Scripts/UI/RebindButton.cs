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
    private InputActionRebindingExtensions.RebindingOperation rebindOperation;
    public InputAction currentBindingInput;
    private InputAction focusedInputAction;
    public PlayerInput pI;
    public void UpdateDisplay(string action, string button, InputAction BindingInput)
    {
        actionName.text = action;
        buttonName.text = button;

        currentBindingInput = BindingInput;
    }

    public void ButtonPressedStartRebind()
    {
        pI.enabled = false;
        focusedInputAction = pI.actions.FindAction(currentBindingInput.name);
        rebindOperation = focusedInputAction.PerformInteractiveRebinding()
            .WithControlsExcluding("<Mouse>/position")
            .WithControlsExcluding("<Mouse>/delta")
            .WithControlsExcluding("<Gamepad>/Start")
            .WithControlsExcluding("<Keyboard>/p")
            .WithControlsExcluding("<Keyboard>/escape")
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(operation => RebindCompleted());
        rebindOperation.Start();
    }


    void RebindCompleted()
    {
        rebindOperation.Dispose();
        rebindOperation = null;
        pI.enabled = true;
        UpdateDisplay(currentBindingInput.name, currentBindingInput.GetBindingDisplayString(InputBinding.DisplayStringOptions.DontIncludeInteractions), currentBindingInput);
        
    }

    public void ButtonPressedResetBinding()
    {
        ResetBinding();
    }

    public void ResetBinding()
    {
        InputActionRebindingExtensions.RemoveAllBindingOverrides(currentBindingInput);
    }

    void ToggleGameObjectState(GameObject targetGameObject, bool newState)
    {
        targetGameObject.SetActive(newState);
    }
}
