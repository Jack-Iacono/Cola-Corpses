using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileFurn
{
    //Stores the data for each part of the tile
    public int type;
    public int furnIndex;
    public int[] addData = new int[0];
    public int[] subIndexList = new int[0];

    public TileFurn()
    {
        type = -2;
        furnIndex = -2;
        addData = new int[0];
        subIndexList = new int[0];
    }
    public TileFurn(int newType, int newFurnIndex, int[] newSubIndexList)
    {
        type = newType;
        furnIndex = newFurnIndex;
        subIndexList = newSubIndexList;
    }
}
