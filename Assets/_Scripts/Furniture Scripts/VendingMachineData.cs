using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class VendingMachineData
{

    public int vendIndex;
    public int roomDistance;

    public SodaPart[] stockList = new SodaPart[12];
    public int[] priceList = new int[12];

}
