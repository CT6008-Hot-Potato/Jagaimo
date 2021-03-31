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

    public void Start()
    {
        SwitchOpenMenu(0);
    }
  
    public void LoadScene(string ThisScene)
    {
        if (ThisScene == null) return;
        CameraBlink[] cameras =  FindObjectsOfType<CameraBlink>();
        Debug.Log(cameras.Length);
        if (cameras.Length != 0)
        {
            float Fuse = Mathf.Infinity;
            foreach (CameraBlink blink in cameras)
            {
                blink.CloseLens();

                if (blink.TransitionTime < Fuse)
                    Fuse = blink.TransitionTime;
            }
            StartCoroutine(DelayedLoad(Fuse, ThisScene));
        }
        else
        {
            SceneManager.LoadScene(ThisScene);
        }
    }

    IEnumerator DelayedLoad(float Delay, string Scene)
    {
        yield return new WaitForSeconds(Delay);
        SceneManager.LoadScene(Scene);
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

    public void CloseGame()
    {
        Application.Quit();
    }
}
