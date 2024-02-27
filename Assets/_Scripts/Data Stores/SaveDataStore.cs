using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaveDataStore
{
    public static string saveFileName { get; } = "/SaveData";
    public static string resetFileName { get; } = "/ResetData";
    public static string seedFileName { get; } = "/SeedData";
    public static string fileType { get; } = ".txt";

    #region Section Names
    public static string gameStoreName { get; } = "Game Data Store";
    public static string weaponStoreName { get; } = "Weapon Data Store";
    public static string partStoreName { get; } = "Part Data Store";
    public static string mapStoreName { get; } = "Map Data Store";
    public static string playerStoreName { get; } = "Player Data Store";
    public static string enemyStoreName { get; } = "Enemy Data Store";
    public static string vendStoreName { get; } = "Vend Data Store";
    public static string invStoreName { get; } = "Inventory Data Store";

    public static string sectionBreakString = "This is a section break";
    #endregion

    #region Map Data

    private static string tileName = "Tile:";
    private static string roomName = "Room:";
    private static string mapName = "Map:";
    private static string spawnerName = "Spawner:";

    #endregion

    //One method passes through an empty addname and the other allows for input
    public static void SaveData(GameSaveData gameSaveData, int saveType)
    {
        SaveData(gameSaveData, saveType, "");
    }
    public static void SaveData(GameSaveData gameSaveData, int saveType, string newAddName)
    {
        switch (saveType)
        {
            case 0:
                SaveData(gameSaveData, saveFileName + newAddName);
                break;
            case 1:
                SaveData(gameSaveData, resetFileName + newAddName);
                break;
            case 2:
                SaveData(gameSaveData, seedFileName + newAddName);
                break;
        }
    }
    public static GameSaveData LoadData(int saveType)
    {
        return LoadData(saveType, "");
    }
    public static GameSaveData LoadData(int saveType, string newAddName)
    {
        switch (saveType)
        {
            case 0:
                return LoadData(saveFileName + newAddName);
            case 1:
                return LoadData(resetFileName + newAddName);
            case 2:
                return LoadData(seedFileName + newAddName);
            default:
                return null;
        }
    }
    

    public static void SaveData(GameSaveData gameSaveData, string fileName)
    {
        //Opens writer connection to the document
        StreamWriter writer = new StreamWriter(Application.persistentDataPath + fileName + fileType);

        #region Game Data

        GameData gameData = gameSaveData.gameData;

        writer.WriteLine(gameStoreName);

        writer.WriteLine(JsonUtility.ToJson(gameData));

        string gameDataString = "";
        for(int i = 0; i < gameData.checkList.Count; i++)
        {
            if (gameDataString != "")
                gameDataString += "|";

            if (gameData.checkList[i].Count != 0)
            {
                for (int j = 0; j < gameData.checkList[i].Count; j++)
                {
                    if (j != 0)
                        gameDataString += ",";

                    gameDataString += gameData.checkList[i][j].ToString();
                }
            }
            else
            {
                gameDataString += "Null";
            }
            
        }
        writer.WriteLine(gameDataString);

        for (int i = 0; i < gameData.vendList.Count; i++)
        {
            string fullData = "VM*";

            if (gameData.vendList[i] != null)
            {
                //Saves the vendID and roomDistance into the string
                fullData += gameData.vendList[i].vendIndex + "*" + gameData.vendList[i].roomDistance + "*";

                //Saves the list of items in the machine
                for (int j = 0; j < gameData.vendList[i].stockList.Length; j++)
                {
                    if (gameData.vendList[i].stockList[j] != null)
                    {
                        if (j != 0)
                        {
                            fullData += "|" + JsonUtility.ToJson(gameData.vendList[i].stockList[j]);
                        }
                        else
                        {
                            fullData += JsonUtility.ToJson(gameData.vendList[i].stockList[j]);
                        }
                    }
                    else
                    {
                        if (j != 0)
                        {
                            fullData += "|";
                        }
                        else
                        {
                            fullData += "";
                        }
                    }
                }

                fullData += "*";

                //Saves the list of prices
                for (int j = 0; j < gameData.vendList[i].priceList.Length; j++)
                {
                    if (j != 0)
                    {
                        fullData += "|" + gameData.vendList[i].priceList[j];
                    }
                    else
                    {
                        fullData += gameData.vendList[i].priceList[j];
                    }
                }
            }
            else
            {
                fullData = "null";
            }

            writer.WriteLine(fullData);
        }
        #endregion

        writer.WriteLine(sectionBreakString);

        #region Weapon
        WeaponData weaponData = gameSaveData.weaponData;

        //Writes the designated identifier before this data
        writer.WriteLine(weaponStoreName);

        //Converts the data to a JSON file format and writes the data
        for(int i = 0; i < weaponData.sodaParts.Length; i++)
        {
            if (weaponData.sodaParts[i] != null)
            {
                writer.WriteLine(JsonUtility.ToJson(weaponData.sodaParts[i]));
            }
            else
            {
                writer.WriteLine("null");
            }
        }
        #endregion

        writer.WriteLine(sectionBreakString);

        #region Inventory

        InventoryData invData = gameSaveData.invData;

        //Writes the designated identifier before this data
        writer.WriteLine(invStoreName);

        writer.WriteLine(JsonUtility.ToJson(invData));

        foreach (SodaPart data in invData.invParts)
        {
            if(data != null)
            {
                writer.WriteLine(JsonUtility.ToJson(data));
            }
            else
            {
                writer.WriteLine("NULL");
            }
        }

        #endregion

        writer.WriteLine(sectionBreakString);

        #region Map
        Map map = gameSaveData.mapData;

        //Writes the designated identifier before this data
        writer.WriteLine(mapStoreName);

        writer.WriteLine
        (mapName +
        map.mapSize + "|" +
        map.tileSize + "|" +
        map.wallHeight + "|" +
        map.currentRoom + "|" +
        map.spawnLocation.x + "," + map.spawnLocation.y + "," + map.spawnLocation.z
        );

        foreach (Room room in map.roomList)
        {
            //Writes the word ROOM so that the data can be decoded later
            writer.WriteLine(roomName + room.roomNumber);

            //Format is:
            // distance|position|array[0],array[1],array[2],array[3]

            foreach (Tile tile in room.tileList)
            {
                string tileData =
                    tileName +
                    tile.distanceNumber + "|" +
                    tile.position.x + "," + tile.position.y + "|" +
                    tile.borderTypeArray[0] + "," + tile.borderTypeArray[1] + "," + tile.borderTypeArray[2] + "," + tile.borderTypeArray[3] + "|" +
                    JsonUtility.ToJson(tile.furnArray[0]) + "/" + JsonUtility.ToJson(tile.furnArray[1]) + "/" + JsonUtility.ToJson(tile.furnArray[2]) + "/" + JsonUtility.ToJson(tile.furnArray[3]) + "|" +
                    JsonUtility.ToJson(tile.partArray[0]) + "/" + JsonUtility.ToJson(tile.partArray[1]) + "/" + JsonUtility.ToJson(tile.partArray[2]) + "/" + JsonUtility.ToJson(tile.partArray[3]) + "|" +
                    tile.minimapVisible + "|" +
                    JsonUtility.ToJson(tile.spawner);
                writer.WriteLine(tileData);
            }

            foreach (Spawner s in room.spawnerList)
            {
                string spawnerData = spawnerName + (int)s.spawnerLocation.x + "," + (int)s.spawnerLocation.y;
                writer.WriteLine(spawnerData);
            }

            
        }
        #endregion

        writer.WriteLine(sectionBreakString);

        #region Player
        PlayerData playerData = gameSaveData.playerData;

        //Writes the designated identifier before this data
        writer.WriteLine(playerStoreName);

        //Converts the data to a JSON file format and writes the data
        writer.WriteLine(JsonUtility.ToJson(playerData));
        writer.WriteLine(playerData.position.x + "," + playerData.position.y + "," + playerData.position.z);
        writer.WriteLine(playerData.rotation.x + "," + playerData.rotation.y + "," + playerData.rotation.z);

        #endregion

        writer.WriteLine(sectionBreakString);

        #region Enemies
        List<EnemyData> enemyData = gameSaveData.enemyData;

        //Writes the designated identifier before this data
        writer.WriteLine(enemyStoreName);

        foreach (EnemyData data in enemyData)
        {
            writer.WriteLine(JsonUtility.ToJson(data));
        }

        #endregion

        //Closes the connection with the writer
        writer.Close();
    }
    public static GameSaveData LoadData(string fileName)
    {
        if (File.Exists(Application.persistentDataPath + fileName + fileType))
        {
            //Opens reader connection to the document
            StreamReader reader = new StreamReader(Application.persistentDataPath + fileName + fileType);

            //Creates the GameSaveData instance for return
            GameSaveData gameSaveData = new GameSaveData();

            //Some controller variables
            bool sectionFound = false;
            bool sectionFinish = false;

            #region GameData
            //Moves the reader to just before the readable area
            sectionFound = false;

            while (!sectionFound)
            {
                if (reader.ReadLine() == gameStoreName)
                {
                    sectionFound = true;
                }
            }

            GameData gameData = JsonUtility.FromJson<GameData>(reader.ReadLine());

            List<List<int>> checkList = new List<List<int>>();
            string[] splitFullData = reader.ReadLine().Split('|');

            for (int i = 0; i < splitFullData.Length; i++)
            {
                checkList.Add(new List<int>());
                string[] splitData = splitFullData[i].Split(',');

                if (splitData[0] != "Null")
                {
                    foreach (string s in splitData)
                    {
                        checkList[i].Add(int.Parse(s));
                    }
                }
            }
            gameData.checkList = checkList;

            sectionFinish = false;

            while (!sectionFinish)
            {
                VendingMachineData vData = new VendingMachineData();
                string fullData = reader.ReadLine();

                if (fullData != null && fullData.Contains("VM"))
                {
                    //Splits the data into parts: identifier, roomNumber, roomDistance, stockList
                    string[] splitData = fullData.Split('*');

                    vData.vendIndex = int.Parse(splitData[1]);
                    vData.roomDistance = int.Parse(splitData[2]);

                    string[] splitParts = splitData[3].Split('|');

                    for (int i = 0; i < splitParts.Length; i++)
                    {
                        gameData.vendList.Add(null);

                        if (splitParts[i] != "")
                            vData.stockList[i] = JsonUtility.FromJson<SodaPart>(splitParts[i]);
                        else
                            vData.stockList[i] = null;
                    }

                    splitParts = splitData[4].Split('|');

                    for (int i = 0; i < splitParts.Length; i++)
                    {
                        if (splitParts[i] != "")
                            vData.priceList[i] = int.Parse(splitParts[i]);
                        else
                            vData.priceList[i] = -1;
                    }

                    gameData.vendList[vData.vendIndex] = vData;
                }
                else
                {
                    sectionFinish = true;
                }
            }

            gameSaveData.gameData = gameData;

            #endregion

            #region Weapon Data

            //Moves the reader to just before the readable area
            sectionFound = false;

            while (!sectionFound)
            {
                if (reader.ReadLine() == weaponStoreName)
                {
                    sectionFound = true;
                }
            }

            WeaponData weapon = new WeaponData();

            for (int i = 0; i < weapon.sodaParts.Length; i++)
            {
                string text = reader.ReadLine();

                if (text != "null")
                {
                    weapon.sodaParts[i] = JsonUtility.FromJson<SodaPart>(text);
                }
                else
                {
                    weapon.sodaParts[i] = null;
                }
            }

            gameSaveData.weaponData = weapon;

            #endregion

            #region Inventory Data

            //Moves the reader to just before the readable area
            sectionFound = false;

            while (!sectionFound)
            {
                if (reader.ReadLine() == invStoreName)
                {
                    sectionFound = true;
                }
            }

            InventoryData invData = new InventoryData();

            invData = JsonUtility.FromJson<InventoryData>(reader.ReadLine());

            //Reads the data
            List<SodaPart> inventoryDataList = new List<SodaPart>();
            sectionFinish = false;

            while (!sectionFinish)
            {
                string invStringData = reader.ReadLine();

                if (invStringData != "NULL" && invStringData != sectionBreakString)
                {
                    inventoryDataList.Add(JsonUtility.FromJson<SodaPart>(invStringData));
                }
                else if (invStringData == "NULL")
                {
                    inventoryDataList.Add(null);
                }
                else
                {
                    sectionFinish = true;
                }
            }

            for (int i = 0; i < inventoryDataList.Count; i++)
            {
                invData.invParts[i] = inventoryDataList[i];
            }

            gameSaveData.invData = invData;

            #endregion

            #region Map Data

            //Moves the reader to just before the readable area
            sectionFound = false;

            while (!sectionFound)
            {
                if (reader.ReadLine() == mapStoreName)
                {
                    sectionFound = true;
                }
            }

            Map map = new Map(200, 5, 3);

            //Creates a check which will trip when document is done
            bool docFinish = false;

            int currentRoomNumber = -1;

            while (!docFinish)
            {
                //Attempts to get next line of data
                string currentData = reader.ReadLine();

                //Checks if there is data left in the document
                if (currentData != null && currentData != sectionBreakString)
                {
                    if (currentData.Contains("Map"))
                    {
                        currentData = currentData.Replace(mapName, "");
                        string[] dataString = currentData.Split('|');

                        map.mapSize = int.Parse(dataString[0]);
                        map.tileSize = int.Parse(dataString[1]);
                        map.wallHeight = int.Parse(dataString[2]);
                        map.currentRoom = int.Parse(dataString[3]);

                        string[] spawnLocationString = dataString[4].Split(',');
                        map.spawnLocation = new Vector3(float.Parse(spawnLocationString[0]), float.Parse(spawnLocationString[1]), float.Parse(spawnLocationString[2]));
                    }
                    else if (currentData.Contains("Room"))
                    {
                        currentData = currentData.Replace(roomName, "");

                        currentRoomNumber = int.Parse(currentData);
                        Room room = new Room(currentRoomNumber);
                        map.roomList.Add(room);
                    }
                    else if (currentData.Contains(tileName))
                    {
                        currentData = currentData.Replace(tileName, "");

                        string[] dataString = currentData.Split('|');

                        int distance = int.Parse(dataString[0]);

                        string[] positionString = dataString[1].Split(',');
                        Vector2 position = new Vector2(int.Parse(positionString[0]), int.Parse(positionString[1]));

                        string[] borderArrayString = dataString[2].Split(',');
                        int[] borderArray = { int.Parse(borderArrayString[0]), int.Parse(borderArrayString[1]), int.Parse(borderArrayString[2]), int.Parse(borderArrayString[3]) };

                        //Splits the TileFurn's apart and Parses them
                        string[] furnArrayString = dataString[3].Split('/');
                        TileFurn[] furnList = new TileFurn[4];

                        for (int i = 0; i < furnArrayString.Length; i++)
                        {
                            furnList[i] = JsonUtility.FromJson<TileFurn>(furnArrayString[i]);
                        }

                        //Splits the TilePart's apart and Parses them
                        string[] partArrayString = dataString[4].Split('/');
                        TilePart[] partList = new TilePart[4];

                        for (int i = 0; i < partArrayString.Length; i++)
                        {
                            partList[i] = JsonUtility.FromJson<TilePart>(partArrayString[i]);
                        }

                        bool minimapVisible = bool.Parse(dataString[5]);

                        Spawner spawner = JsonUtility.FromJson<Spawner>(dataString[6]);

                        map.roomList[currentRoomNumber].tileList.Add(new Tile(distance, position, borderArray, furnList, partList, minimapVisible, spawner));
                    }
                    else if (currentData.Contains("Spawner"))
                    {
                        currentData = currentData.Replace(spawnerName, "");

                        string[] dataString = currentData.Split(',');
                        Vector2 spawnerData = new Vector2(float.Parse(dataString[0]), float.Parse(dataString[1]));
                        map.roomList[currentRoomNumber].spawnerList.Add(new Spawner(spawnerData));
                    }
                }
                else
                {
                    //Ends the reading process
                    docFinish = true;
                }
            }

            gameSaveData.mapData = map;

            #endregion

            #region Player Data

            //Moves the reader to just before the readable area
            sectionFound = false;

            while (!sectionFound)
            {
                if (reader.ReadLine() == playerStoreName)
                {
                    sectionFound = true;
                }
            }

            //Reads the data
            PlayerData playerCont;
            playerCont = JsonUtility.FromJson<PlayerData>(reader.ReadLine());

            string locString = reader.ReadLine();
            string[] positionList = locString.Split(',');
            playerCont.position = new Vector3(float.Parse(positionList[0]), float.Parse(positionList[1]), float.Parse(positionList[2]));

            string rotationString = reader.ReadLine();
            string[] rotationList = rotationString.Split(',');
            playerCont.rotation = new Vector3(float.Parse(rotationList[0]), float.Parse(rotationList[1]), float.Parse(rotationList[2]));

            gameSaveData.playerData = playerCont;

            #endregion

            #region Enemy Data

            //Moves the reader to just before the readable area
            sectionFound = false;

            while (!sectionFound)
            {
                if (reader.ReadLine() == enemyStoreName)
                {
                    sectionFound = true;
                }
            }

            //Reads the data
            List<EnemyData> enemyDataList = new List<EnemyData>();
            sectionFinish = false;

            while (!sectionFinish)
            {
                string data = reader.ReadLine();

                if (data != null && data.Contains("lootPoolID"))
                {
                    enemyDataList.Add(JsonUtility.FromJson<EnemyData>(data));
                }
                else
                {
                    sectionFinish = true;
                }
            }

            gameSaveData.enemyData = enemyDataList;

            #endregion

            //Closes the reader
            reader.Close();

            return gameSaveData;
        }

        return null;
    }

    #region Generic File Methods
    public static bool FileExists(int saveType)
    {
        return FileExists(saveType, "");
    }
    public static bool FileExists(int saveType, string addName)
    {
        switch (saveType)
        {
            case 0:
                return File.Exists(Application.persistentDataPath + saveFileName + addName + fileType);
            case 1:
                return File.Exists(Application.persistentDataPath + resetFileName + addName + fileType);
            case 2:
                return File.Exists(Application.persistentDataPath + seedFileName + addName + fileType);
            default:
                return false;
        }
    }
    public static void DeleteData(int saveType)
    {
        DeleteData(saveType, "");
    }
    public static void DeleteData(int saveType, string addName)
    {
        switch (saveType)
        {
            case 0:
                File.Delete(Application.persistentDataPath + saveFileName + addName + fileType);
                break;
            case 1:
                File.Delete(Application.persistentDataPath + resetFileName + addName + fileType);
                break;
            case 2:
                File.Delete(Application.persistentDataPath + seedFileName + addName + fileType);
                break;
        }
    }

    #endregion

    #region Seed Methods
    public static List<string> GetSeedNames()
    {
        //Gets all files in the folder
        string[] fileNames = Directory.GetFiles(Application.persistentDataPath);
        List<string> nameList = new List<string>();

        //Takes away unnecessary files
        for (int i = 0; i < fileNames.Length; i++)
        {
            //Checks if the file is a profile save file
            if (fileNames[i].Contains(seedFileName.Replace("/", "\\")))
            {
                string name = fileNames[i];

                //Removes everything except for the name from the string

                //Application.persistentDataPath sucks, why is it like this. the slash is fkn backwards
                name = name.Replace(Application.persistentDataPath, "");
                name = name.Replace(seedFileName.Replace("/", "\\"), "");
                name = name.Replace(fileType, "");

                //Adds the name to the name list
                nameList.Add(name);
            }
        }

        return nameList;
    }

    #endregion
}
