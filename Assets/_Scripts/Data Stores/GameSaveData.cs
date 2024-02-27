using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSaveData
{
    public GameData gameData;
    public WeaponData weaponData;
    public InventoryData invData;
    public Map mapData;
    public PlayerData playerData;
    public List<EnemyData> enemyData;

    public GameSaveData()
    {
        gameData = null;
        weaponData = null;
        invData = null;
        mapData = null;
        playerData = null;
        enemyData = null;
    }
    public GameSaveData(GameData newGameData, WeaponData newWeaponData, InventoryData newInvdata, Map newMapData, PlayerData newPlayerData, List<EnemyData> newEnemyData)
    {
        gameData = newGameData;
        weaponData = newWeaponData;
        invData = newInvdata;
        mapData = newMapData;
        playerData = newPlayerData;
        enemyData = newEnemyData;
    }

}
