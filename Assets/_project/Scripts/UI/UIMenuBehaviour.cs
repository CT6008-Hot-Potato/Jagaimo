using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIMenuBehaviour : MonoBehaviour
{

    [Header("Core Object References")]

    [SerializeField]
    private ScriptableSounds.Sounds clickSound;

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
    //    UpdateUIMenuState(false);
         StartCoroutine(CountdownCoroutine());
    }


    IEnumerator CountdownCoroutine()
    {
        yield return new WaitForSeconds(3);
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
