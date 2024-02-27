using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

[Serializable]
public class Tile
{
    #region Data Related Things
    //Represents the distance that this tile is from the starting position
    public int distanceNumber { get; set; }

    //The position of the tile within the map
    public Vector2 position { get; set; }

    //Denotes whether you can see this tile on the minimap
    public bool minimapVisible;
    [NonSerialized]
    public static string minimapLayerName = "MinimapItem";

    //Holds the data for if a spawner is on this tile or not
    public Spawner spawner { get; set; }

    //This array is used to denote which wall slots on the tile are occupied and by what type (-1: null, 0: wall, 1: door)
    public int[] borderTypeArray { get; set; }

    //This array denotes what interactables are on this tile
    /*
     * Indexes:
     * 0: +z, 1: -z, 2: +x, 3: -x
     * 
     * Values:
     * -2: Closed
     * -1: Open
     * 0: Crafting Table
     * 1: Vending Machine
     * 2, 3, 4: Arcade Machines
     * 5: Table
     * 6: Trash Can
    */
    public TileFurn[] furnArray { get; set; } = new TileFurn[4];

    //This array denotes what Easter Egg Parts are on this tile x represents the type and y represents the index that it sits at on the actual gameobject
    /*
     * Indexes:
     * 0: +z, 1: -z, 2: +x, 3: -x
     * 
     * Values:
     * -2: Closed
     * -1: Open
     * 0: Basic Part
     * 1: Target
     * 2: Moving Soul Box
     * 3: Hide and Seek
    */
    public TilePart[] partArray { get; set; } = new TilePart[4];

    #endregion

    #region Object Related Things
    //This array is used to store the actual gameobjects which are contained within the tile
    public GameObject[] borderObjectArray { get; set; }
    public GameObject[] furnObjectArray { get; set; }

    //The object represented by this tile
    public GameObject tileObject { get; set; }
    #endregion

    #region Constructors
    public Tile(int newDistanceNumber, Vector2 newPosition)
    {
        distanceNumber = newDistanceNumber;
        position = newPosition;
        borderObjectArray = new GameObject[4];
        furnObjectArray = new GameObject[4];
        spawner = null;

        borderTypeArray = new int[4];
        for(int i = 0; i < borderTypeArray.Length; i++)
        {
            borderTypeArray[i] = -1;
            furnArray[i] = new TileFurn();
            partArray[i] = new TilePart();
        }
    }
    public Tile(int newDistanceNumber, Vector2 newPosition, int[] newBorderTypeArray, TileFurn[] newFurnArray, TilePart[] newPartArray, bool newMinimapVisible, Spawner newSpawner)
    {
        distanceNumber = newDistanceNumber;
        position = newPosition;
        borderObjectArray = new GameObject[4];
        furnObjectArray = new GameObject[4];

        borderTypeArray = newBorderTypeArray;
        furnArray = newFurnArray;
        partArray = newPartArray;

        minimapVisible = newMinimapVisible;

        spawner = newSpawner;
    }
    #endregion

    #region Minimap Methods

    public void SetVisible(bool b)
    {
        minimapVisible = b;

        //Enables or disables the renderers for the object
        foreach (Renderer rend in tileObject.GetComponentsInChildren<Renderer>())
        {
            //Checks if the renderer is on the correct layer
            if (rend.gameObject.layer == LayerMask.NameToLayer(minimapLayerName))
            {
                rend.enabled = b;
            }
        }
    }

    #endregion

    public void SetMinimapActive()
    {
        //Sets the object to be visible
        SetVisible(minimapVisible);
        if (!minimapVisible)
        {
            tileObject.GetComponentInChildren<MinimapActivatorController>().Activate(this);
        }
        else
        {
            if(tileObject.GetComponentInChildren<MinimapActivatorController>())
                tileObject.GetComponentInChildren<MinimapActivatorController>().DestroyActivator();
        }
            
    }

    public bool HasDoor()
    {
        foreach(int i in borderTypeArray)
        {
            if(i == 1 || i == -2)
            {
                return true;
            }
        }

        return false;
    }
    public DoorController GetDoor(int directionIndex)
    {
        for(int i = 0; i < 4; i++)
        {
            if (i == directionIndex && borderTypeArray[i] == 1)
            {
                return borderObjectArray[i].GetComponentInChildren<DoorController>();
                
            }
        }

        return null;
    }
    public List<int> GetAllDoors()
    {
        List<int> doors = new List<int>();

        for(int i = 0; i < borderTypeArray.Length; i++)
        {
            if (borderTypeArray[i] == 1 || borderTypeArray[i] == 2 || borderTypeArray[i] == -2)
                doors.Add(i);
        }

        if (doors.Count > 0)
            return doors;
        else
            return null;
    }

    public bool HasOpenFurn()
    {
        foreach(TileFurn t in furnArray)
        {
            if (t.type == -1)
            {
                return true;
            }
        }

        return false;
    }
    public bool HasOpenPart()
    {
        foreach (TilePart t in partArray)
        {
            if (t.type == -1)
            {
                return true;
            }
        }

        return false;
    }
}
