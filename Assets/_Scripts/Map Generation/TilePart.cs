using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TilePart
{
    //Stores the data for each part of the tile
    public int type;
    public int posIndex;
    public int partIndex;
    public int[] addData = new int[0];
    public int[] subIndexList = new int[0];

    public TilePart()
    {
        type = -2;
        posIndex = 0;
        partIndex = -2;
        addData = new int[0];
        subIndexList = new int[0];
    }
    public TilePart(int newType, int newPosIndex, int newPartIndex)
    {
        type = newType;
        posIndex = newPosIndex;
        partIndex = newPartIndex;
    }

}
