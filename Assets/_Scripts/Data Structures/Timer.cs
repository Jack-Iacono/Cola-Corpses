using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer
{
    public float timerLength;
    public float currentTime;

    private bool running = false;

    public delegate void TimerMethod();
    public TimerMethod endMethod;

    public Timer(float timerLength)
    {
        this.timerLength = timerLength;
        currentTime = 0;
        running = false;

        endMethod = null;
    }
    public Timer(float timerLength, TimerMethod endMethod)
    {
        this.timerLength = timerLength;
        currentTime = 0;
        running = false;

        this.endMethod = endMethod;
    }

    public bool CheckTime(float dt)
    {
        if (running)
        {
            if (currentTime < timerLength)
            {
                currentTime += dt;
                return false;
            }
            else
            {
                Stop();
                endMethod();
                return true;
            }
        }
        else { return false; }
    }

    public void ChangeLength(float length)
    {
        timerLength = length;
        currentTime = 0;
    }
    public void Restart()
    {
        currentTime = 0;
        running = true;
    }
    public void Reset()
    {
        currentTime = 0;
        running = false;
    }
    public void Start()
    {
        running = true;
    }
    public void Start(float time)
    {
        ChangeLength(time);
        running = true;
    }
    public void Stop()
    {
        running = false;
    }
}
