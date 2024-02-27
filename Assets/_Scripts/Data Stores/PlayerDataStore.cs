using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public static class PlayerDataStore
{
    public static string fileName { get; } = "-PlayerData";
    public static string fileType { get; } = ".txt";

    public static void SaveData(PlayerController playerData, int fileNumber)
    {
        //Opens writer connection to the document
        StreamWriter writer = new StreamWriter(Application.persistentDataPath + fileName + fileNumber + fileType);

        //Converts the data to a JSON file format and writes the data
        writer.WriteLine(JsonUtility.ToJson(playerData));

        //Closes the connection with the writer
        writer.Close();
    }
    public static PlayerController LoadData(int fileNumber)
    {
        //Opens the stream reader
        StreamReader reader = new StreamReader(Application.persistentDataPath + fileName + fileNumber + fileType);

        //Reads the data
        PlayerController playerCont = JsonUtility.FromJson<PlayerController>(reader.ReadLine());

        //Closes the reader
        reader.Close();

        return playerCont;
    }
    public static bool FileExists(int fileNumber)
    {
        return File.Exists(Application.persistentDataPath + fileName + fileNumber + fileType);
    }
    public static void DeleteData(int fileNumber)
    {
        File.Delete(Application.persistentDataPath + fileName + fileNumber + fileType);
    }

}
