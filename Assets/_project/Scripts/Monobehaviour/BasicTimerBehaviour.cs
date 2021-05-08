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

//A timer behaviour script seperate from the countdown
public class BasicTimerBehaviour : MonoBehaviour
{
    #region Variables Needed

    [Header("The base timer")]
    [SerializeField]
    private float duration = 1f;
    Timer timer;
    public UnityEvent TimerBehaviour;

    [Header("If there's UI needed")]
    [SerializeField]
    private Text timerText;
    //[SerializeField]
    //private bool isDramatic = false;

    #endregion

    #region Public Methods

    //Starting the timer
    public void CallOnTimerStart()
    {
        timerText = timerText ?? GetComponent<Text>();

        if (timerText && !timerText.enabled)
        {
            timerText.enabled = true;
        }

        StartCoroutine(Co_RunTimer());
    }

    public void SetDuration(float newDuration)
    {
        duration = newDuration;
    }

    #endregion

    #region Private Methods

    //Running the timer
    private IEnumerator Co_RunTimer()
    {
        //It's technically a new timer ontop of the class in use
        timer = new Timer(duration);

        //Whilst it has time left
        while (timer.isActive)
        {
            //Tick each frame
            timer.Tick(Time.deltaTime);

            //And if there's text, update it
            if (timerText)
            {
                UpdateUI();
            }

            yield return null;
        }

        //The text shouldn't show the timer anymore
        timerText.enabled = false;

        //Run whatever is hooked up in the inspector
        if (TimerBehaviour != null)
        {
            TimerBehaviour.Invoke();
        }
    }

    //Updating the timer text
    private void UpdateUI()
    {
        //Getting the closest second
        int seconds = Mathf.FloorToInt(timer.current_time % 60);

        //If it's above 0 display it as a second
        if (seconds > 0)
        {
            timerText.text = seconds.ToString();
        }
        //Or display "GO" since the only timers with texts are the pause ones/start of rounds
        else
        {
            timerText.text = "GO!";
        }
    }

    #endregion
}
