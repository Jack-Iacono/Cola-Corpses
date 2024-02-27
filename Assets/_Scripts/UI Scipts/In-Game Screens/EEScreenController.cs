using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EEScreenController : InterfaceScreenController
{

    public GameObject checkListObject;
    public GameObject checkListParent;
    private List<GameObject> checkListAreas = new List<GameObject>();

    public GameObject endButton;

    public override void InitializeScreen()
    {
        base.InitializeScreen(); 
    }

    public override void ShowScreen()
    {
        base.ShowScreen();

        NewEasterEgg();
    }
    public override void HideScreen()
    {
        base.HideScreen();
    }

    public void NewEasterEgg()
    {
        bool[] easterEggSteps = GameController.gameCont.activeEE;
        List<List<int>> checkList = GameController.gameCont.checkList;

        foreach (GameObject g in checkListAreas)
        {
            Destroy(g);
        }
        checkListAreas = new List<GameObject>();

        for (int i = 0; i < easterEggSteps.Length; i++)
        {
            //Checks if there is an active easter egg step
            if (easterEggSteps[i])
            {
                // Creates a button from the given object and sets its parent
                GameObject text = Instantiate(checkListObject, checkListParent.transform);
                TMP_Text[] texts = text.GetComponentsInChildren<TMP_Text>();

                texts[0].text = GameController.eeNames[i];
                texts[1].text = GetEasterEggString(i, checkList[i]);

                RectTransform rect = text.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(0, checkListAreas.Count * -160 + -70);

                checkListAreas.Add(text);
            }
        }

        //Adjusts the size of the area on the canvas
        checkListParent.GetComponent<RectTransform>().sizeDelta = new Vector2(1, checkListAreas.Count * 160);

        if (GameController.gameCont.CheckEndCriteria())
        {
            endButton.GetComponent<MenuButtonController>().enabled = true;
            endButton.GetComponent<Image>().color = Color.white;
            endButton.GetComponentInChildren<TMP_Text>().text = "Call\nMom";
        }
        else
        {
            endButton.GetComponent<MenuButtonController>().enabled = false;
            endButton.GetComponent<Image>().color = Color.grey;
            endButton.GetComponentInChildren<TMP_Text>().text = "Not\nYet";
        }
    }
    public static string GetEasterEggString(int i, List<int> checkList)
    {
        string text = "";
        switch (i)
        {
            case 0:
            case 1:
            case 2:
            case 3:
                //Runs through the list for the denoted i index to check which indexes are true or false
                for (int k = 0; k < checkList.Count; k++)
                {
                    if (k != 0)
                        text += " ";

                    if (checkList[k] == -1)
                        text += "b";
                    else
                        text += "s";
                }
                break;
            case 4:
            case 5:
                for (int k = 0; k < checkList.Count; k++)
                {
                    if (checkList[k] > -1)
                    {
                        for (int l = 0; l < checkList[k] + 1; l++)
                        {
                            if (l != 0)
                                text += " ";

                            text += "q";
                        }
                    }
                    else
                    {
                        text += "i";
                    }

                }
                break;
        }

        return text;
    }

    #region Buttons

    public void EndGame()
    {
        GameController.gameCont.EndGame(true);
    }

    #endregion
}
