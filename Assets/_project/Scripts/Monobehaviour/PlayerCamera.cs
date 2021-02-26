/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///PlayerCamera.cs
///Developed by Charlie Bullock
///This class is responsible for first/third person camera of the player.
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    //Variables
    #region Variables
    [SerializeField]
    private float clampDegree = 70;
    [SerializeField]
    private float cameraPushSpeed = 120;
    [SerializeField]
    private float cameraZoomSpeed;
    [SerializeField]
    private float cameraTransferDistance = 0.05f;
    //First person camera
    [SerializeField]
    public Camera firstPersonCamera;
    //Serialized third person camera
    [SerializeField]
    public Camera thirdPersonCamera;
    //Zoom out position of third person camera
    [SerializeField]
    private Transform zoomOutPosition;
    //Player index
    [SerializeField]
    private float playerIndex;
    //Zoom in position of third person camera
    [SerializeField]
    private Transform zoomInPosition;
    [SerializeField]
    private Transform zoomPosition;
    private RaycastHit hit;
    private Ray ray;
    //Audio listener
    private AudioListener firstPersonListener;
    private AudioListener thirdPersonListener;
    //Player movement
    private PlayerController pC;
    public Vector3 firstPersonCamPosition;
    public float otherCamSpeed = 2f;
    //Rotation position
    [SerializeField]
    private Transform rotationPosition;
    private CapsuleCollider collider;
    [SerializeField]
    private float mouseSensitivity;
    private float pitch;
    private float yaw;
    private CharacterManager cM;
    [SerializeField]
    private PlayerInput playerInput = null;
    public PlayerInput PlayerInput => playerInput;
    private float zoomValue = 0;
    private float cameraRotateValue = 0;
    private float escapeValue = 0;
    private Vector2 cameraValue = Vector2.zero;
    #endregion Variables

    #region Enums
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
    }
    [SerializeField]
    private mI mouseInversion;
    [SerializeField]
    public cS cameraState;
    #endregion Enums

    public void SetCameraView(bool firstPerson)
    {
        if (firstPerson)
        {
            cameraState = cS.FIRSTPERSON;
        }
        else
        {
            cameraState = cS.THIRDPERSON;
        }
    }

    // Assigning audio listeners, setting correct camera state and making sure queriesHitBackfaces is true for raycasting later
    void Start()
    {
        cM = GetComponent<CharacterManager>();
        Physics.queriesHitBackfaces = true;
        firstPersonCamPosition = firstPersonCamera.transform.localPosition;
        firstPersonListener = firstPersonCamera.GetComponent<AudioListener>();
        thirdPersonListener = thirdPersonCamera.GetComponent<AudioListener>();
        collider = GetComponent<CapsuleCollider>();
        pC = GetComponent<PlayerController>();
        //Set to first or third person
        if (cameraState != cS.FIRSTPERSON)
        {
            thirdPersonCamera.enabled = true;
            firstPersonCamera.enabled = false;
            if (playerIndex == 0)
            {
                thirdPersonListener.enabled = true;
                firstPersonListener.enabled = false;
            }
        }
        else
        {
            thirdPersonCamera.enabled = false;
            firstPersonCamera.enabled = true;
            if (playerIndex == 0)
            {
                thirdPersonListener.enabled = false;
                firstPersonListener.enabled = true;
            }
        }
        Debug.Log("This player" + playerIndex);
        SetupCameraAspectRatio();
    }

    private void SetupCameraAspectRatio()
    {
        switch (cM.playerIndex - 2)
        {
            case 0:
                if (playerIndex == 0)
                {
                    firstPersonCamera.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
                    thirdPersonCamera.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
                }
                else
                {
                    Debug.Log("Value too high");
                }
                break;
            case 1:
                switch (playerIndex)
                {
                    case 0:
                        firstPersonCamera.rect = new Rect(0.0f, 0.5f, 1.0f, 0.5f);
                        thirdPersonCamera.rect = new Rect(0.0f, 0.5f, 1.0f, 0.5f);
                        break;
                    case 1:
                        firstPersonCamera.rect = new Rect(0.0f, 0.0f, 1.0f, 0.5f);
                        thirdPersonCamera.rect = new Rect(0.0f, 0.0f, 1.0f, 0.5f);
                        break;
                    default:
                        Debug.Log("Value too high");
                        break;
                }
                break;
            case 2:
                switch (playerIndex)
                {
                    case 0:
                        firstPersonCamera.rect = new Rect(0.0f, 0.5f, 1.0f, 0.5f);
                        thirdPersonCamera.rect = new Rect(0.0f, 0.5f, 1.0f, 0.5f);
                        break;
                    case 1:
                        firstPersonCamera.rect = new Rect(0.0f, 0.0f, 0.5f, 0.5f);
                        thirdPersonCamera.rect = new Rect(0.0f, 0.0f, 0.5f, 0.5f);
                        break;
                    case 2:
                        firstPersonCamera.rect = new Rect(0.5f, 0.0f, 0.5f, 0.5f);
                        thirdPersonCamera.rect = new Rect(0.5f, 0.0f, 0.5f, 0.5f);
                        break;
                    default:
                        Debug.Log("Value too high");
                        break;
                }
                break;
            case 3:
                switch (playerIndex)
                {
                    case 0:
                        firstPersonCamera.rect = new Rect(0.0f, 0.5f, 0.5f, 0.5f);
                        thirdPersonCamera.rect = new Rect(0.0f, 0.5f, 0.5f, 0.5f);
                        break;
                    case 1:
                        firstPersonCamera.rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
                        thirdPersonCamera.rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
                        break;
                    case 2:
                        firstPersonCamera.rect = new Rect(0.0f, 0.0f, 0.5f, 0.5f);
                        thirdPersonCamera.rect = new Rect(0.0f, 0.0f, 0.5f, 0.5f);
                        break;
                    case 3:
                        firstPersonCamera.rect = new Rect(0.5f, 0.0f, 0.5f, 0.5f);
                        thirdPersonCamera.rect = new Rect(0.5f, 0.0f, 0.5f, 0.5f);
                        break;
                    default:
                        Debug.Log("Value too high");
                        break;
                }
                break;
        }
    }

    void Update()
    {
        //Function for aspects of the player movement to if the camera is in third or first person mode
        CameraType();
    }
    //Called to crouch the player
    public void Crouch()
    {
        firstPersonCamera.transform.localPosition = new UnityEngine.Vector3(0, 0, 0);
        zoomInPosition.localPosition = Vector3.zero;
    }
    //Called to uncrouch the player
    public void UnCrouch()
    {
        firstPersonCamera.transform.localPosition = new UnityEngine.Vector3(0, 0.75f, 0);
        zoomInPosition.localPosition = Vector3.zero;
    }


    //Camera type function which is responsible for managing the rotation and type of camera which the player utilises
    void CameraType()
    {
        if (pC.GetMovement() != 0)
        {
            switch (cameraState)
            {
                //First person camera
                case cS.FIRSTPERSON:
                    //Check if wrong camera enabled and if so setup correct camera
                    if (thirdPersonCamera.enabled)
                    {
                        thirdPersonCamera.enabled = false;
                        firstPersonCamera.enabled = true;
                        if (cM.playerIndex == 0)
                        {
                            thirdPersonListener.enabled = false;
                            firstPersonListener.enabled = true;
                        }
                    }
                    else
                    {
                        rotationPosition.rotation = UnityEngine.Quaternion.Euler(0, firstPersonCamera.transform.localRotation.eulerAngles.y, 0);
                        if (zoomValue < 0f)
                        {
                            //Make sure that player not crouching      
                            collider.center = new Vector3(0, 0, 0);
                            collider.height = 2f;
                            firstPersonCamera.transform.localPosition = new UnityEngine.Vector3(0, 0.75f, 0);
                            zoomInPosition.localPosition = Vector3.zero;
                            GetComponent<PlayerController>().SetMovement(0);
                            //Raycast when in third person checking if there is an obstacle between camera and player
                            ray = firstPersonCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
                            ray.direction *= -1;
                            Physics.Raycast(ray, out hit, 3);
                            //If an obstacle is found then zoom in
                            if (hit.transform == null)
                            {
                                zoomPosition.position = UnityEngine.Vector3.MoveTowards(zoomPosition.position, zoomInPosition.position, 90 * Time.deltaTime);
                                cameraState = cS.THIRDPERSON;
                            }
                            else
                            {
                                zoomPosition.position = UnityEngine.Vector3.MoveTowards(zoomPosition.position, hit.point, 90 * Time.deltaTime);
                            }
                        }
                        //firstPersonCamera.transform.localPosition = Vector3.Lerp(firstPersonCamera.transform.localPosition, firstPersonCamPosition, otherCamSpeed * 0.5f * Time.deltaTime);
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
                        if (cameraRotateValue > 0.1f)
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
                        if (cameraRotateValue <= 0.1f)
                        {
                            cameraState = cS.THIRDPERSON;
                        }
                    }
                    break;
            }

            //Goes to first person mode and unlocks cursor when unlock pressed
            if (escapeValue > 0.1f)
            {
                pC.SetMovement(0);
                UnityEngine.Cursor.lockState = CursorLockMode.None;
                thirdPersonCamera.enabled = false;
                firstPersonCamera.enabled = true;
                if (cM.playerIndex == 0)
                {
                    thirdPersonListener.enabled = false;
                    firstPersonListener.enabled = true;
                }
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
                    yaw += mouseSensitivity * cameraValue.x;
                    pitch += mouseSensitivity * cameraValue.y;
                    pitch = Mathf.Clamp(pitch, -clampDegree, clampDegree);
                    break;
                //InvertY
                case mI.INVERTY:
                    yaw -= mouseSensitivity * cameraValue.x;
                    pitch -= mouseSensitivity * cameraValue.y;
                    pitch = Mathf.Clamp(pitch, -clampDegree, clampDegree);
                    break;
                //Both
                case mI.INVERTBOTH:
                    yaw -= mouseSensitivity * cameraValue.x;
                    pitch += mouseSensitivity * cameraValue.y;
                    pitch = Mathf.Clamp(pitch, -clampDegree, clampDegree);
                    break;
                //None
                case mI.INVERTNONE:
                    yaw += mouseSensitivity * cameraValue.x;
                    pitch -= mouseSensitivity * cameraValue.y;
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
                    if (cM.playerIndex == 0)
                    {
                        thirdPersonListener.enabled = false;
                        firstPersonListener.enabled = true;
                    }
                    break;
                //Third person camera
                case cS.THIRDPERSON:
                    thirdPersonCamera.enabled = true;
                    firstPersonCamera.enabled = false;
                    if (cM.playerIndex == 0)
                    {
                        firstPersonListener.enabled = false;
                        thirdPersonListener.enabled = true;
                    }
                    break;
                default:
                    Debug.Log("Different value given.");
                    break;
            }

        }
    }

    //This function managed zooming and mesh clipping avoidance for the third person camera if it is 
    void DoOnEitherThirdPersonMode()
    {
        //if (thirdPersonCamera.transform.rotation.x < 15)
        //{
        //    thirdPersonCamera.transform.localRotation = Quaternion.Lerp(thirdPersonCamera.transform.rotation, Quaternion.Euler(15, thirdPersonCamera.transform.rotation.y, thirdPersonCamera.transform.rotation.z), 1);
        //}
        //If an obstacle is found then zoom in
        if (Physics.Linecast(firstPersonCamera.transform.position, zoomPosition.transform.position, out hit) && hit.collider.gameObject != gameObject && !hit.collider.isTrigger)
        {
            zoomPosition.position = UnityEngine.Vector3.MoveTowards(zoomPosition.position, zoomInPosition.position, cameraPushSpeed * Time.deltaTime);
            if (UnityEngine.Vector3.Distance(zoomPosition.position, zoomInPosition.position) < cameraTransferDistance)
            {
                cameraState = cS.FIRSTPERSON;
            }
        }
        else
        {
            //Zoom in
            if (zoomValue > 0f)
            {
                zoomPosition.position = UnityEngine.Vector3.MoveTowards(zoomPosition.position, zoomInPosition.position, cameraZoomSpeed/* * Time.deltaTime*/);
                if (UnityEngine.Vector3.Distance(zoomPosition.position, zoomInPosition.position) < cameraTransferDistance)
                {
                    cameraState = cS.FIRSTPERSON;
                    transform.localRotation = Quaternion.Euler(0, thirdPersonCamera.transform.rotation.y, thirdPersonCamera.transform.rotation.z);
                }
            }
            //Zoom out
            else if (zoomValue < 0f)
            {
                zoomPosition.position = UnityEngine.Vector3.MoveTowards(zoomPosition.position, zoomOutPosition.position, cameraZoomSpeed/*/* * Time.deltaTime*/);
            }
            //Added this so that it smooth lerps to a new zoom posiiton rather than imitadly set cam pos. this also fixes a cam issue when interacting 
            thirdPersonCamera.transform.position = Vector3.Lerp(thirdPersonCamera.transform.position, zoomPosition.position, otherCamSpeed * Time.deltaTime);

            if (Vector3.Angle(thirdPersonCamera.transform.localEulerAngles, new Vector3(15, 0, 0)) > 0.1f)
            {
                thirdPersonCamera.transform.localRotation = Quaternion.Lerp(thirdPersonCamera.transform.localRotation, Quaternion.Euler(15, 0, 0), otherCamSpeed * Time.deltaTime);
            }
        }
    }

    //Function to enable third person mode camera, vfx and audio listener
    void EnableThirdPerson()
    {
        thirdPersonCamera.enabled = true;
        firstPersonCamera.enabled = false;
        if (cM.playerIndex == 0)
        {
            thirdPersonListener.enabled = true;
            firstPersonListener.enabled = false;
        }
    }

    public void Camera(InputAction.CallbackContext ctx)
    {
        cameraValue = new Vector3(ctx.ReadValue<Vector2>().x, ctx.ReadValue<Vector2>().y, 0);
    }

    public void CameraZoom(InputAction.CallbackContext ctx)
    {
        zoomValue = ctx.ReadValue<float>();
    }

    public void CameraRotate(InputAction.CallbackContext ctx)
    {
        cameraRotateValue = ctx.ReadValue<float>();
    }

    public void Escape(InputAction.CallbackContext ctx)
    {
        escapeValue = ctx.ReadValue<float>();
    }
}