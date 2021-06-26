/////////////////////////////////////////////////////////////
//
//  Script Name: PopUpImage.cs
//  Creator: James Bradbury
//  Description: A script that, when attached to a UI element, will create a texxt object that will appear as a tooltip when the mouse is over the object
//  
/////////////////////////////////////////////////////////////


using UnityEngine;
using UnityEngine.UI;

// Class inherits from Ipointerenter & IPointExit for detecting when the mouse is over the Element
public class PopUpImage : PopUpObject
{

    [SerializeField] Sprite[] sprites = new Sprite[1];
    [SerializeField] Color spritecolor = Color.white;

    protected override void InstantiateAsset()  // Instantiates a gameobject using the image, scale, etc provided in the inspector, ready for use 
    {
        if (MyGameObject.TryGetComponent(out SpriteRenderer MySprite))
        {
            MySprite.sprite = sprites[0];
            MySprite.color = spritecolor;
            return;
        }
        else
        {
            SpriteRenderer[] MySprites = MyGameObject.GetComponentsInChildren<SpriteRenderer>();
            if (MySprites.Length != 0)
            {
                for (int i = 0; i < sprites.Length; i++)
                {
                    if (i < MySprites.Length)
                        MySprites[i].sprite = sprites[i];
                        MySprites[i].color = spritecolor;
                }
                return;
            }
        }


        if (MyGameObject.TryGetComponent(out Image MyImage))
        {
            MyImage.sprite = sprites[0];
            MyImage.color = spritecolor;

            return;
        }
        else
        {
            Image[] MySprites = MyGameObject.GetComponentsInChildren<Image>();
            if (MySprites.Length != 0)
            {
                for (int i = 0; i < sprites.Length; i++)
                {
                    if (i < MySprites.Length)
                        MySprites[i].sprite = sprites[i];
                        MySprites[i].color = spritecolor;
                }
                return;

            }
        }

        Image NewImage = MyGameObject.AddComponent<Image>();
        NewImage.sprite = sprites[0];
        NewImage.color = spritecolor;

    }

    public override void UpdateContent(Sprite content)  // updates content after creation
    {
        if (MyGameObject.TryGetComponent(out SpriteRenderer MySprite))
            MySprite.sprite = content;
        if (MyGameObject.TryGetComponent(out Image MyImage))
            MyImage.sprite = content;


    }



}
