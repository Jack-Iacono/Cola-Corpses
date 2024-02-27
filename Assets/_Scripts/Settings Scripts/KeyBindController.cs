using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;


public static class KeyBindController
{

    private static string fileName = "-KeyBindData";
    private static string fileType = ".txt";

    #region Keybinds

    //Player Controls
    public static List<KeyCode> keyBindList { get; set; } = new List<KeyCode>()
    {
        KeyCode.W, //Player Forward
        KeyCode.S, //Player Backward
        KeyCode.A, //Player Left
        KeyCode.D, //Player Right
        KeyCode.Space, //Player Jump

        KeyCode.Mouse0, //Gun Fire

        KeyCode.E, //Interact
        KeyCode.Escape //Pause
    };

    #endregion

    #region Key Bind Saving and Loading

    public static void SaveKeyBinds(List<KeyCode> list)
    {
        //Opens writer connection to the document
        StreamWriter writer = new StreamWriter(Application.persistentDataPath + fileName + fileType);

        //writes in all of the keycodes
        foreach(KeyCode key in list)
        {
            writer.WriteLine(KeybindData.KeyCodeToString(key));
        }

        //Closes the connection with the writer
        writer.Close();
    }
    public static List<KeyCode> LoadAllKeyBinds()
    {
        List<KeyCode> finalList = new List<KeyCode>();

        //Checks if the file exists and if it does not, makes the file
        if (File.Exists(Application.persistentDataPath + fileName + fileType))
        {
            //Opens reader connection to the document
            StreamReader reader = new StreamReader(Application.persistentDataPath + fileName + fileType);

            //Creates a check which will trip when document is done
            bool docFinish = false;

            //Creates an array to place the results in
            List<string> keyCodeList = new List<string>();

            while (!docFinish)
            {
                //Attempts to get next line of data
                string keyCodeStrings = reader.ReadLine();

                if (keyCodeStrings != null)
                {
                    //Writes data into the list
                    keyCodeList.Add(keyCodeStrings);
                }
                else
                {
                    //Ends the reading process
                    docFinish = true;
                }
            }

            //Closes the connection with the reader
            reader.Close();

            //Converts the list to a keycode list
            for (int i = 0; i < keyCodeList.Count; i++)
            {
                finalList.Add(KeybindData.StringToKeyCode(keyCodeList[i]));
            }
        }
        else
        {
            SaveKeyBinds(keyBindList);
            return keyBindList;
        }

        return finalList;
    }
    public static KeyCode LoadKeyBind(string inputName)
    {
        List<KeyCode> finalList = new List<KeyCode>();

        //Checks if the file exists and if it does not, makes the file
        if (File.Exists(Application.persistentDataPath + fileName + fileType))
        {
            //Opens reader connection to the document
            StreamReader reader = new StreamReader(Application.persistentDataPath + fileName + fileType);

            //Creates a check which will trip when document is done
            bool docFinish = false;

            //Creates an array to place the results in
            List<string> keyCodeList = new List<string>();

            while (!docFinish)
            {
                //Attempts to get next line of data
                string keyCodeStrings = reader.ReadLine();

                if (keyCodeStrings != null)
                {
                    //Writes data into the list
                    keyCodeList.Add(keyCodeStrings);
                }
                else
                {
                    //Ends the reading process
                    docFinish = true;
                }
            }

            //Closes the connection with the reader
            reader.Close();

            //Converts the list to a keycode list
            for (int i = 0; i < keyCodeList.Count; i++)
            {
                finalList.Add(KeybindData.StringToKeyCode(keyCodeList[i]));
            }
        }
        else
        {
            SaveKeyBinds(keyBindList);
        }

        inputName.ToLower();

        switch (inputName)
        {
            case "forward":
                return finalList[0];
            case "backward":
                return finalList[1];
            case "left":
                return finalList[3];
            case "right":
                return finalList[2];
            case "jump":
                return finalList[4];
            case "fire":
                return finalList[5];
            case "interact":
                return finalList[6];
            case "pause":
                return finalList[7];
            default:
                return KeyCode.None;
        }
    }

    #endregion

    #region Key Checking Methods

    public static List<KeyCode> CheckDuplicateKeys(KeyCode key)
    {
        List<KeyCode> keyList = LoadAllKeyBinds();

        for(int i = 0; i < keyList.Count; i++)
        {
            if(key == keyList[i])
            {
                keyList[i] = KeyCode.None;
            }
        }

        SaveKeyBinds(keyList);
        return keyList;
    }

    #endregion

}
