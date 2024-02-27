using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class LoseScreenController : InterfaceScreenController
{
    public TMP_Text loseText;

    public override void InitializeScreen()
    {
        base.InitializeScreen();
    }

    public override void ShowScreen()
    {
        base.ShowScreen();

        NewLose();
    }
    public override void HideScreen()
    {
        base.HideScreen();
    }

    public void NewLose()
    {
        int rand = UnityEngine.Random.Range(0, 5);

        switch (rand)
        {

            case 0:
                loseText.text = "Better luck next time";
                break;
            case 1:
                loseText.text = "Might want to try living longer. I ain't your mom though, so you don't really have to listen.";
                break;
            case 2:
                loseText.text = "I'm not sure that soda combination was very good.";
                break;
            case 3:
                loseText.text = "I had to write like 5 of these and I couldn't do it. I have nothing interesting to say.";
                break;
            case 4:
                loseText.text = "That funny pilk cat would love this game.";
                break;

        }
    }

    #region Buttons

    public void GotoMainMenu()
    {
        // Return to main menu without saving, cuz you lose

        InterfaceController.intCont.levelLoadController.LoadLevel(InterfaceController.mainMenu);
        Cursor.lockState = CursorLockMode.None;
    }
    public void GoToMapGenLevelReset()
    {
        ValueStoreController.LoadGameData(1);
        SaveDataStore.DeleteData(0);

        InterfaceController.intCont.levelLoadController.LoadLevel(InterfaceController.startingRoom);
    }

    #endregion
}
