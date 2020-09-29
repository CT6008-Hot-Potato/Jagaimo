/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///MoveObject.cs
///Developed by Charlie Bullock
///This class is responsible for controlling the correct movement and parenting of rigidbody objects picked up by players.
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//This class is using:
using System.Collections;
using UnityEngine;

public class MoveObject : MonoBehaviour
{
    //Variables
    #region Variables
    private GameObject movingObject;
    [SerializeField]
    private GameObject movingParent;
    [SerializeField]
    private GameObject mainCamera;
    private RaycastHit hit;
    private Ray ray;
    [SerializeField]
    private Transform closestPosition;
    [SerializeField]
    private Transform furthestPosition;
    [SerializeField]
    private Transform position;
    [SerializeField]
    private float throwStrength = 15;
    [SerializeField]
    private float grabDistance = 5;
    private Rigidbody rbObject;
    private Rigidbody rbParent;
    private CharacterManager cM;

    #endregion Variables
    //Start method setting up and assigning values
    void Start()
    {
        cM = GameObject.FindObjectOfType<CharacterManager>();
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
        if (Input.GetAxis("LeftClick" + cM.playerIndex) > 0.1)
        {
            //Try get cameras and then quickly enable the main camera regardless of if third or first person to do raycast
            ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);


            //Do a raycast and check if the object needs to be dropped or picked up
            if (Physics.Raycast(ray, out hit, grabDistance))
            {
                if (movingObject != null)
                {
                    Drop(false);
                }
                else if (hit.rigidbody != null && hit.transform.tag != "Player")
                {
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
                    AdjustCollision();
                    //Set the smaller trigger component to true
                    movingParent.GetComponent<Collider>().isTrigger = true;
                    SetComponent(movingObject.GetComponent<Collider>(), movingParent);
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
        else if (Input.GetAxis("RightClick" + cM.playerIndex) > 0.1)
        {
            ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);

            if (movingObject != null)
            {
                Drop(true);
            }
        }
        //Otherwise if holding the moving object it can be moved closer or further from player
        else if (movingObject != null)
        {
            movingParent.transform.position = UnityEngine.Vector3.MoveTowards(movingParent.transform.position, position.position, 1000 * Time.deltaTime);
            //Zoom in
            if (Input.GetKey(KeyCode.RightBracket) || Input.GetKey(KeyCode.JoystickButton7))
            {
                position.position = UnityEngine.Vector3.MoveTowards(position.position, closestPosition.position, 5 * Time.deltaTime);
            }
            //Zoom out
            else if (Input.GetKey(KeyCode.LeftBracket) || Input.GetKey(KeyCode.JoystickButton6))
            {
                position.position = UnityEngine.Vector3.MoveTowards(position.position, furthestPosition.position, 5 * Time.deltaTime);
            }
        }
    }

    //This function will set the paramaters of the collision for a carried object based upon what type of colliders it is
    private void AdjustCollision()
    {
        if (movingParent.TryGetComponent(out Collider cD))
        {
            //Set collision parameters if box collider
            if (cD.GetType() == typeof(BoxCollider))
            {
                cD.GetComponent<BoxCollider>().size = new Vector3(cD.GetComponent<BoxCollider>().size.x * 0.5f, cD.GetComponent<BoxCollider>().size.y * 0.5f, cD.GetComponent<BoxCollider>().size.z * 0.5f);
            }
            //Set collision parameters if sphere collider
            else if (cD.GetType() == typeof(SphereCollider))
            {
                cD.GetComponent<SphereCollider>().radius = cD.GetComponent<SphereCollider>().radius * 0.5f;
            }
            else if (cD.GetType() == typeof(CapsuleCollider))
            {
                cD.GetComponent<CapsuleCollider>().height = cD.GetComponent<CapsuleCollider>().height * 0.5f;
                cD.GetComponent<CapsuleCollider>().radius = cD.GetComponent<CapsuleCollider>().radius * 0.5f;
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
                    rB.AddRelativeForce(transform.forward * throwStrength, ForceMode.Impulse);
                }
            }
            //Unassign moving object
            movingObject = null;
        }
        //Loop through the parent destroying it's components
        foreach (var comp in movingParent.GetComponents<Component>())
        {
            if (!(comp is Transform) && comp.GetType() != typeof(MoveObject))
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
}

