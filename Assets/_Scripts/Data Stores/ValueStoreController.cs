using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class ValueStoreController
{
    //This script will store all values needed throughout the game and in the menus

    #region Data Stores
    public static bool loadData { get; set; } = false;
    public static bool loadedFile { get; set; } = false;

    //Stores data to be used by the map generation controller
    public static int mapDiffIndex { get; set; } = 0;

    //Controls which file number is being used currently
    public static int fileNumber { get; set; } = 0;
    public static string fileOwner { get; set; } = "Default";
    public static string seedName { get; set; } = "";
    #endregion

    #region Data Loading

    public static GameSaveData loadedGameData { get; set; }
    public static bool isTutorial = false;

    #endregion

    #region Profile Data
    public static KeybindData keyData { get; set; } = ProfileSaveController.LoadKeyData(fileOwner);
    public static PreferenceData prefData { get; set; } = ProfileSaveController.LoadPreferenceData(fileOwner);
    public static TotalData totalData { get; set; } = ProfileSaveController.LoadTotalData(fileOwner);
    #endregion

    #region Game Data Saving

    public static void SaveGameData(int saveType)
    {
        SaveDataStore.SaveData
            (
                new GameSaveData
                (
                    GameController.gameCont.SaveData(),
                    WeaponController.weaponCont.GetSaveData(),
                    InventoryController.invCont.GetSaveData(),
                    MapGenerationController.mapGenCont.map,
                    PlayerController.playerCont.GetPlayerData(),
                    Enemy.SaveData()
                ),
                saveType
            );

        SaveProfileData();
    }
    public static void LoadGameData(int saveType)
    {
        loadData = true;

        loadedGameData = SaveDataStore.LoadData(saveType);
    }
    public static void SaveGameData(int saveType, string addName)
    {
        if(saveType != 2)
        {
            SaveDataStore.SaveData
            (
                new GameSaveData
                (
                    GameController.gameCont.SaveData(),
                    WeaponController.weaponCont.GetSaveData(),
                    InventoryController.invCont.GetSaveData(),
                    MapGenerationController.mapGenCont.map,
                    PlayerController.playerCont.GetPlayerData(),
                    Enemy.SaveData()
                ),
                saveType,
                addName
            );
        }
        else
        {
            SaveDataStore.SaveData
            (
                SaveDataStore.LoadData(1),
                saveType,
                addName
            );
        }
        

        SaveProfileData();
    }
    public static void LoadGameData(int saveType, string addName)
    {
        loadData = true;

        loadedGameData = SaveDataStore.LoadData(saveType, addName);
    }
    public static void SetLoadData(bool b)
    {
        loadData = b;
    }

    #endregion

    public static void SaveProfileData()
    {
        ProfileSaveController.SaveData(prefData, keyData, totalData, fileOwner);
    }
    public static void UpdateProfileData()
    {
        keyData = ProfileSaveController.LoadKeyData(fileOwner);
        prefData = ProfileSaveController.LoadPreferenceData(fileOwner);
        totalData = ProfileSaveController.LoadTotalData(fileOwner);
    }
    public static void SetNewOwner(string name)
    {
        fileOwner = name;

        UpdateProfileData();

        SaveProfileData();

        fileOwner = name;
    }
}
