using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class DoorController : InteractableController
{
    #region Characteristics
    public int baseCost = 50;
    private int cost;

    private float doorOpenLength = 3f;
    private List<float> timers = new List<float>();
    private bool opening = false;
    #endregion

    #region Gameobjects
    private InventoryController invCont;
    private InterfaceController intCont;
    private SoundManager soundCont;

    public TMP_Text frontText;
    public TMP_Text backText;

    [SerializeField]
    public List<GameObject> connectedObjects { get; set; } = new List<GameObject>();
    #endregion

    // Prepare door is used to set up variables from the map controller without effecting the interactable controller base initialize method
    public void PrepareDoor(int distNum)
    {
        SetCostMultiplier(distNum);

        Initialize();
    }
    public override void Initialize()
    {
        base.Initialize();

        intCont = FindObjectOfType<InterfaceController>();
        invCont = FindObjectOfType<InventoryController>();
        soundCont = FindObjectOfType<SoundManager>();

        frontText.text = cost.ToString();
        backText.text = cost.ToString();

        timers.Add(0f);
    }

    protected override void ExtraUpdate()
    {
        if (opening)
        {
            TimerManager();
            transform.position += Vector3.up * 2 * Time.deltaTime;
        }
    }

    protected override void Interact()
    {
        if (invCont.tokens >= cost)
        {
            ActivateConnectedObjects();
            FindObjectOfType<MapGenerationController>().OpenDoor(transform.parent.gameObject);

            SoundManager.PlaySoundSource(transform.position, SoundManager.success);

            invCont.tokens -= cost;

            intCont.screenPair[InterfaceController.Screen.HUD].GetComponent<HUDScreenController>().NewToken(invCont.tokens);
            intCont.screenPair[InterfaceController.Screen.HUD].GetComponent<HUDScreenController>().NewMessage("");

            timers[0] = doorOpenLength;
            opening = true;

            InteractPopUpController.EndPopup(gameObject);
        }
        else
        {
            SoundManager.PlaySoundSource(transform.position, SoundManager.error);
        }

        base.Interact();
    }

    protected override void InRangeAction()
    {
        InteractPopUpController.StartPopup(gameObject, ValueStoreController.keyData.keyInteract.ToString());
    }
    protected override void OutRangeAction()
    {
        InteractPopUpController.EndPopup(gameObject);
    }

    private void ActivateConnectedObjects()
    {
        for(int i = 0; i < connectedObjects.Count; i++)
        {
            switch (connectedObjects[i].tag)
            {
                case "Spawner":
                    connectedObjects[i].GetComponent<EnemySpawnController>().Activate(true);
                    break;
                default:
                    Debug.Log("Object Script Not Found");
                    break;
            }
        }
    }
    private void SetCostMultiplier(int roomNum)
    {
        if(roomNum > 0)
        {
            //Sets the door cost equal to the base cost plus the multuplier for the room number
            // Equation is 0.005x^2 + 1
            cost = Mathf.FloorToInt(baseCost * (0.005f * (roomNum * roomNum) + 1));
        }
        else
        {
            cost = baseCost;
        }
    }
    public void LinkObjectList(List<GameObject> list)
    {
        for(int i = 0; i < list.Count; i++)
        {
            connectedObjects.Add(list[i]);
        }
    }

    #region Timer Methods

    public void TimerManager()
    {
        if (timers.Count > 0)
        {
            for (int i = 0; i < timers.Count; i++)
            {
                if (timers[i] <= 0 && timers[i] != -1)
                {
                    timers[i] = -1;

                    switch (i)
                    {
                        case 0:
                            Destroy(gameObject);
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

    #endregion
}
