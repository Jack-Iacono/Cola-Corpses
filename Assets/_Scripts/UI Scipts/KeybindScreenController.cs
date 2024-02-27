using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class KeybindScreenController : MonoBehaviour
{
    private int currentControl = -1;
    private TMP_Text currentText;

    private bool searchKey = false;
    private bool checkKeyRelease = false;

    List<KeyCode> keyList = new List<KeyCode>();

    #region Button Text Objects

    public List<GameObject> buttonObjects;

    #endregion

    //A list of all keybinds which can be used in the game
    KeyCode[] keyCodes = new KeyCode[]
    {
        KeyCode.A, KeyCode.B, KeyCode.C, KeyCode.D, KeyCode.E, KeyCode.F, KeyCode.G, KeyCode.H, KeyCode.I, KeyCode.J, KeyCode.K, KeyCode.L, KeyCode.M, KeyCode.N, KeyCode.O, KeyCode.P, KeyCode.Q, KeyCode.R, KeyCode.S, KeyCode.T, KeyCode.U, KeyCode.V, KeyCode.W, KeyCode.X, KeyCode.Y, KeyCode.Z,
        KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.UpArrow, KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.Space,
        KeyCode.Mouse0, KeyCode.Mouse1, KeyCode.Mouse2, KeyCode.Mouse3, KeyCode. Mouse4, KeyCode.Mouse5, KeyCode.Mouse6,
        KeyCode.Comma, KeyCode.Period, KeyCode.Backslash, KeyCode.Slash, KeyCode.Semicolon, KeyCode.Quote, KeyCode.LeftBracket, KeyCode.RightBracket, KeyCode.Tilde, KeyCode.Plus, KeyCode.Minus,
        KeyCode.Alpha0, KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9,
        KeyCode.Escape, KeyCode.Tab, KeyCode.BackQuote
    };

    private void Start()
    {
        keyList = KeyBindController.LoadAllKeyBinds();

        for(int i = 0; i < keyList.Count; i++)
        {
            buttonObjects[i].GetComponentInChildren<TMP_Text>().text = keyList[i].ToString();
        }
    }

    private void Update()
    {
        //If the search for a key is needed then proceed to get key related inputs
        if (searchKey)
        {
            KeyCode key = GetKeyBind();

            if(key != KeyCode.None)
            {
                //Changes duplicates to null
                keyList = KeyBindController.CheckDuplicateKeys(key);

                //Sets the current checked key to the key
                keyList[currentControl] = key;

                //Saves the key binds
                KeyBindController.SaveKeyBinds(keyList);
                buttonObjects[currentControl].GetComponentInChildren<TMP_Text>().text = key.ToString();
                searchKey = false;

                checkKeyRelease = true;

                RefreshButtons();
            }
        }

        //This ensures that the button is only active after the key is released. Mainly for mouse button
        if (checkKeyRelease)
        {
            KeyCode key = GetKeyBind();

            if(key == KeyCode.None)
            {
                buttonObjects[currentControl].GetComponent<Button>().enabled = true;
                checkKeyRelease = false;
            }
        }
    }

    #region KeyBind Retrieval

    //Gets the input which should be changed next
    public void GetNewControl(string s)
    {

        switch (s)
        {

            case "forward":
                currentControl = 0;
                break;
            case "backward":
                currentControl = 1;
                break;
            case "right":
                currentControl = 2;
                break;
            case "left":
                currentControl = 3;
                break;
            case "jump":
                currentControl = 4;
                break;
            case "fire":
                currentControl = 5;
                break;
            case "interact":
                currentControl = 6;
                break;
            case "pause":
                currentControl = 7;
                break;
        }

        buttonObjects[currentControl].GetComponentInChildren<TMP_Text>().text = "";
        buttonObjects[currentControl].GetComponent<Button>().enabled = false;
        searchKey = true;
    }

    //Gets the KeyCode from the key input
    public KeyCode GetKeyBind()
    {
        foreach(KeyCode key in keyCodes)
        {
            if (Input.GetKey(key))
            {
                return key;
            }
        }

        return KeyCode.None;
    }

    #endregion

    #region Button Methods

    public void RefreshButtons()
    {
        for (int i = 0; i < keyList.Count; i++)
        {
            buttonObjects[i].GetComponentInChildren<TMP_Text>().text = keyList[i].ToString();
        }
    }

    #endregion

}
