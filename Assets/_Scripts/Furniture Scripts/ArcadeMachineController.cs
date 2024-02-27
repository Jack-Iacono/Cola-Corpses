using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcadeMachineController : MonoBehaviour
{
    public List<Material> cabinetSkins;
    public int index = -1;

    public int Initialize(int i)
    {
        if(i < 0)
            index = Random.Range(0, cabinetSkins.Count);
        else
            index = i;

        return index;
    }
    public int[] GetSubIndexArray(int[] subList)
    {
        Renderer[] rendArray = GetComponentsInChildren<Renderer>();

        if(subList.Length == 0)
        {
            int[] newArray = new int[rendArray.Length];

            for (int i = 0; i < newArray.Length; i++)
            {
                newArray[i] = Random.Range(0, cabinetSkins.Count);
                rendArray[i].material = cabinetSkins[newArray[i]];
            }

            return newArray;
        }
        else
        {
            for (int i = 0; i < subList.Length; i++)
            {
                rendArray[i].material = cabinetSkins[subList[i]];
            }

            return subList;
        }
        
    }
}
