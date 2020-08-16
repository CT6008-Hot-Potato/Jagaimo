/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///PlayerController.cs
///Developed by Charlie Bullock
///This class primarily acts as the main script for controlling the player movemeny and first/third person camera management.
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//This class is using:
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //Variables
    #region Variables
    [SerializeField] float minDist = 1;
    [SerializeField] float maxDist = 4f;
    [SerializeField] float maxZoomDist = 15f;
    [SerializeField] float smooth = 10.0f;
    [SerializeField] float speedZoom = 10.0f;
    [SerializeField] Vector3 dollyDir;
    [SerializeField] Vector3 dollyDirAdjusted;
    [SerializeField] float distance;


    //A private float called mouseSmoothing which can be value assigned from within the inspector.
    [SerializeField]
    private float mouseSensitivity;
    [SerializeField]
    private float velocityClamp = 10f;
    [SerializeField]
    private float jumpVelocity = 50;
    [SerializeField]
    private float speedMultiplier = 0.5f;
    [SerializeField]
    private float walkSpeed = 5f;
    [SerializeField]
    private float runSpeed = 7f;
    [SerializeField]
    private float crouchSpeed = 1.5f;
    [SerializeField]
    private float clampDegree = 70;
    //First person camera
    [SerializeField]
    public Camera firstPersonCamera;
    //Serialized third person camera
    [SerializeField]
    public Camera thirdPersonCamera;
    //Zoom out position of third person camera
    [SerializeField]
    private Transform zoomOutPosition;
    //Zoom in position of third person camera
    [SerializeField]
    private Transform zoomInPosition;
    [SerializeField]
    private Transform zoomPosition;
    public Vector3 firstPersonCamPosition;
    public float otherCamSpeed = 2f;
    //Rotation position
    [SerializeField]
    private Transform rotationPosition;
    private Rigidbody rb;
    private float speed;
    private float pitch;
    private float yaw;
    private bool grounded = false;
    private bool transitionPeriod = false;
    private RaycastHit hit;
    private Ray ray;
    //Audio listener
    private AudioListener firstPersonListener;
    private AudioListener thirdPersonListener;
    private CapsuleCollider collider;
    #endregion
    //Enums
    #region Enums
    //Enum for player movement type
    private enum pM
    {
        INTERACTING,
        CROUCHING,
        WALKING,
    }

    //Enum for mouse input type
    private enum mI
    {
        INVERTX,
        INVERTY,
        INVERTBOTH,
        INVERTNONE
    }
    //Enum for camera type
    public enum cS
    {
        FIRSTPERSON,
        THIRDPERSON,
        FREECAM,
        CHANGETOTHIRD,
        CHANGETOFIRST
    }
    //Enum variables
    private pM playerMovement;
    [SerializeField]
    private mI mouseInversion;
    [SerializeField]
    public cS cameraState;
    public cS previousCameraState;

    #endregion Enum
    //Start function sets up numerous aspects of the player ready for use
    void Start()
    {
        dollyDir = zoomPosition.localPosition.normalized;
        distance = zoomPosition.localPosition.magnitude;

        Physics.queriesHitBackfaces = true;
        firstPersonCamPosition = firstPersonCamera.transform.localPosition;
        playerMovement = pM.INTERACTING;
        rb = GetComponent<Rigidbody>();
        firstPersonListener = firstPersonCamera.GetComponent<AudioListener>();
        thirdPersonListener = thirdPersonCamera.GetComponent<AudioListener>();
        collider = GetComponent<CapsuleCollider>();
        //Set to first or third person
        if (cameraState != cS.FIRSTPERSON && playerMovement != pM.INTERACTING)
        {
            thirdPersonCamera.enabled = true;
            thirdPersonListener.enabled = true;
            firstPersonCamera.enabled = false;
            firstPersonListener.enabled = false;
        }
        else
        {
            thirdPersonCamera.enabled = false;
            thirdPersonListener.enabled = false;
            firstPersonCamera.enabled = true;
            firstPersonListener.enabled = true;
        }

        rb.freezeRotation = true;
        rb.useGravity = false;
    }
    //Function where collision checking if collider just staying on ground in order to determine if grounded or not is done
    private void OnCollisionStay(Collision collision)
    {
        //Get the direction the collision point from relative to position of player
        Vector3 dir = collision.contacts[0].point - transform.position;
        //Normalise it
        dir = -dir.normalized;
        //Check the normalised y is greater or equal to 0.9 (0.9 generally sprinting while 1.0 standing stil)
        if (dir.y >= 0.9f)
        {
            if (collision.gameObject.name == "CarryPosition")
            {
                GetComponent<MoveObject>().Drop(true);
            }
            grounded = true;
        }
    }

    //Fixed update function is responsible for jumping and player movement as these need to be done based on fixed update not update
    private void FixedUpdate()
    {
        //If player movement state isn't iiinteracting
        if (playerMovement != pM.INTERACTING)
        {
            //Calculate how fast player should be moving
            Vector3 targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            targetVelocity = rotationPosition.TransformDirection(targetVelocity);
            targetVelocity *= speed * speedMultiplier;

            // Apply a force that attempts to reach target velocity
            Vector3 velocity = rb.velocity;
            Vector3 velocityChange = (targetVelocity - velocity);
            velocityChange.x = Mathf.Clamp(velocityChange.x, -velocityClamp, velocityClamp);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -velocityClamp, velocityClamp);
            velocityChange.y = 0;
            rb.AddForce(velocityChange, ForceMode.VelocityChange);

            // Jump if grounded and input for jump pressed
            if (grounded && Input.GetButton("Jump"))
            {
                //Jumpine velocity for the player
                rb.velocity = new Vector3(velocity.x, Mathf.Sqrt(jumpVelocity), velocity.z);
            }
        }

        // We apply gravity manually for more tuning control
        rb.AddForce(new Vector3(0, -15 * rb.mass, 0));

        grounded = false;
    }
    //Update method calls camera type and movement type constantly
    private void Update()
    {
        //Function hecks and manages aspects of player movement relevant to movement type 
        MovementType();
        //Function for aspects of the player movement to if the camera is in third or first person mode
        CameraType();
    }
    //Simple public function for changing movement state
    public void ChangeMovement(int state)
    {
        //Movement states
        switch (state)
        {
            case 0:
                playerMovement = pM.INTERACTING;
                break;
            case 1:
                playerMovement = pM.CROUCHING;
                break;
            case 2:
                playerMovement = pM.WALKING;
                break;
            default:
                Debug.Log("Given value for ChangeMovement is too high.");
                break;
        }
    }
    //Camera type function which is responsible for managing the rotation and type of camera which the player utilises
    void CameraType()
    {
        if (playerMovement != pM.INTERACTING)
        {

            switch (cameraState)
            {
                //First person camera
                case cS.FIRSTPERSON:
                    //Check if wrong camera enabled and if so setup correct camera
                    if (thirdPersonCamera.enabled)
                    {
                        thirdPersonCamera.enabled = false;
                        thirdPersonListener.enabled = false;
                        firstPersonCamera.enabled = true;
                        firstPersonListener.enabled = true;
                    }
                    else
                    {
                        rotationPosition.rotation = UnityEngine.Quaternion.Euler(0, firstPersonCamera.transform.localRotation.eulerAngles.y, 0);
                        if (Input.GetAxis("Mouse ScrollWheel") < 0f || Input.GetAxis("Zoom") < 0f)
                        {
                            //Make sure that player not crouching      
                            collider.center = new Vector3(0, 0.35f, 0);
                            collider.height = 2.7f;
                            firstPersonCamera.transform.localPosition = new UnityEngine.Vector3(0, 1.17f, 0);
                            playerMovement = pM.WALKING;
                            //Raycast when in third person checking if there is an obstacle between camera and player
                            ray = firstPersonCamera.ScreenPointToRay(Input.mousePosition);
                            ray.direction *= -1;
                            Physics.Raycast(ray, out hit, 3);
                            //If an obstacle is found then zoom in
                            if (hit.transform == null)
                            {
                                zoomPosition.position = UnityEngine.Vector3.MoveTowards(zoomPosition.position, zoomInPosition.position, 90 * Time.deltaTime);
                                cameraState = cS.CHANGETOTHIRD;
                            }
                            else
                            {
                                zoomPosition.position = UnityEngine.Vector3.MoveTowards(zoomPosition.position, hit.point, 90 * Time.deltaTime);
                            }
                        }
                        firstPersonCamera.transform.localPosition = Vector3.Lerp(firstPersonCamera.transform.localPosition, firstPersonCamPosition, otherCamSpeed * 0.5f * Time.deltaTime);
                        firstPersonCamera.transform.localRotation = Quaternion.Lerp(firstPersonCamera.transform.localRotation, Quaternion.Euler(0, 0, 0), otherCamSpeed * 0.5f * Time.deltaTime);
                    }
                    break;
                //Third person camera
                case cS.THIRDPERSON:
                    //Check if wrong camera enabled and if so setup correct camera
                    if (firstPersonCamera.enabled)
                    {
                        EnableThirdPerson();
                    }
                    else
                    {
                        rotationPosition.rotation = UnityEngine.Quaternion.Euler(0, firstPersonCamera.transform.localRotation.eulerAngles.y, 0);
                        DoOnEitherThirdPersonMode();
                        //Begins freecam movement
                        if (Input.GetMouseButtonDown(2) || Input.GetButtonDown("CameraRotate"))
                        {
                            cameraState = cS.FREECAM;
                        }
                    }
                    break;
                //Free camera
                case cS.FREECAM:
                    //Check if wrong camera enabled and if so setup correct camera
                    if (firstPersonCamera.enabled)
                    {
                        EnableThirdPerson();
                    }
                    else
                    {
                        if (GetComponent<MoveObject>() && GetComponent<MoveObject>().enabled)
                            GetComponent<MoveObject>().Drop(false);
                        DoOnEitherThirdPersonMode();
                        if (Input.GetMouseButtonUp(2) || Input.GetButtonUp("CameraRotate"))
                        {
                            cameraState = cS.THIRDPERSON;
                        }
                    }
                    break;
                //Transition to third person camera
                case cS.CHANGETOTHIRD:
                    //If not almost at the same position continue to lerp towards the first person camera
                    if (transitionPeriod)
                    {
                        thirdPersonCamera.transform.position = Vector3.Lerp(thirdPersonCamera.transform.position, zoomInPosition.position, otherCamSpeed * Time.deltaTime);
                        if (Vector3.Distance(thirdPersonCamera.transform.position, zoomInPosition.position) <= 0.01f)
                        {
                            cameraState = cS.THIRDPERSON;
                            transitionPeriod = false;
                            Vector3 rotation = thirdPersonCamera.transform.rotation.eulerAngles;
                            rotation.x = 15;
                            thirdPersonCamera.transform.rotation = Quaternion.Euler(rotation);

                        }
                    }
                    else
                    {
                        thirdPersonCamera.transform.position = firstPersonCamera.transform.position;
                        thirdPersonCamera.transform.rotation = firstPersonCamera.transform.rotation;
                        thirdPersonCamera.enabled = true;
                        firstPersonCamera.enabled = false;
                        transitionPeriod = true;
                    }
                    break;
                //Transition to first person camera
                case cS.CHANGETOFIRST:
                    //If not almost at the same position continue to lerp towards the third person camera
                    if (transitionPeriod)
                    {
                        thirdPersonCamera.transform.position = Vector3.Lerp(thirdPersonCamera.transform.position, firstPersonCamera.transform.position, otherCamSpeed * Time.deltaTime);
                        if (Vector3.Distance(thirdPersonCamera.transform.position, firstPersonCamera.transform.position) <= 0.01f)
                        {
                            cameraState = cS.FIRSTPERSON;
                            transitionPeriod = false;
                        }
                    }
                    else
                    {
                        thirdPersonCamera.transform.position = zoomInPosition.position;
                        thirdPersonCamera.transform.rotation = zoomInPosition.rotation;
                        transitionPeriod = true;
                    }
                    break;
                default:
                    Debug.Log("Given value too high.");
                    break;

            }

            //Goes to first person mode and unlocks cursor when unlock pressed
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                playerMovement = pM.INTERACTING;
                UnityEngine.Cursor.lockState = CursorLockMode.None;
                thirdPersonCamera.enabled = false;
                thirdPersonListener.enabled = false;
                firstPersonCamera.enabled = true;
                firstPersonListener.enabled = true;
            }
            else if (UnityEngine.Cursor.lockState == CursorLockMode.None)
            {
                UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            }

            //X & Y axis camera can be either inverted or not
            switch (mouseInversion)
            {
                //InvertX
                case mI.INVERTX:
                    yaw -= mouseSensitivity * Input.GetAxis("Mouse X");
                    pitch += mouseSensitivity * Input.GetAxis("Mouse Y");
                    pitch = Mathf.Clamp(pitch, -clampDegree, clampDegree);
                    break;
                //InvertY
                case mI.INVERTY:
                    yaw += mouseSensitivity * Input.GetAxis("Mouse X");
                    pitch -= mouseSensitivity * Input.GetAxis("Mouse Y");
                    pitch = Mathf.Clamp(pitch, -clampDegree, clampDegree);
                    break;
                //Both
                case mI.INVERTBOTH:
                    yaw -= mouseSensitivity * Input.GetAxis("Mouse X");
                    pitch -= mouseSensitivity * Input.GetAxis("Mouse Y");
                    pitch = Mathf.Clamp(pitch, -clampDegree, clampDegree);
                    break;
                //None
                case mI.INVERTNONE:
                    yaw += mouseSensitivity * Input.GetAxis("Mouse X");
                    pitch += mouseSensitivity * Input.GetAxis("Mouse Y");
                    pitch = Mathf.Clamp(pitch, -clampDegree, clampDegree);
                    break;
            }
            //Vector3 currentRotation
            firstPersonCamera.transform.eulerAngles = new UnityEngine.Vector3(pitch, yaw, 0.0f);

        }
        else
        {
            //Enable correct camera based on state
            switch (cameraState)
            {
                //First person camera
                case cS.FIRSTPERSON:
                    thirdPersonCamera.enabled = false;
                    firstPersonCamera.enabled = true;
                    break;
                //Third person camera
                case cS.THIRDPERSON:
                    thirdPersonCamera.enabled = true;
                    firstPersonCamera.enabled = false;
                    break;
                default:
                    Debug.Log("Different value given.");
                    break;
            }

        }
    }

    //Function for the movement types if the player
    void MovementType()
    {
        //Switch for different movement types
        switch (playerMovement)
        {
            case pM.INTERACTING:
                //Checks for player walking
                if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
                {

                    //Unlock cursor
                    UnityEngine.Cursor.lockState = CursorLockMode.Locked;
                    playerMovement = pM.WALKING;
                }
                else if (UnityEngine.Cursor.lockState == CursorLockMode.Locked)
                {
                    UnityEngine.Cursor.lockState = CursorLockMode.None;
                }
                break;
            //Crouching
            case pM.CROUCHING:
                speed = crouchSpeed;
                if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetButtonDown("Crouch"))
                {
                    speed = walkSpeed;
                    collider.center = new Vector3(0, 0.35f, 0);
                    collider.height = 2.7f;
                    firstPersonCamera.transform.localPosition = new UnityEngine.Vector3(0, 1.17f, 0);
                    playerMovement = pM.WALKING;
                }

                break;
            //Walking
            case pM.WALKING:
                if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetButtonDown("Crouch"))
                {
                    speed = crouchSpeed;
                    collider.center = new Vector3(0, 0.32f, 0);
                    collider.height = 1.6f;
                    firstPersonCamera.transform.localPosition = new UnityEngine.Vector3(0, 0.91f, 0);
                    playerMovement = pM.CROUCHING;
                }
                else if (Input.GetButton("Sprint"))
                {
                    speed = runSpeed;
                }
                else if (!Input.GetButton("Sprint"))
                {
                    speed = walkSpeed;
                }
                break;
            default:
                Debug.Log("Given value for MovementType is too high.");
                break;
        }
    }

    //This function managed zooming and mesh clipping avoidance for the third person camera if it is 
    void DoOnEitherThirdPersonMode()
    {
        //Zoom in
        maxDist += -Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * speedZoom;
        maxDist = Mathf.Clamp(maxDist, 0, maxZoomDist);


        Vector3 desiredCamPos = firstPersonCamera.transform.TransformPoint(dollyDir * maxDist);
        RaycastHit hit;

        if (Physics.Linecast(firstPersonCamera.transform.position, desiredCamPos, out hit) && hit.collider.tag != "Player" && !hit.collider.isTrigger)
        {
            distance = Mathf.Clamp((hit.distance * 0.8f), minDist, maxDist);
        }
        else
        {
            distance = maxDist;
        }
        zoomPosition.position = Vector3.Lerp(zoomPosition.position, firstPersonCamera.transform.position + ((thirdPersonCamera.transform.position - firstPersonCamera.transform.position).normalized * distance), Time.deltaTime * smooth);


        if (distance <= 0.1f)
        {
            cameraState = cS.CHANGETOFIRST;
        }


        //Added this so that it smooth lerps to a new zoom posiiton rather than imitadly set cam pos. this also fixes a cam issue when interacting 
        thirdPersonCamera.transform.position = Vector3.Lerp(thirdPersonCamera.transform.position, zoomPosition.position, otherCamSpeed * Time.deltaTime);

        if (Vector3.Angle(thirdPersonCamera.transform.localEulerAngles, new Vector3(15, 0, 0)) > 0.1f)
        {
            thirdPersonCamera.transform.localRotation = Quaternion.Lerp(thirdPersonCamera.transform.localRotation, Quaternion.Euler(15, 0, 0), otherCamSpeed * Time.deltaTime);
        }
    }

    //Function to enable third person mode camera, vfx and audio listener
    void EnableThirdPerson()
    {
        thirdPersonCamera.enabled = true;
        thirdPersonListener.enabled = true;
        firstPersonCamera.enabled = false;
        firstPersonListener.enabled = false;
    }
}