using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReticleSwitcher : MonoBehaviour
{
    [SerializeField] Sprite[] Reticles;// this should work?

    // Update is called once per frame
    public void ChangeReticle(int i)
    {

        if (TryGetComponent(out SpriteRenderer changeme))
        {
            changeme.sprite = Reticles[i];
            if (i == 0)
            {
                transform.localEulerAngles = new Vector3(0, 0, 45);
                transform.localScale = Vector3.one * 0.03f;
            }
            else
            {
                transform.localEulerAngles = new Vector3(0, 0, 0);
                transform.localScale = Vector3.one * 0.01f;
            }

        }



            

    }

}   
