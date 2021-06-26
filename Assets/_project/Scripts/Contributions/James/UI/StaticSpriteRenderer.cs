/////////////////////////////////////////////////////////////
//
//  Script Name: StaticSpriteRenderer.cs
//  Creator: James Bradbury
//  Description: The script for altering billboarded sprites (ie: powerups) at runtime
//  
/////////////////////////////////////////////////////////////


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticSpriteRenderer : MonoBehaviour
{
    ParticleSystem MySpriteRenderer;
    ParticleSystemRenderer ParticleRenderer;
    
    private void OnEnable() // When Created, grabs the particle system that this script is attached to
    {
        TryGetComponent(out MySpriteRenderer);
        TryGetComponent(out ParticleRenderer);
        SetActive(true);
    }

    public void SetActive(bool isActive)
    {
        if(isActive)
        {
            MySpriteRenderer.Play();
        }
        else
        {
            MySpriteRenderer.Clear();
            MySpriteRenderer.Stop();
        }
    } // Turns the Particle effect on and off
    public void ChangeSprite(Material material) // Switches the Particle to a new material
    {
        ParticleRenderer.material = material;
    }


}
