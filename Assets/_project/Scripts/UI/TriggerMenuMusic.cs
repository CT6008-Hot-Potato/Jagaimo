////////////////////////////////////////////////////////////
// File: TriggerMenuMusic
// Author: James Bradbury
// Brief: A script to simply play the main menu music after the game is done loading
//////////////////////////////////////////////////////////// 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class TriggerMenuMusic : MonoBehaviour
{
    [SerializeField] AudioClip DefaultMusic;
    [SerializeField] AudioMixerGroup MixerGroup;
    private void OnEnable()
    {
        if (Camera.main.gameObject.TryGetComponent( out AudioSource audio))
        {

        }
        else
        {
            AudioSource MenuMusic = Camera.main.gameObject.AddComponent<AudioSource>();
            MenuMusic.clip = DefaultMusic;
            MenuMusic.loop = true;
            MenuMusic.outputAudioMixerGroup = MixerGroup;
            MenuMusic.Play();
        }
    }
}
