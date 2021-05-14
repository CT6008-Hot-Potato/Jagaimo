////////////////////////////////////////////////////////////
// File: Scriptable sound
// Author: James Bradbury
// Date Created: 1/05/21
// Brief: A script to handle profiling of particle assets
//////////////////////////////////////////////////////////// 

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ParticleProfile", menuName = "ScriptableObjects/ParticleProfile", order = 3)]

public class ScriptableParticles : ScriptableObject
{
    public enum Particle // All registered game particles 
    {
        None,
        Fuse,
        AimTrail,
        AirImpact,
        BloodBurst,
        ConfettiBurst,
        Confetti,
        JumpDust,
        LandImpact,
        GoalExplosion

    };


    [Serializable]
    public class ParticleSettings // Each particle prefab is stored here with a corresponding name for easy finding
    {
        public string Name;
        public Particle particle;
        public GameObject ParticlePrefab;
        public float Duration;
    }

    public ParticleSettings[] Particles;    // array of all particles

    public ParticleSettings GetParticleByName(string myName) // Returns a registered prefab with a specific string
    {
        foreach (ParticleSettings i in Particles)
        {
            if (i.Name == myName)
            {
                return i;
            }
        }
        return null;
    }

    public ParticleSettings GetParticleByType(Particle myName) // Returns a registered prefab via enum, which is easier to keep track of
    {
        foreach (ParticleSettings i in Particles)
        {
            if (i.particle == myName)
            {
                return i;
            }
        }
        return null;
    }

    public GameObject CreateParticle(Particle particle, Vector3 AtHere) // creates a particle at a desired location. It automatically destroys itself after it's lifetime has elapsed
    {
        ParticleSettings i = GetParticleByType(particle);

        GameObject instance = Instantiate(i.ParticlePrefab);
        instance.name = i.Name;
        instance.transform.position = AtHere;

        if (i.Duration > 0)
        {
            Destroy(instance, i.Duration);
        }
        return instance;
    }

}