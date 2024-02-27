using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InventoryData
{
    public int tokens;
    [NonSerialized]
    public SodaPart[] invParts = new SodaPart[12];
}
