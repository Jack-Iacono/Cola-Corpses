using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffData
{

    public int[] activeBuff;
    public float[] buffTimers;
    public float[] buffMaxTime;

    public BuffData(int buffListSize)
    {
        activeBuff = new int[buffListSize];
        buffTimers = new float[buffListSize];
        buffMaxTime = new float[buffListSize];
    }

}
