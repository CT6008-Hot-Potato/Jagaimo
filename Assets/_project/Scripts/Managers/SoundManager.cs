////////////////////////////////////////////////////////////
// File: SoundManager
// Author: James Bradbury
// Date Created: 15/03/21
// Brief: A script to manage the playing of sounds over mixers

//Attach this to a singleton, and call it when you need a sound played!
//////////////////////////////////////////////////////////// 

using UnityEngine;
using UnityEngine.Audio;
public class SoundManager : MonoBehaviour
{
    [SerializeField] AudioMixerGroup DefaultAudioMixer; // refererence to the audio mixer
  
    [SerializeField] ScriptableSounds SoundBoard; // reference to the sound profiler
    public void PlaySound(ScriptableSounds.Sounds Sound, Vector3 AtHere)
    {
        ScriptableSounds.SoundIdentity i = SoundBoard.GetSoundFromPalette(Sound);
        AudioClip PlayMe = i.file[Random.Range(0, i.file.Length)];
        
        GameObject MyObject = new GameObject(PlayMe.name);
        MyObject.transform.position = AtHere;
        AudioSource MyAudio = MyObject.AddComponent<AudioSource>();


        MyAudio.volume = i.defaultVolume;
        MyAudio.pitch = MyAudio.pitch + Random.Range(-i.pitchVariation, i.pitchVariation);

        MyAudio.clip = PlayMe;
        MyAudio.outputAudioMixerGroup = i.audioMixer;

        if (MyAudio != null)
        {
            MyAudio.Play();
        }

        Destroy(MyObject, PlayMe.length);
    } // Plays a sound at a specific location, using a volume in the profiler

    public void PlaySound(ScriptableSounds.Sounds Sound)
    {
        if (Sound != 0)
        {
            ScriptableSounds.SoundIdentity i = SoundBoard.GetSoundFromPalette(Sound);
            AudioClip PlayMe = i.file[Random.Range(0, i.file.Length)];

            GameObject MyObject = new GameObject(PlayMe.name);
            MyObject.transform.parent = transform;
            MyObject.transform.position = Vector3.zero;
            AudioSource MyAudio = MyObject.AddComponent<AudioSource>();

            MyAudio.volume = i.defaultVolume;
            MyAudio.pitch = MyAudio.pitch + Random.Range(-i.pitchVariation, i.pitchVariation);
            MyAudio.clip = PlayMe;
            MyAudio.outputAudioMixerGroup = i.audioMixer;

            if (MyAudio != null)
            {
                MyAudio.Play();
            }
            Destroy(MyObject, PlayMe.length);

        }
    } // If no location is supplied, the sound defaults to the position of the main camera



}
