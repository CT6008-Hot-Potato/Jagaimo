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
    [SerializeField]
    private float speedMultiplier = 0.5f;
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
    private float speed;
    private float downForce = 15;
    private bool grounded = false;
    private bool sliding = false;
    private PlayerCamera pC;
    private CharacterManager cM;
    private CapsuleCollider collider;
    private bool crouching = false;
    private bool slowStand = false;
    public PlayerInput PlayerInput => playerInput;
    private Vector3 movementValue = Vector3.zero;
    private float jumpValue = 0;
    private float crouchValue = 0;
    private float sprintValue = 0;
    private RebindingDisplay rebindingDisplay;
    private Timer timer;
    public UIMenuBehaviour uiMenu;
    [SerializeField]
    private AudioClip slideSound;
    [SerializeField]  
    private AudioClip crouchSound;
    [SerializeField]  
    private AudioClip jumpSound;
    private SoundManager sM;
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
    //Enum variable
    private pM playerMovement;

    #endregion Enum

    //Start function sets up numerous aspects of the player ready for use
    void Start()
    {
        sM = FindObjectOfType<SoundManager>();
        rebindingDisplay = FindObjectOfType<RebindingDisplay>();
        speed = walkSpeed;
        cM = GetComponent<CharacterManager>();
        Physics.queriesHitBackfaces = true;
        pC = GetComponent<PlayerCamera>();
        playerMovement = pM.INTERACTING;
        rb = GetComponent<Rigidbody>();
        collider = GetComponent<CapsuleCollider>();
        rb.freezeRotation = true;
        rb.useGravity = false;
        if (!uiMenu)
        {
            Debug.LogWarning("Missing ui menu reference!");
        }
        
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
                sM.PlaySound(jumpSound);
                //Jumpine velocity for the player
                rb.velocity = new Vector3(velocity.x, Mathf.Sqrt(jumpVelocity), velocity.z);
            }
            else if (grounded == false)
            {
                downForce = 22;
            }
            else if (downForce != 15)
            {
                downForce = 15;
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
                    //rebindingDisplay.DisplayBindingMenu(false);
                    //Unlock cursor
                    uiMenu.UpdateUIMenuState(false);
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
                if (crouching == false)
                {
                    if (crouchValue > 0.1f || sprintValue > 0.1f)
                    {
                        sM.PlaySound(crouchSound);
                        crouching = true;
                        speed = walkSpeed;
                        collider.center = new Vector3(0, 0, 0);
                        collider.height = 2;
                        pC.UnCrouch();
                        playerMovement = pM.WALKING;
                    }
                }
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
                    pC.Crouch();
                    if (speed == runSpeed && movementValue.z > 0.1f)
                    {
                        sliding = true;
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
            default:
                Debug.Log("Given value for MovementType is too high.");
                break;
        }
    }

    private IEnumerator Co_SlideTime()
    {
        timer = new Timer(slideTime);

        while (timer.isActive)
        {
            timer.Tick(Time.deltaTime);

            movementValue = new Vector3(0, 0, 1);
            if (speed != (runSpeed * 2))
            {
                speed = (runSpeed * 2);
            }
            yield return null;
        }

        sliding = false;
        speed = walkSpeed;
        collider.center = new Vector3(0, 0, 0);
        collider.height = 2;
        pC.UnCrouch();
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
       jumpValue = ctx.ReadValue<float>();
    }

    public void Crouch(InputAction.CallbackContext ctx)
    {

        crouchValue = ctx.ReadValue<float>();
    }

    public void Sprint(InputAction.CallbackContext ctx)
    {
        sprintValue = ctx.ReadValue<float>();
    }

}