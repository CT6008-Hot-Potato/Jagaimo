/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///PlayerInteraction.cs
///Developed by Charlie Bullock
///This class is responsible for controlling the correct movement and parenting of rigidbody objects picked up by players.
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//This class is using:
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    //Variables
    #region Variables
    private GameObject movingObject;
    [SerializeField]
    private GameObject movingParent;
    [SerializeField]
    private GameObject firstPersonCamera;
    private RaycastHit hit;
    private Ray ray;
    [SerializeField]
    private Transform closestPosition;
    [SerializeField]
    private Transform furthestPosition;
    [SerializeField]
    private Transform position;
    [SerializeField]
    private float throwStrength = 10;
    [SerializeField]
    private float grabDistance = 5;
    private Rigidbody rbObject;
    private Rigidbody rbParent;
    private LocalMPScreenPartioning cM;
    private bool grabbing = false;
    [SerializeField]
    private PlayerInput playerInput = null;
    [SerializeField]
    private ScriptableSounds.Sounds grabSound, throwSound;
    private PlayerAnimation pA;
    private SoundManager sM;
    private PlayerController pC;
    public PlayerInput PlayerInput => playerInput;
    private float leftClick = 0;
    private float rightClick = 0;
    private float zoomIn = 0;
    private float zoomOut = 0;
    #endregion Variables

    private void Awake()
    {
        //Intergrating the throw speed mutator (Code Here by Charles Carter)
        GameSettingsContainer settings = GameSettingsContainer.instance;

        if (settings)
        {
            //The potato throw strength mutater is changed so the value is not 1
            if (settings.HasGamMutator(4))
            {
                //Adding on 50% of the strength * the multiplier from the mutator
                throwStrength += throwStrength * 0.2f * (int)settings.FindGeneralMutatorValue(4);
            }
        }
        //End of mutator intergration code
    }

    //Start method setting up and assigning values
    void Start()
    {
        pC = FindObjectOfType<PlayerController>();
        pA = GetComponent<PlayerAnimation>();
        sM = FindObjectOfType<SoundManager>();
        cM = GameObject.FindObjectOfType<LocalMPScreenPartioning>();
        position.position = movingParent.transform.position;
        if (!movingObject || !movingObject.GetComponent<Rigidbody>())
        {
            return;
        }
        rbObject = movingObject.GetComponent<Rigidbody>();
        //Set the component Rigidbody's useGravity to true in the modelItem.
        rbObject.useGravity = true;       
    }

    //Function to return a ray from camera
    public Ray getRay()
    {
        return ray;
    }

    //Update method checking for clicks to throw,drop or grab rigidbody objects to move and also move closer or further/
    void Update()
    {

        //If the player has interacted
        if (leftClick > 0.1 && grabbing == false)
        {
            grabbing = true;
            //Try get cameras and then quickly enable the main camera regardless of if third or first person to do raycast
            ray = new Ray(firstPersonCamera.transform.position, firstPersonCamera.transform.forward);

            if (pC.grounded)
            {
                grabDistance = 5;
            }
            else
            {
                grabDistance = 1;
            }

            //Do a raycast and check if the object needs to be dropped or picked up
            if (Physics.Raycast(ray, out hit, grabDistance))
            {
                //If moving object already held drop it
                if (movingObject != null)
                {
                    Drop(false);
                }
                //Else if hit object has a rigidbody and isn't tagged a player grab it
                else if (hit.rigidbody != null && hit.transform.tag != "Player")
                {
                    //Play grabbing sound if sound manager present
                    if (sM)
                    {
                        sM.PlaySound(grabSound);
                    }

                    movingObject = hit.transform.gameObject;
                    //Here we are simply assigning the rbObject the rb component on moving object then setting it's gravity to false and kinematic to true, this is done so this object doesn't drag around.
                    rbObject = movingObject.GetComponent<Rigidbody>();
                    rbObject.useGravity = false;
                    rbObject.isKinematic = true;
                    //Here movingObject position, rotation and parent are assigned to that of moving parent, this is done to keep the moving object positioned where moving parent is and parented to it too.
                    movingObject.transform.position = movingParent.transform.position;
                    movingObject.transform.rotation = movingParent.transform.rotation;
                    movingObject.transform.parent = movingParent.transform;
                    //The collider component on movingObject is assigned the collider type of that of the movingParent and it's collision is then adjusted to the correct type accordingly. 
                    SetComponent(movingObject.GetComponent<Collider>(), movingParent);
                    //Set the smaller trigger component to true
                    movingParent.GetComponent<Collider>().isTrigger = true;
                    AdjustCollision(movingObject);
                    movingObject.GetComponent<Collider>().enabled = false;
                    //Assign the parent rigidbody to the moving parent rigidbody and setting aspects of it true and false
                    rbParent = movingParent.AddComponent<Rigidbody>();
                    rbParent.freezeRotation = true;
                    rbParent.isKinematic = false;
                    rbParent.useGravity = true;
                    //RbParent rigidbody collision is set to ContinuousDynamic as this is the best collision for this fast moving object, also below the carry collision class/component is added to moving parent.
                    rbParent.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                    movingParent.AddComponent<CarryCollision>();
                }
            }

        }
        //Throw the object when right click or right trigger pressed
        else if (rightClick > 0.1)
        {
            ray = new Ray(firstPersonCamera.transform.position, firstPersonCamera.transform.forward);

            if (movingObject != null)
            {
                Drop(true);
            }
        }
        //Set grabbing back to false if already grabbing
        else if (grabbing && leftClick == 0)
        {
            grabbing = false;
        }
        //Otherwise if holding the moving object it can be moved closer or further from player
        else if (movingObject != null)
        {
            movingParent.transform.position = UnityEngine.Vector3.MoveTowards(movingParent.transform.position, position.position, 1000 * Time.deltaTime);
            //Zoom in
            if (zoomIn > 0.1f)
            {
                position.position = UnityEngine.Vector3.MoveTowards(position.position, closestPosition.position, 5 * Time.deltaTime);
            }
            //Zoom out
            else if (zoomOut > 0.1f)
            {
                position.position = UnityEngine.Vector3.MoveTowards(position.position, furthestPosition.position, 5 * Time.deltaTime);
            }
        }
    }

    //Called to uncrouch the player
    public bool UnCrouch()
    {
        //Raycast from first person camera
        ray = new Ray(new Vector3(firstPersonCamera.transform.position.x, firstPersonCamera.transform.position.y - 0.25f, firstPersonCamera.transform.position.z), firstPersonCamera.transform.up);
        //Do a raycast and check if facing wall
        Physics.Raycast(ray, out hit, 0.75f);
        if (hit.collider != null)
        {
            return false;
        }
        else
        {
            firstPersonCamera.transform.localPosition = new UnityEngine.Vector3(0, 0.75f, 0);
            //zoomInPosition.localPosition = Vector3.zero;
            return true;
        }
    }

    //Check if able to perform a wall kick by checking how close to a collider
    public bool WallKick()
    {
        //Raycast from first person camera
        ray = new Ray(new Vector3(firstPersonCamera.transform.position.x, firstPersonCamera.transform.position.y - 0.5f, firstPersonCamera.transform.position.z), firstPersonCamera.transform.forward);
        //Do a raycast and check if facing wall
        Physics.Raycast(ray, out hit, 0.5f);

        if (hit.rigidbody == null && hit.collider != null)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    //Called to crouch the player
    public void Crouch()
    {
        firstPersonCamera.transform.localPosition = new UnityEngine.Vector3(0, 0, 0);
        //zoomInPosition.localPosition = Vector3.zero;
    }

    //This function will set the paramaters of the collision for a carried object based upon what type of colliders it is
    private void AdjustCollision(GameObject movingObject)
    {
        if (movingParent.TryGetComponent(out Collider cD))
        {
            //Set collision parameters if box collider
            if (cD.GetType() == typeof(BoxCollider))
            {
                cD.GetComponent<BoxCollider>().size = new Vector3(cD.GetComponent<BoxCollider>().size.x * movingObject.transform.localScale.x, cD.GetComponent<BoxCollider>().size.y * movingObject.transform.localScale.y,cD.GetComponent<BoxCollider>().size.z * movingObject.transform.localScale.z);
            }
            //Set collision parameters if sphere collider
            else if (cD.GetType() == typeof(SphereCollider))
            {
                cD.GetComponent<SphereCollider>().radius = cD.GetComponent<SphereCollider>().radius * movingObject.transform.localScale.x;
            }
            else if (cD.GetType() == typeof(CapsuleCollider))
            {
                cD.GetComponent<CapsuleCollider>().height = cD.GetComponent<CapsuleCollider>().height * movingObject.transform.localScale.y;
                cD.GetComponent<CapsuleCollider>().radius = cD.GetComponent<CapsuleCollider>().radius * movingObject.transform.localScale.x;
            }
            //Debug if other type of collider
            else
            {
                Debug.Log("Other collider type");
            }
        }
    }


    //Function which when called will drop the current object which is being carried 
    public void Drop(bool throwObject)
    {
        if (sM)
        {
            sM.PlaySound(throwSound);
        }

        movingParent.transform.position = position.position;
        //Null check on moving object
        if (movingObject != null)
        {
            //Try to get rigidbody and collider
            if (movingObject.TryGetComponent(out Rigidbody rB) && movingObject.TryGetComponent(out Collider cD))
            {
                rB.useGravity = true;
                rB.isKinematic = false;
                //Try to get collider
                cD.enabled = true;
                movingObject.transform.parent = null;
                movingObject.transform.position = movingParent.transform.position;
                //When object is being thrown first will check got rigidbody and then throw it
                if (throwObject)
                {
                    rB.AddForce(firstPersonCamera.transform.forward * pC.speed, ForceMode.Impulse);
                    //rB.velocity = GetComponent<Rigidbody>().velocity;
                }
            }
            //Unassign moving object
            movingObject = null;
        }
        //Loop through the parent destroying it's components
        foreach (var comp in movingParent.GetComponents<Component>())
        {
            if (!(comp is Transform) && comp.GetType() != typeof(PlayerInteraction))
            {
                Destroy(comp);
            }
        }
        //Set position back to the closesty position for when it picks something up again
        position.position = closestPosition.position;
    }



    //Function assigned component given as paramater to the gameobject given as parameter along with ensuring it
    public Component SetComponent(Component originalComponent, GameObject gameobjectToSet)
    {
        System.Type type = originalComponent.GetType();
        Component copy = gameobjectToSet.AddComponent(type);
        // Copied fields can be restricted with BindingFlags
        System.Reflection.FieldInfo[] fields = type.GetFields();
        foreach (System.Reflection.FieldInfo field in fields)
        {
            field.SetValue(copy, field.GetValue(originalComponent));
        }
        return copy;
    }

    //Method for using unity's new input system to detect left click
    public void LeftClick(InputAction.CallbackContext ctx)
    {
        leftClick = ctx.ReadValue<float>();
        if (!pA)
        {
            pA.CheckToChangeState("Grab", true);
        }
    }

    //Method for using unity's new input system to detect right click
    public void RightClick(InputAction.CallbackContext ctx)
    {
        rightClick = ctx.ReadValue<float>();
        if (!pA)
        {
            pA.CheckToChangeState("Throw", true);
        }
    }

    //Method for using unity's new input system to detect zooming in
    public void ZoomIn(InputAction.CallbackContext ctx)
    {
        zoomIn = ctx.ReadValue<float>();
    }

    //Method for using unity's new input system to detect zooming out
    public void ZoomOut(InputAction.CallbackContext ctx)
    {
        zoomOut = ctx.ReadValue<float>();
    }
}

