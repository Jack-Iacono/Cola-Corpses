using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsScreenController : InterfaceScreenController
{

    #region Preferences

    [Header("FullScreen Variables")]
    public MenuButtonController fullScreenButton;
    private bool fullscreen = false;

    [Header("Sensitivity Variables")]
    public Slider sensSlider;
    public TMP_Text sensText;
    private float sensMax = 200;
    private float sensMin = 50;

    #endregion

    #region Controls

    private GameObject currentControl;
    private string currentKey;

    private TMP_Text currentText;

    private bool searchKey = false;
    private bool checkKeyRelease = false;

    [Header("Control Buttons")]
    public GameObject buttonForward;
    public GameObject buttonBackward;
    public GameObject buttonRight;
    public GameObject buttonLeft;
    public GameObject buttonJump;
    public GameObject buttonInteract;
    public GameObject buttonPause;
    public GameObject buttonFire;
    public GameObject buttonDrink;

    #endregion

    private void Awake()
    {
        // This will run the fullscreen stuff now
        Screen.fullScreen = fullscreen;
    }

    public override void InitializeScreen()
    {
        PreferenceData prefData = ProfileSaveController.LoadPreferenceData(ValueStoreController.fileOwner);

        sensText.text = prefData.sensitivity.ToString();
        sensSlider.value = (prefData.sensitivity - sensMin) / (sensMax - sensMin);
        SetFullScreenButton(prefData.fullScreen);

        UpdateKeys();
        ValueStoreController.SaveProfileData();
    }

    public override void ShowScreen()
    {
        base.ShowScreen();

        PreferenceData prefData = ProfileSaveController.LoadPreferenceData(ValueStoreController.fileOwner);

        sensText.text = prefData.sensitivity.ToString();
        sensSlider.value = (prefData.sensitivity - sensMin) / (sensMax - sensMin);
        SetFullScreenButton(prefData.fullScreen);

        UpdateKeys();
        ValueStoreController.SaveProfileData();
    }
    public override void HideScreen()
    {
        base.HideScreen();
    }

    private void Update()
    {
        //If the search for a key is needed then proceed to get key related inputs
        if (searchKey)
        {
            KeyCode key = GetKeyBind();

            if (key != KeyCode.None)
            {
                //Changes duplicates to null
                ValueStoreController.keyData = ValueStoreController.keyData.CheckDuplicateKeys(key);

                //Sets the current checked key to the key
                ValueStoreController.keyData.SetKeybindString(currentKey, key);
                ValueStoreController.SaveProfileData();

                currentControl.GetComponentInChildren<TMP_Text>().text = key.ToString();
                searchKey = false;

                checkKeyRelease = true;

                UpdateKeys();
            }
        }

        //This ensures that the button is only active after the key is released. Mainly for mouse button
        if (checkKeyRelease)
        {
            KeyCode key = GetKeyBind();

            if (key == KeyCode.None)
            {
                currentControl.GetComponent<MenuButtonController>().enabled = true;
                checkKeyRelease = false;
            }
        }
    }


    private void SetFullScreenButton(bool b)
    {
        if (b)
            fullScreenButton.SetSpriteSetIndex(1);
        else
            fullScreenButton.SetSpriteSetIndex(0);

        Screen.fullScreen = fullscreen;

        ValueStoreController.prefData.fullScreen = fullscreen;
        ValueStoreController.SaveProfileData();
    }

    #region Preferences
    public void UpdateSensSlider()
    {
        sensText.text = Mathf.FloorToInt((sensMax - sensMin) * sensSlider.value + sensMin).ToString();
    }
    public void SaveSensValue()
    {
        ValueStoreController.prefData.sensitivity = Mathf.FloorToInt((sensMax - sensMin) * sensSlider.value + sensMin);

        ValueStoreController.SaveProfileData();
    }

    public void ToggleFullScreen()
    {
        fullscreen = !fullscreen;

        SetFullScreenButton(fullscreen);
    }
    #endregion

    #region Keybind Things
    public void UpdateKeys()
    {
        KeybindData keyData = ProfileSaveController.LoadKeyData(ValueStoreController.fileOwner);

        buttonForward.GetComponentInChildren<TMP_Text>().text = KeybindData.KeyCodeToString(keyData.keyForward);
        buttonBackward.GetComponentInChildren<TMP_Text>().text = KeybindData.KeyCodeToString(keyData.keyBackward);
        buttonRight.GetComponentInChildren<TMP_Text>().text = KeybindData.KeyCodeToString(keyData.keyRight);
        buttonLeft.GetComponentInChildren<TMP_Text>().text = KeybindData.KeyCodeToString(keyData.keyLeft);
        buttonJump.GetComponentInChildren<TMP_Text>().text = KeybindData.KeyCodeToString(keyData.keyJump);
        buttonInteract.GetComponentInChildren<TMP_Text>().text = KeybindData.KeyCodeToString(keyData.keyInteract);
        buttonPause.GetComponentInChildren<TMP_Text>().text = KeybindData.KeyCodeToString(keyData.keyPause);
        buttonFire.GetComponentInChildren<TMP_Text>().text = KeybindData.KeyCodeToString(keyData.keyFire);
        buttonDrink.GetComponentInChildren<TMP_Text>().text = KeybindData.KeyCodeToString(keyData.keyDrink);
    }

    #region KeyBind Retrieval

    //Gets the input which should be changed next
    public void GetNewControl(string s)
    {
        currentKey = s;

        switch (s)
        {
            case "forward":
                currentControl = buttonForward;
                break;
            case "backward":
                currentControl = buttonBackward;
                break;
            case "right":
                currentControl = buttonRight;
                break;
            case "left":
                currentControl = buttonLeft;
                break;
            case "jump":
                currentControl = buttonJump;
                break;
            case "interact":
                currentControl = buttonInteract;
                break;
            case "pause":
                currentControl = buttonPause;
                break;
            case "fire":
                currentControl = buttonFire;
                break;
            case "drink":
                currentControl = buttonDrink;
                break;
        }

        currentControl.GetComponentInChildren<TMP_Text>().text = "";
        currentControl.GetComponent<MenuButtonController>().enabled = false;

        searchKey = true;
    }

    //Gets the KeyCode from the key input
    public KeyCode GetKeyBind()
    {
        foreach (KeyCode key in KeybindData.keyCodes)
        {
            if (Input.GetKey(key))
            {
                return key;
            }
        }

        return KeyCode.None;
    }

    #endregion

    #endregion
}
