using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//A class so I can start the game whenever I want to in the test scene
public class TestStartTrigger : MonoBehaviour
{
    public static TestStartTrigger instance;

    //Variables
    //The object that kicks the round off
    [SerializeField]
    BasicTimerBehaviour startCountdown;
    [SerializeField]
    private PlayerInput playerInput = null;
    public PlayerInput PlayerInput => playerInput;
    private float playValue = 0;

    private bool canStart = false;
    private bool isLocked = false;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }

        playerInput = playerInput ?? FindObjectOfType<PlayerInput>();
    }

    // Update is called once per frame
    void Update()
    {
        //P for play game
        if (playValue > 0.1f && canStart && !isLocked)
        {
            startCountdown.CallOnTimerStart();
            isLocked = true;
        }
    }

    public void Play(InputAction.CallbackContext ctx)
    {
        playValue = ctx.ReadValue<float>();
    }

    public void CanStart()
    {
        playerInput = playerInput ?? FindObjectOfType<PlayerInput>();
        canStart = true;
    }

}
