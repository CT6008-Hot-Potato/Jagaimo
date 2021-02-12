using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class RebindingDisplay : MonoBehaviour
{
    //Variables
    #region Variables
    [SerializeField] 
    private InputActionReference jumpAction = null;
    [SerializeField] 
    private PlayerController playerController = null;
    [SerializeField] 
    private TMP_Text bindingDisplayNameText = null;
    [SerializeField] 
    private GameObject startRebindObject = null;
    [SerializeField] 
    private GameObject waitingForInputObject = null;
    [SerializeField]
    private GameObject bindingMenu;
    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;
    private const string RebindsKey = "rebinds";
    #endregion Variables

    private void Start()
    {
        string rebinds = PlayerPrefs.GetString(RebindsKey, string.Empty);

        if (string.IsNullOrEmpty(rebinds)) { return; }

        playerController.PlayerInput.actions.LoadFromJson(rebinds);

        int bindingIndex = jumpAction.action.GetBindingIndexForControl(jumpAction.action.controls[0]);

        bindingDisplayNameText.text = InputControlPath.ToHumanReadableString(
            jumpAction.action.bindings[bindingIndex].effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice);
    }

    public void DisplayBindingMenu (bool display)
    {
        bindingMenu.SetActive(display);
    }

    public void Save()
    {
        string rebinds = playerController.PlayerInput.actions.ToJson();

        PlayerPrefs.SetString(RebindsKey, rebinds);
    }

    public void StartRebinding()
    {
        startRebindObject.SetActive(false);
        waitingForInputObject.SetActive(true);

        playerController.PlayerInput.SwitchCurrentActionMap("Menu");

        rebindingOperation = jumpAction.action.PerformInteractiveRebinding()
            .WithControlsExcluding("Mouse")
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(operation => RebindComplete())
            .Start();
    }

    private void RebindComplete()
    {
        int bindingIndex = jumpAction.action.GetBindingIndexForControl(jumpAction.action.controls[0]);

        bindingDisplayNameText.text = InputControlPath.ToHumanReadableString(
            jumpAction.action.bindings[bindingIndex].effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice);

        rebindingOperation.Dispose();

        startRebindObject.SetActive(true);
        waitingForInputObject.SetActive(false);

        playerController.PlayerInput.SwitchCurrentActionMap("Gameplay");
    }
}