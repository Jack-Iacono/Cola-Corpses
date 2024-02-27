using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class AmmoData
{

    [Serialize]
    public int[] sodaAmounts { get; set; } = new int[6];
    [Serialize]
    public int[] maxSodaAmounts { get; set; } = new int[6];
    [Serialize]
    public int[] sodaPerUnit { get; set; } = new int[6];

    public AmmoData()
    {
        sodaAmounts = new int[6];
        maxSodaAmounts = new int[6];
        sodaPerUnit = new int[6];
    }

    public AmmoData(int[] sodaList, int[] maxList, int[] unitList)
    {
        sodaAmounts = sodaList;
        maxSodaAmounts = maxList;
        sodaPerUnit = unitList;
    }

}
