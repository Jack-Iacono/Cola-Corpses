using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class ProfileSaveController
{
    /*This script will save all the data for each profile including:
     * Keybinds
     * Totals
     * Achievements
    */

    private static string fileName = "/ProfileSave";
    private static string fileType = ".txt";

    private static string keyDataStoreName = "Keybind Data Store";
    private static string prefDataStoreName = "Preference Data Store";
    private static string totalDataStoreName = "Total Data Store";
    private static string sectionEndString = "********************************";

    public static void SaveData
        (
            PreferenceData prefData,
            KeybindData keyData,
            TotalData totalData,
            string fileOwner
        )
    {
        //Opens writer connection to the document
        StreamWriter writer = new StreamWriter(Application.persistentDataPath + fileName + fileOwner + fileType);

        #region KeyBinds

        writer.WriteLine(keyDataStoreName);

        foreach (KeyCode key in keyData.GetKeyList())
        {
            writer.WriteLine(key.ToString());
        }

        #endregion

        writer.WriteLine(sectionEndString);

        #region Preferences

        writer.WriteLine(prefDataStoreName);

        writer.WriteLine(JsonUtility.ToJson(prefData));

        #endregion

        writer.WriteLine(sectionEndString);

        #region Total

        writer.WriteLine(totalDataStoreName);

        writer.WriteLine(JsonUtility.ToJson(totalData));

        #endregion

        writer.Close();
    }

    public static KeybindData LoadKeyData(string fileOwner)
    {
        if (File.Exists(Application.persistentDataPath + fileName + fileOwner + fileType))
        {
            StreamReader reader = new StreamReader(Application.persistentDataPath + fileName + fileOwner + fileType);

            //Moves the reader to just before the readable area
            bool sectionFound = false;

            while (!sectionFound)
            {
                if (reader.ReadLine() == keyDataStoreName)
                {
                    sectionFound = true;
                }
            }

            bool sectionEnd = false;
            KeybindData keyData = new KeybindData();
            List<KeyCode> keyList = new List<KeyCode>();

            while (!sectionEnd)
            {

                string currentKey = reader.ReadLine();

                if (currentKey != sectionEndString)
                {
                    keyList.Add(KeybindData.StringToKeyCode(currentKey));
                }
                else
                    sectionEnd = true;
            }

            keyData.SetKeys(keyList);

            reader.Close();

            return keyData;
        }
        else
        {
            Debug.Log("No file found: " + Application.persistentDataPath + fileName + fileOwner + fileType);

            //ValueStoreController.SaveProfileData();

            return new KeybindData();
        }
    }
    public static PreferenceData LoadPreferenceData(string fileOwner)
    {
        if (File.Exists(Application.persistentDataPath + fileName + fileOwner + fileType))
        {
            StreamReader reader = new StreamReader(Application.persistentDataPath + fileName + fileOwner + fileType);

            //Moves the reader to just before the readable area
            bool sectionFound = false;

            while (!sectionFound)
            {
                if (reader.ReadLine() == prefDataStoreName)
                {
                    sectionFound = true;
                }
            }

            PreferenceData prefData = JsonUtility.FromJson<PreferenceData>(reader.ReadLine());

            reader.Close();

            return prefData;
        }
        else
        {
            Debug.Log("No file found: " + Application.persistentDataPath + fileName + fileOwner + fileType);

            //ValueStoreController.SaveProfileData();

            return new PreferenceData();
        }
    }
    public static TotalData LoadTotalData(string fileOwner)
    {
        if (File.Exists(Application.persistentDataPath + fileName + fileOwner + fileType))
        {
            StreamReader reader = new StreamReader(Application.persistentDataPath + fileName + fileOwner + fileType);

            //Moves the reader to just before the readable area
            bool sectionFound = false;

            while (!sectionFound)
            {
                if (reader.ReadLine() == totalDataStoreName)
                {
                    sectionFound = true;
                }
            }

            TotalData totalData = JsonUtility.FromJson<TotalData>(reader.ReadLine());

            reader.Close();

            return totalData;
        }
        else
        {
            Debug.Log("No file found: " + Application.persistentDataPath + fileName + fileOwner + fileType);

            //ValueStoreController.SaveProfileData();

            return new TotalData();
        }
    }

    #region File Access Methods

    public static List<string> GetProfileNames()
    {
        //Gets all files in the folder
        string[] fileNames = Directory.GetFiles(Application.persistentDataPath);
        List<string> nameList = new List<string>();

        //Takes away unnecessary files
        for(int i = 0; i < fileNames.Length; i++)
        {
            //Checks if the file is a profile save file
            if (fileNames[i].Contains(fileName.Replace("/", "\\")))
            {
                string name = fileNames[i];

                //Removes everything except for the name from the string

                //Application.persistentDataPath sucks, why is it like this. the slash is fkn backwards
                name = name.Replace(Application.persistentDataPath, "");
                name = name.Replace(fileName.Replace("/", "\\"), "");
                name = name.Replace(fileType, "");

                //Adds the name to the name list
                nameList.Add(name);
            }
        }

        return nameList;
    }
    public static void DeleteProfile(string fileOwner)
    {
        if (File.Exists(Application.persistentDataPath + fileName + fileOwner + fileType))
        {
            File.Delete(Application.persistentDataPath + fileName + fileOwner + fileType);

            SaveDataStore.DeleteData(0);
            SaveDataStore.DeleteData(1);
        }
    }

    #endregion
}
