using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class Room
{
    public int roomNumber { get; set; }
    public List<Tile> tileList { get; set; }
    public List<Spawner> spawnerList { get; set; }
    public GameObject roomObject { get; set; }

    #region Constructors
    public Room(int newRoomNumber)
    {
        roomNumber = newRoomNumber;
        tileList = new List<Tile>();
        spawnerList = new List<Spawner>();
    }
    public Room(int newRoomNumber, GameObject newRoomObject)
    {
        roomNumber = newRoomNumber;
        roomObject = newRoomObject;
        tileList = new List<Tile>();
        spawnerList = new List<Spawner>();
    }
    #endregion

    public List<Vector2> GetTilePostions()
    {
        List<Vector2> tiles = new List<Vector2>();

        foreach (Tile tile in tileList)
        {
            tiles.Add(tile.position);
        }

        return tiles;
    }
    public Tile GetTile(Vector2 pos)
    {
        for(int i = 0; i < tileList.Count; i++)
        {
            if (tileList[i].position == pos)
            {
                return tileList[i];
            }
        }
        return null;
    }
    public DoorController GetDoor(Vector2 pos, int directionIndex)
    {
        for(int j = 0; j < tileList.Count; j++)
        {
            if (tileList[j].position == pos)
            {
                return tileList[j].GetDoor(directionIndex);
            }
        }

        return null;
    }
    public Vector2 AssignOpenFurn(int objectType)
    {
        List<int> indexList = new List<int>();

        //Gets the list of indexes of tiles that have open spaces
        for(int i = 0; i < tileList.Count; i++)
        {
            if (tileList[i].HasOpenFurn())
            {
                indexList.Add(i);
            }
        }

        //Moves into choosing a location if the there are tiles with open spaces
        if(indexList.Count > 0)
        {
            int tileIndex = Random.Range(0, indexList.Count);
            List<int> openIndex = new List<int>();

            //Takes the opens spaces' indeces and places them into an array to be chosen from
            for(int i = 0; i < tileList[indexList[tileIndex]].furnArray.Length; i++)
            {
                if (tileList[indexList[tileIndex]].furnArray[i].type == -1)
                {
                    openIndex.Add(i);
                }
            }

            int rand = Random.Range(0, openIndex.Count);

            //Places the given object at the chosen point in the array
            tileList[indexList[tileIndex]].furnArray[openIndex[rand]].type = objectType;

            //Ensures that the part array will exclude crafting benches and vending machines
            if(objectType == 0 || objectType == 1 || objectType == 9 || objectType == 10)
            {
                tileList[indexList[tileIndex]].partArray[openIndex[rand]].type = -2;
            }

            return new Vector2(indexList[tileIndex], openIndex[rand]);
        }

        return Vector2.zero;
    }
    public Vector2 AssignOpenPart(int objectType)
    {
        List<int> indexList = new List<int>();

        //Gets the list of indexes of tiles that have open spaces
        for (int i = 0; i < tileList.Count; i++)
        {
            if (tileList[i].HasOpenPart())
            {
                indexList.Add(i);
            }
        }

        //Moves into choosing a location if the there are tiles with open spaces
        if (indexList.Count > 0)
        {
            int tileIndex = Random.Range(0, indexList.Count);
            List<int> openIndex = new List<int>();

            //Takes the opens spaces' indeces and places them into an array to be chosen from
            for (int i = 0; i < tileList[indexList[tileIndex]].partArray.Length; i++)
            {
                if (tileList[indexList[tileIndex]].partArray[i].type == -1)
                {
                    openIndex.Add(i);
                }
            }

            int rand = Random.Range(0, openIndex.Count);

            //Places the given object at the chosen point in the array
            tileList[indexList[tileIndex]].partArray[openIndex[rand]] = new TilePart(objectType, -1, -1);

            return new Vector2(indexList[tileIndex], openIndex[rand]);
        }

        return Vector2.zero;
    }
    public int GetAverageDistance()
    {
        int total = 0;
        int count = 0;

        foreach(Tile t in tileList)
        {
            total += t.distanceNumber;
            count++;
        }

        return total / count;
    }

    public bool IsOpen()
    {
        foreach (Tile t in tileList)
        {
            if (t.borderTypeArray.Contains(2))
            {
                return true;
            }
        }

        return false;
    }

    public void RenderRoom(bool b)
    {
        foreach(Tile tile in tileList)
        {
            if (!b)
            {
                tile.tileObject.SetActive(false);
            }
            else
            {
                tile.tileObject.SetActive(true);
            }
            
        }
    }
    public void RenderRoom(List<Tile> exceptionList)
    {
        foreach (Tile tile in tileList)
        {
            bool check = false;

            //Checks if the tile has a door on it
            foreach (int i in tile.borderTypeArray)
            {
                if (i == 1)
                    check = true;
            }

            //Checks if the tile is in the exception list
            if (exceptionList.Contains(tile))
            {
                check = true;
            }

            if (!check)
                tile.tileObject.SetActive(false);
            else
                tile.tileObject.SetActive(true);
        }
    }

    public void RevealMinimap()
    {
        for(int i = 0; i < tileList.Count; i++)
        {
            tileList[i].SetVisible(true);
        }
    }
}
