using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonInput<T>
{
    public bool hold;
    public bool down;
    public bool up;
    public float downTime;
    public float upTime;
    public T value;
    public float upDuration { get { return Time.time - upTime; } }
    public float downDuration { get { return Time.time - downTime; } }

    public void Update(T value, bool condition)
    {
        this.value = value;
        if (condition)
        {
            if (!hold)
            {
                down = true;
                downTime = Time.time;
            }
            else
            {
                down = false;
            }
            hold = true;
            up = false;
        }
        else
        {
            if (hold)
            {
                up = true;
                upTime = Time.time;
            }
            else
            {
                up = false;
            }
            hold = false;
            down = false;
        }
    }
    
}
