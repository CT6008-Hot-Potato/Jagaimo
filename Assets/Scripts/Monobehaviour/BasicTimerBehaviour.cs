using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BasicTimerBehaviour : MonoBehaviour
{
    [Header("The base timer")]
    [SerializeField]
    float duration;
    Timer timer;
    public UnityEvent TimerBehaviour;

    [Header("If there's UI needed")]
    [SerializeField]
    Text timerText;
    [SerializeField]
    bool isDramatic = false;

    // Start is called before the first frame update
    void Start()
    {
        CallOnTimerStart();    
    }

    //Starting the timer
    void CallOnTimerStart()
    {
        if (!timerText)
        {
            timerText = GetComponent<Text>();
        }
        StartCoroutine(TimerStart());
    }

    //Running the timer
    IEnumerator TimerStart()
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

        TimerBehaviour.Invoke();
    }

    //Updating the timer text
    void UpdateUI()
    {
        float seconds = Mathf.FloorToInt(timer.current_time % 60);
        if (seconds > 0)
        {
            timerText.text = seconds.ToString();
        }
        else
        {
            timerText.enabled = false;
        }
    }
}
