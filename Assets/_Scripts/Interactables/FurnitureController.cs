using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurnitureController : MonoBehaviour
{
    public List<GameObject> partPositionList;
    public List<GameObject> targetPositionList;

    public GameObject GetObjectPosition(int type, int index)
    {
        //This code takes the x value of the Vector 2 used to store the data for the easter egg parts and decides which list to take from

        switch (type)
        {
            case 0:
            case 2:
            case 3:
                return partPositionList[index];
            case 1:
                return targetPositionList[index];
            default:
                return null;
        }
    }
    public int GetRandomPosition(int type)
    {
        //This code takes the type of object and decides which type of target it will get
        switch (type)
        {
            case 0:
            case 2:
            case 3:
                return Random.Range(0, partPositionList.Count);
            case 1:
                return Random.Range(0, targetPositionList.Count);
            default:
                return -1;
        }
    }
}
