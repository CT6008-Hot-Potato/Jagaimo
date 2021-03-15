/////////////////////////////////////////////////////////////
//
//  Script Name: OverlayImage.cs
//  Creator: James Bradbury
//  Description: A script that, when attached to a UI element, will create an image that will appear as a tooltip when the mouse is over the object
//  
/////////////////////////////////////////////////////////////


using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// Class inherits from Ipointerenter & IPointExit for detecting when the mouse is over the Element
public class OverlayImage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    
    //Refference to the asset that will appear as a tooltip (assigned in inspector)
    [SerializeField] Sprite imageObject;

    //Refference to the image object after being created by this script
    GameObject imageGameObject;

    // Allows the tooltip image to be resized as appropriate on creation (assigned in inspector)
    [SerializeField] Vector3 Scale;

    // Allows the tooltip to be recoloured, intended for elements that should be slightly translucent via assigning a colour with lower alpha. Leave as white if undesired (assigned in inspector)
    [SerializeField] Color OverlayColor = Color.white;
    
    // Allows the designer to decide how the tooltip should be presented in relation to the cursor (assigned in inspector)
    [SerializeField] Orientation Offset;

    // Refference to the tooltips position and scale, relative to the inspector
    RectTransform ImageTransform;

    // Vector used to store the offset of the tooltip, which is stored on instantiation
    Vector2 Displacement;

    enum Orientation    // Options for the tooltips relative placement, can be expanded on if neccessary 

    {
        Centre,
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    }

    void Start()  // Instantiates a gameobject using the image, scale, etc provided in the inspector, ready for use 
    {
        imageGameObject = new GameObject( gameObject.name+ "'s overlay");
        imageGameObject.transform.parent = transform;
        imageGameObject.transform.localScale = Scale;
        imageGameObject.transform.localPosition = Vector3.zero;

        imageGameObject.AddComponent<RectTransform>();
        ImageTransform = imageGameObject.GetComponent<RectTransform>();

        imageGameObject.layer = 5; // (5 is the ui layer
        
        Image Renderer = imageGameObject.AddComponent<Image>();
        Renderer.sprite = imageObject;
        Renderer.color = OverlayColor;


        imageGameObject.SetActive(false);


        // Assigns displacement depending on the option chosen by the designer
        if(Offset == Orientation.Centre)
        {
            Displacement = Vector2.zero;
        }
        else if (Offset == Orientation.BottomLeft)
        {
            Displacement=  ImageTransform.sizeDelta * -0.5f;
        }
        else if (Offset == Orientation.TopRight)
        {
            Displacement = ImageTransform.sizeDelta * 0.5f;
        }
        else if (Offset == Orientation.BottomRight)
        {
            Displacement = ImageTransform.sizeDelta * -0.5f;
            Displacement.x *= -1;
        }
        else if (Offset == Orientation.TopLeft)
        {
            Displacement = ImageTransform.sizeDelta * 0.5f;
            Displacement.x *= -1;

        }

    }

    void LateUpdate() // Late update is called after all other important calculations, which is optimal for UI elements 
    {
        if (imageGameObject.activeSelf) // If the element has a mouse over it, change the position of the overlay image
        {
            ImageTransform.anchoredPosition = ScreenToRectPos(Input.mousePosition) + (Displacement) ;
        }
    }

    public Vector2 ScreenToRectPos(Vector2 screen_pos) // Calculates the new position based on the mouse position 
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas.renderMode != RenderMode.ScreenSpaceOverlay && canvas.worldCamera != null)
        {
            Vector2 localpoint = Vector2.zero;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(gameObject.GetComponent<RectTransform>(), screen_pos, canvas.worldCamera, out localpoint);
            return localpoint;
        }
        else // The previous calculation only works  if the Render mode is using screenspace. If not, we can atleast ensure that the tooltip won't crash the build
        { 
            return Vector2.zero;
        }
    }

    public void OnPointerEnter(PointerEventData eventData) // When the mouse pointer enters the UI Element, make the image tooltip appear 
    {
        imageGameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData) // When the mouse pointer exits the UI Element, make the image dissappear
    {
        imageGameObject.SetActive(false);
     }


}
