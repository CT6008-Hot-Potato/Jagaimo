/////////////////////////////////////////////////////////////
//
//  Script Name: TipLoader.cs
//  Creator: James Bradbury
//  Description: A script that loads a tip from a string array at initialisation
//  
/////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TipLoader : MonoBehaviour
{

    [SerializeField] string[] tips;
    [SerializeField] TextMeshProUGUI textObject;

    [SerializeField] Slider loadingScreen;
    [SerializeField] float loadingSpeed;
    [SerializeField] string LoadPrompt;
    [SerializeField] TextMeshProUGUI LoadObject;

    [SerializeField] GameObject menuObject;
    MenuManager menuComponent;
    [SerializeField] int menuNumber;

    float sliderProgress;
    InputAction myAction;

    AudioSource myAudio;

    void Start()
    {
        if (textObject == null)
            return;
        if (tips.Length == 0)
            return;

        textObject.text = tips[Random.Range(0, tips.Length)];

        if (loadingScreen == null)
            sliderProgress = 1;
        else
            sliderProgress = loadingScreen.value;

        menuComponent = menuObject.GetComponent<MenuManager>();

        TryGetComponent(out myAudio);



    }

    void LoadMenu()
    {
        if (menuComponent != null)
        {
            myAction.Disable();
            menuComponent.SwitchOpenMenu(menuNumber);
            
            gameObject.SetActive(false);


        }
    }

    // Update is called once per frame
    void Update()
    {
        if(sliderProgress < 1)
        {
            sliderProgress += (loadingSpeed * Mathf.Pow(Mathf.Sin(sliderProgress + loadingSpeed), 2)) * Time.deltaTime ;
            loadingScreen.value = sliderProgress;
        }
        else
            return;

        if (sliderProgress >= 1)
        {

            if (myAudio != null)
            {
                myAudio.Play();

            }
            LoadObject.text = LoadPrompt;

            myAction = new InputAction(binding: "/*/<button>");
            myAction.performed += (actioncontrol) => LoadMenu();
            myAction.Enable();
        }


    }
}
