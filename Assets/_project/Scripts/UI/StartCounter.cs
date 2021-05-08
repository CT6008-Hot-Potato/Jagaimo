using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StartCounter : MonoBehaviour
{

    [SerializeField] string[] ToShow;

    [SerializeField] TextMeshProUGUI textAsset;

    private void Start()
    {
        StartCoroutine(CountdownCoroutine(0));
    }


    IEnumerator CountdownCoroutine(int Progress)
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
