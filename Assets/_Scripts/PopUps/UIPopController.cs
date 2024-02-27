using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class UIPopController : MonoBehaviour
{
    private float xVel = 0;
    private float yVel = 0;
    private bool grav = false;

    private float lifeSpan = 4;
    private List<float> timers = new List<float>();

    // Update is called once per frame
    void Update()
    {
        if (grav)
        {
            transform.position += new Vector3(xVel, yVel, 0) * Time.deltaTime;

            yVel -= 200f * Time.deltaTime;
        }
        else
        {
            transform.position += new Vector3(xVel, yVel, 0) * Time.deltaTime;
        }

        TimerManager();
    }

    public void StartMove(float x, float y, bool g, string s, float scale, Vector3 pos, GameObject UI)
    {
        //runs setup if not run before
        if (timers.Count == 0)
        {
            //Adds the elements to the timer list
            timers.Add(0f);
        }

        transform.SetParent(UI.transform);

        transform.position = pos;

        transform.localScale = new Vector3(scale,scale,scale);

        xVel = x;
        yVel = y;
        grav = g;
        GetComponent<TMP_Text>().text = s;

        //Sets a timer for the life time of the particle
        timers[0] = lifeSpan;
    }

    public void EndMove()
    {
        xVel = 0;
        yVel = 0;
        grav = false;
        transform.localScale = new Vector3(1, 1, 1);
        transform.position = Vector3.zero;

        gameObject.SetActive(false);
    }

    #region Timer Methods

    public void TimerManager()
    {
        if (timers.Count > 0)
        {
            for (int i = 0; i < timers.Count; i++)
            {
                if (timers[i] <= 0)
                {
                    timers[i] = 0;

                    switch (i)
                    {
                        case 0:
                            EndMove();
                            break;
                    }

                }
                else
                {
                    timers[i] -= 1 * Time.deltaTime;
                }
            }
        }
    }

    /*  The below method uses integers to represent the different timers that may be used within this script, the ints represent the following timers
     * 0: attackReady Timer
    */
    public void SetTimer(int i, float f)
    {
        timers[i] = f;
    }

    #endregion
}
