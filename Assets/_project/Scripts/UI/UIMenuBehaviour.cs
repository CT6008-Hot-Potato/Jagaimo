﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIMenuBehaviour : MonoBehaviour
{

    [Header("Core Object References")]

    [SerializeField]
    private AudioClip clickSound;

    public GameObject uiMenuCanvasObject;
  
    private SoundManager sM;


    public void Awake()
    {
        sM = FindObjectOfType<SoundManager>();
        SetupBehaviour();
    }

    //private void OnEnable()
    //{
    //    playerActionControls.Enable();
    //}
    //private void OnDisable()
    //{
    //    playerActionControls.Disable();
    //}

    public void SetupBehaviour()
    {   
        UpdateUIMenuState(false);
    }

    public bool GetMenuStatus()
    {
        return uiMenuCanvasObject.activeSelf;
    }

    public void UpdateUIMenuState(bool newState)
    {        
     
        UpdateCoreUIObjectsState(newState);
    }
            

    void UpdateCoreUIObjectsState(bool newState)
    {
        if (sM)
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
