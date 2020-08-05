
public delegate void TimerEvent();  // delegate

//A class not attached to unity to handle the timer float
public class Timer
{
    //Could be locked
    public bool isLocked;
    public bool isActive;

    //Needed to tick up or down
    float max_time;
    public float current_time { get; private set; }

    public event TimerEvent timerEnd;

    //Constructor
    public Timer(float duration)
    {
        max_time = duration;
        current_time = duration;

        isActive = true;
    }

    //Ticking the timer down
    public void Tick(float delta_time)
    {
        if (current_time.Equals(0f) || isLocked)
        {
            return;
        }

        current_time -= delta_time;
        EndCheck();
    } 

    //Function to check if timer ran out
    void EndCheck()
    {
        //Timer is over
        if (current_time <= 0f)
        {
            current_time = 0f;
            isActive = false;

            if (timerEnd != null)
            {
                timerEnd.Invoke();
            }
        }
    }

    public void OverrideCurrentTime(float amountToChangeBy)
    {
        current_time += amountToChangeBy;
    }
}
