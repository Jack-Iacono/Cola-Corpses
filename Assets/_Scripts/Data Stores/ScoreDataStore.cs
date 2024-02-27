/*
 * This file save sthe data related to:
 *  - The Player position
 *  - The Player health
 *  - The Player Ammo Count for each ammo type
 *  - The Total Time Played
 *  - The Total Kills
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System;

public static class ScoreDataStore
{

    //The Deafult Name of the file and the type of file to save it as
    public static string fileName { get; } = "-ScoreData";
    public static string fileType { get; } = ".txt";
    public static void SaveData(List<int> recordList, int fileNumber)
    {
        //Opens writer connection to the document
        StreamWriter writer = new StreamWriter(Application.persistentDataPath + fileName + fileNumber + fileType);

        writer.WriteLine(DateTime.Now.Month + "/" + DateTime.Now.Day + "/" + DateTime.Now.Year);
        writer.WriteLine(recordList[0]);
        writer.WriteLine(recordList[1]);
        writer.WriteLine(recordList[2]);
        writer.WriteLine(recordList[3]);

        //Closes the connection with the writer
        writer.Close();
    }
    public static List<string> LoadData(int fileNumber)
    {
        if (File.Exists(Application.persistentDataPath + fileName + fileNumber + fileType))
        {
            List<string> data = new List<string>();
            StreamReader reader = new StreamReader(Application.persistentDataPath + fileName + fileNumber + fileType);

            data.Add(reader.ReadLine());
            data.Add(reader.ReadLine());
            data.Add(reader.ReadLine());
            data.Add(reader.ReadLine());
            data.Add(reader.ReadLine());

            reader.Close();

            return data;
        }
        else
        {
            Debug.Log("No Data Saved, Please Save Data");
            return null;
        }
    }
    public static void DeleteData(int fileNumber)
    {
        File.Delete(Application.persistentDataPath + fileName + fileNumber + fileType);
    }
    public static bool FileExists(int fileNumber)
    {
        return File.Exists(Application.persistentDataPath + fileName + fileNumber + fileType);
    }

}
