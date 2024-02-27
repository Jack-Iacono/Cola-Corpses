using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextMovementController : MonoBehaviour
{
    private Vector2 originPosition;
    private RectTransform trans;

    private float period = 0;

    public float xAmplitude = 5;
    public float yAmplitude = 5;
    public float speed = 1;

    // Start is called before the first frame update
    void Start()
    {
        trans = GetComponent<RectTransform>();
        originPosition = trans.anchoredPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (period < 360)
            period += Time.deltaTime * speed;
        else
            period = 0;

        trans.anchoredPosition = new Vector2(xAmplitude * Mathf.Cos(Mathf.Deg2Rad * period), yAmplitude * Mathf.Sin(Mathf.Deg2Rad * period));
    }
}
