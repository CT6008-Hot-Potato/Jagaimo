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

// Class inherits from Ipointerenter & IPointExit for detecting when the mouse is over the Element
public class PopUpText : PopUpObject
{

    [SerializeField] string[] text = new string[1];
    [SerializeField] Color textcolor = Color.white;
    [SerializeField] Font textFont;

    protected override void InstantiateAsset()  // Instantiates a gameobject using the image, scale, etc provided in the inspector, ready for use 
    {
        if (MyGameObject.TryGetComponent(out Text MyText))
        {
            MyText.text = text[0];
            MyText.color = textcolor;
            MyText.font = textFont;
            return;
        }
        else 
        {
            Text[] MyTexts =  MyGameObject.GetComponentsInChildren<Text>();
            if (MyTexts.Length != 0)
            {
                for( int i = 0; i< text.Length; i ++)
                {
                    if (i < MyTexts.Length)
                        MyTexts[i].text = text[i];
                        MyTexts[i].color = textcolor;
                        MyTexts[i].font = textFont;
                }
                return;
            }
        }


        if (MyGameObject.TryGetComponent(out TextMeshPro MyTMProText))
        {
            MyTMProText.text = text[0];
            MyTMProText.color = textcolor;
            return;
        }
        else
        {
            TextMeshPro[] MyTexts = MyGameObject.GetComponentsInChildren<TextMeshPro>();
            if (MyTexts.Length != 0)
            {
                for (int i = 0; i < text.Length; i++)
                {
                    if (i < MyTexts.Length)
                        MyTexts[i].text = text[i];
                        MyTexts[i].color = textcolor;

                }
                return;
            }
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
            MyText.text = content;
        if (MyGameObject.TryGetComponent(out TextMeshPro MyTMProText))
            MyTMProText.text = content;
                
                
       }




}
