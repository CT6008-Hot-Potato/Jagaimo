/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///MoveObject.cs
///Developed by Charlie Bullock
///This class is responsible for controlling the correct movement and parenting of rigidbody objects picked up by players.
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//This script is using the following:
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : MonoBehaviour
{
    //Variables
    #region Variables
    private GameObject movingObject;
    [SerializeField]
    private GameObject movingParent;
    private RaycastHit hit;
    private Ray ray;
    [SerializeField]
    private Transform closestPosition;
    [SerializeField]
    private Transform furthestPosition;
    [SerializeField]
    private Transform position;
    private float throwStrength;
    #endregion Variables
    //Start method setting up and assigning values
    void Start()
    {
        if (throwStrength == 0)
        {
            throwStrength = 15;
        }
        position.position = movingParent.transform.position;
        if (!movingObject || !movingObject.GetComponent<Rigidbody>())
        {
            return;
        }
        //Set the component Rigidbody's useGravity to true in the modelItem.
        movingObject.GetComponent<Rigidbody>().useGravity = true;

    }

    //OnMouseDown method.
    void Update()
    {
        //If the player has interacted
        if (Input.GetMouseButtonDown(0) || Input.GetAxis("LeftClick") > 0.1)
        {
            //Quickly enable the main camera regardless of if third or first person to do raycast
            if (GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().enabled)
            {
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            }
            else
            {
                GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().enabled = true;
                GameObject.FindGameObjectWithTag("ThirdPersonCamera").GetComponent<Camera>().enabled = false;
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                GameObject.FindGameObjectWithTag("ThirdPersonCamera").GetComponent<Camera>().enabled = true;
                GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().enabled = false;
            }
            //Do a raycast and check if the object needs to be dropped or picked up
            if (Physics.Raycast(ray, out hit, 5))
            {
                if (movingObject != null)
                {
                    Drop(false);
                }
                else if (hit.rigidbody != null)
                {
                    movingObject = hit.transform.gameObject;
                    movingObject.GetComponent<Rigidbody>().useGravity = false;
                    movingObject.GetComponent<Rigidbody>().isKinematic = true;
                    movingObject.transform.position = movingParent.transform.position;
                    movingObject.transform.rotation = movingParent.transform.rotation;
                    movingObject.transform.parent = movingParent.transform;
                    SetComponent(movingObject.GetComponent<Collider>(), movingParent);
                    AdjustCollision();
                    movingParent.GetComponent<Collider>().isTrigger = true;
                    SetComponent(movingObject.GetComponent<Collider>(), movingParent);
                    movingObject.GetComponent<Collider>().enabled = false;
                    movingParent.AddComponent<Rigidbody>();
                    movingParent.GetComponent<Rigidbody>().freezeRotation = true;
                    movingParent.GetComponent<Rigidbody>().isKinematic = false;
                    movingParent.GetComponent<Rigidbody>().useGravity = true;
                    //movingParent.GetComponenMt<Rigidbody>().mass = 0.000001f;
                    //movingParent.GetComponent<Rigidbody>().angularDrag = 0;                    
                    movingParent.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Continuous;
                    movingParent.AddComponent<CarryCollision>();
                }
            }

        }
        //Throw the object when right click or right trigger pressed
        else if (Input.GetMouseButtonDown(1) || Input.GetAxis("RightClick") > 0.1)
        {
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
        //Set collision parameters if box collider
        if (movingParent.GetComponent<Collider>().GetType() == typeof(BoxCollider))
        {
            movingParent.GetComponent<BoxCollider>().size = new Vector3(movingParent.GetComponent<BoxCollider>().size.x * 0.5f, movingParent.GetComponent<BoxCollider>().size.y * 0.5f, movingParent.GetComponent<BoxCollider>().size.z * 0.5f);
        }
        //Set collision parameters if sphere collider
        else if (movingParent.GetComponent<Collider>().GetType() == typeof(SphereCollider))
        {
            movingParent.GetComponent<SphereCollider>().radius = movingParent.GetComponent<SphereCollider>().radius * 0.5f;
        }
        //Debug if other type of collider
        else
        {
            Debug.Log("Other collider type");
        }
    }


    //Function which when called will drop the current object which is being carried 
    public void Drop(bool throwObject)
    {
        movingParent.transform.position = position.position;
        //Try to get rigidbody
        if (movingObject.TryGetComponent(out Rigidbody rB))
        {
            rB.useGravity = true;
            rB.isKinematic = false;
        }
        //Try to get collider
        if (movingObject.TryGetComponent(out Collider cD))
        {
            cD.enabled = true;
        }
        movingObject.transform.parent = null;
        movingObject.transform.position = movingParent.transform.position;
        //When object is being thrown first will check got rigidbody and then throw it
        if (throwObject && movingObject.GetComponent<Rigidbody>() != null)
        {
            movingObject.GetComponent<Rigidbody>().AddRelativeForce(transform.forward * throwStrength, ForceMode.Impulse);
        }
        //Unassign moving object
        movingObject = null;
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