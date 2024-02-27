using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PopController : MonoBehaviour
{
    private Rigidbody rBody;
    private GameObject target;

    private List<float> timers = new List<float>();

    private float xMove;
    private float yMove;
    private float grav = -9.8f;

    public void Initialize()
    {
        target = PlayerController.playerCont.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        TimerManager();

        transform.position += new Vector3(xMove, yMove, 0) * Time.deltaTime;
        yMove += grav * Time.deltaTime;

        if(target)
            transform.LookAt(target.transform);
    }

    public void StartMove(GameObject obj)
    {
        //runs setup if not run before
        if(timers.Count == 0)
        {
            //Adds the elements to the timer list
            timers.Add(0f);
        }

        //Sets popup location
        transform.position = obj.transform.position + ((Vector3.up * 0.5f) * (obj.transform.lossyScale.y / 2));

        //Sets the lifespan of the popUp
        SetTimer(0, 1);

        //Sets movement pattern of popUps
        xMove = Random.Range(-3, 3);
        yMove = Random.Range(3, 5);
    }
    public void SetColor(Color c)
    {
        GetComponentInChildren<TextMeshPro>().color = c;
    }

    public void EndMove()
    {
        xMove = 0;
        yMove = 0;
        transform.position = Vector3.zero;

        gameObject.SetActive(false);
    }

    public void SetText(string t)
    {
        TextMeshPro textTemp = GetComponentInChildren<TextMeshPro>();
        textTemp.text = t;
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
