using System.Collections.Generic;
using UnityEngine;

public class InterfaceController : MonoBehaviour
{
    public static InterfaceController intCont;

    public const string mainMenu = "Main Menu";
    public const string startingRoom = "Generator Level";
    public const string tutorialLevel = "Tutorial Level";

    [Header("UI/Menu Screens")]
    public List<GameObject> overlays = new List<GameObject>();

    [Header("Transition Screens")]
    public LevelLoadController levelLoadController;

    [Header("Screens")]
    public HUDScreenController hudScreenController;
    public PauseScreenController pauseScreenController;
    public MixScreenController mixScreenController;
    public VendScreenController vendScreenController;
    public SellScreenController sellScreenController;
    public EEScreenController eeScreenController;
    public WinScreenController winScreenController;
    public LoseScreenController loseScreenController;
    public SettingsScreenController settingsScreenController;

    public SeedScreenController seedScreenController;

    // Create a Key Value Pair between the screens and the enum above for easy access by extrernal scripts
    public enum Screen { NULL, HUD, PAUSE, MIX, VEND, SELL, EE, WIN, LOSE, SETTINGS};
    public Dictionary<Screen, InterfaceScreenController> screenPair { get; private set; } = new Dictionary<Screen, InterfaceScreenController>();
    public Screen currentScreen { get; private set; } = Screen.HUD;

    private int currentOverlay = 0;

    [Space(10)]
    public GameObject background;

    #region Initialization
    public void SetStaticInstance()
    {
        //Creates a singelton
        if (intCont != null && intCont != this)
        {
            Destroy(this);
        }
        else
        {
            intCont = this;
        }
    }
    public void Initialize()
    {
        // Populate the key value pairs
        screenPair.Add(Screen.HUD, hudScreenController);
        screenPair.Add(Screen.PAUSE, pauseScreenController);
        screenPair.Add(Screen.MIX, mixScreenController);
        screenPair.Add(Screen.VEND, vendScreenController);
        screenPair.Add(Screen.SELL, sellScreenController);
        screenPair.Add(Screen.EE, eeScreenController);
        screenPair.Add(Screen.WIN, winScreenController);
        screenPair.Add(Screen.LOSE, loseScreenController);
        screenPair.Add(Screen.SETTINGS, settingsScreenController);

        // TEMPORARY !!!!!!
        if(seedScreenController != null)
            seedScreenController.HideScreen();

        GameController.RegisterObserver(this);

        // Loop through each value (the screen) and call it's initialize function
        foreach(InterfaceScreenController s in screenPair.Values)
        {
            s.InitializeScreen();
        }

        background.SetActive(false);

        SetUpCurrentScreen();
        RemoveOverlay();
    }
    #endregion

    #region Menu Methods

    #region Menu Navigation
    public void SetUpScreen(string s)
    {
        // fix later for stations
        currentScreen = Screen.HUD;
        SetUpCurrentScreen();
    }

    public void SetUpScreen(int i)
    {
        currentScreen = (Screen)i;
        SetUpCurrentScreen();
    }
    public void SetUpScreen(Screen s)
    {
        currentScreen = s;
        SetUpCurrentScreen();
    }
    public void SetUpCurrentScreen()
    {
        // Checks to see if there is an open station menu, and if there is, is the menu the one that is currently being displayed, if not close it
        if (InteractableStationController.currentOpenMenu != null && InteractableStationController.currentOpenMenu.openMenu != currentScreen)
            InteractableStationController.currentOpenMenu.CloseMenu();

        RemoveOverlay();

        // Loop through the dictionary
        foreach(KeyValuePair<Screen,InterfaceScreenController> s in screenPair)
        {
            if(s.Key == currentScreen)
            {
                s.Value.ShowScreen();
            }
            else
            {
                s.Value.HideScreen();
            }
        }

        if(currentScreen != Screen.HUD)
        {
            Cursor.lockState = CursorLockMode.None;
            background.SetActive(true);
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            background.SetActive(false);
        }
    }
    public void GamePause(bool b)
    {
        if(!b)
        {
            SetUpScreen(Screen.HUD);
        }
        else
        {
            if (currentScreen == Screen.HUD)
                SetUpScreen(Screen.PAUSE);
        }
    }

    public void RemoveMenus()
    {
        currentOverlay = -1;
        currentScreen = Screen.HUD;

        GameController.PauseGame(false);
    }
    #endregion

    #region Overlay Methods

    public void RemoveOverlay()
    {
        currentOverlay = -1;
        SetUpCurrentOverlay();
    }
    public void SetUpOverlay(string s)
    {
        switch (s)
        {
            case "seed":
                currentOverlay = 0;
                seedScreenController.ShowScreen();
                break;
            default:
                currentOverlay = -1;
                break;
        }

        SetUpCurrentOverlay();
    }
    public void SetUpCurrentOverlay()
    {
        for (int i = 0; i < overlays.Count; i++)
        {
            if (i == currentOverlay)
            {
                overlays[i].SetActive(true);
            }
            else
            {
                overlays[i].SetActive(false);
            }
        }
    }

    #endregion

    #region Toggle Methods

    public void TogglePauseMenu()
    {
        //Checks whether the game is paused or not
        if (GameController.gamePaused)
        {
            SetUpScreen(Screen.PAUSE);
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            SetUpScreen(Screen.HUD);
            Cursor.lockState = CursorLockMode.Locked;
            RemoveOverlay();
        }
    }
    public void TogglePauseMenu(bool b)
    {
        GameController.gamePaused = b;

        TogglePauseMenu();
    }

    #endregion

    #endregion
}