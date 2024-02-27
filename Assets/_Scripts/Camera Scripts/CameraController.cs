using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    #region Gameobjects
    public static CameraController Instance { get; private set; }

    public GameObject player;
    #endregion

    #region Control Variables
    public Vector3 cameraOffset = Vector3.zero;
    public LayerMask sightCollision;
    #endregion

    #region Private Variables
    private float xRotation;
    private float yRotation;

    private Vector2 recoilPosition = Vector2.zero;
    private Vector2 recoilForce = Vector2.zero;
    private Vector3 recoilReturnAngle = Vector3.zero;

    private float[] timers = new float[1];
    #endregion

    // Start is called before the first frame update
    public void Initialize()
    {
        //Sets camera rotation to player's rotation
        xRotation = 0;
        yRotation = player.transform.rotation.eulerAngles.y;
    }
    public void SetStaticInstance()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }
    ~CameraController() { if (Instance == this) Instance = null; } 

    private void Update()
    {
        if (!GameController.gamePaused)
        {
            MoveCamera();
            TimerManager();
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!GameController.gamePaused)
        {
            RotateCamera();
        }
    }

    private void RotateCamera()
    {
        float mouseX = Input.GetAxis("Mouse X") * ValueStoreController.prefData.sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * ValueStoreController.prefData.sensitivity * Time.deltaTime;

        //Gets the real rotation of the camera
        xRotation = xRotation - mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        yRotation = yRotation + mouseX;
        yRotation = ClampAngle(yRotation);

        transform.localRotation = Quaternion.Euler(Mathf.Clamp(xRotation, -90, 90), 0f, 0f);
        player.transform.localRotation = Quaternion.Euler(0f, yRotation, 0f);
    }

    private void MoveCamera()
    {
        transform.position = player.transform.position + cameraOffset;
    }
    private float ClampAngle(float f)
    {
        if (f > 360)
        {
            return 0 + (f - 360);
        }
        else if (f < 0)
        {
            return 360 + f;
        }
        else
        {
            return f;
        }
    }

    #region Sight Methods

    public bool GetCameraSight(Collider col, float dist)
    {
        float distance = Vector3.SqrMagnitude(col.transform.position - (transform.position - transform.up * -0.15f));

        if(distance <= dist * dist)
        {
            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hit;

            float range = 50;

            if (Physics.Raycast(ray, out hit, range, sightCollision))
            {
                if (hit.collider == col)
                {
                    return true;
                }
            }
        }

        return false;
    }

    #endregion

    #region Timer Methods

    /*
     * 0: Recoil Return Timer 
     * 
     * 
    */
    public void TimerManager()
    {
        if (timers.Length > 0)
        {
            for (int i = 0; i < timers.Length; i++)
            {
                if (timers[i] <= 0 && timers[i] != -1)
                {
                    timers[i] = -1;

                    switch (i)
                    {
                        case 0:
                            
                            break;
                    }

                }
                else
                {
                    if (timers[i] > 0)
                    {
                        timers[i] -= 1 * Time.deltaTime;
                    }
                }
            }
        }
    }

    /*  The below method uses integers to represent the different timers that may be used within this script, the ints represent the following timers
     * 0: fireReady Timer
     * 1: burstTimer
     * 2: reloadFinish Timer
    */
    public void SetTimer(int i, float f)
    {
        timers[i] = f;
    }

    #endregion
}