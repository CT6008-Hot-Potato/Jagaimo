////////////////////////////////////////////////////////////
// File: UIMenuBehaviour
// Author: James Bradbury
// Brief: The script for controlling the pause menu. It connects to a menumanager
//////////////////////////////////////////////////////////// 

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMenuBehaviour : MonoBehaviour
{

   [SerializeField]    private ScriptableSounds.Sounds clickSound; // Sound connected to the pause menu
    public GameObject uiMenuCanvasObject; // refferences the menumanager object
    private SoundManager sM; // references the sound manager for sound effects

    public PlayerCamera CameraManager;

    Resolution[] resolutions;
    public TMP_Dropdown resolutionsDropdown;

    public Slider[] sliders;

    [SerializeField]
    private TextMeshProUGUI currentResolution; 

    public void Awake() // On enable, grab the neccisary scripts and start internal countdown timer
    {
        sM = FindObjectOfType<SoundManager>();
        StartCoroutine(CountdownCoroutine());


        
    }


    public void Start()
    {
        if (resolutionsDropdown == null)
            return;

        resolutions = Screen.resolutions;
        resolutionsDropdown.ClearOptions();
        List<string> options = new List<string>();
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            if (resolutions[i].width <= Screen.width && resolutions[i].height <= Screen.height)
            {
                options.Add(option);
            }
        }
        resolutionsDropdown.AddOptions(options);

        StartCoroutine(LateResolutionChanger());


        if (CameraManager != null)
        {
            CameraManager.cameraSensitivity = GetPrefFloat("Mouse");
            sliders[0].value = CameraManager.cameraSensitivity;
            CameraManager.controllerCameraSensitivityMultiplier = GetPrefFloat("Controller");
            sliders[1].value = CameraManager.controllerCameraSensitivityMultiplier;


            CameraManager.otherCamSpeed = GetPrefFloat("Spectator");
            sliders[2].value = CameraManager.otherCamSpeed;

        }

    }

    private IEnumerator LateResolutionChanger()
    {
        yield return new WaitForSeconds(1);
        if (PlayerPrefs.GetInt("playerGameResolutionx", 1920) > 0 && PlayerPrefs.GetInt("playerGameResolutionY", 1080) > 0)
        {
            currentResolution.text = "1920" + " x " + "1080";
        }
        else
        {
            currentResolution.text = (PlayerPrefs.GetInt("playerGameResolutionX", 1920).ToString() + " x " + PlayerPrefs.GetInt("playerGameResolutionY", 1080).ToString());
        }
     
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
        if(newState)
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }


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

    public void SetPrefFloat(string key, float value)
    {

        string keywithindex = key + CameraManager.playerIndex;
        PlayerPrefs.SetFloat(keywithindex, value);
    }
    public float GetPrefFloat(string key)
    {

        string keywithindex = key + CameraManager.playerIndex;
        return PlayerPrefs.GetFloat(keywithindex);

    }

    public void UpdateMouseSens(float i)
    {
        if (CameraManager == null) return;

        SetPrefFloat("Mouse", i);

        CameraManager.cameraSensitivity = i;
    }

    public void UpdateControllerSens(float i)
    {
        if (CameraManager == null) return;

        SetPrefFloat("Controller", i);

        CameraManager.controllerCameraSensitivityMultiplier = i;
    }

    public void UpdateSpectatorSpeed(float i)
    {
        if (CameraManager == null) return;


        SetPrefFloat("Spectator", i);

        CameraManager.otherCamSpeed = i;
    }

    public void UpdateFullScreen(bool i)
    {
        Screen.fullScreen = i;
        Screen.SetResolution(Screen.width, Screen.height, i);
    }


    public void UpdateResolution(int i)
    {
        Screen.SetResolution(resolutions[i].width, resolutions[i].height, Screen.fullScreen);
    }


}
