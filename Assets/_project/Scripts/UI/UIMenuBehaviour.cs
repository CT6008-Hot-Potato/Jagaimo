////////////////////////////////////////////////////////////
// File: UIMenuBehaviour
// Author: James Bradbury
// Brief: The script for controlling the pause menu. It connects to a menumanager
//////////////////////////////////////////////////////////// 

using System.Collections;
using UnityEngine;

public class UIMenuBehaviour : MonoBehaviour
{

   [SerializeField]    private ScriptableSounds.Sounds clickSound; // Sound connected to the pause menu
    public GameObject uiMenuCanvasObject; // refferences the menumanager object
    private SoundManager sM; // references the sound manager for sound effects


    public void Awake() // On enable, grab the neccisary scripts and start internal countdown timer
    {
        sM = FindObjectOfType<SoundManager>();
        StartCoroutine(CountdownCoroutine());
    }

    IEnumerator CountdownCoroutine() // The menu starts enabled for the initial countdown, and then closes
    {
        yield return new WaitForSeconds(3);
        UpdateUIMenuState(false);
    }

    public bool GetMenuStatus() // Used to check if the pause menu is open 
    {
        return uiMenuCanvasObject.activeSelf;
    }

    public void UpdateUIMenuState(bool newState) // open or closes the pause menu  
    {        
     
        UpdateCoreUIObjectsState(newState);
    }
            

    void UpdateCoreUIObjectsState(bool newState) // plays sound effect when opening pause menu, setting the menu back to its original state    
    {
        if (sM !=null)
        {
            sM.PlaySound(clickSound);
        }
        if (newState)
        { 
            GetComponent<MenuManager>().SwitchOpenMenu(0); 
        }
        uiMenuCanvasObject.SetActive(newState);
    }

    
}
