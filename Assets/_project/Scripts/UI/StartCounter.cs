/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///StartCounter.cs
///Developed by James Bradbury
///Test script to countdown text
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StartCounter : MonoBehaviour
{

    [SerializeField] string[] ToShow; // Strings to show can be edited in inspector
    [SerializeField] TextMeshProUGUI textAsset; // Reference to the text component

    private void Start() // At the start of the scene, start the countdown
    {
        StartCoroutine(CountdownCoroutine(0));
    }


    IEnumerator CountdownCoroutine(int Progress) // A recursive call that iterates through the ToShow array, once per second
    {
        textAsset.text = ToShow[Progress];
        yield return new WaitForSeconds(1);

        Progress += 1;
        if (Progress < ToShow.Length)
        {
            StartCoroutine(CountdownCoroutine(Progress));
        }
    }


}
