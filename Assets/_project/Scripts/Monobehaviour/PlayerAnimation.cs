using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
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

    [SerializeField]
    private Animation animation;
    private bool animationLock = false;
    private Timer timer;

    private void Start()
    {
        animation.Play("Idle");
    }

    public bool CheckToChangeState(string animationState)
    {
        if (animation.name == animationState)
        {
            return false;
        }
        else if (!animationLock)
        {
            animation.CrossFade(animationState);
            return true;
        }
        return false;
    }

    public bool CheckToChangeState(string animationState, bool lockAnimation)
    {
        if (animation.name == animationState)
        {
            return false;
        }
        else
        {
            animationLock = lockAnimation;
            animation.CrossFade(animationState);
            StartCoroutine(Co_AnimationTime(animation.GetClip(animationState).length));
            return true;
        }
    }

    //This coroutine is called to wait for specific animations to play
    private IEnumerator Co_AnimationTime(float time)
    {
        timer = new Timer(time);
        //Push up while timer active
        while (timer.isActive)
        {
            timer.Tick(Time.deltaTime);
            yield return null;
        }
            animationLock = false;
    }
}
