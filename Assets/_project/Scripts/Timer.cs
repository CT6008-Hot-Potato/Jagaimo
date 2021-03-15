/////////////////////////////////////////////////////////////
//
//  Script Name: Timer.cs
//  Creator: Charles Carter
//  Description: A base script for the timer functionality
//  
/////////////////////////////////////////////////////////////

//A class not attached to unity to handle the timer float
public class Timer
{
    //Could be locked
    public bool isLocked;
    public bool isActive;

    //Needed to tick up or down
    public float max_time { get; private set; }

    public float min_time { get; private set; }
    public float current_time { get; private set; }

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
        if (current_time.Equals(min_time) || isLocked)
        {
            return;
        }

        current_time -= delta_time;
        EndCheck();
    } 

    //Function to check if timer ran out
    private void EndCheck()
    {
        //Timer is over
        if (current_time <= min_time)
        {
            current_time = min_time;
            isActive = false;
        }
    }

    //If something wants to add/remove time
    public void OverrideCurrentTime(float amountToChangeBy)
    {
        current_time += amountToChangeBy;
    }
}
