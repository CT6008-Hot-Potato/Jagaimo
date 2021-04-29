using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAnimation : MonoBehaviour
{

    public Animation Playme;

//    [SerializeField] Animator AnimationComponent;
    private void OnEnable()
    {
        Debug.Log(Playme.name);
        Playme.Play();
    }


}
