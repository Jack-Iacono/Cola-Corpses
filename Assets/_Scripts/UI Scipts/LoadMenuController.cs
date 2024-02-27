using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LoadMenuController : MonoBehaviour
{
    [Header("Map Display Area")]
    public GameObject mapDataArea;
    public GameObject mapButtonObject;
    public float mapButtonHeight;

    private List<GameObject> mapButtons = new List<GameObject>();

    [Header("Other Buttons")]
    public GameObject loadButton;
    public GameObject loadSeedButton;
    public GameObject deleteButton;

    public static string selectedMap = "";

    // Start is called before the first frame update
    void Start()
    {
        NewLoadMenu();
    }

    // Update is called once per frame
    public void NewLoadMenu()
    {
        foreach(GameObject g in mapButtons)
        {
            Destroy(g);
        }
        mapButtons = new List<GameObject>();

        List<string> mapNames = SaveDataStore.GetSeedNames();

        for (int i = 0; i < mapNames.Count; i++)
        {
            // Creates a button from the given object and sets its parent
            GameObject text = Instantiate(mapButtonObject, mapDataArea.transform);
            text.GetComponentInChildren<TMP_Text>().text = mapNames[i];

            RectTransform rect = text.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(0, i * -mapButtonHeight + -mapButtonHeight / 2);

            text.GetComponent<LoadMenuButtonController>().seedName = mapNames[i];

            if (mapNames[i] == selectedMap)
            {
                text.GetComponent<Image>().color = MyColor.grey;
            }
            else
            {
                text.GetComponent<Image>().color = MyColor.iceBlue;
            }

            mapButtons.Add(text);
        }

        //Adjusts the size of the area on the canvas
        mapDataArea.GetComponent<RectTransform>().sizeDelta = new Vector2(1, mapButtons.Count * mapButtonHeight);

        CheckLoadButton();
        CheckSeedButtons();
    }
    public void CheckLoadButton()
    {
        if (SaveDataStore.FileExists(0))
        {
            loadButton.GetComponent<Button>().enabled = true;
            loadButton.GetComponent<Image>().color = MyColor.iceBlue;
            loadButton.GetComponentInChildren<TMP_Text>().text = "Load";
            loadButton.GetComponent<EventTrigger>().enabled = true;
        }
        else
        {
            loadButton.GetComponent<Button>().enabled = false;
            loadButton.GetComponent<Image>().color = Color.grey;
            loadButton.GetComponentInChildren<TMP_Text>().text = "No Data";
            loadButton.GetComponent<EventTrigger>().enabled = false;
        }
    }
    public void CheckSeedButtons()
    {
        if(selectedMap != "")
        {
            loadSeedButton.GetComponent<Button>().enabled = true;
            loadSeedButton.GetComponent<Image>().color = MyColor.iceBlue;
            loadSeedButton.GetComponent<EventTrigger>().enabled = true;

            deleteButton.GetComponent<Button>().enabled = true;
            deleteButton.GetComponent<Image>().color = MyColor.iceBlue;
            deleteButton.GetComponent<EventTrigger>().enabled = true;
        }
        else
        {
            loadSeedButton.GetComponent<Button>().enabled = false;
            loadSeedButton.GetComponent<Image>().color = Color.grey;
            loadSeedButton.GetComponent<EventTrigger>().enabled = false;

            deleteButton.GetComponent<Button>().enabled = false;
            deleteButton.GetComponent<Image>().color = Color.grey;
            deleteButton.GetComponent<EventTrigger>().enabled = false;
        }
    }

    public void SetSeedName(string seed)
    {
        selectedMap = seed;

        NewLoadMenu();
    }

    public void DeleteSelectedItem()
    {
        SaveDataStore.DeleteData(2, selectedMap);
        selectedMap = "";
        NewLoadMenu();
    }
}
