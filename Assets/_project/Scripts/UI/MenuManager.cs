////////////////////////////////////////////////////////////
// File: MenuManager
// Author: James Bradbury
// Brief: A script to control context menus, for both main menus and game menus
//////////////////////////////////////////////////////////// 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;


public class MenuManager : MonoBehaviour
{

    [SerializeField] private int StartScene; // On start up, opens the menu with this reference
    [SerializeField] private bool SlowTransitions; // If enabled, uses the fader script to smoothly interpolate between menus
    [SerializeField] private float TransitionSpeed, TransitionPauseAmount; // When Slow Transitions are enabled, these time values are used
    [SerializeField] private GameObject[] MenuObjects; // Stores the gameobjects of all menus attached to this object

    [SerializeField] SoundManager Sm;

    public void Start() // At start of game, go to start menu  
    {
        SwitchOpenMenu(StartScene);
    }

    class FadeObject : ScriptableObject
    {
        public List<Color> textColors, textProColors, imageColors;
        public List<Text> textAssets;
        public List<TextMeshProUGUI> textProAssets;

        public List<Image> imageAssets;
        public GameObject MainObject;

        public float FadeSpeed;
        public void initialise(GameObject MyObject, Text[] textassets, TextMeshProUGUI[] textproassets, Image[] images, float FadeRate)
        {
            MainObject = MyObject;



            textAssets = new List<Text>();
            textColors = new List<Color>();


            textProAssets = new List<TextMeshProUGUI>();
            textProColors = new List<Color>();

            imageAssets = new List<Image>();
            imageColors = new List<Color>();


            FadeSpeed = FadeRate;


            foreach (Text i in textassets)
            {
                textAssets.Add(i);
                textColors.Add(i.color);
            }

            foreach (TextMeshProUGUI i in textproassets)
            {
                textProAssets.Add(i);
                textProColors.Add(i.color);
            }

            foreach (Image i in images)
            {
                imageAssets.Add(i);
                imageColors.Add(i.color);
            }


        } // stores all instances of text assets, text pro assets, and images

    } // When Fading, all ui objects are stored in these fadeobkects  for lerping


    public void LoadScene(string ThisScene) // Loads a scene from a string. If there is a transition camera enabled, close it first   
    {
        if (ThisScene == null) return;
        CameraBlink[] cameras = FindObjectsOfType<CameraBlink>();
  
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

    IEnumerator DelayedLoad(float Delay, string Scene) // Delays the loading of a scene until after a transition is complete  
    {
        yield return new WaitForSeconds(Delay);
        SceneManager.LoadScene(Scene);
    }

    public void SwitchOpenMenu(int SelectedMenu) // Switches the current menu to another on the array    

    {
        if (Sm != null)
        {
            Sm.PlaySound();
        }
        


        if (SlowTransitions)
        {
            List<GameObject> ToFadeIn = new List<GameObject>();
            List<GameObject> ToFadeOut = new List<GameObject>();
            for (int i = 0; i < MenuObjects.Length; i++)
            {
                if (i == SelectedMenu)
                {
                    if (!MenuObjects[i].activeSelf)
                    {
                        ToFadeIn.Add(MenuObjects[i]);
                    }
                }
                else
                {
                    if (MenuObjects[i].activeSelf)
                    {
                        ToFadeOut.Add(MenuObjects[i]);
                    }
                }
            }
            StartCoroutine(FadeIntermission(ToFadeIn.ToArray(), ToFadeOut.ToArray()));

        }

        else
        {
            for (int i = 0; i < MenuObjects.Length; i++)
            {

                if (i == SelectedMenu)

                { MenuObjects[i].SetActive(true); }
                else
                { MenuObjects[i].SetActive(false); }

            }

        }



        Button startSelected = MenuObjects[SelectedMenu].GetComponentInChildren<Button>();
        if (startSelected != null)
        {
            SelectThisButton(startSelected.gameObject);
        }
    }

    void SelectThisButton(GameObject Button) // For controller support, the first button needs to be enabled. This selects an inputted button   
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(Button);

    }

    IEnumerator FadeIntermission(GameObject[] ToFadeIn, GameObject[] ToFadeOut) // Fades the menu gradually. First by fading out the original menu, pause for a little time, and then fading in the new menu 
    {
        //Print the time of when the function is first called.
        List<FadeObject> fadeObjects = new List<FadeObject>();

        float FadeRate = 1 / TransitionSpeed;

        foreach (GameObject i in ToFadeOut)
        {

            FadeObject newFadeObject = ScriptableObject.CreateInstance<FadeObject>();//new FadeObject();
            newFadeObject.initialise(i, i.GetComponentsInChildren<Text>(), i.GetComponentsInChildren<TextMeshProUGUI>(), i.GetComponentsInChildren<Image>(), FadeRate);

            fadeObjects.Add(newFadeObject);
        }

        foreach (FadeObject i in fadeObjects)
        {
            FadeOut(i);
        }

        yield return new WaitForSeconds(TransitionSpeed);

        foreach (GameObject i in ToFadeOut)
        {
            i.SetActive(false);
        }

        yield return new WaitForSeconds(TransitionPauseAmount);

        fadeObjects = new List<FadeObject>();

        foreach (GameObject i in ToFadeIn)
        {
            i.SetActive(true);

            FadeObject newFadeObject = ScriptableObject.CreateInstance<FadeObject>();//new FadeObject();
            newFadeObject.initialise(i, i.GetComponentsInChildren<Text>(), i.GetComponentsInChildren<TextMeshProUGUI>(), i.GetComponentsInChildren<Image>(), FadeRate);
            fadeObjects.Add(newFadeObject);
        }

        foreach (FadeObject i in fadeObjects)
        {

            FadeIn(i);
        }


        yield return new WaitForSeconds(TransitionSpeed);

        Button startSelected = GetComponentInChildren<Button>();
        if (startSelected != null)
        {
            SelectThisButton(startSelected.gameObject);
        }
    }

    void FadeOut(FadeObject i) // Fades the menu out
    {
        StartCoroutine(Fader(0, 0, i));
    }
    void FadeIn(FadeObject i) // Fades the menu in
    {
        StartCoroutine(Fader(0, 1, i));
    }

    IEnumerator Fader(float BlendFactor, float Displacement, FadeObject j) // Fades a menu in or out using linear interpolation   
    {
        BlendFactor += j.FadeSpeed * Time.deltaTime;

        for (int i = 0; i < j.textProAssets.Count; i++)
            j.textProAssets[i].color = Color.Lerp(j.textProColors[i], Color.clear, Mathf.Abs(Displacement - BlendFactor));
        for (int i = 0; i < j.textAssets.Count; i++)
            j.textAssets[i].color = Color.Lerp(j.textColors[i], Color.clear, Mathf.Abs(Displacement - BlendFactor));
        for (int i = 0; i < j.imageAssets.Count; i++)
            j.imageAssets[i].color = Color.Lerp(j.imageColors[i], Color.clear, Mathf.Abs(Displacement - BlendFactor));

        yield return null;

        if (BlendFactor <= 1)
        {
            StartCoroutine(Fader(BlendFactor, Displacement, j));
        }
        else
        {
            BlendFactor = 1;
            for (int i = 0; i < j.textProAssets.Count; i++)
                j.textProAssets[i].color = Color.Lerp(j.textProColors[i],  j.textProColors[i], Mathf.Abs(Displacement - BlendFactor));
            for (int i = 0; i < j.textAssets.Count; i++)
                j.textAssets[i].color = Color.Lerp(j.textColors[i], j.textColors[i], Mathf.Abs(Displacement - BlendFactor));
            for (int i = 0; i < j.imageAssets.Count; i++)
                j.imageAssets[i].color = Color.Lerp(j.imageColors[i], j.imageColors[i], Mathf.Abs(Displacement - BlendFactor));

            if (Displacement == 0)
            {
                j.MainObject.SetActive(false);
            }

        }

    }



    public void CloseGame() // Calling this closes the game completely
    {
        Application.Quit();
    }

   
}
