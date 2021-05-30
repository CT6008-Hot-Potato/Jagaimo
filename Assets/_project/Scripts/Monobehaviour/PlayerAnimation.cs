/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///PlayerAnimation.cs
///Developed by Charlie Bullock
///This class is rotates an object constantly at a set speed.
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//This class is using:
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour {
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

    //Variables
    [SerializeField]
    private Animation m_animation;
    [SerializeField]
    private Animation animationArms;
    private bool animationLock = false;
    private Timer timer;

    //Start animation at idle by default
    private void Start() {
        m_animation.Play("Idle");
        if (animationArms != null) {
            animationArms.CrossFade("Idle");
        }
    }

    //Function to check to change the animation state for a player
    public bool CheckToChangeState(string animationState) {
        //If animation already playing return false
        if (m_animation.name == animationState) {
            return false;
        }
        //Else if animation is not locked crossfade play the animation for the arms and body respectively
        else if (!animationLock) {
            m_animation.CrossFade(animationState);
            if (animationArms != null) {
                animationArms.CrossFade(animationState);
            }
            return true;
        }

        //Return false if neither
        return false;
    }

    //Overload of above function which allows for the locking of an animation going to be played
    public bool CheckToChangeState(string animationState, bool lockAnimation) {
        //If animation already playing return false
        if (m_animation.name == animationState) {
            return false;
        }
        //Else play that animation and lock it if true
        else {
            animationLock = lockAnimation;
            m_animation.CrossFade(animationState);
            if (animationArms != null) {
                animationArms.CrossFade(animationState);
            }
            StartCoroutine(Co_AnimationTime(m_animation.GetClip(animationState).length));
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
