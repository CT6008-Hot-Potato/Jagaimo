using System;
using System.Collections;
using UnityEngine;
using Mirror;

//written by Charlie Bullock

public class JagaimoPlayerAnimation : NetworkBehaviour
{
      //CarryingWalking
    //CarryingRunning
    //CrouchedWalking
    //CrouchingIdle
    //Dying
    //FallingBackDeath
    //FallingIdle
    //ActionPose
    //Idle
    //JogForward
    //JumpFromWall
    //JumpToHang
    //Jump
    //JumpingUp
    //LeftStrafe
    //RightStrafe
    //Running
    //Throw
    //Grab

    public enum AnimStates : byte
    {
        None,
        Idle,
        CarryingRunning,
        CrouchedWalking,
        CrouchingIdle,
        Dying,
        FallingBackDeath,
        FallingIdle,
        ActionPose,
        JogForward,
        JogBackward,
        JumpFromWall,
        JumpToHang,
        Jump,
        JumpingUp,
        LeftStrafe,
        RightStrafe,
        Running,
        RunBackward,
        Throw,
        Grab,
    }

    //Variables
    [SerializeField]
    private Animation animation;
    [SerializeField]
    private Animation animationArms;
    private bool animationLock = false;
    private Timer timer;

    [SyncVar]
    public AnimStates animState;

    //Start animation at idle by default
    private void Start()
    {
        if (!isLocalPlayer)
        {
            return;
            
        }
        animation.Play("Idle");
        if (animationArms != null) {
            animationArms.CrossFade("Idle");
        }
    }

    // void OnAnimationStateChanged(AnimStates oldState, AnimStates newState)
    // {
    //     if (animation.name == newState.ToString())
    //     {
    //         return;
    //     }
    //     animation.CrossFade(newState.ToString());
    //     //animationArms.CrossFade(newState.ToString());
    // }
    
    [Command]
    private void CmdChangeAnimState(string state)
    {
        animState = (AnimStates) System.Enum.Parse(typeof(AnimStates), state);
        RpcOnChangeAnimState(state);
    }
    
    [ClientRpc]
    private void RpcOnChangeAnimState(string state)
    {
        if (animation.name.Equals(state))
        {
            return;
        }
        animation.CrossFade(state);
    }
    
    //Function to check to change the animation state for a player
    public bool CheckToChangeState(string animationState) {
        //If animation already playing return false
        if (animation.name == animationState) {
            return false;
        }
        //Else if animation is not locked crossfade play the animation for the arms and body respectively
        else if (!animationLock)
        {
            CmdChangeAnimState(animationState);
            //animation.CrossFade(animationState);
            if (animationArms != null) {
                //animationArms.CrossFade(animationState);
            }
            return true;
        }

        //Return false if neither
        return false;
    }

    //Overload of above function which allows for the locking of an animation going to be played
    public bool CheckToChangeState(string animationState, bool lockAnimation) {
        //If animation already playing return false
        if (animation.name == animationState) {
            return false;
        }
        //Else play that animation and lock it if true
        else {
            animationLock = lockAnimation;
            //animation.CrossFade(animationState);
            CmdChangeAnimState(animationState);
            if (animationArms != null) {
                //animationArms.CrossFade(animationState);
            }
            StartCoroutine(Co_AnimationTime(animation.GetClip(animationState).length));
            return true;
        }
    }

    //This coroutine is called to wait for specific animations to play
    private IEnumerator Co_AnimationTime(float time) {
        timer = new Timer(time);
        //Push up while timer active
        while (timer.isActive) {
            timer.Tick(Time.deltaTime);
            yield return null;
        }
        animationLock = false;
    }
}