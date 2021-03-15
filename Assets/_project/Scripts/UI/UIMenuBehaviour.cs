using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIMenuBehaviour : MonoBehaviour
{

    [Header("Sub-Behaviours")]
    //public UIContextPanelsBehaviour uiContextPanelsBehaviour;
    //public List<UIPanelRebindBehaviour> uiPanelRebindBehaviours = new List<UIPanelRebindBehaviour>();

    [Header("Core Object References")]
    public GameObject uiMenuCameraObject;
    public GameObject uiMenuCanvasObject;

    [Header("Default Selected")]
    public GameObject defaultSelectedGameObject;

    
    [Header("Player Display")]
    //public DeviceDisplayConfigurator deviceDisplayconfigurator;
    public Image deviceDisplayIcon;
    public TextMeshProUGUI IDDisplayText;


    //public void SetupRebindObjects(List<UIPanelRebindBehaviour> InputRebinds)
    //{
    //    foreach(UIPanelRebindBehaviour i in InputRebinds)
    //    {
    //        uiPanelRebindBehaviours.Add(i);
    //    }

    //}

    public void SetupBehaviour()
    {   
        UpdateUIMenuState(false);
    }

    public void UpdateUIMenuState(bool newState)
    {
        switch (newState)
        {
            case true:

                break;

            case false:
                
                break;
        }

        UpdateCoreUIObjectsState(newState);
    }

    
    

    void UpdateCoreUIObjectsState(bool newState)
    {
        uiMenuCameraObject.SetActive(newState);
        uiMenuCanvasObject.SetActive(newState);
    }

    

    //void UpdateEventSystemDefaultSelected()
    //{
    //    EventSystemManager.Instance.SetCurrentSelectedGameObject(defaultSelectedGameObject);
    //}

    //void UpdateEventSystemUIInputModule()
    //{
    //    EventSystemManager.Instance.UpdateActionAssetToFocusedPlayer();
    //}

    //void UpdateUIRebindActions()
    //{
    //    foreach(UIPanelRebindBehaviour i in uiPanelRebindBehaviours)
    //    {
    //        i.UpdateRebindActions();
    //    }
    //}

    //public void SwitchUIContextPanels(int selectedPanelID)
    //{
    //    uiContextPanelsBehaviour.UpdateContextPanels(selectedPanelID);
    //}
    
}
