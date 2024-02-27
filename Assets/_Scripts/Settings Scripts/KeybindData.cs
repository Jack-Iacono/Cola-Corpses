using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class KeybindData
{
    public KeyCode keyForward;
    public KeyCode keyBackward;
    public KeyCode keyLeft;
    public KeyCode keyRight;
    public KeyCode keyJump;

    public KeyCode keyInteract;
    public KeyCode keyPause;

    public KeyCode keyFire;
    public KeyCode keyDrink;

    //A list of all keybinds which can be used in the game
    [NonSerialized]
    public static KeyCode[] keyCodes = new KeyCode[]
    {
        KeyCode.A, KeyCode.B, KeyCode.C, KeyCode.D, KeyCode.E, KeyCode.F, KeyCode.G, KeyCode.H, KeyCode.I, KeyCode.J, KeyCode.K, KeyCode.L, KeyCode.M, KeyCode.N, KeyCode.O, KeyCode.P, KeyCode.Q, KeyCode.R, KeyCode.S, KeyCode.T, KeyCode.U, KeyCode.V, KeyCode.W, KeyCode.X, KeyCode.Y, KeyCode.Z,
        KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.UpArrow, KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.Space, KeyCode.Backspace,
        KeyCode.Mouse0, KeyCode.Mouse1, KeyCode.Mouse2, KeyCode.Mouse3, KeyCode. Mouse4, KeyCode.Mouse5, KeyCode.Mouse6,
        KeyCode.Comma, KeyCode.Period, KeyCode.Backslash, KeyCode.Slash, KeyCode.Semicolon, KeyCode.Quote, KeyCode.LeftBracket, KeyCode.RightBracket, KeyCode.Tilde, KeyCode.Plus, KeyCode.Minus,
        KeyCode.Alpha0, KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9,
        KeyCode.Escape, KeyCode.Tab, KeyCode.BackQuote
    };

    public KeybindData()
    {
        keyForward = KeyCode.W;
        keyBackward = KeyCode.S;
        keyRight = KeyCode.D;
        keyLeft = KeyCode.A;
        keyJump = KeyCode.Space;
        keyInteract = KeyCode.E;
        keyPause = KeyCode.Escape;
        keyFire = KeyCode.Mouse0;
        keyDrink = KeyCode.Mouse1;
    }

    public static KeyCode StringToKeyCode(string s)
    {
        foreach (KeyCode key in keyCodes)
        {
            if (s == key.ToString())
            {
                return key;
            }
        }

        return KeyCode.None;
    }
    public static string KeyCodeToString(KeyCode key)
    {
        //This handles the various keycodes with bad names
        switch(key)
        {
            case KeyCode.Mouse0:
                return "L-Click";
            case KeyCode.Mouse1:
                return "R-Click";
            case KeyCode.Backslash:
                return "\\";
            case KeyCode.Slash:
                return "/";
            default:
                return key.ToString();
        }
    }

    public KeybindData CheckDuplicateKeys(KeyCode key)
    {
        List<KeyCode> keyList = GetKeyList();

        for (int i = 0; i < keyList.Count; i++)
        {
            if (key == keyList[i])
            {
                keyList[i] = KeyCode.None;
            }
        }

        SetKeys(keyList);

        return this;
    }

    public List<KeyCode> GetKeyList()
    {
        //Returns all keys contained in a list

        return new List<KeyCode>
            {
                keyForward,
                keyBackward,
                keyLeft,
                keyRight,
                keyJump,
                keyInteract,
                keyPause,
                keyFire,
                keyDrink
            };
    }
    public void SetKeys(List<KeyCode> keyList)
    {
        keyForward = keyList[0];
        keyBackward = keyList[1];
        keyLeft = keyList[2];
        keyRight = keyList[3];
        keyJump = keyList[4];
        keyInteract = keyList[5];
        keyPause = keyList[6];
        keyFire= keyList[7];
        keyDrink = keyList[8];
    }

    public void SetKeybindString(string s, KeyCode key)
    {

        switch (s)
        {
            case "forward":
                keyForward = key;
                break;
            case "backward":
                keyBackward = key;
                break;
            case "right":
                keyRight = key;
                break;
            case "left":
                keyLeft = key;
                break;
            case "jump":
                keyJump = key;
                break;
            case "interact":
                keyInteract = key;
                break;
            case "pause":
                keyPause = key;
                break;
            case "fire":
                keyFire = key;
                break;
            case "drink":
                keyDrink = key;
                break;
        }

    }
}
