/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///PlayerController.cs
///Developed by Charlie Bullock
///This class primarily acts as the main script for controlling the player movement.
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//This class is using:
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerController : MonoBehaviour
{
     //Variables
    #region Variables
    [SerializeField] 
    private PlayerInput playerInput = null;
    [SerializeField]
    private float velocityClamp = 10f;
    [SerializeField]
    private float jumpVelocity = 50;
    public float speedMultiplier = 0.5f;
    [SerializeField]
    private float walkSpeed = 5f;
    [SerializeField]
    private float runSpeed = 7f;
    [SerializeField]
    private float crouchSpeed = 1.5f;
    [SerializeField]
    private float slideTime = 3;
    //Rotation position
    [SerializeField]
    private Transform rotationPosition;
    private Rigidbody rb;
    public float speed;
    private float downForce = 15;
    public bool grounded = false;
    private bool sliding = false;
    private bool climbing = false;
    private bool touchingWall;
    private PlayerCamera pC;
    private PlayerInteraction pI;
    private CapsuleCollider collider;
    private bool crouching = false;
    private bool slowStand = false;
    public PlayerInput PlayerInput => playerInput;
    //Values to be assigned via inputaction
    private Vector3 movementValue = Vector3.zero;
    private float jumpValue = 0;
    private float crouchValue = 0;
    private float sprintValue = 0;
    private RebindingDisplay rebindingDisplay;
    private Timer timer;
    public UIMenuBehaviour uiMenu;
    [SerializeField]
    private ScriptableSounds.Sounds slideSound, crouchSound, jumpSound;
    private SoundManager sM;
    private PlayerAnimation pA;
    [SerializeField]
    private ScriptableParticles particles;
    [SerializeField]
    private PhysicMaterial wall;
    #endregion
    //Enums
    #region Enums
    //Enum for player movement type
    private enum pM {
        INTERACTING,
        CROUCHING,
        WALKING,
        STANDING
    }
    //Enum variable
    private pM playerMovement;
    #endregion Enum

    //Setting up and assigning on start
    private void Start() {
        //Get components for player animation, interaction, camera and the sound manager
        pA = GetComponent<PlayerAnimation>();
        pI = GetComponent<PlayerInteraction>();
        pC = GetComponent<PlayerCamera>();
        sM = FindObjectOfType<SoundManager>();
        //Find the rebinding display from the user interface/menu
        rebindingDisplay = FindObjectOfType<RebindingDisplay>();
        //Set default speed to the walking speed
        speed = walkSpeed;
        //Enable hitbackfaces for the third person camera mesh clipping
        Physics.queriesHitBackfaces = true;
        //Set the movement to interacting by  default
        playerMovement = pM.INTERACTING;
        //Get the rigidbody and capsule collider components
        rb = GetComponent<Rigidbody>();
        collider = GetComponent<CapsuleCollider>();
        //Freeze the rigidbody rotation and gravity (this is done because it is essential for this hybrid camera system for the player)
        rb.freezeRotation = true;
        rb.useGravity = false;
        
        //If the uimenu is null debug the warning that it is missing
        if (!uiMenu) {
            Debug.LogWarning("Missing ui menu reference!");
        }        
    }

    //Function where collision checking if collider just staying on ground in order to determine if grounded or not is done
    private void OnCollisionStay(Collision collision) {
        //Get the direction the collision point from relative to position of player
        Vector3 dir = collision.contacts[0].point - transform.position;
        //Normalise it
        dir = -dir.normalized;

        //Check the normalised y is greater or equal to 0.9 (0.9 generally sprinting while 1.0 standing stil)
        if (dir.y >= 0.9f) {
            //If collision of this gameobject is the carry position whan touching the ground drop it
            if (collision.gameObject.name == "CarryPosition") {
                Debug.Log("Hmmmmmm");
                GetComponent<PlayerInteraction>().Drop(true);
            }

            //Set these correctly
            grounded = true;
            collider.material = null;
        }
        //Else they are touching the wall
        else {
            touchingWall = true;
        }
    }

    //Function to set touching wall when players
    private void OnCollisionExit(Collision collision) {
        if (!grounded) {
            touchingWall = false;
        }
    }

    //Fixed update function is responsible for jumping and player movement as these need to be done based on fixed update not update
    private void FixedUpdate()
    {
        //If player movement state isn't interacting
        if (playerMovement != pM.INTERACTING && playerMovement != pM.STANDING && pC.cameraState != PlayerCamera.cS.FREECAMUNCONSTRAINED)
        {
            //Calculate how fast player should be moving
            Vector3 targetVelocity = movementValue;
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
            if (grounded && jumpValue > 0.1f)
            {
                particles.CreateParticle(ScriptableParticles.Particle.LandImpact, new Vector3(transform.position.x, transform.position.y - 1, transform.position.z));
                pA.CheckToChangeState("Jump");
                sM.PlaySound(jumpSound);
                //Jumpine velocity for the player
                rb.velocity = new Vector3(velocity.x, Mathf.Sqrt(jumpVelocity), velocity.z);
            }
            //Else if not grounded and not climbing can be pushed down stronger or do wall kick
            else if (!grounded && !climbing)
            {
                //If jump button pressed and wall kick returns true then do a wall kick effect
                if (jumpValue > 0.1f && pI.WallKick())
                {
                    pA.CheckToChangeState("JumpFromWall");
                    particles.CreateParticle(ScriptableParticles.Particle.AirImpact, new Vector3(transform.position.x, transform.position.y, transform.position.z + 0.5f));
                    climbing = true;
                    StartCoroutine(Co_ClimbTime());
                }
                //Else push down more intense
                else
                {
                    if (touchingWall)
                    {
                        rb.AddForce((-rotationPosition.TransformDirection(movementValue) * Time.deltaTime) * 100, ForceMode.Impulse);
                        collider.material = wall;
                    }
                    if (!sliding)
                    {
                        pA.CheckToChangeState("FallingIdle");
                    }
                    downForce = 22;
                }
            }
            //Default force pushed down with 
            else if (downForce != 15)
            {
                particles.CreateParticle(ScriptableParticles.Particle.JumpDust,new Vector3 (transform.position.x, transform.position.y - 1, transform.position.z));
                sM.PlaySound(ScriptableSounds.Sounds.Landing);
                downForce = 15;
            }
            else if (!sliding)
            {
                if (movementValue == Vector3.zero)
                {
                    if (playerMovement == pM.CROUCHING)
                    {
                        pA.CheckToChangeState("CrouchingIdle");
                    }
                    else
                    {
                        pA.CheckToChangeState("Idle");
                    }
                }
                else
                {
                    if (playerMovement == pM.CROUCHING)
                    {
                        pA.CheckToChangeState("CrouchedWalking");
                    }
                    else
                    {
                        if (sprintValue >= 0.1f)
                        {
                            if (movementValue.z > 0)
                            {
                                pA.CheckToChangeState("Running");
                            }
                            else if (movementValue.z < 0)
                            {
                                pA.CheckToChangeState("RunBackward");
                            }
                        }
                        else
                        {
                            if (movementValue.z > 0)
                            {
                                pA.CheckToChangeState("JogForward");
                            }
                            else if (movementValue.z < 0)
                            {
                                pA.CheckToChangeState("JogBackward");
                            }
                            else
                            {
                                if (movementValue.x > 0)
                                {
                                    pA.CheckToChangeState("RightStrafe");
                                }
                                else if (movementValue.x < 0)
                                {
                                    pA.CheckToChangeState("LeftStrafe");
                                }
                            }
                        }
                    }
                }
            }

        }
            // We apply gravity manually for more tuning control
            rb.AddForce(new Vector3(0, -downForce * rb.mass, 0));
            grounded = false;
    }
    //Update method calls camera type and movement type constantly
    private void Update()
    {
        //Function hecks and manages aspects of player movement relevant to movement type 
        MovementType();
    }
    //Simple public function for changing movement state
    public void SetMovement(int state)
    {
        //Movement states
        switch (state)
        {
            case 0:
                //rebindingDisplay.DisplayBindingMenu(true);
                playerMovement = pM.INTERACTING;
                break;
            case 1:
                //rebindingDisplay.DisplayBindingMenu(false);
                playerMovement = pM.CROUCHING;
                break;
            case 2:
                //rebindingDisplay.DisplayBindingMenu(false);
                playerMovement = pM.WALKING;
                break;
            case 3:
                //MAKE SURE PLAYERS CAN EXIT THIS STATE
                playerMovement = pM.STANDING;
                break;
            default:
                Debug.Log("Given value for ChangeMovement is too high.");
                break;
        }
    }

    //Simple public function
    public int GetMovement()
    {
        return (int)playerMovement;
    }

    //Function for the movement types if the player
    void MovementType()
    {
        //Switch for different movement types
        switch (playerMovement)
        {
            case pM.INTERACTING:
                //Checks for player walking
                if (movementValue != Vector3.zero)
                {
                    //Unlock cursor
                    uiMenu.UpdateUIMenuState(false);
                    UnityEngine.Cursor.lockState = CursorLockMode.Locked;
                    playerMovement = pM.WALKING;
                }
                //Else if cursor is locked set it to not be locked
                else if (UnityEngine.Cursor.lockState == CursorLockMode.Locked)
                {
                    UnityEngine.Cursor.lockState = CursorLockMode.None;
                }
                break;
            //Crouching
            case pM.CROUCHING:
                speed = crouchSpeed;
                //If crouching is false check if crouch button lifted up or sprinting key pressed and uncrouch
                if (crouching == false)
                {
                    if (crouchValue > 0.1f || sprintValue > 0.1f)
                    {
                        //Uncrouch
                        if (pI.UnCrouch())
                        {
                            sM.PlaySound(crouchSound);
                            crouching = true;
                            speed = walkSpeed;
                            collider.center = new Vector3(0, 0, 0);
                            collider.height = 2;
                            playerMovement = pM.WALKING;
                        }
                    }
                }
                //Crouching set to false when key lifted
                else if (crouching && crouchValue == 0)
                {
                    crouching = false;
                }
                break;
            //Walking
            case pM.WALKING:
                if (crouchValue > 0.1f && crouching == false && sliding == false)
                {
                    collider.center = new Vector3(0, -0.5f, 0);
                    collider.height = 1;
                    pI.Crouch();
                    if (grounded)
                    {
                        if (speed == runSpeed && movementValue.z > 0.1f)
                        {
                            sliding = true;
                            particles.CreateParticle(ScriptableParticles.Particle.Fuse, new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z));

                            sM.PlaySound(slideSound);
                            StartCoroutine(Co_SlideTime());
                        }
                        else
                        {
                            sM.PlaySound(crouchSound);
                            crouching = true;
                            speed = crouchSpeed;
                            playerMovement = pM.CROUCHING;
                        }
                    }
                    else
                    {
                        rb.AddForce(-transform.up , ForceMode.Impulse);
                    }
                }
                else if (crouching && crouchValue == 0)
                {
                    crouching = false;
                }
                if (sprintValue > 0.1f && !sliding)
                {
                    speed = runSpeed;
                }
                else if (speed == runSpeed && !sliding)
                {
                    speed = walkSpeed;
                }
                break;
            case pM.STANDING:
                break;
            default:
                Debug.Log("Given value for MovementType is too high.");
                break;
        }
    }

    //This coroutine is called to climb up and turn around when doing a wall click
    private IEnumerator Co_ClimbTime()
    {
        rb.useGravity = false;
        timer = new Timer(0.25f);
        //Push up while timer active
        while (timer.isActive)
        {
            timer.Tick(Time.deltaTime);
            pC.ChangeYaw(720);
            yield return null;
        }
        rb.velocity = Vector3.zero;
        rb.AddForce(transform.up * 20, ForceMode.Impulse);       
        rb.AddForce(rotationPosition.forward * 50, ForceMode.Impulse);
        rb.useGravity = true;
        //Flip around
        pC.flipSpin = !pC.flipSpin;
        rb.velocity = Vector3.zero;
        climbing = false;
    }

    //This corouting is called to climb and
    private IEnumerator Co_SlideTime()
    {
        timer = new Timer(slideTime);

        while (timer.isActive)
        {
            timer.Tick(Time.deltaTime);
            pA.CheckToChangeState("ActionPose");
            movementValue = new Vector3(0, 0, 1);
            if (speed != (runSpeed * 2))
            {
                speed = (runSpeed * 2);
            }
            yield return null;
        }

        //If can't uncrouch then stay crouch
        if (!pI.UnCrouch())
        {
            speed = crouchSpeed;
            playerMovement = pM.CROUCHING;
        }
        //Else uncrouch
        else
        {
            speed = walkSpeed;
            collider.center = new Vector3(0, 0, 0);
            collider.height = 2;
        }

        //Set crouching true, sliding false
        crouching = true;
        sliding = false;
        //Stop sliding
        if (slowStand)
        {
            movementValue = new Vector3(0, 0, 0);
        }
    }

    public void Movement(InputAction.CallbackContext ctx)
    {
        if (ctx.ReadValue<Vector2>().y <= 0 && sliding)
        {
            slowStand = true;
        }
        else if (slowStand == true){
            slowStand = false;
        }
        if (!sliding)
        {
            movementValue = new Vector3 (ctx.ReadValue<Vector2>().x,0, ctx.ReadValue<Vector2>().y);
        }
    }

    public void Jump(InputAction.CallbackContext ctx)
    {

        //if (pC.PlayerInput.currentActionMap.name == "Menu")
        //{
        //    pC.PlayerInput.SwitchCurrentActionMap("Menu");
        //}
        //else
        //{
        //    pC.PlayerInput.SwitchCurrentActionMap("Gameplay");
        //}

        if (pC.cameraState == PlayerCamera.cS.FREECAMUNCONSTRAINED)
        {
            pC.MoveFreeCamY(true, ctx.ReadValue<float>());
        }
        else if (pC.cameraState != PlayerCamera.cS.FREECAMCONSTRAINED)
        {
           jumpValue = ctx.ReadValue<float>();
        }
    }

    public void Crouch(InputAction.CallbackContext ctx)
    {
        if (!grounded)
        {
            if (crouchValue >= 0.1f)
            {
                pA.CheckToChangeState("CrouchingIdle");
            }
            else
            {
                pA.CheckToChangeState("Idle");
            }
        }

            if (pC.cameraState == PlayerCamera.cS.FREECAMUNCONSTRAINED)
        {
            pC.MoveFreeCamY(false, ctx.ReadValue<float>());
        }
        else if (pC.cameraState != PlayerCamera.cS.FREECAMCONSTRAINED)
        { 
            crouchValue = ctx.ReadValue<float>();
        }
    }

    public void Sprint(InputAction.CallbackContext ctx)
    {
        sprintValue = ctx.ReadValue<float>();
    }

}