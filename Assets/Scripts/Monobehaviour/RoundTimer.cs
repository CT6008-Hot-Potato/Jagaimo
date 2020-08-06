/////////////////////////////////////////////////////////////
//
//  Script Name: RoundTimer.cs
//  Creator: Charles Carter
//  Description: The script for the round timer itself
//  
/////////////////////////////////////////////////////////////

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

//Bit different from other basic timers as it needs dev options
public class RoundTimer : MonoBehaviour
{
    [SerializeField]
    private RoundManager roundManager;
    public Timer roundTimer { get; private set; }

    //Duation is in seconds
    [SerializeField]
    private float duration = 301;

    [SerializeField]
    private Text timerText;

    private void Awake()
    {
        roundManager = roundManager ?? FindObjectOfType<RoundManager>();
        timerText = timerText ?? GetComponent<Text>();
    }

    private void OnEnable()
    {
        RoundManager.RoundStarted   += StartTimer;
        RoundManager.RoundEnded     += EndTimer;
        //RoundManager.RoundPauseToggle += LockTimer;
    }

    private void OnDisable()
    {
        RoundManager.RoundStarted   -= StartTimer;
        RoundManager.RoundEnded     -= EndTimer;
        //RoundManager.RoundPauseToggle -= LockTimer;

        //Script could get disabled without ever defining the timer
        if (roundTimer != null)
        {
            roundTimer.timerEnd -= EndTimer;
        }
    }

    //Start the timer
    private void StartTimer()
    {
        timerText.enabled = true;
        StartCoroutine(Co_TimerBehaviour());

        roundTimer.timerEnd += EndTimer;
    }

    //End the timer forcefully
    private void EndTimer()
    {
        Debug.Log("Round Timer Over", this);
    }

    //The timer behaviour (maybe should be refactored into a multiuse script)
    private IEnumerator Co_TimerBehaviour()
    {
        roundTimer = new Timer(duration);
        //Debug.Log(roundTimer.isActive);

        while (roundTimer.isActive)
        {
            roundTimer.Tick(Time.deltaTime);
            UpdateTimerUI();
            yield return null;
        }
    }

    //Updates the text UI
    private void UpdateTimerUI()
    {
        float minutes = Mathf.FloorToInt(roundTimer.current_time / 60);
        float seconds = Mathf.FloorToInt(roundTimer.current_time % 60);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    //Dev Options

    //Toggles Locked/Unlocked (For Dev UI)
    public void LockTimer(bool newLocked)
    {
        roundTimer.isLocked = newLocked;
    }

    //Changing the time by an amount given
    public void EditTime(float change)
    {
        if (roundTimer.current_time + change < 0)
        {
            return;
        }

        roundTimer.OverrideCurrentTime(change);
    }

    //Note: The timer wont work when run out
    public void ResetTimer()
    {
        StartTimer();
    }
}
