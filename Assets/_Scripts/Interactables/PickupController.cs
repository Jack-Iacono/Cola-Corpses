using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class PickupController : MonoBehaviour
{
    //Singleton Stuff
    public static PickupController selectInstance { get; private set; }

    public Collider sightCollider;
    private float interactDistance = 3;
    private bool inRange = false;

    private CameraController mainCamera;
    private GameController gameCont;

    [Tooltip("-1: Nothing\n0: Loot\n1: Part")]
    public int pickupType = -1;
    public bool showInteractMessage = false;

    private InterfaceController intCont;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = FindObjectOfType<CameraController>();
        gameCont = FindObjectOfType<GameController>();
        intCont = FindObjectOfType<InterfaceController>();
    }

    // Update is called once per frame
    void Update()
    {
        //Run while the game is not paused
        if (!GameController.gamePaused)
        {
            //Creates a Singleton where only one of these can be selected at a time which prevents methods from clearing out the text from other methods
            if (selectInstance == null || selectInstance == this)
            {
                //Check if the camera is looking at the item
                if (mainCamera.GetCameraSight(sightCollider, interactDistance))
                {
                    inRange = true;

                    if (showInteractMessage)
                        intCont.screenPair[InterfaceController.Screen.HUD].GetComponent<HUDScreenController>().NewMessage("Press " + ValueStoreController.keyData.keyInteract.ToString() + " to pick up");

                    selectInstance = this;
                }
                else if (inRange)
                {
                    inRange = false;

                    if (showInteractMessage)
                        intCont.screenPair[InterfaceController.Screen.HUD].GetComponent<HUDScreenController>().NewMessage("");

                    selectInstance = null;
                }

                if (inRange)
                {
                    //Do Something
                    if (Input.GetKeyDown(ValueStoreController.keyData.keyInteract))
                    {
                        if (showInteractMessage)
                            intCont.screenPair[InterfaceController.Screen.HUD].GetComponent<HUDScreenController>().NewMessage("");

                        ActivatePickup();

                        selectInstance = null;
                    }
                }
            }
        }
    }

    private void ActivatePickup()
    {
        switch (pickupType)
        {

            case 0:
                GetComponent<LootDropController>().SendMessage("PickUp");
                break;
            case 1:
                GetComponent<EEPieceController>().SendMessage("ActivatePiece", SendMessageOptions.DontRequireReceiver);
                break;
            case 2:
                GetComponent<EEHSController>().SendMessage("ActivateHS", SendMessageOptions.DontRequireReceiver);
                break;
            default:
                Debug.Log("No Item Selected");
                break;

        }
    }
}
