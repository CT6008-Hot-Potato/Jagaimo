////////////////////////////////////////////////////////////
// File: SoundManager
// Author: James Bradbury
// Brief: A script for testing menu functions
//////////////////////////////////////////////////////////// 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSingleton : MonoBehaviour
{
    
    public static MenuSingleton instance;
    
    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this.gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

    } // sets up the Singleton patern

    public void CloseGame()
    {
        Application.Quit();
    } // closes the game
    public void ChangeLevel(int level)
    {

        SceneManager.LoadScene(level);
    
    } // changes the scene by int ref
    public void PlaySound(AudioClip PlayMe, Vector3 AtHere, float PitchVariation)
    {
        GameObject MyObject = new GameObject(PlayMe.name);
        MyObject.transform.position = AtHere;
        AudioSource MyAudio = MyObject.AddComponent<AudioSource>();

        MyAudio.pitch = MyAudio.pitch + Random.Range(-PitchVariation, PitchVariation);
        MyAudio.clip = PlayMe;
        MyAudio.Play();

        Destroy(MyObject, PlayMe.length);
    } // plays a sound at a position and pitch varaiation


}
