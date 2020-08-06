/////////////////////////////////////////////////////////////
//
//  Script Name: CountdownTimer.cs
//  Creator: Charles Carter
//  Description: The script for the round timer itself
//  
/////////////////////////////////////////////////////////////

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

//Bit different from other basic timers as it needs dev options
public class CountdownTimer : MonoBehaviour
{
    [SerializeField]
    private RoundManager roundManager;
    public Timer roundTimer { get; private set; }

    //Duation is in seconds
    [SerializeField]
    private float duration = 301;

    [SerializeField]
    private Text timerText;

    public UnityEvent timerEnd;

    private void Awake()
    {
        roundManager = roundManager ?? FindObjectOfType<RoundManager>();
        timerText = timerText ?? GetComponent<Text>();
    }

    private void OnEnable()
    {
        RoundManager.CountdownStarted   += StartTimer;
        //RoundManager.RoundPauseToggle += LockTimer;
    }

    private void OnDisable()
    {
        RoundManager.CountdownStarted -= StartTimer;
        //RoundManager.RoundPauseToggle -= LockTimer;
    }

    //Start the timer
    private void StartTimer()
    {
        timerText.enabled = true;
        StartCoroutine(Co_TimerBehaviour());
    }

    //End the timer forcefully
    public void TimerEnded()
    {
        Debug.Log("Round Timer Over", this);
    }

    //The timer behaviour (maybe should be refactored into a multiuse script)
    private IEnumerator Co_TimerBehaviour()
    {
        roundTimer = new Timer(duration);

        while (roundTimer.isActive)
        {
            roundTimer.Tick(Time.deltaTime);
            UpdateTimerUI();
            yield return null;
        }

        timerEnd.Invoke();
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
