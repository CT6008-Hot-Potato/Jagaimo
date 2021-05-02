/////////////////////////////////////////////////////////////
//
//  Script Name: CountdownTimer.cs
//  Creator: Charles Carter
//  Description: The script for the round timer itself
//  
/////////////////////////////////////////////////////////////

//This script uses these namespaces
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

//Bit different from other basic timers as it needs dev options
public class CountdownTimer : MonoBehaviour
{
    #region Variables Needed

    [SerializeField]
    private RoundManager roundManager;
    private GameSettingsContainer settings;

    public Timer roundTimer { get; private set; }

    //Duation is in seconds
    [SerializeField]
    private float duration = 301;

    [SerializeField]
    private Text timerText;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        settings = GameSettingsContainer.instance;
        roundManager = roundManager ?? RoundManager.roundManager;
        timerText = timerText ?? GetComponent<Text>();
    }

    private void Start()
    {
        if (settings)
        {
            //If any come out with weird values, it'll need a fixing on the menu
            if (settings.HasGenMutator(2))
            {
                //This is the value of the countdown time mutator
                float newDur = (float)settings.FindGeneralMutatorValue(2);

                //Making sure it doesnt return a nothing value
                if (newDur != 0)
                {
                    //Making it into minutes + an offset for the timer
                    duration = (newDur * 60) + 1;
                }
            }
        }
    }

    private void OnEnable()
    {
        RoundManager.CountdownStarted   += StartTimer;
        RoundManager.CountdownEnded += TimerEndedDebug;
        RoundManager.RoundEnded += RoundEnded;
    }

    private void OnDisable()
    {
        RoundManager.CountdownStarted -= StartTimer;
        RoundManager.CountdownEnded -= TimerEndedDebug;
        RoundManager.RoundEnded -= RoundEnded;
    }

    #endregion

    #region Public Methods

    //End the timer forcefully
    public void TimerEndedDebug()
    {
        if (Debug.isDebugBuild)
        {
            Debug.Log("Round Timer Over", this);
        }
    }

    public void RoundEnded()
    {
        enabled = false;
    }

    //Dev Options
    //Toggles Locked/Unlocked
    public void LockTimer(bool newLocked)
    {
        if (roundTimer != null)
        {
            roundTimer.isLocked = newLocked;
        }
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

    //Reset really just starts a new timer
    public void ResetTimer()
    {
        StartTimer();
    }

    //Start the timer
    private void StartTimer()
    {
        timerText.enabled = true;
        StartCoroutine(Co_TimerBehaviour());
    }

    #endregion

    #region Private Methods

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

        roundManager.CallOnCountdownEnd();
    }

    //Updates the text UI
    private void UpdateTimerUI()
    {
        float minutes = Mathf.FloorToInt(roundTimer.current_time / 60);
        float seconds = Mathf.FloorToInt(roundTimer.current_time % 60);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    #endregion

    #region Protected Methods

    protected float GetCurrentTime()
    {
        return roundTimer.current_time;
    }

    protected float GetMaxTime()
    {
        return roundTimer.max_time;
    }

    protected float GetMinTime()
    {
        return roundTimer.min_time;
    }

    #endregion
}
