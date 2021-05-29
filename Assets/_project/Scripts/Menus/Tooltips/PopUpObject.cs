/////////////////////////////////////////////////////////////
//
//  Script Name: PopUpObject.cs
//  Creator: James Bradbury
//  Description: A script that, when attached to a UI element, will create an object that will appear as a tooltip when the mouse is over the object
//  
/////////////////////////////////////////////////////////////


using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// Class inherits from Ipointerenter & IPointExit for detecting when the mouse is over the Element
public class PopUpObject: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    //Refference to the asset that will appear as a tooltip (assigned in inspector)
    [SerializeField] GameObject Prefab;

    //Refference to the object after being created by this script
    public GameObject MyGameObject;

    // Allows the tooltip to be resized as appropriate on creation (assigned in inspector)
    [SerializeField] Vector3 Scale = Vector3.one;

    // Allows the tooltip to be recoloured, intended for elements that should be slightly translucent via assigning a colour with lower alpha. Leave as white if undesired (assigned in inspector)
    [SerializeField] protected Color OverlayColor = Color.white;


    public RectTransform GetPosition()
    {
        if (TryGetComponent(out RectTransform ThisPosition))
            return ThisPosition;
        else
            return null;
    }
  
    public class Tooltip 
    {
        public enum Orientation    // Options for the tooltips relative placement, can be expanded on if neccessary 

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
        
        public GameObject toolObject;
        

        // Refference to the tooltips position and scale, relative to the inspector
        RectTransform MyTransform;
        Orientation myOrientation = Orientation.BottomRight;
        // Allows the designer to decide how the tooltip should be presented in relation to the cursor (assigned in inspector)
      //  [SerializeField] Orientation Offset;
        // Vector used to store the offset of the tooltip, which is stored on instantiation

        Vector2 Displacement;
        public void Start()
        {
            MyTransform = toolObject.GetComponent<RectTransform>();
            ChangeOrientation(myOrientation);
        }
        
        public void SetPosition(RectTransform newposition)
        {

            MyTransform.parent = newposition;
            MyTransform.anchoredPosition = Vector2.zero;
        }

        public void ChangeOrientation(Orientation Offset)
        {
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
        }

        public void LateUpdate() // Late update is called after all other important calculations, which is optimal for UI elements 
        {

            

            if (MyTransform != null)
            {
                if (Mouse.current.leftButton.wasPressedThisFrame)
                {
                    MyTransform.gameObject.SetActive(false);
                }

            }


            if (toolObject.activeSelf) // If the element has a mouse over it, change the position of the overlay 
            {

                var mouse = Mouse.current;

    //            MyTransform.anchorMin = new Vector2(0, 0);
  //              MyTransform.anchorMax = new Vector2(1, 1);
//                MyTransform.pivot = new Vector2(0.5f, 0.5f);


//                MyTransform.anchoredPosition =  ///  ScreenToRectPos( 1000* (Camera.main.ScreenToViewportPoint(Mouse.current.position.ReadValue()) ));//+ (Vector3)Displacement; // ScreenToRectPos(  /* mouse.position.ReadValue()*/) + (Displacement);

          
            }


        }

        protected Vector2 ScreenToRectPos(Vector2 screen_pos) // Calculates the new position based on the mouse position 
        {
            Canvas canvas = toolObject.GetComponentInParent<Canvas>();
            if (canvas.renderMode != RenderMode.ScreenSpaceOverlay && canvas.worldCamera != null)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(toolObject.GetComponent<RectTransform>(), screen_pos, canvas.worldCamera, out Vector2 localpoint);
                return localpoint;
            }
            else // The previous calculation only works  if the Render mode is using screenspace. If not, we can atleast ensure that the tooltip won't crash the build
            {
                Vector2 localpoint = screen_pos;


                localpoint.x = screen_pos.x - (Display.main.renderingWidth * 0.5f);
                localpoint.y = screen_pos.y - (Display.main.renderingHeight * 0.5f);

                Debug.Log(localpoint);
                return localpoint;
       
            }
        }

    }
    public static Tooltip instance;

    void Awake()
    {
        
        if (instance == null)
        {

            instance = new Tooltip();
        }
        else if (instance != null)
        {
            MyGameObject = instance.toolObject;
            return;
        }
        

        if (Prefab == null)
        {
            instance.toolObject.transform.SetParent(  transform.root );
        }
        else
        {
            instance.toolObject = Instantiate(Prefab, transform.root);
            instance.toolObject.name = ("Tooltip");
        }

        instance.toolObject.transform.localScale = Scale;
    //    instance.toolObject.transform.localPosition = Vector3.zero;

        if(!instance.toolObject.TryGetComponent(out RectTransform rect))
        {
            instance.toolObject.AddComponent<RectTransform>(); 
        }

        if (instance.toolObject.TryGetComponent(out Renderer MyRenderer))
        {
            MyRenderer.material.color = OverlayColor;
        }

//        MyTransform = instance.toolObject.GetComponent<RectTransform>();
        instance.toolObject.layer = 5; // (5 is the ui layer
        instance.toolObject.SetActive(false);
        
        instance.Start();
        
        MyGameObject = instance.toolObject;
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

    public virtual void OnPointerEnter(PointerEventData eventData) // When the mouse pointer enters the UI Element, make the tooltip appear 
    {
//        MyGameObject.SetActive(true);

       
    }

    public void OnPointerEnter(PointerEventData eventData, string newString) // When the mouse pointer enters the UI Element, make the tooltip appear 
    {
        MyGameObject.SetActive(true);
        MyGameObject.TryGetComponent(out Tooltip i);
     //   i.SetPosition(GetPosition());
            
    }

    public void OnPointerExit(PointerEventData eventData) // When the mouse pointer exits the UI Element, make the dissappear
    {
        MyGameObject.SetActive(false);
    }

    public void LateUpdate() // Late update is called after all other important calculations, which is optimal for UI elements 
    {

        instance.LateUpdate();


    }

    }
