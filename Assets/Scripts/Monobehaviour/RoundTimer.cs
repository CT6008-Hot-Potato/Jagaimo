using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Just for testing
public class RoundTimer : MonoBehaviour
{
    [SerializeField]
    RoundManager roundManager;
    public Timer roundTimer { get; private set; }

    [SerializeField]
    float duration;

    [SerializeField]
    Text timerText;

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

        roundTimer.timerEnd -= EndTimer;
    }

    //Start the timer
    void StartTimer()
    {
        roundTimer = new Timer(duration);
        StartCoroutine(TimerBehaviour(roundTimer));

        roundTimer.timerEnd += EndTimer;
    }

    //End the timer forcefully
    void EndTimer()
    {
        Debug.Log("Timer Over");
    }

    //The timer behaviour
    IEnumerator TimerBehaviour(Timer timer)
    {
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            timer.Tick(Time.deltaTime);
            UpdateTimerUI();
            yield return null;
        }
    }

    //Updates the text UI
    void UpdateTimerUI()
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
}
