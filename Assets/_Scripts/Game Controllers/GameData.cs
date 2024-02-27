using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameData
{
    public int totalKills = 0;
    public int tokenSpent = 0;
    public int tokenCollected = 0;
    public int waveNum = 0;
    public float totalTime = 0.00f;
    public int roundKills = 0;
    public int roundSpawned = 0;

    public bool[] activeEE = new bool[6];
    public List<List<int>> checkList = new List<List<int>>();

    [NonSerialized]
    public List<VendingMachineData> vendList = new List<VendingMachineData>();
}
