/////////////////////////////////////////////////////////////
//
//  Script Name: BasicTimeBehaviour.cs
//  Creator: Charles Carter
//  Description: A script for a simple timer
//  
/////////////////////////////////////////////////////////////

//This script uses these namespaces
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BasicTimerBehaviour : MonoBehaviour
{
    [Header("The base timer")]
    [SerializeField]
    private float duration;
    Timer timer;
    public UnityEvent TimerBehaviour;

    [Header("If there's UI needed")]
    [SerializeField]
    private Text timerText;
    [SerializeField]
    private bool isDramatic = false;

    // Start is called before the first frame update
    void Start()
    {
        //This will be called on round start event at some point
        CallOnTimerStart();    
    }

    //Starting the timer
    private void CallOnTimerStart()
    {
        timerText = timerText ?? GetComponent<Text>();
        StartCoroutine(Co_RunTimer());
    }

    //Running the timer
    private IEnumerator Co_RunTimer()
    {
        timer = new Timer(duration);

        while (timer.isActive)
        {
            timer.Tick(Time.deltaTime);

            if (timerText)
            {
                UpdateUI();
            }
            yield return null;
        }

        timerText.enabled = false;
        TimerBehaviour.Invoke();
    }

    //Updating the timer text
    private void UpdateUI()
    {
        float seconds = Mathf.FloorToInt(timer.current_time % 60);
        if (seconds > 0)
        {
            timerText.text = seconds.ToString();
        }
        else
        {
            timerText.text = "GO!";
        }
    }
}
