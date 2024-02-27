using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class ButtonController : MonoBehaviour
{
    private string mainMenu = "Main Menu";
    private string startingRoom = "Generator Level";
    private string tutorialLevel = "Tutorial Level";

    public LevelLoadController loadCont;
    public void GotoMainMenu()
    {
        if(SceneManager.GetActiveScene().name == startingRoom)
        {
            ValueStoreController.SaveGameData(0);
        }

        loadCont.LoadLevel(mainMenu);
        UnityEngine.Cursor.lockState = CursorLockMode.None;
    }

    public void SaveData()
    {
        ValueStoreController.SaveGameData(0);
    }
    public void GoToMapGenLevelRandom()
    {
        ValueStoreController.SetLoadData(false);

        SaveDataStore.DeleteData(0);

        loadCont.LoadLevel(startingRoom);
    }
    public void GoToMapGenLevelLoaded()
    {
        ValueStoreController.LoadGameData(0);

        loadCont.LoadLevel(startingRoom);
    }
    public void GoToMapGenLevelReset()
    {
        ValueStoreController.LoadGameData(1);

        SaveDataStore.DeleteData(0);

        loadCont.LoadLevel(startingRoom);
    }
    public void GoToMapGenLevelSeed()
    {
        ValueStoreController.LoadGameData(2, LoadScreenController.selectedMap);

        SaveDataStore.DeleteData(0);

        loadCont.LoadLevel(startingRoom);
    }
    public void GoToTutorial()
    {
        // Stops the pause menu from showing up automatically
        ValueStoreController.loadData = false;

        loadCont.LoadLevel(tutorialLevel);
    }
    public void QuitGame()
    {
        Application.Quit();
    }

}
