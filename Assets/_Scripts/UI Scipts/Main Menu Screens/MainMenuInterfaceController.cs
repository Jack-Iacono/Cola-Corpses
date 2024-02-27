using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuInterfaceController : MonoBehaviour
{
    public enum Screen { MAIN, STAT, SETTINGS, LOAD };

    public MainScreenController mainScreen;
    public LoadScreenController loadScreen;
    public SettingsScreenController settingsScreen;
    public StatScreenController statScreen;

    private Screen currentScreen = Screen.MAIN;
    private Screen previousScreen = Screen.MAIN;

    public Dictionary<Screen, InterfaceScreenController> screenPair { get; private set; } = new Dictionary<Screen, InterfaceScreenController>();

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;

        screenPair.Add(Screen.MAIN, mainScreen);
        screenPair.Add(Screen.LOAD, loadScreen);
        screenPair.Add(Screen.STAT, statScreen);
        screenPair.Add(Screen.SETTINGS, settingsScreen);

        SetUpScreen(Screen.MAIN);
    }

    [Tooltip("0: Main\n1: Stat\n2: Settings\n3: Load")]
    public void SetUpScreen(int i)
    {
        previousScreen = currentScreen;
        currentScreen = (Screen)i;
        SetUpCurrentScreen();
    }
    public void SetUpScreen(Screen s)
    {
        previousScreen = currentScreen;
        currentScreen = s;
        SetUpCurrentScreen();
    }
    public void SetUpCurrentScreen()
    {
        // Loop through the dictionary
        foreach (KeyValuePair<Screen, InterfaceScreenController> s in screenPair)
        {
            if (s.Key != currentScreen && s.Key != previousScreen)
            {
                s.Value.HideScreen();
            }
        }

        StartCoroutine(ScreenAnim());
    }
    IEnumerator ScreenAnim()
    {
        StopAllCoroutines();

        screenPair[previousScreen].GetComponent<Animator>().SetTrigger("Leave");

        screenPair[currentScreen].ShowScreen();
        screenPair[currentScreen].SendMessage("InitializeScreen", SendMessageOptions.DontRequireReceiver);
        screenPair[currentScreen].GetComponent<Animator>().SetTrigger("Enter");

        //Waits for the set seconds before moving to the next scene
        yield return new WaitForSeconds(1);

        screenPair[previousScreen].HideScreen();
    }
}
