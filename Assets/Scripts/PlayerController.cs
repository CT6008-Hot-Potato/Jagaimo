/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///PlayerController.cs
///Developed by Charlie Bullock
///This class primarily acts as the main script for controlling the player movement and first/third person camera management.
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    #region Variables
    //Vector variable called mouseLook.
    UnityEngine.Vector2 mouseLook;
    //Vector variable called smoothV
    UnityEngine.Vector2 smoothV;
    [SerializeField]
    private bool thirdPersonView;
    [SerializeField]
    private float mouseSensitivity;
    //A private float called mouseSmoothing which can be value assigned from within the inspector.
    //Serialized boolean called mouseInvertXAxis.
    [SerializeField]
    private bool mouseInvertXAxis;
    //Serialized boolean called mouseInvertAxis
    [SerializeField]
    private bool mouseInvertYAxis;
    //Serialized first person camera
    [SerializeField]
    private Camera firstPersonCamera;
    //Serialized third person camera
    [SerializeField]
    private Camera thirdPersonCamera;
    //Zoom out position of third person camera
    public Transform otherCamPosition;
    public Vector3 firstPersonCamPosition;
    public bool shouldOverrideCam = false;
    public float otherCamSpeed = 2f;
    //Zoom out position of third person camera
    [SerializeField]
    private Transform zoomOutPosition;
    //Zoom out position of third person camera
    [SerializeField]
    private Transform zoomInPosition;
    [SerializeField]
    private Transform zoomPosition;
    //Rotation position
    [SerializeField]
    private Transform rotationPosition;
    private Rigidbody rb;
    private float speed;
    private float tempPitch;
    private float tempYaw;
    private float floorDistance;
    private float pitch;
    private float yaw;
    private float forwardBackward;
    private float leftRight;
    private int currentlyChecking;
    private bool grounded = false;
    private bool canJump = true;
    private RaycastHit hit;
    private Ray ray;
    #endregion
    public enum playerMovement
    {
        INTERACTING,
        CROUCHING,
        WALKING,
    }
    enum inputMethod
    {
        KEYBOARDMOUSE,
        CONTROLLER,
        VR,
        OTHER
    }

    public playerMovement pM;
    private inputMethod iM;

    // Start is called before the first frame update
    void Start()
    {
        Physics.queriesHitBackfaces = true;
        firstPersonCamPosition = firstPersonCamera.transform.localPosition;
        rb = GetComponent<Rigidbody>();
        pM = playerMovement.INTERACTING;
        iM = inputMethod.KEYBOARDMOUSE;
        floorDistance = GetComponent<CapsuleCollider>().bounds.extents.y;
        if (SerializeNullCheck())
        {
            Debug.Log("Missing serialize element!");
        }
        rb.freezeRotation = true;
        rb.useGravity = false;
    }

    private void OnCollisionStay(Collision collision)
    {
        grounded = true;
    }
    private bool SerializeNullCheck()
    {
        if (thirdPersonCamera != null || firstPersonCamera != null)
        {
            if (thirdPersonView && pM != playerMovement.INTERACTING)
            {
                thirdPersonCamera.GetComponent<Camera>().enabled = true;
                thirdPersonCamera.GetComponent<AudioListener>().enabled = true;
                firstPersonCamera.GetComponent<Camera>().enabled = false;
                firstPersonCamera.GetComponent<AudioListener>().enabled = false;
            }
            else
            {
                thirdPersonCamera.GetComponent<Camera>().enabled = false;
                thirdPersonCamera.GetComponent<AudioListener>().enabled = false;
                firstPersonCamera.GetComponent<Camera>().enabled = true;
                firstPersonCamera.GetComponent<AudioListener>().enabled = true;
            }
            return false;
        }
        else
        {
            return true;
        }
    }
    private void FixedUpdate()
    {
        if (grounded && pM != playerMovement.INTERACTING)
        {
            // Calculate how fast we should be moving
            Vector3 targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            targetVelocity = rotationPosition.TransformDirection(targetVelocity);
            targetVelocity *= speed;

            // Apply a force that attempts to reach our target velocity
            Vector3 velocity = rb.velocity;
            Vector3 velocityChange = (targetVelocity - velocity);
            velocityChange.x = Mathf.Clamp(velocityChange.x, -10, 10);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -10, 10);
            velocityChange.y = 0;
            rb.AddForce(velocityChange, ForceMode.VelocityChange);

            // Jump
            if (canJump && Input.GetButton("Jump"))
            {
                rb.velocity = new Vector3(velocity.x, JumpSpeed(), velocity.z);
            }
        }

        // We apply gravity manually for more tuning control
        rb.AddForce(new Vector3(0, -15 * rb.mass, 0));

        grounded = false;
    }

    private void Update()
    {

        //Function hecks and manages aspects of player movement relevant to movement type 
        MovementType();
        //Function for aspects of the player movement to if the camera is in third or first person mode
        CameraType();
        //Function does a few checks which need doing fairly regular but not constantly
        CheckInOrder();

    }

    public void ChangeMovement(int state)
    {
        switch (state)
        {
            case 0:
                pM = playerMovement.INTERACTING;
                break;
            case 1:
                pM = playerMovement.CROUCHING;
                break;
            case 2:
                pM = playerMovement.WALKING;
                break;
            default:
                Debug.Log("Given value for ChangeMovement is too high.");
                break;
        }
    }

    public void ChangeInput(int state)
    {
        switch (state)
        {
            case 0:
                iM = inputMethod.KEYBOARDMOUSE;
                break;
            case 1:
                iM = inputMethod.CONTROLLER;
                break;
            case 2:
                iM = inputMethod.VR;
                break;
            case 3:
                iM = inputMethod.OTHER;
                break;
            default:
                Debug.Log("Given value for ChangeInput is too high.");
                break;
        }
    }

    void CameraType()
    {
        if (pM != playerMovement.INTERACTING)
        {
            //Goes to first person mode and unlocks cursor when unlock pressed
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                pM = playerMovement.INTERACTING;
                UnityEngine.Cursor.lockState = CursorLockMode.None;
                thirdPersonCamera.GetComponent<Camera>().enabled = false;
                thirdPersonCamera.GetComponent<AudioListener>().enabled = false;
                firstPersonCamera.GetComponent<Camera>().enabled = true;
                firstPersonCamera.GetComponent<AudioListener>().enabled = true;
                otherCamPosition = null;
                shouldOverrideCam = false;
            }
            else if (UnityEngine.Cursor.lockState == CursorLockMode.None)
            {
                UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            }
            //X axis camera for either inveted or non inverted
            if (mouseInvertXAxis)
            {
                yaw -= mouseSensitivity * Input.GetAxis("Mouse X");
            }
            else
            {
                yaw += mouseSensitivity * Input.GetAxis("Mouse X");
            }
            //Y axis camera for either inveted or non inverted
            if (mouseInvertYAxis)
            {
                pitch -= mouseSensitivity * Input.GetAxis("Mouse Y");
                pitch = Mathf.Clamp(pitch, -70, 70);
            }
            else
            {
                pitch += mouseSensitivity * Input.GetAxis("Mouse Y");
                pitch = Mathf.Clamp(pitch, -70, 70);
            }
            if (thirdPersonView == true)
            {
                //Raycast when in third person checking if there is an obstacle between camera and player
                ray = thirdPersonCamera.ScreenPointToRay(Input.mousePosition);
                Physics.Raycast(ray, out hit, 55);
                //If an obstacle is found then zoom in
                if (hit.transform != null && hit.transform.tag != "Player")
                {
                    zoomPosition.position = UnityEngine.Vector3.MoveTowards(zoomPosition.position, zoomInPosition.position, 90 * Time.deltaTime);
                    if (UnityEngine.Vector3.Distance(zoomPosition.position, zoomInPosition.position) < 0.2f)
                    {
                        thirdPersonView = false;
                    }
                }
                else
                {
                    //Zoom in
                    if (Input.GetAxis("Mouse ScrollWheel") > 0f || Input.GetAxis("Zoom") > 0f)
                    {
                        zoomPosition.position = UnityEngine.Vector3.MoveTowards(zoomPosition.position, zoomInPosition.position, 30 * Time.deltaTime);
                        if (UnityEngine.Vector3.Distance(zoomPosition.position, zoomInPosition.position) < 0.2f)
                        {
                            thirdPersonView = false;
                        }
                    }
                    //Zoom out
                    else if (Input.GetAxis("Mouse ScrollWheel") < 0f || Input.GetAxis("Zoom") < 0f)
                    {
                        zoomPosition.position = UnityEngine.Vector3.MoveTowards(zoomPosition.position, zoomOutPosition.position, 30 * Time.deltaTime);
                    }

                    //Begins freecam movement
                    if (Input.GetMouseButtonDown(2) || Input.GetButtonDown("CameraRotate"))
                    {
                        tempPitch = pitch;
                        tempYaw = yaw;
                    }
                    //Ends freecam movement
                    else if (Input.GetMouseButtonUp(2) || Input.GetButtonUp("CameraRotate"))
                    {
                        pitch = tempPitch;
                        yaw = tempYaw;
                    }
                }

                //Added this so that it smooth lerps to a new zoom posiiton rather than imitadly set cam pos. this also fixes a cam issue when interacting 
                thirdPersonCamera.transform.position = Vector3.Lerp(thirdPersonCamera.transform.position, zoomPosition.position, otherCamSpeed * Time.deltaTime);

                if (Vector3.Angle(thirdPersonCamera.transform.localEulerAngles, new Vector3(15, 0, 0)) > 0.1f)
                {
                    thirdPersonCamera.transform.localRotation = Quaternion.Lerp(thirdPersonCamera.transform.localRotation, Quaternion.Euler(15, 0, 0), otherCamSpeed * Time.deltaTime);
                }
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f || Input.GetAxis("Zoom") < 0f)
            {
                if (pM == playerMovement.CROUCHING)
                {
                    GetComponent<CapsuleCollider>().height = 2;
                    firstPersonCamera.transform.localPosition = new UnityEngine.Vector3(0, 0.5f, 0);
                    pM = playerMovement.WALKING;
                }
                thirdPersonView = true;
            }

            if (thirdPersonView == false)
            {
                firstPersonCamera.transform.localPosition = Vector3.Lerp(firstPersonCamera.transform.localPosition, firstPersonCamPosition, otherCamSpeed * 0.5f * Time.deltaTime);
                firstPersonCamera.transform.localRotation = Quaternion.Lerp(firstPersonCamera.transform.localRotation, Quaternion.Euler(0, 0, 0), otherCamSpeed * 0.5f * Time.deltaTime);
            }
            //Vector3 currentRotation
            firstPersonCamera.transform.eulerAngles = new UnityEngine.Vector3(pitch, yaw, 0.0f);
            if (Input.GetMouseButton(2))
            {
                rotationPosition.rotation = UnityEngine.Quaternion.Euler(0, 0, 0);
            }
            else
            {
                rotationPosition.rotation = UnityEngine.Quaternion.Euler(0, firstPersonCamera.transform.localRotation.eulerAngles.y, 0);
            }
        }
        else if (shouldOverrideCam)
        {
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            if (thirdPersonView == true) // lets move third person cam
            {
                thirdPersonCamera.transform.position = Vector3.Lerp(thirdPersonCamera.transform.position, otherCamPosition.position, otherCamSpeed * Time.deltaTime);
                thirdPersonCamera.transform.rotation = Quaternion.Lerp(thirdPersonCamera.transform.rotation, otherCamPosition.rotation, otherCamSpeed * Time.deltaTime);
            }
            else // lets move first person cam
            {
                firstPersonCamera.transform.position = Vector3.Lerp(firstPersonCamera.transform.position, otherCamPosition.position, otherCamSpeed * Time.deltaTime);
                firstPersonCamera.transform.rotation = Quaternion.Lerp(firstPersonCamera.transform.rotation, otherCamPosition.rotation, otherCamSpeed * Time.deltaTime);
            }
        }
    }

    void MovementType()

    {
        switch (pM)
        {
            case playerMovement.INTERACTING:
                //Checks for player walking
                if (Input.anyKey)
                {
                    UnityEngine.Cursor.lockState = CursorLockMode.Locked;
                    if (thirdPersonView)
                    {
                        thirdPersonCamera.GetComponent<Camera>().enabled = false;
                        thirdPersonCamera.GetComponent<AudioListener>().enabled = false;
                        firstPersonCamera.GetComponent<Camera>().enabled = true;
                        firstPersonCamera.GetComponent<AudioListener>().enabled = true;
                    }
                    pM = playerMovement.WALKING;
                }
                else if (UnityEngine.Cursor.lockState == CursorLockMode.Locked)
                {
                    UnityEngine.Cursor.lockState = CursorLockMode.None;
                }
                break;
            case playerMovement.CROUCHING:
                if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetButtonDown("Crouch"))
                {
                    GetComponent<CapsuleCollider>().height = 2;
                    firstPersonCamera.transform.localPosition = new UnityEngine.Vector3(0, 0.5f, 0);
                    pM = playerMovement.WALKING;
                }
                else if (Input.GetButton("Sprint") && speed != 10)
                {
                    speed = 10;
                }
                else if (speed != 5)
                {
                    speed = 5;
                }
                break;
            case playerMovement.WALKING:
                if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetButtonDown("Crouch"))
                {
                    GetComponent<CapsuleCollider>().height = 1;
                    firstPersonCamera.transform.localPosition = new UnityEngine.Vector3(0, -0.25f, 0);
                    pM = playerMovement.CROUCHING;
                }
                else if (Input.GetButton("Sprint") && speed != 15)
                {
                    speed = 15;
                }
                else if (speed != 7.5f)
                {
                    speed = 7.5f;
                }
                break;
            default:
                Debug.Log("Given value for MovementType is too high.");
                break;
        }
    }

    private float JumpSpeed()
    {
        return Mathf.Sqrt(5 * 10);
    }

    void CheckInOrder()
    {
        switch (currentlyChecking)
        {
            case 0:
                if (pM != playerMovement.INTERACTING)
                {
                    if (thirdPersonView)
                    {
                        thirdPersonCamera.GetComponent<Camera>().enabled = true;
                        thirdPersonCamera.GetComponent<AudioListener>().enabled = true;
                        firstPersonCamera.GetComponent<Camera>().enabled = false;
                        firstPersonCamera.GetComponent<AudioListener>().enabled = false;
                    }
                    else if (thirdPersonView == false)
                    {
                        thirdPersonCamera.GetComponent<Camera>().enabled = false;
                        thirdPersonCamera.GetComponent<AudioListener>().enabled = false;
                        firstPersonCamera.GetComponent<Camera>().enabled = true;
                        firstPersonCamera.GetComponent<AudioListener>().enabled = true;
                    }
                }
                break;
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;
            case 5:
                break;
        }
        if (currentlyChecking >= 5)
        {
            currentlyChecking = 0;
        }
        else
        {
            ++currentlyChecking;
        }
    }
}

