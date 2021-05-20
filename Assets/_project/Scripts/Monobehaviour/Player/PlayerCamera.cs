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
    public Camera firstPersonCamera;
    //Serialized third person camera
    public Camera thirdPersonCamera;
    //Zoom out position of third person camera
    [SerializeField]
    private Transform zoomOutPosition;
    //Player index
    public int playerIndex = 0;
    //Zoom in position of third person camera
    [SerializeField]
    private Transform zoomInPosition;
    [SerializeField]
    private Transform zoomPosition;
    private RaycastHit hit;
    private Ray ray;
    //Player movement
    private PlayerController pC;
    public Vector3 firstPersonCamPosition;
    public float otherCamSpeed = 2f;
    //Rotation position
    [SerializeField]
    private Transform rotationPosition;
    private CapsuleCollider collider;
    //Pitch and yaw speed for camera
    private float pitch;
    private float yaw;
    private LocalMPScreenPartioning cM;
    [SerializeField]
    private PlayerInput playerInput = null;
    public PlayerInput PlayerInput => playerInput;
    //For input system 
    private float zoomValue = 0;
    private float cameraRotateValue = 0;
    private float escapeValue = 0;
    private Vector2 cameraValue = Vector2.zero;
    //Character related 
    [SerializeField]
    private Mesh[] characterMesh;
    [SerializeField]
    private GameObject character;
    [SerializeField]
    private GameObject characterArms;
    [SerializeField]
    private Material[] materials;
    [SerializeField]
    private LayerMask[] mask;
    [SerializeField]
    private GameObject playerThirdPerson;
    private SkinnedMeshRenderer characterSkinnedMesh;
    private SkinnedMeshRenderer characterArmsSkinnedMesh;
    //First and third person cameras
    private Vector3 cameraMovementValue = Vector3.zero;
    private Vector3 freecamRotation;
    private float freeCamValueY;
    public bool flipSpin;
    public bool freecamLock = false;
    public bool cameraRotationLock = false;
    public float cameraSensitivity = 1;
    //Amplify camera sensitivity specific to controllers
    public float controllerCameraSensitivityMultiplier = 3;
    [SerializeField]
    private GameObject crosshair;   
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
        FREECAMCONSTRAINED,
        FREECAMUNCONSTRAINED
    }
    [SerializeField]
    private mI mouseInversion;
    [SerializeField]
    public cS cameraState;
    #endregion Enums
    
    //This method sets the camera view
    public void SetCameraView(bool firstPerson)
    {
        if (firstPerson)
        {
            cameraState = cS.FIRSTPERSON;
            crosshair.SetActive(true);
        }
        else
        {
            cameraState = cS.THIRDPERSON;
            crosshair.SetActive(false);
        }
    }

    // Assigning audio listeners, setting correct camera state and making sure queriesHitBackfaces is true for raycasting later
    void Start()
    {
        characterArmsSkinnedMesh = characterArms.GetComponent<SkinnedMeshRenderer>();
        characterSkinnedMesh = character.GetComponent<SkinnedMeshRenderer>();
        cM = GetComponent<LocalMPScreenPartioning>();
        Physics.queriesHitBackfaces = true;
        firstPersonCamPosition = firstPersonCamera.transform.localPosition;
        collider = GetComponent<CapsuleCollider>();
        pC = GetComponent<PlayerController>();

        //Set to first person
        if (cameraState != cS.FIRSTPERSON)
        {
            thirdPersonCamera.enabled = true;
            firstPersonCamera.enabled = false;
        }
        //Set to third person
        else
        {
            thirdPersonCamera.enabled = false;
            firstPersonCamera.enabled = true;
        }

        SetPlayerMask();
    }

    //Function for setting the correct player mask up
    public void SetPlayerMask()
    { 
        character.layer = 9 + playerIndex;
        characterArms.layer = 13 + playerIndex;
        if (!characterSkinnedMesh || !characterArmsSkinnedMesh)
        {
            characterArmsSkinnedMesh = characterArms.GetComponent<SkinnedMeshRenderer>();
            characterSkinnedMesh = character.GetComponent<SkinnedMeshRenderer>();
        }
        characterSkinnedMesh.sharedMesh = characterMesh[playerIndex];
        characterArmsSkinnedMesh.material = characterSkinnedMesh.material = materials[playerIndex];
        firstPersonCamera.cullingMask = mask[playerIndex + 4];
        thirdPersonCamera.cullingMask = mask[playerIndex];
    }


    //Update function calles camera type function constantly
    void Update()
    {
        //Function for aspects of the player movement to if the camera is in third or first person mode
        CameraType();
    }

    //Function to set yaw valye
    public void ChangeYaw(float timeMultiplier)
    {
        if (flipSpin)
        {
            yaw = yaw - Time.deltaTime * timeMultiplier;
        }
        else
        {
            yaw = yaw + Time.deltaTime * timeMultiplier;
        }
    }

    //Camera type function which is responsible for managing the rotation and type of camera which the player utilises
    void CameraType()
    {        

        if (pC.GetMovement() != 0 && cameraRotationLock == false)
        {
            switch (cameraState)
            {
                //First person camera
                case cS.FIRSTPERSON:
                    //Check if wrong camera enabled and if so setup correct camera
                    if (thirdPersonCamera.enabled)
                    {
                        crosshair.SetActive(true);
                        thirdPersonCamera.enabled = false;
                        firstPersonCamera.enabled = true;
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
                            //Raycast when in third person checking if there is an obstacle between camera and player
                            ray = firstPersonCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
                            ray.direction *= -1;
                            Physics.Raycast(ray, out hit, 3);
                            //If an obstacle is found then zoom in
                            if (hit.transform == null)
                            {
                                zoomPosition.position = UnityEngine.Vector3.MoveTowards(zoomPosition.position, zoomOutPosition.position, 90 * Time.deltaTime);
                                cameraState = cS.THIRDPERSON;
                            }
                            else
                            {
                                zoomPosition.position = UnityEngine.Vector3.MoveTowards(zoomPosition.position, hit.point, 90 * Time.deltaTime);
                            }
                        }
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
                            cameraState = cS.FREECAMCONSTRAINED;
                        }
                    }
                    break;
                //Free camera constrainged
                case cS.FREECAMCONSTRAINED:
                    //Check if wrong camera enabled and if so setup correct camera
                    if (firstPersonCamera.enabled)
                    {
                        EnableThirdPerson();
                    }
                    else
                    {
                        //Drop a picked up object if currently being held when in this state
                        if (GetComponent<PlayerInteraction>() && GetComponent<PlayerInteraction>().enabled)
                            GetComponent<PlayerInteraction>().Drop(false);
                        DoOnEitherThirdPersonMode();
                        if (cameraRotateValue <= 0.1f)
                        {
                            cameraState = cS.THIRDPERSON;
                        }
                        else if (cameraMovementValue != Vector3.zero)
                        {
                            thirdPersonCamera.gameObject.AddComponent<SphereCollider>().radius = 0.25f;
                            freecamRotation = thirdPersonCamera.transform.rotation.eulerAngles;
                            cameraState = cS.FREECAMUNCONSTRAINED;
                            thirdPersonCamera.gameObject.AddComponent<Rigidbody>().useGravity = false;
                            thirdPersonCamera.gameObject.AddComponent<CameraCollision>();
                        }
                    }
                    break;
                //Free camera unconstrained
                case cS.FREECAMUNCONSTRAINED:
                    //If freecam button lifted up and freecam is not locked go back to third person
                    if (cameraRotateValue <= 0.1f && !freecamLock)
                    {
                        //If there is a rigidbody attached to the camera destroy it and destroy sphere collider
                        if (thirdPersonCamera.gameObject.GetComponent<Rigidbody>() != null)
                        {
                            Destroy(thirdPersonCamera.gameObject.GetComponent<Rigidbody>());
                            Destroy(thirdPersonCamera.gameObject.GetComponent<SphereCollider>());
                            Destroy(thirdPersonCamera.gameObject.GetComponent<CameraCollision>());
                        }
                        thirdPersonCamera.transform.rotation = Quaternion.Euler(freecamRotation);
                        Destroy(thirdPersonCamera.gameObject.GetComponent<SphereCollider>());
                        cameraState = cS.THIRDPERSON;
                    }   
                    //Else if beyond 200 metres from centre of map add rigidbody & collider
                    else if (Vector3.Distance(thirdPersonCamera.transform.position,Vector3.zero) > 200)
                    {
                        thirdPersonCamera.gameObject.GetComponent<Rigidbody>().useGravity = true;
                        thirdPersonCamera.gameObject.GetComponent<Rigidbody>().mass = 10000;
                    }
                    //Remove it and sphere collider when within 150 metres of the centre of map
                    if (Vector3.Distance(thirdPersonCamera.transform.position, Vector3.zero) < 150)
                    {
                        thirdPersonCamera.gameObject.GetComponent<Rigidbody>().useGravity = false;
                    }
                    //Moving third person camera position around freely
                    thirdPersonCamera.transform.position = new Vector3(thirdPersonCamera.transform.position.x, thirdPersonCamera.transform.position.y + freeCamValueY, thirdPersonCamera.transform.position.z);
                    thirdPersonCamera.transform.Translate(cameraMovementValue * 0.1f,thirdPersonCamera.transform);                   
                    break;
            }

            //Goes to first person mode and unlocks cursor when unlock pressed
            if (escapeValue > 0.1f)
            {
                pC.SetMovement(0);
                UnityEngine.Cursor.lockState = CursorLockMode.None;                
                
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
                    yaw += cameraSensitivity * cameraValue.x;
                    pitch += cameraSensitivity * cameraValue.y;
                    pitch = Mathf.Clamp(pitch, -clampDegree, clampDegree);
                    break;
                //InvertY
                case mI.INVERTY:
                    yaw -= cameraSensitivity * cameraValue.x;
                    pitch -= cameraSensitivity * cameraValue.y;
                    pitch = Mathf.Clamp(pitch, -clampDegree, clampDegree);
                    break;
                //Both
                case mI.INVERTBOTH:
                    yaw -= cameraSensitivity * cameraValue.x;
                    pitch += cameraSensitivity * cameraValue.y;
                    pitch = Mathf.Clamp(pitch, -clampDegree, clampDegree);
                    break;
                //None
                case mI.INVERTNONE:
                    yaw += cameraSensitivity * cameraValue.x;
                    pitch -= cameraSensitivity * cameraValue.y;
                    pitch = Mathf.Clamp(pitch, -clampDegree, clampDegree);
                    break;
            }

            //If camera is not freecamunconstrained then move first person camera else move third person camera
            if (cameraState != cS.FREECAMUNCONSTRAINED)
            {
                firstPersonCamera.transform.eulerAngles = new UnityEngine.Vector3(pitch, yaw, 0.0f);
            }
            else
            {
                thirdPersonCamera.transform.eulerAngles = new UnityEngine.Vector3(pitch, yaw, 0.0f);
            }
        }

    }

    //Function called from player controller to move camera y value via jumping and crouching input action values
    public void MoveFreeCamY(bool upward,float value)
    {
        //Move up
        if (upward)
        {
            freeCamValueY = value;
        }
        //Move down
        else if (value != 0)
        {
            freeCamValueY = -value;
        }
        //Set to 0
        else
        {
            freeCamValueY = 0;
        }
        //Apply delta time and multiplay value
        freeCamValueY = (freeCamValueY * Time.deltaTime) * 12;
    }

    //This function managed zooming and mesh clipping avoidance for the third person camera if it is 
    void DoOnEitherThirdPersonMode()
    {
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
                zoomPosition.position = UnityEngine.Vector3.MoveTowards(zoomPosition.position, zoomInPosition.position, cameraZoomSpeed);
                if (UnityEngine.Vector3.Distance(zoomPosition.position, zoomInPosition.position) < cameraTransferDistance)
                {

                    cameraState = cS.FIRSTPERSON;
                    transform.localRotation = Quaternion.Euler(0, thirdPersonCamera.transform.rotation.y, thirdPersonCamera.transform.rotation.z);
                }
            }
            //Zoom out
            else if (zoomValue < 0f)
            {

                zoomPosition.position = UnityEngine.Vector3.MoveTowards(zoomPosition.position, zoomOutPosition.position, cameraZoomSpeed);
            }
            //Added this so that it smooth lerps to a new zoom posiiton rather than imitadly set cam pos. this also fixes a cam issue when interacting 
            thirdPersonCamera.transform.position = Vector3.Lerp(thirdPersonCamera.transform.position, zoomPosition.position, otherCamSpeed * Time.deltaTime);

            if (Vector3.Angle(thirdPersonCamera.transform.localEulerAngles, new Vector3(15, 0, 0)) > 0.1f)
            {
                thirdPersonCamera.transform.localRotation = Quaternion.Lerp(thirdPersonCamera.transform.localRotation, Quaternion.Euler(15, 0, 0), otherCamSpeed * Time.deltaTime);
            }
        }
    }

    //Function to enable third person mode camera
    void EnableThirdPerson()
    {
        crosshair.SetActive(false);
        thirdPersonCamera.enabled = true;
        firstPersonCamera.enabled = false;
    }

    //Function for getting camera input value as vector 3
    public void Camera(InputAction.CallbackContext ctx)
    {
        if (ctx.action.ToString() == "Gameplay/Camera[/XInputControllerWindows/rightStick]")
        {
            cameraValue = new Vector3(ctx.ReadValue<Vector2>().x * controllerCameraSensitivityMultiplier, ctx.ReadValue<Vector2>().y * controllerCameraSensitivityMultiplier, 0);
        }
        else
        {
            cameraValue = new Vector3(ctx.ReadValue<Vector2>().x, ctx.ReadValue<Vector2>().y, 0);
        }
    }

    //Function for getting camera zoom input value
    public void CameraZoom(InputAction.CallbackContext ctx)
    {
        zoomValue = ctx.ReadValue<float>();
    }

    //Function for gettting camera rotation input value
    public void CameraRotate(InputAction.CallbackContext ctx)
    {
        cameraRotateValue = ctx.ReadValue<float>();
    }

    //Function for getting escape input value
    public void Escape(InputAction.CallbackContext ctx)
    {
        pC.uiMenu.UpdateUIMenuState(!pC.uiMenu.GetMenuStatus());
        escapeValue = ctx.ReadValue<float>();
    }

    //Function for freecamera movement input value
    public void FreeCameraMovement(InputAction.CallbackContext ctx)
    {
        cameraMovementValue = new Vector3(ctx.ReadValue<Vector2>().x, 0, ctx.ReadValue<Vector2>().y);
    }
}