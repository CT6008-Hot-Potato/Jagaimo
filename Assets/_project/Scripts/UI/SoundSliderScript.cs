﻿////////////////////////////////////////////////////////////
// File: SoundSliderScript
// Author: James Bradbury
// Brief: A script to create and control sound sliders in the game
//////////////////////////////////////////////////////////// 


using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
public class SoundSliderScript : MonoBehaviour
{
    
    [SerializeField] GameObject SliderPrefab; // reference to the slider prefab
    [SerializeField] AudioMixer MyMixer;  //  reference to the games Mixer object
    [SerializeField] AudioMixerGroup[] MyMixers; // ref to the groups inside the Mixer

    [SerializeField] float Max, Min, DefaultVal; // Stores the sliders minimum, max, and default values
    void OnEnable() // If the soundslider object is enabled and the sliders aren't there already, create them  
    {
        if(transform.childCount ==0)
        {
            for (int i = 1; i <= MyMixers.Length; i++)
            {
                GameObject newSlider = Instantiate(SliderPrefab, gameObject.transform);
                newSlider.TryGetComponent(out Slider slider);
                slider.maxValue = Max;
                slider.minValue = Min;
                slider.value = Mathf.Lerp(Min, Max, DefaultVal);
                slider.GetComponentInChildren<TextMeshProUGUI>().text = MyMixers[i - 1].name;

                int j = i;
                ChangeMixVolume(slider.value, j);


                slider.onValueChanged.AddListener(value => ChangeMixVolume(slider.value, j));


            }

        }
    }

    void ChangeMixVolume(float newValue, int audioMixerIndex) // When a value on the slider is changed, this function changes the corresponding value on the mixer  
    {
        if (MyMixer.SetFloat("MyExposedParam" + audioMixerIndex, newValue))
        {

        }
        else
        {
            Debug.Log("MyExposedParam" + audioMixerIndex + " didn't work.");
        }
    }

}
