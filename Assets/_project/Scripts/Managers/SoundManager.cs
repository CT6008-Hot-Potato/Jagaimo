////////////////////////////////////////////////////////////
// File: SoundManager
// Author: James Bradbury
// Date Created: 15/03/21
// Brief: A script to manage the playing of sounds over mixers

//Attach this to a singleton, and call it when you need a sound played!
//////////////////////////////////////////////////////////// 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
public class SoundManager : MonoBehaviour
{
    [SerializeField] AudioMixerGroup DefaultAudioMixer;
    [SerializeField]
    private AudioClip sound;
    public void PlaySound(AudioClip PlayMe, Vector3 AtHere, float PitchVariation)
    {
        GameObject MyObject = new GameObject(PlayMe.name);
        MyObject.transform.position = AtHere;
        AudioSource MyAudio = MyObject.AddComponent<AudioSource>();

        MyAudio.pitch = MyAudio.pitch + Random.Range(-PitchVariation, PitchVariation);
        MyAudio.clip = PlayMe;
        MyAudio.outputAudioMixerGroup = DefaultAudioMixer;

        MyAudio.Play();
        Destroy(MyObject, PlayMe.length);
    }

    public void PlaySound(AudioClip PlayMe, float PitchVariation)
    {
        GameObject MyObject = new GameObject(PlayMe.name);
        MyObject.transform.parent = Camera.main.transform;
        MyObject.transform.position = Vector3.zero;

        AudioSource MyAudio = MyObject.AddComponent<AudioSource>();

        MyAudio.pitch = MyAudio.pitch + Random.Range(-PitchVariation, PitchVariation);
        MyAudio.clip = PlayMe;
        MyAudio.outputAudioMixerGroup = DefaultAudioMixer;

        MyAudio.Play();
        Destroy(MyObject, PlayMe.length);
    }
    public void PlaySound(AudioClip PlayMe)
    {
        GameObject MyObject = new GameObject(PlayMe.name);
        MyObject.transform.parent = transform;
        MyObject.transform.position = Vector3.zero;

        AudioSource MyAudio = MyObject.AddComponent<AudioSource>();

        MyAudio.clip = PlayMe;
        MyAudio.outputAudioMixerGroup = DefaultAudioMixer;

        MyAudio.Play();
        Destroy(MyObject, PlayMe.length);
    }
    public void PlaySound(AudioClip PlayMe, AudioMixerGroup audioMixer)
    {
        GameObject MyObject = new GameObject(PlayMe.name);
        MyObject.transform.parent = Camera.main.transform;
        MyObject.transform.position = Vector3.zero;

        AudioSource MyAudio = MyObject.AddComponent<AudioSource>();

        MyAudio.clip = PlayMe;
        MyAudio.outputAudioMixerGroup = audioMixer;

        MyAudio.Play();
        Destroy(MyObject, PlayMe.length);
    }
    public void PlaySound(AudioClip PlayMe, Vector3 AtHere, AudioMixerGroup audioMixer)
    {
        GameObject MyObject = new GameObject(PlayMe.name);
        MyObject.transform.position = AtHere;

        AudioSource MyAudio = MyObject.AddComponent<AudioSource>();
        MyAudio.clip = PlayMe;
        MyAudio.outputAudioMixerGroup = audioMixer;
        MyAudio.Play();
        Destroy(MyObject, PlayMe.length);
    }
    public void PlaySound(AudioClip PlayMe, Vector3 AtHere, float PitchVariation, AudioMixerGroup audioMixer)
    {
        GameObject MyObject = new GameObject(PlayMe.name);
        MyObject.transform.position = AtHere;
        AudioSource MyAudio = MyObject.AddComponent<AudioSource>();

        MyAudio.pitch = MyAudio.pitch + Random.Range(-PitchVariation, PitchVariation);
        MyAudio.clip = PlayMe;
        MyAudio.outputAudioMixerGroup = audioMixer;

        MyAudio.Play();
        Destroy(MyObject, PlayMe.length);
    }
}
