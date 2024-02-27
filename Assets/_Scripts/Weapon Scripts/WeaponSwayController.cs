using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwayController : MonoBehaviour
{
    public GameObject followCamera;

    public float smoothing;
    public float sway;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * sway;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sway;
    }
}
