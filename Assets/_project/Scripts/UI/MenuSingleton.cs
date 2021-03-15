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

    }



    public void CloseGame()
    {
        Application.Quit();
    }
    public void ChangeLevel(int level)
    {

        SceneManager.LoadScene(level);
    
    }

    public void PlaySound(AudioClip PlayMe, Vector3 AtHere, float PitchVariation)
    {
        GameObject MyObject = new GameObject(PlayMe.name);
        MyObject.transform.position = AtHere;
        AudioSource MyAudio = MyObject.AddComponent<AudioSource>();

        MyAudio.pitch = MyAudio.pitch + Random.Range(-PitchVariation, PitchVariation);
        MyAudio.clip = PlayMe;
        MyAudio.Play();

        Destroy(MyObject, PlayMe.length);
    }


}
