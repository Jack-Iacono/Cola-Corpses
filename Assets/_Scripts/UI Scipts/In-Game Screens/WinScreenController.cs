using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinScreenController : InterfaceScreenController
{
    public TMP_Text winStats;

    public override void InitializeScreen()
    {
        base.InitializeScreen();
    }

    public override void ShowScreen()
    {
        base.ShowScreen();

        NewWin();
    }
    public override void HideScreen()
    {
        base.HideScreen();
    }

    public void NewWin()
    {
        string timeText = "";
        float time = GameController.gameCont.totalTime;
        int timeSub = 0;

        //Hours
        if (time >= 3600)
        {
            timeSub = Mathf.FloorToInt(time / 3600);
            timeText += timeSub.ToString() + ":";
            time -= timeSub;
        }
        else
        {
            timeText += "00:";
        }
        //Minutes
        if (time >= 60)
        {
            timeSub = Mathf.FloorToInt(time / 60);

            if (timeSub < 10)
            {
                timeText += "0" + timeSub.ToString() + ":";
                time -= timeSub;
            }
            else
            {
                timeText += timeSub.ToString() + ":";
                time -= timeSub;
            }
        }
        else
        {
            timeText += "00:";
        }
        //Seconds
        if (time < 10)
        {
            timeSub = Mathf.FloorToInt(time);
            timeText += "0" + timeSub.ToString();
        }
        else
        {
            timeSub = Mathf.FloorToInt(time);
            timeText += timeSub.ToString();
        }


        winStats.text = "Time: " + timeText + "\nKills: " + GameController.totalKills + "\nTokens Collected: " + GameController.tokenCollected + "\nTokens Spent: " + GameController.tokenSpent;
    }

    #region Buttons

    public void GotoMainMenu()
    {
        // Return to main menu without saving, cuz you won

        InterfaceController.intCont.levelLoadController.LoadLevel(InterfaceController.mainMenu);
        Cursor.lockState = CursorLockMode.None;
    }

    #endregion
}
