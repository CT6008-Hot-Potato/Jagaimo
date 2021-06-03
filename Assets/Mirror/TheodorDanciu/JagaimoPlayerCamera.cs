/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// JagaimoPlayerCamera.cs
// Developed by Charlie Bullock
// Edited by Theodor Danciu for Multiplayer Networking
// This class is responsible for first/third person camera of the player.
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class JagaimoPlayerCamera : NetworkBehaviour
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
    //Zoom in position of third person camera
    [SerializeField]
    private Transform zoomInPosition;
    [SerializeField]
    private Transform zoomPosition;
    private RaycastHit hit;
    private Ray ray;
    //Player movement
    private JagaimoPlayerController pC;
    public Vector3 firstPersonCamPosition;
    public float otherCamSpeed = 2f;
    //Rotation position
    [SerializeField]
    private Transform rotationPosition;
    private CapsuleCollider collider;
    //Pitch and yaw speed for camera
    private float pitch;
    private float yaw;
    [SerializeField]
    private PlayerInput playerInput = null;
    public PlayerInput PlayerInput => playerInput;
    //For input system 
    private float zoomValue = 0;
    private float cameraRotateValue = 0;
    private float escapeValue = 0;
    private Vector2 cameraValue = Vector2.zero;
    public bool useControllerSensitivity = false;
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
    private SpriteRenderer crosshair;   
    #endregion Variables
    
    //Multiplayer references
    private JagaimoGamePlayer     jagaimoGamePlayer;
    private JagaimoNetworkManager jagaimoNetworkManager;
    
    [SyncVar(hook = nameof(OnChangeModel))]
    public CharacterModel currentCharacterModel;
    
    [SyncVar(hook = nameof(OnChangeLayer))]
    public int clientObjectLayer;

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

    public enum CharacterModel : byte
    {
        redModel,
        blueModel,
        yellowModel,
        greenModel,
        defaultModel
    }
    
    #endregion Enums

    //This method sets the camera view
    public void SetCameraView(bool firstPerson) 
    {
        //Set to first person
        if (firstPerson) {
            cameraState = cS.FIRSTPERSON;


            crosshair.color = Color.white;  //.SetActive(true);
        }
        //Set to third person
        else {
            cameraState = cS.THIRDPERSON;
            crosshair.color = Color.clear;//crosshair.SetActive(false);
        }
    }
    

    // Assigning audio listeners, setting correct camera state and making sure queriesHitBackfaces is true for raycasting later
    public void Start()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        
        jagaimoGamePlayer = gameObject.GetComponent<JagaimoGamePlayer>();

        CharacterModel tempCharacterModel = (CharacterModel)jagaimoGamePlayer.playerIndex;
        CmdChangeCharacterModel(tempCharacterModel);
        CmdChangeLayerPlayer(jagaimoGamePlayer.playerIndex);
        
        characterArmsSkinnedMesh = characterArms.GetComponent<SkinnedMeshRenderer>();
        characterSkinnedMesh = character.GetComponent<SkinnedMeshRenderer>();
        Physics.queriesHitBackfaces = true;
        firstPersonCamPosition = firstPersonCamera.transform.localPosition;
        collider = GetComponent<CapsuleCollider>();
        pC = GetComponent<JagaimoPlayerController>();

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

    /// <summary>
    /// 
    /// </summary>
    //Function for setting the correct player mask up
    private void SetPlayerMask()
    {
        //Set correct shared mesh and culling masks dependent on the player index plus nessecary value offsets
        firstPersonCamera.cullingMask = mask[jagaimoGamePlayer.playerIndex + 4];
        thirdPersonCamera.cullingMask = mask[jagaimoGamePlayer.playerIndex];
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="oldObjectLayer"></param>
    /// <param name="newObjectLayer"></param>
    private void OnChangeLayer(int oldObjectLayer, int newObjectLayer)
    {
        this.character.layer = 9 + newObjectLayer;
        this.characterArms.layer = 13 + newObjectLayer;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="oldCharacterModel"></param>
    /// <param name="newCharacterModel"></param>
    private void OnChangeModel(CharacterModel oldCharacterModel, CharacterModel newCharacterModel)
    {
        if (!characterSkinnedMesh || !characterArmsSkinnedMesh) {
            characterArmsSkinnedMesh = characterArms.GetComponent<SkinnedMeshRenderer>();
            characterSkinnedMesh = character.GetComponent<SkinnedMeshRenderer>();
        }
        
        //Set correct shared mesh and culling masks dependent on the player index plus nessecary value offsets
        switch (newCharacterModel)
        {
            case CharacterModel.redModel:
                characterSkinnedMesh.sharedMesh = characterMesh[0];
                characterArmsSkinnedMesh.material = characterSkinnedMesh.material = materials[0];
                break;
            case CharacterModel.blueModel:
                characterSkinnedMesh.sharedMesh = characterMesh[1];
                characterArmsSkinnedMesh.material = characterSkinnedMesh.material = materials[1];
                break;
            case CharacterModel.yellowModel:
                characterSkinnedMesh.sharedMesh = characterMesh[2];
                characterArmsSkinnedMesh.material = characterSkinnedMesh.material = materials[2];
                break;
            case CharacterModel.greenModel:
                characterSkinnedMesh.sharedMesh = characterMesh[3];
                characterArmsSkinnedMesh.material = characterSkinnedMesh.material = materials[3];
                break;
            case CharacterModel.defaultModel:
                //todo assign the default values of the model
                break;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="objectLayer"></param>
    [Command]
    private void CmdChangeLayerPlayer(int objectLayer)
    {
        clientObjectLayer = objectLayer;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="characterModel"></param>
    [Command]
    private void CmdChangeCharacterModel(CharacterModel characterModel)
    {
        currentCharacterModel = characterModel;
    }

    //Update function calles camera type function constantly
    void Update()
    {
        //Function for aspects of the player movement to if the camera is in third or first person mode
        if (!isLocalPlayer)
        {
            return;
        }
        CameraType();
    }

    //Function to set yaw valye
    public void ChangeYaw(float timeMultiplier) 
    {
        //If true flip spin in this direction
        if (flipSpin) {
            yaw = yaw - Time.deltaTime * timeMultiplier;
        }
        //Else flip spin rotate in the alternate direction
        else {
            yaw = yaw + Time.deltaTime * timeMultiplier;
        }
    }
//Camera type function which is responsible for managing the rotation and type of camera which the player utilises
    void CameraType() {

        //If movement is not interaction and the camera rotational lock is also false
        if (pC.GetMovement() != 0 && cameraRotationLock == false) {
            //Switch statement for the various camera states for the hybrid player systems cameras
            switch (cameraState) {
                //First person camera
                case cS.FIRSTPERSON:
                    //Check if wrong camera enabled and if so setup correct camera
                    if (thirdPersonCamera.enabled) {

                        crosshair.color = Color.white; //                        crosshair.SetActive(true);
                        thirdPersonCamera.enabled = false;
                        firstPersonCamera.enabled = true;
                    }
                    else {
                        //Set the rotation position for the y axis based on first person camera y rotational value
                        rotationPosition.rotation = UnityEngine.Quaternion.Euler(0, firstPersonCamera.transform.localRotation.eulerAngles.y, 0);

                        if (zoomValue < 0f) {
                            //Make sure that player not crouching      
                            collider.center = new Vector3(0, 0, 0);
                            collider.height = 2f;
                            firstPersonCamera.transform.localPosition = new UnityEngine.Vector3(0, 0.75f, 0);
                            //Raycast when in third person checking if there is an obstacle between camera and player
                            ray = firstPersonCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
                            ray.direction *= -1;
                            Physics.Raycast(ray, out hit, 3);

                            //If an obstacle is found then zoom in
                            if (hit.transform == null) {
                                zoomPosition.position = UnityEngine.Vector3.MoveTowards(zoomPosition.position, zoomOutPosition.position, 90 * Time.deltaTime);
                                cameraState = cS.THIRDPERSON;
                            }
                            else {
                                zoomPosition.position = UnityEngine.Vector3.MoveTowards(zoomPosition.position, hit.point, 90 * Time.deltaTime);
                            }
                        }
                        firstPersonCamera.transform.localRotation = Quaternion.Lerp(firstPersonCamera.transform.localRotation, Quaternion.Euler(0, 0, 0), otherCamSpeed * 0.5f * Time.deltaTime);
                    }
                    break;
                //Third person camera
                case cS.THIRDPERSON:
                    //Check if wrong camera enabled and if so setup correct camera
                    if (firstPersonCamera.enabled) {
                        EnableThirdPerson();
                    }
                    else {
                        rotationPosition.rotation = UnityEngine.Quaternion.Euler(0, firstPersonCamera.transform.localRotation.eulerAngles.y, 0);
                        DoOnEitherThirdPersonMode();
                        //Begins freecam movement
                        if (cameraRotateValue > 0.1f) {
                            cameraState = cS.FREECAMCONSTRAINED;
                        }
                    }
                    break;
                //Free camera constrainged
                case cS.FREECAMCONSTRAINED:
                    //Check if wrong camera enabled and if so setup correct camera
                    if (firstPersonCamera.enabled) {
                        EnableThirdPerson();
                    }
                    else {
                        //Drop a picked up object if currently being held when in this state
                        if (GetComponent<PlayerInteraction>() && GetComponent<PlayerInteraction>().enabled)
                            GetComponent<PlayerInteraction>().Drop(false);
                        DoOnEitherThirdPersonMode();

                        if (cameraRotateValue <= 0.1f) {
                            cameraState = cS.THIRDPERSON;
                        }
                        //Else the if the camera movement value is not zero then add the correct components for the freecam unconstrained mode
                        else if (cameraMovementValue != Vector3.zero) {
                            thirdPersonCamera.gameObject.AddComponent<SphereCollider>().radius = 0.25f;
                            freecamRotation = thirdPersonCamera.transform.rotation.eulerAngles;
                            cameraState = cS.FREECAMUNCONSTRAINED;
                            if (thirdPersonCamera.gameObject.GetComponent<Rigidbody>() == null)
                            {
                                thirdPersonCamera.gameObject.AddComponent<Rigidbody>().useGravity = false;
                            }
                            else
                            {
                                thirdPersonCamera.gameObject.GetComponent<Rigidbody>().useGravity = true;
                            }
                            thirdPersonCamera.gameObject.AddComponent<CameraCollision>();
                        }
                    }
                    break;
                //Free camera unconstrained
                case cS.FREECAMUNCONSTRAINED:
                    //If freecam button lifted up and freecam is not locked go back to third person
                    if (thirdPersonCamera.gameObject.TryGetComponent(out Rigidbody rigidbody)) {

                        if (cameraRotateValue <= 0.1f && !freecamLock) {
                            //Destroy the rigidbody, sphere collision and camera collision class
                            Destroy(rigidbody.GetComponent<SphereCollider>());
                            Destroy(rigidbody.GetComponent<CameraCollision>());
                            Destroy(rigidbody);
                            thirdPersonCamera.transform.rotation = Quaternion.Euler(freecamRotation);
                            cameraState = cS.THIRDPERSON;
                        }   
                        //Else if beyond 200 metres from centre of map add rigidbody & collider
                        else if (Vector3.Distance(thirdPersonCamera.transform.position,Vector3.zero) > 200) {
                            rigidbody.useGravity = true;
                            rigidbody.mass = 10000;
                        }
                        //Remove it and sphere collider when within 150 metres of the centre of map
                        if (Vector3.Distance(thirdPersonCamera.transform.position, Vector3.zero) < 150) {

                            rigidbody.useGravity = false;
                        }
                    }
                    //Moving third person camera position around freely
                    thirdPersonCamera.transform.position = new Vector3(thirdPersonCamera.transform.position.x, thirdPersonCamera.transform.position.y + freeCamValueY, thirdPersonCamera.transform.position.z);
                    thirdPersonCamera.transform.Translate(cameraMovementValue * 0.1f,thirdPersonCamera.transform);                   
                    break;
            }

            //Goes to first person mode and unlocks cursor when unlock pressed
            if (escapeValue > 0.1f) {
                pC.SetMovement(0);
                UnityEngine.Cursor.lockState = CursorLockMode.None;                               
            }
            else if (UnityEngine.Cursor.lockState == CursorLockMode.None) {
                UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            }

            //X & Y axis camera can be either inverted or not
            switch (mouseInversion) {
                //InvertX
                case mI.INVERTX:
                    //If using controller apply controller sensitivity otherwise mouse camera sensitivity
                    if (useControllerSensitivity) {
                        yaw += controllerCameraSensitivityMultiplier * cameraValue.x;
                        pitch += controllerCameraSensitivityMultiplier * cameraValue.y;
                    }
                    else {
                        yaw += cameraSensitivity * cameraValue.x;
                        pitch += cameraSensitivity * cameraValue.y;
                    }

                    //Clamp the camera pitch into it's respective limits
                    pitch = Mathf.Clamp(pitch, -clampDegree, clampDegree);
                    break;
                //InvertY
                case mI.INVERTY:
                    //If using controller apply controller sensitivity otherwise mouse camera sensitivity
                    if (useControllerSensitivity) {
                        yaw -= controllerCameraSensitivityMultiplier * cameraValue.x;
                        pitch -= controllerCameraSensitivityMultiplier * cameraValue.y;
                    }
                    else {
                        yaw -= cameraSensitivity * cameraValue.x;
                        pitch -= cameraSensitivity * cameraValue.y;
                    }

                    //Clamp the camera pitch into it's respective limits
                    pitch = Mathf.Clamp(pitch, -clampDegree, clampDegree);
                    break;
                //Both
                case mI.INVERTBOTH:
                    //If using controller apply controller sensitivity otherwise mouse camera sensitivity
                    if (useControllerSensitivity) {
                        yaw -= controllerCameraSensitivityMultiplier * cameraValue.x;
                        pitch += controllerCameraSensitivityMultiplier * cameraValue.y;
                    }
                    else {
                        yaw -= cameraSensitivity * cameraValue.x;
                        pitch += cameraSensitivity * cameraValue.y;
                    }

                    //Clamp the camera pitch into it's respective limits
                    pitch = Mathf.Clamp(pitch, -clampDegree, clampDegree);
                    break;
                //None
                case mI.INVERTNONE:
                    //If using controller apply controller sensitivity otherwise mouse camera sensitivity
                    if (useControllerSensitivity) {
                        yaw += controllerCameraSensitivityMultiplier * cameraValue.x;
                        pitch -= controllerCameraSensitivityMultiplier * cameraValue.y;
                    }
                    else {
                        yaw += cameraSensitivity * cameraValue.x;
                        pitch -= cameraSensitivity * cameraValue.y;
                    }

                    //Clamp the camera pitch into it's respective limits
                    pitch = Mathf.Clamp(pitch, -clampDegree, clampDegree);
                    break;
            }

            //If camera is not freecamunconstrained then move first person camera else move third person camera
            if (cameraState != cS.FREECAMUNCONSTRAINED) {
                firstPersonCamera.transform.eulerAngles = new UnityEngine.Vector3(pitch, yaw, 0.0f);
            }
            else {
                thirdPersonCamera.transform.eulerAngles = new UnityEngine.Vector3(pitch, yaw, 0.0f);
            }
        }

    }

    //Function called from player controller to move camera y value via jumping and crouching input action values
    public void MoveFreeCamY(bool upward,float value) {
        //Move up
        if (upward) {
            freeCamValueY = value;
        }
        //Move down
        else if (value != 0) {
            freeCamValueY = -value;
        }
        //Set to 0
        else {
            freeCamValueY = 0;
        }
        //Apply delta time and multiplay value
        freeCamValueY = (freeCamValueY * Time.deltaTime) * 12;
    }

    //This function managed zooming and mesh clipping avoidance for the third person camera if it is 
    void DoOnEitherThirdPersonMode() {
        //If an obstacle is found then zoom in
        if (Physics.Linecast(firstPersonCamera.transform.position, zoomPosition.transform.position, out hit) && hit.collider.gameObject != gameObject && !hit.collider.isTrigger) {
            //Move towards the zoom in position
            zoomPosition.position = UnityEngine.Vector3.MoveTowards(zoomPosition.position, zoomInPosition.position, cameraPushSpeed * Time.deltaTime);
            
            //If within transfer distance then go to third person mode
            if (UnityEngine.Vector3.Distance(zoomPosition.position, zoomInPosition.position) < cameraTransferDistance) {
                cameraState = cS.FIRSTPERSON;
            }
        }
        else {

            //Zoom in
            if (zoomValue > 0f) {
                //Zoom in towards the zoom in position
                zoomPosition.position = UnityEngine.Vector3.MoveTowards(zoomPosition.position, zoomInPosition.position, cameraZoomSpeed);

                if (UnityEngine.Vector3.Distance(zoomPosition.position, zoomInPosition.position) < cameraTransferDistance) {
                    cameraState = cS.FIRSTPERSON;
                    transform.localRotation = Quaternion.Euler(0, thirdPersonCamera.transform.rotation.y, thirdPersonCamera.transform.rotation.z);
                }
            }
            //Zoom out
            else if (zoomValue < 0f) {
                //Zoom out towards the zoom out position
                zoomPosition.position = UnityEngine.Vector3.MoveTowards(zoomPosition.position, zoomOutPosition.position, cameraZoomSpeed);
            }

            //Added this so that it smooth lerps to a new zoom posiiton rather than imitadly set cam pos. this also fixes a cam issue when interacting 
            thirdPersonCamera.transform.position = Vector3.Lerp(thirdPersonCamera.transform.position, zoomPosition.position, otherCamSpeed * Time.deltaTime);

            //Correct the angle
            if (Vector3.Angle(thirdPersonCamera.transform.localEulerAngles, new Vector3(15, 0, 0)) > 0.1f) {
                thirdPersonCamera.transform.localRotation = Quaternion.Lerp(thirdPersonCamera.transform.localRotation, Quaternion.Euler(15, 0, 0), otherCamSpeed * Time.deltaTime);
            }
        }
    }

    //Function to enable third person mode camera
    void EnableThirdPerson() {
        crosshair.color = Color.clear;//        crosshair.SetActive(false);
        thirdPersonCamera.enabled = true;
        firstPersonCamera.enabled = false;
    }

    //Function for getting camera input value as vector 3
    public void Camera(InputAction.CallbackContext ctx) {
        cameraValue = new Vector3(ctx.ReadValue<Vector2>().x, ctx.ReadValue<Vector2>().y, 0);
    }

    //Function for getting camera zoom input value
    public void CameraZoom(InputAction.CallbackContext ctx) {
        zoomValue = ctx.ReadValue<float>();
    }

    //Function for gettting camera rotation input value
    public void CameraRotate(InputAction.CallbackContext ctx) {
        cameraRotateValue = ctx.ReadValue<float>();
    }

    //Function for getting escape input value
    public void Escape(InputAction.CallbackContext ctx) {
        if (pC == null) {
            pC = GetComponent<JagaimoPlayerController>();
        }
        pC.uiMenu.UpdateUIMenuState(!pC.uiMenu.GetMenuStatus());
        escapeValue = ctx.ReadValue<float>();
    }

    //Function for freecamera movement input value
    public void FreeCameraMovement(InputAction.CallbackContext ctx) {
        cameraMovementValue = new Vector3(ctx.ReadValue<Vector2>().x, 0, ctx.ReadValue<Vector2>().y);
    }
}