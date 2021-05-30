/////////////////////////////////////////////////////////////
//
//  Script Name: PopUpText.cs
//  Creator: James Bradbury
//  Description: A script that, when attached to a UI element, will create a texxt object that will appear as a tooltip when the mouse is over the object
//  
/////////////////////////////////////////////////////////////


using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

// Class inherits from Ipointerenter & IPointExit for detecting when the mouse is over the Element
public class PopUpText : PopUpObject
{

    [SerializeField] string[] text = new string[1];
    [SerializeField] Color textcolor = Color.white;
    [SerializeField] Font textFont;

    protected override void InstantiateAsset()  // Instantiates a gameobject using the image, scale, etc provided in the inspector, ready for use 
    {


        if (MyGameObject.TryGetComponent(out Text MyText) )
        {
            MyText.text = text[0];
            MyText.color = textcolor;
            MyText.font = textFont;
            return;
        }
        else if (MyGameObject.TryGetComponent(out TextMeshProUGUI MyTMProText) )
        {
            MyTMProText.text = text[0];
            MyTMProText.color = textcolor;
            return;
        }

        TextMeshProUGUI[] MyChildTMProText = MyGameObject.GetComponentsInChildren<TextMeshProUGUI>();
        
        if (MyChildTMProText.Length > 0)
        {
            for (int i = 0; i < MyChildTMProText.Length; i++)
            {
                if (i < text.Length)
                {
        
                    MyChildTMProText[i].text = text[i];
                    MyChildTMProText[i].color = textcolor;
                }
            }
            return;
        }

        Text NewText = MyGameObject.AddComponent<Text>();
        NewText.text = "";
        NewText.color = textcolor;
        NewText.font = textFont;
        foreach (string i in text)
        {
            NewText.text += i;
            NewText.text += '\n';

        }

    }

    public override void UpdateContent(string content)  // updates content after creation
    {
        if (MyGameObject.TryGetComponent(out Text MyText))
        { 
            MyText.text = content;
            return;
        }
        if (MyGameObject.TryGetComponent(out TextMeshProUGUI MyTMProText))
        {
            MyTMProText.text = content;
            return;
        }

        TextMeshProUGUI[] MyChildTMProText = MyGameObject.GetComponentsInChildren<TextMeshProUGUI>();

        if (MyChildTMProText.Length > 0)
        {
            for (int i = 0; i < MyChildTMProText.Length; i++)
            {
                if (i < text.Length)
                {
                    MyChildTMProText[i].text = text[i];
                }
            }
            return;
        }

    }

    public override void OnPointerEnter(PointerEventData eventData) // When the mouse pointer enters the UI Element, make the tooltip appear 
    {
        MyGameObject.SetActive(true);

        TextMeshProUGUI[] MyChildTMProText = MyGameObject.GetComponentsInChildren<TextMeshProUGUI>();

        if (MyChildTMProText.Length > 0)
        {
            UpdateContent(text[0]);

        }


    }

}
