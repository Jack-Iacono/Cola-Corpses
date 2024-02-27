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

public static class RecordDataStore
{

    //The Deafult Name of the file and the type of file to save it as
    public static string fileName { get; } = "-RecordData";
    public static string fileType { get; } = ".txt";

    private static char[] timeCharTrim = { 'T', 'i', 'm', 'e', ':', ' ' };
    private static char[] killCharTrim = { 'K', 'i', 'l', ':', ' ' };
    private static char[] pointCharTrim = { 'P', 'o', 'i', 'n', 't', 's', ':', ' '};
    private static char[] dateCharTrim = { 'D', 'a', 't', 'e', ':', ' ' };
    public static void SaveData(List<int> recordList, int fileNumber)
    {
        //Opens writer connection to the document
        StreamWriter writer = new StreamWriter(Application.persistentDataPath + fileName + fileNumber + fileType);

        writer.WriteLine("Time: " + recordList[0]);
        writer.WriteLine("Kill: " + recordList[1]);
        writer.WriteLine("Point: " + recordList[2]);
        writer.WriteLine("Date: " + DateTime.Now.Month + "/" + DateTime.Now.Day + "/" + DateTime.Now.Year);

        //Closes the connection with the writer
        writer.Close();
    }
    public static List<int> LoadData(int fileNumber, out string dateData)
    {
        if (File.Exists(Application.persistentDataPath + fileName + fileNumber + fileType))
        {
            List<int> data = new List<int>();
            StreamReader reader = new StreamReader(Application.persistentDataPath + fileName + fileNumber + fileType);

            string timeData = reader.ReadLine().Trim(timeCharTrim);
            data.Add(int.Parse(timeData));

            string killData = reader.ReadLine().Trim(killCharTrim);
            data.Add(int.Parse(killData));

            string pointData = reader.ReadLine().Trim(pointCharTrim);
            data.Add(int.Parse(pointData));

            dateData = reader.ReadLine().Trim(dateCharTrim);

            reader.Close();

            return data;
        }
        else
        {
            Debug.Log("No Data Saved, Please Save Data");
            dateData = null;
            return null;
        }
    }

}
