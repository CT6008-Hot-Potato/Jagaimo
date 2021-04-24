/////////////////////////////////////////////////////////////
//
//  Script Name: MenuManager.cs
//  Creator: James Bradbury
//  Description: A manager script that handles the menu buttons
//  
/////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] MenuObjects;

    [SerializeField] 
    private JagaimoNetworkManagerLobby jagaimoNetworkManger = null;

    [Header("Multiplayer UI")] [SerializeField]
    private GameObject mpConnectionType = null;

    public void Start()
    {
        SwitchOpenMenu(0);
    }
  
    public void LoadScene(string ThisScene)
    {
        if (ThisScene == null) return;

        SceneManager.LoadScene(ThisScene);
    }

    public void SwitchOpenMenu(int SelectedMenu)
    {
        for (int i = 0; i < MenuObjects.Length ; i++)
        {

            if (i == SelectedMenu)
                
            { MenuObjects[i].SetActive(true); }
            else
            { MenuObjects[i].SetActive(false);}

        }
    }

    public void HostLobby()
    {
        jagaimoNetworkManger.StartHost();
    }
    
    public void CloseGame()
    {
        Application.Quit();
    }
}
