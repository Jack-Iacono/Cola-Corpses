using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseScreenController :InterfaceScreenController
{
    public List<TMP_Text> statList = new List<TMP_Text>();

    public override void InitializeScreen()
    {
        base.InitializeScreen();
    }

    public override void ShowScreen()
    {
        base.ShowScreen();
    }
    public override void HideScreen()
    {
        base.HideScreen();
    }

    public void NewStats(WeaponController wCont)
    {
        statList[0].text = wCont.rawDamage.ToString();
        statList[1].text = wCont.rawThrowRate.ToString();
        statList[2].text = wCont.rawThrowSpeed.ToString();
        statList[3].text = wCont.rawExplodeRadius.ToString();

        statList[4].text = InventoryController.GetTotalNames(wCont.sodaTraits);
        statList[5].text = wCont.GetFlavorNames() + " Soda";
    }

    #region Button Methods

    public void GotoMainMenu()
    {
        if (SceneManager.GetActiveScene().name == InterfaceController.startingRoom)
        {
            ValueStoreController.SaveGameData(0);
        }

        InterfaceController.intCont.levelLoadController.LoadLevel(InterfaceController.mainMenu);
        UnityEngine.Cursor.lockState = CursorLockMode.None;
    }

    #endregion
}
