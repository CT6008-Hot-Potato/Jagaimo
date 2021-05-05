////////////////////////////////////////////////////////////
// File: Scriptable sound
// Author: James Bradbury
// Date Created: 1/04/21
// Brief: A script to handle profiling of sound assets
//////////////////////////////////////////////////////////// 

using System;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "SoundProfile", menuName = "ScriptableObjects/SoundProfile", order = 1)]
public class ScriptableSounds : ScriptableObject
{
    // Scriptable Object containing all the potential palette options
    public enum Sounds // All registered game sounds 
    {
        None,
        ButtonPress,
        Explosion,
        PowerUp,
        MenuMusic,
        Grabbing,
        Throwing,
        Sliding,
        Crouching,
        Jumping
    };



    [Serializable]
    public class SoundIdentity 
    {
        public Sounds name;
        public AudioClip[]  file;
        public AudioMixerGroup audioMixer;
        public float defaultVolume;
        public float pitchVariation;
    }

    public SoundIdentity[] soundPalette;    // array of all palettes

   public SoundIdentity GetSoundFromPalette( Sounds soundName)
    {
        SoundIdentity returnSound = new SoundIdentity();

        foreach (SoundIdentity i in soundPalette)
        {
            if(i.name == soundName)
            {
                returnSound = i;
                return i;
            }
        }
        return returnSound;
    }

    

}

