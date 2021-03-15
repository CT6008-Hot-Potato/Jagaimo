/////////////////////////////////////////////////////////////
//
//  Script Name: PopUpObject.cs
//  Creator: James Bradbury
//  Description: A script that, when attached to a UI element, will create an object that will appear as a tooltip when the mouse is over the object
//  
/////////////////////////////////////////////////////////////


using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Class inherits from Ipointerenter & IPointExit for detecting when the mouse is over the Element
public class PopUpObject: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    //Refference to the asset that will appear as a tooltip (assigned in inspector)
    [SerializeField] GameObject Prefab;

    //Refference to the object after being created by this script
    protected GameObject MyGameObject;

    // Allows the tooltip to be resized as appropriate on creation (assigned in inspector)
    [SerializeField] Vector3 Scale = Vector3.one;

    // Allows the tooltip to be recoloured, intended for elements that should be slightly translucent via assigning a colour with lower alpha. Leave as white if undesired (assigned in inspector)
    [SerializeField] protected Color OverlayColor = Color.white;

    // Allows the designer to decide how the tooltip should be presented in relation to the cursor (assigned in inspector)
    [SerializeField] Orientation Offset;

    // Refference to the tooltips position and scale, relative to the inspector
    RectTransform MyTransform;

    // Vector used to store the offset of the tooltip, which is stored on instantiation
    Vector2 Displacement;

    enum Orientation    // Options for the tooltips relative placement, can be expanded on if neccessary 

    {
        Centre,

        Top,
        Bottom, 
        Left, 
        Right,

        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    }

    void Awake()
    {

        if (Prefab == null)
        {

            MyGameObject = new GameObject(gameObject.name + "'s overlay");
            MyGameObject.transform.parent = transform;
        }
        else
        {
            MyGameObject = Instantiate(Prefab, transform);
            MyGameObject.name = (gameObject.name + "'s overlay");
        }
        MyGameObject.transform.localScale = Scale;
        MyGameObject.transform.localPosition = Vector3.zero;
        MyGameObject.AddComponent<RectTransform>();

        if (MyGameObject.TryGetComponent(out Renderer MyRenderer))
        {
            MyRenderer.material.color = OverlayColor;
        }

        MyTransform = MyGameObject.GetComponent<RectTransform>();
        MyGameObject.layer = 5; // (5 is the ui layer
        MyGameObject.SetActive(false);


        // Assigns displacement depending on the option chosen by the designer

        Displacement = Vector2.zero;
        if (Offset != Orientation.Centre)
        {
            if (Offset == Orientation.Top)
            {
                Displacement.y = MyTransform.sizeDelta.y * 0.5f;
            }
            else if (Offset == Orientation.Bottom)
            {
                Displacement.y = MyTransform.sizeDelta.y * -0.5f;
            }
            else if (Offset == Orientation.Left)
            {
                Displacement.x = MyTransform.sizeDelta.x * -0.5f;
            }
            else if (Offset == Orientation.Right)
            {
                Displacement.x = MyTransform.sizeDelta.x * 0.5f;
            }

            else if (Offset == Orientation.BottomLeft)
            {
                Displacement = MyTransform.sizeDelta * -0.5f;
            }
            else if (Offset == Orientation.TopRight)
            {
                Displacement = MyTransform.sizeDelta * 0.5f;
            }
            else if (Offset == Orientation.BottomRight)
            {
                Displacement = MyTransform.sizeDelta * -0.5f;
                Displacement.x *= -1;
            }
            else if (Offset == Orientation.TopLeft)
            {
                Displacement = MyTransform.sizeDelta * 0.5f;
                Displacement.x *= -1;
            }
        }

        InstantiateAsset();
    }

    protected virtual void InstantiateAsset()  // Instantiates a gameobject using the scale, etc provided in the inspector, ready for use 
    {
      
    }

    
    public virtual void UpdateContent(string content)  // updates content after creation
    {

    }
    public virtual void UpdateContent(Sprite content)  // updates content after creation
    {

    }



    protected void LateUpdate() // Late update is called after all other important calculations, which is optimal for UI elements 
    {
        if (MyGameObject.activeSelf) // If the element has a mouse over it, change the position of the overlay 
        {
            MyTransform.anchoredPosition = ScreenToRectPos(Input.mousePosition) + (Displacement);
        }
    }

    protected Vector2 ScreenToRectPos(Vector2 screen_pos) // Calculates the new position based on the mouse position 
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas.renderMode != RenderMode.ScreenSpaceOverlay && canvas.worldCamera != null)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(gameObject.GetComponent<RectTransform>(), screen_pos, canvas.worldCamera, out Vector2 localpoint);
            return localpoint;
        }
        else // The previous calculation only works  if the Render mode is using screenspace. If not, we can atleast ensure that the tooltip won't crash the build
        {
            Vector2 localpoint;
            localpoint.x = screen_pos.x - (Screen.width * 0.5f);
            localpoint.y = screen_pos.y - (Screen.height * 0.5f);
            return localpoint;
//            return Vector2.zero;
        }
    }

    public void OnPointerEnter(PointerEventData eventData) // When the mouse pointer enters the UI Element, make the tooltip appear 
    {
        MyGameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData) // When the mouse pointer exits the UI Element, make the dissappear
    {
        MyGameObject.SetActive(false);
    }


}
