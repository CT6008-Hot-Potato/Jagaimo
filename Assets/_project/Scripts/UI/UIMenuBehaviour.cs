using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIMenuBehaviour : MonoBehaviour
{
 
    [Header("Core Object References")]

    public PlayerControls playerActionControls;
    

    public GameObject uiMenuCanvasObject;
  
    [Header("Player Display")]
    public Image deviceDisplayIcon;
    public TextMeshProUGUI IDDisplayText;


    public void Awake()
    {
        playerActionControls = new PlayerControls();
        playerActionControls.Gameplay.Escape.performed += _ => UpdateUIMenuState(true);
        playerActionControls.Menu.Escape.performed += _ => UpdateUIMenuState(false);
        SetupBehaviour();
    }

    private void OnEnable()
    {
        playerActionControls.Enable();
    }
    private void OnDisable()
    {
        playerActionControls.Disable();
    }

    public void SetupBehaviour()
    {   
        UpdateUIMenuState(false);
    }

    public void UpdateUIMenuState(bool newState)
    {        
     
        UpdateCoreUIObjectsState(newState);
    }

    
    

    void UpdateCoreUIObjectsState(bool newState)
    {
        //uiMenuCameraObject.SetActive(newState);
        uiMenuCanvasObject.SetActive(newState);
    }

    
}
