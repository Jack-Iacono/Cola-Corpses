using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

/*
 *  NOTES
 *  
 *  - The first index represents X and the second represents Y
 *  - + represents up and right while - represents down and left
 *  - 
 * 
*/

public class MapGenerationController : MonoBehaviour
{
    public static MapGenerationController mapGenCont;

    /* Value Meanings for borderArray
        * 0: Places a wall 
        * 1: Places a door
        * 2: Places a door frame
        * -1: Places nothing
        * -2: Places nothing, but links to a previously placed door
        * -3: Places nothing, but links to a previously places wall
    */

    #region Map Variables
    public static int mapSize = 200;
    public float tileSize = 5;
    public float wallHeight = 3;

    // Represents the difficulties that can be selected from in game
    public static readonly int[] mapTileCounts = new int[]{ 150, 300, 600 };
    public static readonly int[] mapRoomSizes = new int[]{ 30, 25, 25 };

    private float roomSizeMargin = 0.4f;
    #endregion

    int roomNumber = 0;

    public Map map { get; set; }

    [Header ("Basic Map Parts")]
    public GameObject floorObject;
    public GameObject ceilingObject;
    public GameObject wallObject;
    public GameObject doorObject;
    public GameObject doorFrameObject;
    public GameObject roomDetectorObject;
    public GameObject spawnerObject;

    [Header("Interactables")]
    public GameObject vendingMachine;
    public GameObject craftingTable;
    public GameObject sodaSeller;

    [Header("Decorative Furniture")]
    public GameObject arcadeMachine1;
    public GameObject arcadeMachine2;
    public GameObject arcadeMachine3;
    public List<Material> arcadeMachineMaterials;

    public GameObject tables;
    public GameObject trashCan;

    [Header("Easter Egg Items")]
    public GameObject eePart;
    public GameObject eeTarget;
    public GameObject eeHideAndSeek;
    public GameObject eeSoulBoxMove;
    public GameObject eeSoulBox;
    public GameObject eeWallCode;
    public GameObject eeExitObject;

    private int currentTileCount = 0;

    //Loading Stuff
    VendingMachineData[] vData;

    //For Accessing Vending Machines
    public List<GameObject> vendList { get; set; } = new List<GameObject>();

    #region Initialization
    //Used for setting static reference
    public void SetStaticInstance()
    {
        //Creating a singleton
        if (mapGenCont != null && mapGenCont != this)
        {
            Destroy(this);
        }
        else
        {
            mapGenCont = this;
        }
    }
    public void Initialize()
    {

        if (ValueStoreController.loadData)
        {
            map = ValueStoreController.loadedGameData.mapData;

            if (map != null)
            {
                map = BuildMapLayout(map);
                PlaceMap(map);
            }
            else
            {
                //Initializes the map storing variable
                map = new Map
                (
                    mapSize,
                    tileSize,
                    wallHeight,
                    mapTileCounts[ValueStoreController.mapDiffIndex],
                    mapRoomSizes[ValueStoreController.mapDiffIndex]
                );

                //Generates the map and places the pieces in the scene
                GenerateMap();
            }
        }
        else
        {
            //Initializes the map storing variable
            map = new Map
            (
                mapSize,
                tileSize,
                wallHeight,
                mapTileCounts[ValueStoreController.mapDiffIndex],
                mapRoomSizes[ValueStoreController.mapDiffIndex]
            );

            //Generates the map and places the pieces in the scene
            GenerateMap();
        }

        FindObjectOfType<PlayerController>().WarpPlayer(map.spawnLocation);
    }
    #endregion

    private void GenerateMap()
    {
        #region Map Initialization

        //Sets the starting point for the digging algorithm to the middle
        Vector2 startingPosition = new Vector2(Mathf.FloorToInt(map.mapSize / 2), Mathf.FloorToInt(map.mapSize / 2));
        Vector2 currentPosition = startingPosition;

        map.spawnLocation = new Vector3(startingPosition.x * map.tileSize, 0.5f, startingPosition.y * map.tileSize);
        #endregion

        //Loops to use all tiles
        while (currentTileCount < map.mapTileCount)
        {
            int currentRoomTileCount = 0;

            //Initializes the map storing variable
            Map currentMap = new Map(map.mapSize, tileSize, wallHeight);
            
            //Sets all values in the map to null
            for (int i = 0; i < map.mapSize; i++)
            {
                for (int j = 0; j < map.mapSize; j++)
                {
                    currentMap.mapLayout[i, j] = null;
                }
            }

            //Creates a starting tile with distance relative to the starting room
            currentMap.mapLayout[(int)currentPosition.x, (int)currentPosition.y] = new Tile
                (
                Mathf.FloorToInt(Mathf.Sqrt(Mathf.Pow(startingPosition.x - currentPosition.x, 2) + Mathf.Pow(startingPosition.y - currentPosition.y, 2))), 
                new Vector2((int)currentPosition.x, (int)currentPosition.y)
                );

            bool forceExit = false;

            //Places the tiles in the room
            while (currentRoomTileCount < map.roomTileCount && !forceExit)
            {
                Vector2 dir = GetDirection(currentPosition, currentMap.mapLayout, map.mapLayout);

                if (dir != Vector2.zero)
                {
                    //Adds the direction to the current position
                    currentPosition += dir;

                    //Assigns a value to the array based on distance from the start
                    currentMap.mapLayout[(int)currentPosition.x, (int)currentPosition.y] = new Tile
                        (
                        Mathf.FloorToInt(Mathf.Sqrt(Mathf.Pow(startingPosition.x - currentPosition.x, 2) + Mathf.Pow(startingPosition.y - currentPosition.y, 2))),
                        new Vector2((int)currentPosition.x, (int)currentPosition.y)
                        );

                    currentRoomTileCount++;
                    currentTileCount++;
                }
                else
                {
                    //Room generation failure aka at the end of a path
                    forceExit = true;
                }
            }

            //Checks if the room fits within the size contraints
            if (currentRoomTileCount >= Mathf.FloorToInt(map.roomTileCount * roomSizeMargin))
            {

                for (int i = 0; i < map.mapSize; i++)
                {
                    for (int j = 0; j < map.mapSize; j++)
                    {
                        if (map.mapLayout[i, j] == null)
                        {
                            map.mapLayout[i, j] = currentMap.mapLayout[i, j];
                        }
                    }
                }

                AssignWalls(currentMap.mapLayout, map.mapLayout, roomNumber);
                AssignSpawners(currentMap.mapLayout, roomNumber);

                currentPosition += GetDirection(currentPosition, currentMap.mapLayout, map.mapLayout);
                roomNumber++;
            }
            else
            {
                //Need to have this so that the direction method doesn't error out
                //Basically resets the room if it generates with too few tiles
                if(roomNumber != 0)
                {
                    currentTileCount -= currentRoomTileCount;
                    currentPosition = GetRandomTile(map.mapLayout);
                    currentPosition += GetDirection(currentPosition, map.mapLayout);
                }
                else
                {
                    currentTileCount -= currentRoomTileCount;
                    currentPosition = startingPosition;
                }
            }

        }

        //Need to assign furniture on finalized map or else overrides occur
        AssignFurniture(map);

        //Assigns the parts after everything else has settled
        AssignParts(map);

        //Places the map objects
        PlaceMap(map);
    }
    private void AssignWalls(Tile[,] placeMap, Tile[,] overlapMap, int roomNumber)
    {
        //This will assign values to the borderTypeArray in each tile WITHOUT THE GAMEOBJECTS (For saving purposes)
        #region Initializing Arrays
        //Creates a list which will store the possible door locations for this room
        List<List<Vector2>> roomTilePositionList = new List<List<Vector2>>();

        //Creates a list which will hold the corresponding door/wall location for placement
        List<List<int>> wallPositionList = new List<List<int>>();

        //Adds the correct amount of lists to each array
        for(int i = 0; i < roomNumber; i++)
        {
            roomTilePositionList.Add(new List<Vector2>());
            wallPositionList.Add(new List<int>());
        }

        #endregion

        //Adds the room to the list of rooms in the map for reference later
        map.roomList.Add(new Room(roomNumber));

        for (int i = 0; i < map.mapSize; i++)
        {
            for (int j = 0; j < map.mapSize; j++)
            {
                //If there is a tile in the map that is being placed
                if (placeMap[i, j] != null)
                {
                    #region Wall Placement
                    //The first if determines if there is an open space in the given direction based on the whole map
                    //The second determines if there is another wall in the way of a door and places the CURRENT tile details into the array
                    //the Third accounts for the edge of the world being in the position
                    if(j + 1 < mapSize)
                    {
                        if (overlapMap[i, j + 1] == null)
                        {
                            //Assigns a wall to the UP position
                            map.mapLayout[i, j].borderTypeArray[0] = 0;
                            map.mapLayout[i, j].furnArray[0].type = -1;
                            map.mapLayout[i, j].partArray[0].type = -1;
                        }
                        else if (overlapMap[i, j + 1].borderTypeArray[1] == 0)
                        {
                            //Does not add a wall to the current tile because a wall is already placed there by another tile
                            //Searches for the room and tile which is being collided with to add them to the 
                            for (int k = 0; k < roomNumber; k++)
                            {
                                foreach (Tile tile in map.roomList[k].tileList)
                                {
                                    if (tile == overlapMap[i, j + 1])
                                    {
                                        roomTilePositionList[k].Add(placeMap[i, j].position);
                                        wallPositionList[k].Add(0);
                                    }
                                }
                            }

                            //Designates that there is a wall at this position, but placed by another tile
                            map.mapLayout[i, j].borderTypeArray[0] = -3;
                            map.mapLayout[i, j].furnArray[0].type = -1;
                            map.mapLayout[i, j].partArray[0].type = -1;
                        }
                    }
                    else
                    {
                        //Assigns a wall to the UP position
                        map.mapLayout[i, j].borderTypeArray[0] = 0;
                    }

                    if(j - 1 > 0)
                    {
                        if (overlapMap[i, j - 1] == null)
                        {
                            //Assigns a wall to the DOWN position
                            map.mapLayout[i, j].borderTypeArray[1] = 0;
                            map.mapLayout[i, j].furnArray[1].type = -1;
                            map.mapLayout[i, j].partArray[1].type = -1;
                        }
                        else if (overlapMap[i, j - 1].borderTypeArray[0] == 0)
                        {
                            //Searches for the room and tile which is being collided with to add them to the 
                            for (int k = 0; k < roomNumber; k++)
                            {
                                foreach (Tile tile in map.roomList[k].tileList)
                                {
                                    if (tile == overlapMap[i, j - 1])
                                    {
                                        roomTilePositionList[k].Add(placeMap[i, j].position);
                                        wallPositionList[k].Add(1);
                                    }
                                }
                            }

                            //Designates that there is a wall at this position, but placed by another tile
                            map.mapLayout[i, j].borderTypeArray[1] = -3;
                            map.mapLayout[i, j].furnArray[1].type = -1;
                            map.mapLayout[i, j].partArray[1].type = -1;
                        }
                    }
                    else
                    {
                        //Assigns a wall to the DOWN position
                        map.mapLayout[i, j].borderTypeArray[1] = 0;
                    }

                    if(i + 1 < mapSize)
                    {
                        if (overlapMap[i + 1, j] == null)
                        {
                            //Assigns a wall to the RIGHT position
                            map.mapLayout[i, j].borderTypeArray[2] = 0;
                            map.mapLayout[i, j].furnArray[2].type = -1;
                            map.mapLayout[i, j].partArray[2].type = -1;
                        }
                        else if (overlapMap[i + 1, j].borderTypeArray[3] == 0)
                        {
                            //Searches for the room and tile which is being collided with to add them to the 
                            for (int k = 0; k < roomNumber; k++)
                            {
                                foreach (Tile tile in map.roomList[k].tileList)
                                {
                                    if (tile == overlapMap[i + 1, j])
                                    {
                                        roomTilePositionList[k].Add(placeMap[i, j].position);
                                        wallPositionList[k].Add(2);
                                    }
                                }
                            }

                            //Designates that there is a wall at this position, but placed by another tile
                            map.mapLayout[i, j].borderTypeArray[2] = -3;
                            map.mapLayout[i, j].furnArray[2].type = -1;
                            map.mapLayout[i, j].partArray[2].type = -1;
                        }
                    }
                    else
                    {
                        //Assigns a wall to the RIGHT position
                        map.mapLayout[i, j].borderTypeArray[2] = 0;
                    }

                    if(i - 1 > 0)
                    {
                        if (overlapMap[i - 1, j] == null)
                        {
                            //Assigns a wall to the LEFT position
                            map.mapLayout[i, j].borderTypeArray[3] = 0;
                            map.mapLayout[i, j].furnArray[3].type = -1;
                            map.mapLayout[i, j].partArray[3].type = -1;
                        }
                        else if (overlapMap[i - 1, j].borderTypeArray[2] == 0)
                        {
                            //Searches for the room and tile which is being collided with to add them to the 
                            for (int k = 0; k < roomNumber; k++)
                            {
                                foreach (Tile tile in map.roomList[k].tileList)
                                {
                                    if (tile == overlapMap[i - 1, j])
                                    {
                                        roomTilePositionList[k].Add(placeMap[i, j].position);
                                        wallPositionList[k].Add(3);
                                    }
                                }
                            }

                            //Designates that there is a wall at this position, but placed by another tile
                            map.mapLayout[i, j].borderTypeArray[3] = -3;
                            map.mapLayout[i, j].furnArray[3].type = -1;
                            map.mapLayout[i, j].partArray[3].type = -1;
                        }
                    }
                    else
                    {
                        //Assigns a wall to the LEFT position
                        map.mapLayout[i, j].borderTypeArray[3] = 0;
                    }
                    
                    #endregion
                }
            }
        }

        //Doors
        //Goes through each room
        for(int i = 0; i < roomTilePositionList.Count; i++)
        {
            //Checks if it has any connecting tiles
            if (roomTilePositionList[i].Count > 0)
            {
                //Random index within the range of the position list
                int index = Random.Range(0, roomTilePositionList[i].Count);

                //Convert the index into points on the map
                int x = (int)roomTilePositionList[i][index].x;
                int y = (int)roomTilePositionList[i][index].y;

                //Takes the wall from the connecting room and turns it into a door
                //Also makes the furniture array in each tile unable to place in front of doors
                switch (wallPositionList[i][index])
                {
                    case 0:
                        map.mapLayout[x, y + 1].borderTypeArray[1] = 1;
                        map.mapLayout[x, y + 1].furnArray[1].type = -2;
                        map.mapLayout[x, y + 1].partArray[1].type = -2;
                        break;
                    case 1:
                        map.mapLayout[x, y - 1].borderTypeArray[0] = 1;
                        map.mapLayout[x, y - 1].furnArray[0].type = -2;
                        map.mapLayout[x, y - 1].partArray[0].type = -2;
                        break;
                    case 2:
                        map.mapLayout[x + 1, y].borderTypeArray[3] = 1;
                        map.mapLayout[x + 1, y].furnArray[3].type = -2;
                        map.mapLayout[x + 1, y].partArray[3].type = -2;
                        break;
                    case 3:
                        map.mapLayout[x - 1, y].borderTypeArray[2] = 1;
                        map.mapLayout[x - 1, y].furnArray[2].type = -2;
                        map.mapLayout[x - 1, y].partArray[2].type = -2;
                        break;
                }

                //Sets the randomly selected wall to have a null position where the door will go
                map.mapLayout[x, y].borderTypeArray[wallPositionList[i][index]] = -2;
                placeMap[x, y].borderTypeArray[wallPositionList[i][index]] = -2;

                map.mapLayout[x, y].furnArray[wallPositionList[i][index]].type = -2;
                map.mapLayout[x, y].partArray[wallPositionList[i][index]].type = -2;
            }
        }

        //May need to adjust to account for adding everything from placeMap
        //Adds the finalized tile to the list of tiles in the room list of the map
        for(int i = 0; i < map.mapSize; i++)
        {
            for (int j = 0; j < map.mapSize; j++)
            {
                if(placeMap[i, j] != null)
                {
                    map.roomList[roomNumber].tileList.Add(placeMap[i, j]);
                }
            }
        }
    }
    private void AssignSpawners(Tile[,] placeMap, int roomNumber)
    {
        List<Tile> tiles = new List<Tile>();
        int spawnerCount = Mathf.FloorToInt(map.roomList[roomNumber].tileList.Count / 10);

        //Checks to see if the tile exists and does not have a door on it
        for(int i = 0; i < map.mapSize; i++)
        {
            for (int j = 0; j < map.mapSize; j++)
            {
                if (placeMap[i,j] != null && !placeMap[i, j].HasDoor())
                {
                    tiles.Add(placeMap[i, j]);
                }
            }
        }

        for(int i = 0; i < spawnerCount; i++)
        {
            int index = Random.Range(0,tiles.Count);
            float randomX = 0;
            float randomZ = 0;

            tiles[index].spawner = new Spawner(new Vector3(randomX, 0, randomZ));
            tiles.RemoveAt(index);
        }
    }
    private void AssignFurniture(Map placeMap)
    {
        //Generates the list of easter egg related furniture needed and which rooms it will be in
        GameController gameCont = GetComponent<GameController>();

        //Gets the easter egg step list
        bool[] eeList = GetComponent<GameController>().activeEE;

        //Soul Boxes
        if (eeList[1])
        {
            //Assigns 4 soul boxes to random rooms in the map
            for(int i = 0; i < 4; i++)
            {
                int roomIndex = Random.Range(0, placeMap.roomList.Count);

                //Adds a random room number
                Vector2 tileFurn = placeMap.roomList[roomIndex].AssignOpenFurn(7);
                placeMap.roomList[roomIndex].tileList[(int)tileFurn.x].furnArray[(int)tileFurn.y].furnIndex = gameCont.GetIndex(1);
                placeMap.roomList[roomIndex].tileList[(int)tileFurn.x].furnArray[(int)tileFurn.y].addData = new int[] { 20 + placeMap.roomList.Count / 3 };

                //Adds one place to the game controller
                gameCont.checkList[1].Add(1);
            }
        }

        //Wall Codes
        if (eeList[2])
        {
            //Adds 5 code pieces throughout the map
            for (int i = 0; i < 10; i++)
            {
                int roomIndex = Random.Range(0, placeMap.roomList.Count);

                //Adds a random room number
                Vector2 tileFurn = placeMap.roomList[roomIndex].AssignOpenFurn(8);

                placeMap.roomList[roomIndex].tileList[(int)tileFurn.x].furnArray[(int)tileFurn.y].furnIndex = gameCont.GetIndex(2);

                //Adds one place to the game controller
                //Codes use 1 as the initializer as this helps them work without additional code
                gameCont.checkList[2].Add(1);
            }
        }

        //Assigns the furniture that may be present in the room
        foreach (Room r in placeMap.roomList)
        {
            //Places one vending machine, crafting table, soda seller, and phone box in each room
            Vector2 tileFurn = placeMap.roomList[r.roomNumber].AssignOpenFurn(1);
            placeMap.roomList[r.roomNumber].tileList[(int)tileFurn.x].furnArray[(int)tileFurn.y].furnIndex = gameCont.GetFurnIndex(0);
            gameCont.vendList.Add(null);

            placeMap.roomList[r.roomNumber].AssignOpenFurn(10);

            placeMap.roomList[r.roomNumber].AssignOpenFurn(9);

            placeMap.roomList[r.roomNumber].AssignOpenFurn(0);

            //Arcade Cabinet
            for (int i = 0; i < r.tileList.Count; i++)
            {
                //Make this number n + 1 according to number of options
                switch (Random.Range(0, 3))
                {
                    case 0:
                        //Arcade Machine 1
                        placeMap.roomList[r.roomNumber].AssignOpenFurn(2);
                        break;
                    case 1:
                        //Arcade Machine 2
                        placeMap.roomList[r.roomNumber].AssignOpenFurn(3);
                        break;
                    case 2:
                        //Arcade Machine 3
                        placeMap.roomList[r.roomNumber].AssignOpenFurn(4);
                        break;
                }
            }

            //This places the rest of the furniture to fill out the room
            for (int i = 0; i < 50; i++)
            {
                //Make this number n + 1 according to number of options
                switch (Random.Range(0, 2))
                {
                    case 0:
                        //Table
                        placeMap.roomList[r.roomNumber].AssignOpenFurn(5);
                        break;
                    case 1:
                        //Trash Cans
                        placeMap.roomList[r.roomNumber].AssignOpenFurn(6);
                        break;
                }
            }
        }
    }
    private void AssignParts(Map placeMap)
    {
        GameController gameCont = GetComponent<GameController>();

        //Gets the easter egg step list
        bool[] eeList = gameCont.activeEE;

        List<List<int>> partRooms = new List<List<int>>();

        //Adding all the lists which can be changed later, index corresponds to part Array indexes
        partRooms.Add(new List<int>());
        partRooms.Add(new List<int>());
        partRooms.Add(new List<int>());
        partRooms.Add(new List<int>());

        //Decides if each piece should be placed and which room number they sould be placed in

        //Basic Collect Parts: 3
        if (eeList[0])
        {
            //Runs 3 times and assigns random rooms for each part
            for(int i = 0; i < 5 + placeMap.roomList.Count; i++)
            {
                partRooms[0].Add(Random.Range(0, placeMap.roomList.Count));

                //Adds one place to the game controller
                gameCont.checkList[0].Add(-1);
            }
        }

        //Targets: Depends on room count ~1.5 per room with a minimum of 5 targets
        if (eeList[3])
        {
            //Runs a variable amount of times
            for (int i = 0; i < Mathf.FloorToInt(placeMap.roomList.Count * 1.5f) + 5; i++)
            {
                partRooms[1].Add(Random.Range(0, placeMap.roomList.Count));

                //Adds one place to the game controller
                gameCont.checkList[3].Add(-1);
            }
        }

        //Moving Soul Box: 1, but chooses starting room
        if (eeList[4])
        {
            partRooms[2].Add(Random.Range(0, placeMap.roomList.Count));

            //Adds the amount of moves to the game controller
            gameCont.checkList[4].Add(4);
        }

        //Hide and Seek: 1, but chooses starting room
        if (eeList[5])
        {
            partRooms[3].Add(Random.Range(0, placeMap.roomList.Count));

            //Adds one place to the game controller
            gameCont.checkList[5].Add(-1);
        }

        //Assigns the parts to the rooms
        for(int i = 0; i < partRooms.Count; i++)
        {
            for(int j = 0; j < partRooms[i].Count; j++)
            {
                int partIndex = -1;

                //Converts the indexes into the ee part index
                switch (i)
                {
                    case 0:
                        partIndex = gameCont.GetIndex(0);
                        break;
                    case 1:
                        partIndex = gameCont.GetIndex(3);
                        break;
                    case 2:
                        partIndex = gameCont.GetIndex(4);
                        break;
                    case 3:
                        partIndex = gameCont.GetIndex(5);
                        break;
                }

                Vector2 v = placeMap.roomList[partRooms[i][j]].AssignOpenPart(i);
                placeMap.roomList[partRooms[i][j]].tileList[(int)v.x].partArray[(int)v.y].partIndex = partIndex;

                //Sets the sub index array for the moving soul box
                if(i == 2)
                    placeMap.roomList[partRooms[i][j]].tileList[(int)v.x].partArray[(int)v.y].addData = new int[] { 12 + placeMap.roomList.Count / 3 };
            }
        }
    }

    private void PlaceMap(Map placeMap)
    {
        GameObject mapParent = new GameObject("Map");

        //Cycles through each room in the given map
        for (int i = 0; i < placeMap.roomList.Count; i++)
        {
            //Creates an object to represent the room and hold the other gameObjects
            GameObject roomObject = new GameObject();
            roomObject.transform.parent = mapParent.transform;
            roomObject.name = "Room " + placeMap.roomList[i].roomNumber;

            placeMap.roomList[i].roomObject = roomObject;

            //Places Tiles
            for (int j = 0; j < placeMap.roomList[i].tileList.Count; j++)
            {
                //Places floors
                GameObject floor = Instantiate(floorObject, placeMap.roomList[i].roomObject.transform);
                floor.transform.position = new Vector3(placeMap.roomList[i].tileList[j].position.x * placeMap.tileSize, 0, placeMap.roomList[i].tileList[j].position.y * placeMap.tileSize);
                floor.transform.localScale = new Vector3(placeMap.tileSize, 0.1f, placeMap.tileSize);
                floor.name = "Room " + placeMap.roomList[i].roomNumber + " Floor (" + placeMap.roomList[i].tileList[j].position.x + "," + placeMap.roomList[i].tileList[j].position.y + ")";

                //Instantiates the ceiling and places it under the floor
                GameObject ceiling = Instantiate(ceilingObject, floor.transform);
                ceiling.transform.localPosition = new Vector3(0, (wallHeight / 2 - 0.1f) *  10, 0);
                ceiling.name = "Ceiling (" + placeMap.roomList[i].tileList[j].position.x + "," + placeMap.roomList[i].tileList[j].position.y + ")";

                placeMap.roomList[i].tileList[j].tileObject = floor;

                //Runs through the potential walls in each tile
                for (int k = 0; k < 4; k++)
                {
                    //The Room at index i, the tile at index j, the type at index k
                    int type = placeMap.roomList[i].tileList[j].borderTypeArray[k];

                    //Takes the wall neight and normalizes it for later use
                    float realWallHeight = placeMap.wallHeight * (1 / placeMap.roomList[i].tileList[j].tileObject.transform.localScale.y / 2);

                    /*This switch takes the type which is stored in the borderTypeArray and tells the game what to do with it
                     * 0: Places a wall 
                    * 1: Places a door
                    * 2: Places a door frame
                    * -1: Places nothing
                    * -2: Places nothing, but links to a previously placed door
                    * -3: Places nothing, but links to a previously placed wall
                    */
                    switch (type)
                    {
                        case 0:
                            #region Wall Placing
                            //Placing a wall
                            GameObject wall = Instantiate(wallObject, placeMap.roomList[i].tileList[j].tileObject.transform);

                            switch (k)
                            {
                                case 0:
                                    wall.transform.localPosition = new Vector3(0, realWallHeight / 2 - 0.5f, 0.5f);
                                    wall.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
                                    break;
                                case 1:
                                    wall.transform.localPosition = new Vector3(0, realWallHeight / 2 - 0.5f, -0.5f);
                                    wall.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
                                    break;
                                case 2:
                                    wall.transform.localPosition = new Vector3(0.5f, realWallHeight / 2 - 0.5f, 0);
                                    wall.transform.localRotation = Quaternion.Euler(new Vector3(0, 90, 0));
                                    break;
                                case 3:
                                    wall.transform.localPosition = new Vector3(-0.5f, realWallHeight / 2 - 0.5f, 0);
                                    wall.transform.localRotation = Quaternion.Euler(new Vector3(0, 270, 0));
                                    break;
                            }

                            wall.transform.localScale = new Vector3(1f, realWallHeight, 0.1f / placeMap.tileSize);
                            wall.name = "Wall (" + placeMap.roomList[i].tileList[j].position.x + "," + placeMap.roomList[i].tileList[j].position.y + ")";

                            placeMap.roomList[i].tileList[j].borderObjectArray[k] = wall;
                            #endregion
                            break;
                        case 1:
                            #region Door Placing
                            //Placing a door
                            GameObject door = Instantiate(doorObject, placeMap.roomList[i].tileList[j].tileObject.transform);
                            door.transform.localScale = new Vector3(1, realWallHeight, 0.1f / placeMap.tileSize);
                            door.name = "Door (" + placeMap.roomList[i].tileList[j].position.x + "," + placeMap.roomList[i].tileList[j].position.y + ")";
                  
                            //Creates a detector which will dictate which room the player is in
                            GameObject detector = Instantiate(roomDetectorObject, placeMap.roomList[i].tileList[j].tileObject.transform);
                            detector.GetComponentInChildren<RoomDetectorController>().roomNumber = placeMap.roomList[i].roomNumber;
                            detector.transform.localScale = new Vector3(1, realWallHeight, 0.1f / placeMap.tileSize);
                            detector.name = "Detector (" + placeMap.roomList[i].tileList[j].position.x + "," + placeMap.roomList[i].tileList[j].position.y + ")";

                            switch (k)
                            {
                                case 0:
                                    door.transform.localPosition = new Vector3(0, realWallHeight / 2 - 0.5f, 0.5f);
                                    door.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));

                                    detector.transform.localPosition = new Vector3(0, realWallHeight / 2 - 0.5f, 0.48f);
                                    detector.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
                                    break;
                                case 1:
                                    door.transform.localPosition = new Vector3(0, realWallHeight / 2 - 0.5f, -0.5f);
                                    door.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));

                                    detector.transform.localPosition = new Vector3(0, realWallHeight / 2 - 0.5f, -0.48f);
                                    detector.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
                                    break;
                                case 2:
                                    door.transform.localPosition = new Vector3(0.5f, realWallHeight / 2 - 0.5f, 0);
                                    door.transform.localRotation = Quaternion.Euler(new Vector3(0, 90, 0));

                                    detector.transform.localPosition = new Vector3(0.48f, realWallHeight / 2 - 0.5f, 0);
                                    detector.transform.localRotation = Quaternion.Euler(new Vector3(0, 90, 0));
                                    break;
                                case 3:
                                    door.transform.localPosition = new Vector3(-0.5f, realWallHeight / 2 - 0.5f, 0);
                                    door.transform.localRotation = Quaternion.Euler(new Vector3(0, 270, 0));

                                    detector.transform.localPosition = new Vector3(-0.48f, realWallHeight / 2 - 0.5f, 0);
                                    detector.transform.localRotation = Quaternion.Euler(new Vector3(0, 90, 0));
                                    break;
                            }

                            foreach (Spawner s in placeMap.roomList[i].spawnerList)
                            {
                                door.GetComponentInChildren<DoorController>().connectedObjects.Add(s.spawnerObject);
                            }

                            door.GetComponentInChildren<DoorController>().PrepareDoor(placeMap.roomList[i].GetAverageDistance());
                            placeMap.roomList[i].tileList[j].borderObjectArray[k] = door;
                            #endregion
                            break;
                        case 2:
                            #region Door Frame Placing
                            //Placing a door
                            GameObject doorFrame = Instantiate(doorFrameObject, placeMap.roomList[i].tileList[j].tileObject.transform);
                            doorFrame.transform.localScale = new Vector3(1, realWallHeight, 0.1f / placeMap.tileSize);
                            doorFrame.name = "Door Frame (" + placeMap.roomList[i].tileList[j].position.x + "," + placeMap.roomList[i].tileList[j].position.y + ")";

                            //Creates a detector which will dictate which room the player is in
                            GameObject detectorFrame = Instantiate(roomDetectorObject, placeMap.roomList[i].tileList[j].tileObject.transform);
                            detectorFrame.GetComponentInChildren<RoomDetectorController>().roomNumber = placeMap.roomList[i].roomNumber;
                            detectorFrame.transform.localScale = new Vector3(1, realWallHeight, 0.1f / placeMap.tileSize);
                            detectorFrame.name = "Detector (" + placeMap.roomList[i].tileList[j].position.x + "," + placeMap.roomList[i].tileList[j].position.y + ")";

                            switch (k)
                            {
                                case 0:
                                    doorFrame.transform.localPosition = new Vector3(0, realWallHeight / 2 - 0.5f, 0.5f);
                                    doorFrame.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));

                                    detectorFrame.transform.localPosition = new Vector3(0, realWallHeight / 2 - 0.5f, 0.48f);
                                    detectorFrame.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
                                    break;
                                case 1:
                                    doorFrame.transform.localPosition = new Vector3(0, realWallHeight / 2 - 0.5f, -0.5f);
                                    doorFrame.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));

                                    detectorFrame.transform.localPosition = new Vector3(0, realWallHeight / 2 - 0.5f, -0.48f);
                                    detectorFrame.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
                                    break;
                                case 2:
                                    doorFrame.transform.localPosition = new Vector3(0.5f, realWallHeight / 2 - 0.5f, 0);
                                    doorFrame.transform.localRotation = Quaternion.Euler(new Vector3(0, 90, 0));

                                    detectorFrame.transform.localPosition = new Vector3(0.48f, realWallHeight / 2 - 0.5f, 0);
                                    detectorFrame.transform.localRotation = Quaternion.Euler(new Vector3(0, 90, 0));
                                    break;
                                case 3:
                                    doorFrame.transform.localPosition = new Vector3(-0.5f, realWallHeight / 2 - 0.5f, 0);
                                    doorFrame.transform.localRotation = Quaternion.Euler(new Vector3(0, 270, 0));

                                    detectorFrame.transform.localPosition = new Vector3(-0.48f, realWallHeight / 2 - 0.5f, 0);
                                    detectorFrame.transform.localRotation = Quaternion.Euler(new Vector3(0, 90, 0));
                                    break;
                            }

                            placeMap.roomList[i].tileList[j].borderObjectArray[k] = doorFrame;
                            #endregion
                            break;
                        case -2:
                            #region Door Linking
                            //Creates a detector which will dictate which room the player is in
                            GameObject detector2 = Instantiate(roomDetectorObject, placeMap.roomList[i].tileList[j].tileObject.transform);
                            detector2.GetComponentInChildren<RoomDetectorController>().roomNumber = placeMap.roomList[i].roomNumber;
                            detector2.transform.localScale = new Vector3(1, realWallHeight, 0.1f / placeMap.tileSize);
                            detector2.name = "Detector (" + placeMap.roomList[i].tileList[j].position.x + "," + placeMap.roomList[i].tileList[j].position.y + ")";

                            int directionIndex = -1;
                            Vector2 position = Vector2.zero;
                            List<GameObject> spawnerList = new List<GameObject>();

                            switch (k)
                            {
                                case 0:
                                    directionIndex = 1;
                                    position = placeMap.roomList[i].tileList[j].position + Vector2.up;

                                    detector2.transform.localPosition = new Vector3(0, realWallHeight / 2 - 0.5f, 0.48f);
                                    detector2.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
                                    break;
                                case 1:
                                    directionIndex = 0;
                                    position = placeMap.roomList[i].tileList[j].position + Vector2.down;

                                    detector2.transform.localPosition = new Vector3(0, realWallHeight / 2 - 0.5f, -0.48f);
                                    detector2.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
                                    break;
                                case 2:
                                    directionIndex = 3;
                                    position = placeMap.roomList[i].tileList[j].position + Vector2.right;

                                    detector2.transform.localPosition = new Vector3(0.48f, realWallHeight / 2 - 0.5f, 0);
                                    detector2.transform.localRotation = Quaternion.Euler(new Vector3(0, 90, 0));
                                    break;
                                case 3:
                                    directionIndex = 2;
                                    position = placeMap.roomList[i].tileList[j].position + Vector2.left;

                                    detector2.transform.localPosition = new Vector3(-0.48f, realWallHeight / 2 - 0.5f, 0);
                                    detector2.transform.localRotation = Quaternion.Euler(new Vector3(0, 90, 0));
                                    break;
                            }

                            foreach (Spawner s in placeMap.roomList[i].spawnerList)
                            {
                                spawnerList.Add(s.spawnerObject);
                            }

                            placeMap.GetDoor(position, directionIndex).LinkObjectList(spawnerList);
                            #endregion
                            break;
                        
                    }
                }

                //Runs through the potential furniture in each tile 
                for (int k = 0; k < 4; k++)
                {
                    //The Room at index i, the tile at index j, the type at index k
                    int type = placeMap.roomList[i].tileList[j].furnArray[k].type;
                    Transform parentObject = placeMap.roomList[i].tileList[j].tileObject.transform;

                    Vector3 pos = Vector3.zero;
                    Vector3 rot = Vector3.zero;

                    /*This switch takes the type which is stored in the borderTypeArray and tells the game what to do with it
                     * -1 / -2: Places Nothing
                     * 0: Places a crafting table
                     * 1: Places a vending machine
                    */

                    //dictates direction for object to be placed in
                    switch (k)
                    {
                        //Figure out why this is different from wall placement later
                        case 0:
                            pos = new Vector3(0, 0, 0.5f);
                            rot = new Vector3(0, 180, 0);
                            break;
                        case 1:
                            pos = new Vector3(0, 0, -0.5f);
                            break;
                        case 2:
                            pos = new Vector3(0.5f, 0, 0);
                            rot = new Vector3(0, -90, 0);
                            break;
                        case 3:
                            pos = new Vector3(-0.5f, 0, 0);
                            rot = new Vector3(0, 90, 0);
                            break;

                    }

                    GameObject obj = null;

                    //Decides which object to place
                    switch (type)
                    {
                        case 0:
                            //Crafting Table
                            obj = Instantiate(craftingTable, parentObject);
                            obj.name = "Crafting Table Room " + placeMap.roomList[i].roomNumber + " (" + placeMap.roomList[i].tileList[j].position.x + "," + placeMap.roomList[i].tileList[j].position.y + ")";
                            break;
                        case 1:
                            //Vending Machine
                            obj = Instantiate(vendingMachine, parentObject);
                            obj.name = "Vending Machine: Room " + placeMap.roomList[i].roomNumber + " (" + placeMap.roomList[i].tileList[j].position.x + "," + placeMap.roomList[i].tileList[j].position.y + ")";
                            obj.GetComponent<VendingMachineController>().Activate(placeMap.roomList[i].GetAverageDistance(), placeMap.roomList[i].tileList[j].furnArray[k].furnIndex);
                            StaticBatchingUtility.Combine(obj);
                            break;
                        case 2:
                            //Arcade Machine
                            obj = Instantiate(arcadeMachine1, parentObject);
                            obj.name = "Arcade Machine: Room " + placeMap.roomList[i].roomNumber + " (" + placeMap.roomList[i].tileList[j].position.x + "," + placeMap.roomList[i].tileList[j].position.y + ")";
                            placeMap.roomList[i].tileList[j].furnArray[k].furnIndex = obj.GetComponent<ArcadeMachineController>().Initialize(placeMap.roomList[i].tileList[j].furnArray[k].furnIndex);
                            placeMap.roomList[i].tileList[j].furnArray[k].subIndexList = obj.GetComponent<ArcadeMachineController>().GetSubIndexArray(placeMap.roomList[i].tileList[j].furnArray[k].subIndexList);
                            StaticBatchingUtility.Combine(obj);
                            break;
                        case 3:
                            //Arcade Machine 2
                            obj = Instantiate(arcadeMachine2, parentObject);
                            obj.name = "Arcade Machine: Room " + placeMap.roomList[i].roomNumber + " (" + placeMap.roomList[i].tileList[j].position.x + "," + placeMap.roomList[i].tileList[j].position.y + ")";
                            placeMap.roomList[i].tileList[j].furnArray[k].furnIndex = obj.GetComponent<ArcadeMachineController>().Initialize(placeMap.roomList[i].tileList[j].furnArray[k].furnIndex);
                            placeMap.roomList[i].tileList[j].furnArray[k].subIndexList = obj.GetComponent<ArcadeMachineController>().GetSubIndexArray(placeMap.roomList[i].tileList[j].furnArray[k].subIndexList);
                            StaticBatchingUtility.Combine(obj);
                            break;
                        case 4:
                            //Arcade Machine 3
                            obj = Instantiate(arcadeMachine3, parentObject);
                            obj.name = "Arcade Machine: Room " + placeMap.roomList[i].roomNumber + " (" + placeMap.roomList[i].tileList[j].position.x + "," + placeMap.roomList[i].tileList[j].position.y + ")";
                            placeMap.roomList[i].tileList[j].furnArray[k].furnIndex = obj.GetComponent<ArcadeMachineController>().Initialize(placeMap.roomList[i].tileList[j].furnArray[k].furnIndex);
                            placeMap.roomList[i].tileList[j].furnArray[k].subIndexList = obj.GetComponent<ArcadeMachineController>().GetSubIndexArray(placeMap.roomList[i].tileList[j].furnArray[k].subIndexList);
                            StaticBatchingUtility.Combine(obj);
                            break;
                        case 5:
                            //Tables
                            obj = Instantiate(tables, parentObject);
                            obj.name = "Table: Room " + placeMap.roomList[i].roomNumber + " (" + placeMap.roomList[i].tileList[j].position.x + "," + placeMap.roomList[i].tileList[j].position.y + ")";
                            StaticBatchingUtility.Combine(obj);
                            break;
                        case 6:
                            //Trash Cans
                            obj = Instantiate(trashCan, parentObject);
                            obj.name = "Trash Can: Room " + placeMap.roomList[i].roomNumber + " (" + placeMap.roomList[i].tileList[j].position.x + "," + placeMap.roomList[i].tileList[j].position.y + ")";
                            StaticBatchingUtility.Combine(obj);
                            break;
                        case 7:
                            //Easter Egg Soul Box
                            obj = Instantiate(eeSoulBox, parentObject);
                            obj.name = "Easter Egg Soul Box: Room " + placeMap.roomList[i].roomNumber + " (" + placeMap.roomList[i].tileList[j].position.x + "," + placeMap.roomList[i].tileList[j].position.y + ")";
                            obj.GetComponent<EESoulBoxController>().Initialize(new Vector3(i, j, k), placeMap.roomList[i].tileList[j].furnArray[k].addData[0], placeMap.roomList[i].tileList[j].furnArray[k].furnIndex);
                            break;
                        case 8:
                            //Easter Egg Wall Code
                            obj = Instantiate(eeWallCode, parentObject);
                            obj.name = "Easter Egg Code: Room " + placeMap.roomList[i].roomNumber + " (" + placeMap.roomList[i].tileList[j].position.x + "," + placeMap.roomList[i].tileList[j].position.y + ")";
                            obj.GetComponent<EECodeController>().Initialize(new Vector3(i, j, k), placeMap.roomList[i].tileList[j].furnArray[k].furnIndex);
                            break;
                        case 9:
                            //Soda Seller
                            obj = Instantiate(sodaSeller, parentObject);
                            obj.name = "Soda Seller: Room " + placeMap.roomList[i].roomNumber + " (" + placeMap.roomList[i].tileList[j].position.x + "," + placeMap.roomList[i].tileList[j].position.y + ")";
                            break;
                        case 10:
                            //Exit Object
                            obj = Instantiate(eeExitObject, parentObject);
                            obj.name = "Easter Egg Exit: Room " + placeMap.roomList[i].roomNumber + " (" + placeMap.roomList[i].tileList[j].position.x + "," + placeMap.roomList[i].tileList[j].position.y + ")";
                            break;
                    }

                    if(type >= 0)
                    {
                        obj.transform.localScale = new Vector3(1 / parentObject.localScale.x, 1 / parentObject.localScale.y, 1 / parentObject.localScale.z);
                        obj.transform.localPosition = pos;
                        obj.transform.localRotation = Quaternion.Euler(rot);
                    }

                    //Assigns the object to the array in the tile for referencing later
                    placeMap.roomList[i].tileList[j].furnObjectArray[k] = obj;
                }

                //Runs through the potential parts in each tile 
                for (int k = 0; k < 4; k++)
                {
                    //The Room at index i, the tile at index j, the type at index k
                    int type = placeMap.roomList[i].tileList[j].partArray[k].type;
                    int index = placeMap.roomList[i].tileList[j].partArray[k].posIndex;

                    //The Default
                    Transform parentObject = placeMap.roomList[i].tileList[j].tileObject.transform;

                    Vector3 pos = Vector3.zero;
                    Vector3 rot = Vector3.zero;

                    /*This switch takes the type which is stored in the borderTypeArray and tells the game what to do with it
                     * -1 / -2: Places Nothing
                     * 0: Piece
                     * 1: Target
                     * 2: Moving Soul Box
                     * 3: Hide and Seek
                    */

                    //Checks if there is any furniture in the space, if so changes where the part will be placed
                    int furnNum = placeMap.roomList[i].tileList[j].furnArray[k].type;

                    /*Assigns a random index based on furniture if:
                     *  The type is one which would need a part index a.k.a. not -1 or -2
                     *  There is furniture in the spot
                    */
                    if (type >= 0 && furnNum > -1)
                    {
                        if(index == -1)
                        {
                            //Checks if the index is not present, but a part is
                            placeMap.roomList[i].tileList[j].partArray[k].posIndex = placeMap.roomList[i].tileList[j].furnObjectArray[k].GetComponent<FurnitureController>().GetRandomPosition(type);
                        }

                        index = placeMap.roomList[i].tileList[j].partArray[k].posIndex;
                        parentObject = placeMap.roomList[i].tileList[j].furnObjectArray[k].GetComponent<FurnitureController>().transform;
                    }
                    else
                    {
                        //dictates direction for object to be placed in
                        switch (k)
                        {
                            //Figure out why this is different from wall placement later
                            case 0:
                                pos = new Vector3(0, 0, 0.5f);
                                rot = new Vector3(0, 180, 0);
                                break;
                            case 1:
                                pos = new Vector3(0, 0, -0.5f);
                                break;
                            case 2:
                                pos = new Vector3(0.5f, 0, 0);
                                rot = new Vector3(0, -90, 0);
                                break;
                            case 3:
                                pos = new Vector3(-0.5f, 0, 0);
                                rot = new Vector3(0, 90, 0);
                                break;
                        }
                    }

                    GameObject obj = null;

                    //Decides which object to place
                    switch (type)
                    {
                        case 0:
                            //Basic Part
                            obj = Instantiate(eePart, parentObject);
                            obj.name = "Easter Egg Part " + placeMap.roomList[i].roomNumber + " (" + placeMap.roomList[i].tileList[j].position.x + "," + placeMap.roomList[i].tileList[j].position.y + ")";
                            obj.GetComponent<EEPieceController>().Initialize(new Vector3(i,j,k), placeMap.roomList[i].tileList[j].partArray[k].partIndex);
                            break;
                        case 1:
                            //Target
                            obj = Instantiate(eeTarget, parentObject);
                            obj.name = "Easter Egg Target " + placeMap.roomList[i].roomNumber + " (" + placeMap.roomList[i].tileList[j].position.x + "," + placeMap.roomList[i].tileList[j].position.y + ")";
                            obj.GetComponent<EETargetController>().Initialize(new Vector3(i, j, k), placeMap.roomList[i].tileList[j].partArray[k].partIndex);
                            break;
                        case 2:
                            //Moving Soul Box
                            obj = Instantiate(eeSoulBoxMove, parentObject);
                            obj.name = "Easter Egg Soul Box Move " + placeMap.roomList[i].roomNumber + " (" + placeMap.roomList[i].tileList[j].position.x + "," + placeMap.roomList[i].tileList[j].position.y + ")";
                            obj.GetComponent<EESoulBoxController>().Initialize(new Vector3(i, j, k), placeMap.roomList[i].tileList[j].partArray[k].addData[0], placeMap.roomList[i].tileList[j].partArray[k].partIndex);
                            break;
                        case 3:
                            //Hide and Seek
                            obj = Instantiate(eeHideAndSeek, parentObject);
                            obj.name = "Easter Egg Hide and Seek " + placeMap.roomList[i].roomNumber + " (" + placeMap.roomList[i].tileList[j].position.x + "," + placeMap.roomList[i].tileList[j].position.y + ")";
                            obj.GetComponent<EEHSController>().Initialize(new Vector3(i, j, k), placeMap.roomList.Count / 2 + 3, placeMap.roomList[i].tileList[j].partArray[k].partIndex);
                            break;
                    }

                    if (type >= 0)
                    {
                        if(index != -1)
                        {
                            GameObject locObj = placeMap.roomList[i].tileList[j].furnObjectArray[k].GetComponent<FurnitureController>().GetObjectPosition(type, index);

                            obj.transform.position = locObj.transform.position;
                            obj.transform.rotation = locObj.transform.rotation;
                        }
                        else
                        {
                            obj.transform.localScale = new Vector3(1 / parentObject.localScale.x, 1 / parentObject.localScale.y, 1 / parentObject.localScale.z);
                            obj.transform.localPosition = pos;
                            obj.transform.localRotation = Quaternion.Euler(rot);
                        }
                    }

                    
                }

                //Places the spawner if there is one
                if (placeMap.roomList[i].tileList[j].spawner != null)
                {
                    GameObject spawner = Instantiate(spawnerObject, floor.transform);
                    spawner.name = "Spawner (" + (int)placeMap.roomList[i].tileList[j].position.x + "," + (int)placeMap.roomList[i].tileList[j].position.y + ")";
                    spawner.transform.localPosition = placeMap.roomList[i].tileList[j].spawner.spawnerLocation;
                    spawner.transform.localScale = new Vector3(1 / floor.transform.localScale.x, 1 / floor.transform.localScale.y, 1 / floor.transform.localScale.z);

                    //Adds the new object to the spawner
                    placeMap.roomList[i].tileList[j].spawner.spawnerObject = spawner;

                    //Activates the spawner if in the first room
                    if (placeMap.roomList[i].IsOpen() || i == 0)
                    {
                        spawner.GetComponent<EnemySpawnController>().Activate(true);
                    }
                }

                //Sets the objects to be used with the minimap
                placeMap.roomList[i].tileList[j].SetMinimapActive();
            }
        }
    }
    private Map BuildMapLayout(Map newMap)
    {
        Tile[,] tileArray = new Tile[newMap.mapSize, newMap.mapSize];

        foreach (Room room in newMap.roomList)
        {
            foreach(Tile tile in room.tileList)
            {
                tileArray[(int)tile.position.x, (int)tile.position.y] = tile;
            }
        }

        newMap.mapLayout = tileArray;
        return newMap;
    }

    #region Get Methods
    private Vector2 GetDirection(Vector2 pos, Tile[,] tileMap)
    {
        List<Vector2> directions = new List<Vector2>(); 

        if ((int)pos.x + 1 < map.mapSize && tileMap[(int)pos.x + 1, (int)pos.y] == null)
        {
            //Denotes the RIGHT space as unusable
            directions.Add(Vector2.right);
        }
        if ((int)pos.x - 1 > 0 && tileMap[(int)pos.x - 1, (int)pos.y] == null)
        {
            //Denotes the LEFT space as unusable
            directions.Add(Vector2.left);
        }
        if ((int)pos.y + 1 < map.mapSize && tileMap[(int)pos.x, (int)pos.y + 1] == null)
        {
            //Denotes the UP space as unusable
            directions.Add(Vector2.up);
        }
        if ((int)pos.y - 1 > 0 && tileMap[(int)pos.x, (int)pos.y - 1] == null)
        {
            //Denotes the DOWN space as unusable
            directions.Add(Vector2.down);
        }

        if(directions.Count > 0)
        {
            return directions[Random.Range(0, directions.Count)];
        }
        else
        {
            return Vector2.zero;
        }
        
    }
    private Vector2 GetDirection(Vector2 pos, Tile[,] currentTileMap, Tile[,] fullTileMap)
    {
        List<Vector2> directions = new List<Vector2>();

        if (pos.x + 1 < map.mapSize && (currentTileMap[(int)pos.x + 1, (int)pos.y] == null && fullTileMap[(int)pos.x + 1, (int)pos.y] == null))
        {
            //Denotes the RIGHT space as unusable
            directions.Add(Vector2.right);
        }
        if (pos.x - 1 > 0 && (currentTileMap[(int)pos.x - 1, (int)pos.y] == null && fullTileMap[(int)pos.x - 1, (int)pos.y] == null))
        {
            //Denotes the LEFT space as unusable
            directions.Add(Vector2.left);
        }
        if (pos.y + 1 < map.mapSize && (currentTileMap[(int)pos.x, (int)pos.y + 1] == null && fullTileMap[(int)pos.x, (int)pos.y + 1] == null))
        {
            //Denotes the UP space as unusable
            directions.Add(Vector2.up);
        }
        if (pos.y - 1 > 0 && (currentTileMap[(int)pos.x, (int)pos.y - 1] == null && fullTileMap[(int)pos.x, (int)pos.y - 1] == null))
        {
            //Denotes the DOWN space as unusable
            directions.Add(Vector2.down);
        }

        if (directions.Count > 0)
        {
            return directions[Random.Range(0, directions.Count)];
        }
        else
        {
            return Vector2.zero;
        }

    }
    private Vector2 GetRandomTile(Tile[,] tileMap)
    {
        List<Vector2> coordinateList = new List<Vector2>();

        for(int i = 0; i < map.mapSize; i++)
        {
            for(int j = 0; j < map.mapSize; j++)
            {
                if (tileMap[i,j] != null)
                {
                    coordinateList.Add(new Vector2(i,j));
                }
            }
        }

        if(coordinateList.Count > 0)
        {
            return coordinateList[Random.Range(0,coordinateList.Count)];
        }
        else
        {
            return new Vector2(-1, -1);
        }
    }
    #endregion

    public void OpenDoor(GameObject doorFrame)
    {
        for(int i = 0; i < map.roomList.Count; i++)
        {
            for(int j = 0; j < map.roomList[i].tileList.Count; j++)
            {
                for(int k = 0; k < 4; k++)
                {
                    if (map.roomList[i].tileList[j].borderObjectArray[k] == doorFrame)
                    {
                        int otherDirection = -1;
                        Vector2 otherRoom = Vector2.zero;

                        switch (k)
                        {

                            case 0:
                                otherDirection = 1;
                                otherRoom = Vector2.up;
                                break;
                            case 1:
                                otherDirection = 0;
                                otherRoom = Vector2.down;
                                break;
                            case 2:
                                otherDirection = 3;
                                otherRoom = Vector2.right;
                                break;
                            case 3:
                                otherDirection = 2;
                                otherRoom = Vector2.left;
                                break;

                        }

                        //Replaces the -2 in the saved map with a 2 because the door no longer exists
                        map.GetTile(map.roomList[i].tileList[j].position + otherRoom).borderTypeArray[otherDirection] = 2;

                        //Turns the door spot into an empty area by turing the 1 into a 2
                        map.roomList[i].tileList[j].borderTypeArray[k] = 2;
                    }
                }
            }
        }
    }
    public void DeRenderRooms(int rNumber)
    {
        map.DeRenderRooms(rNumber);
    }
    public void DeRenderRooms()
    {
        map.DeRenderRooms(map.currentRoom);
    }
    public void SetCurrentRoom(int i)
    {
        map.currentRoom = i;
    }

    public void ReAssignPart(GameObject obj, int partType, int partIndex)
    {
        //Gets the new location for the part
        int i = Random.Range(0, map.roomList.Count);
        Vector2 loc = map.roomList[i].AssignOpenPart(partType);
        map.roomList[i].tileList[(int)loc.x].partArray[(int)loc.y].partIndex = partIndex;

        int j = (int)loc.x;
        int k = (int)loc.y;

        //Checks if the index is not present, but a part is
        map.roomList[i].tileList[j].partArray[k].posIndex = map.roomList[i].tileList[j].furnObjectArray[k].GetComponent<FurnitureController>().GetRandomPosition(partType);

        #region Placing Object

        //The Room at index i, the tile at index j, the type at index k
        int index = map.roomList[i].tileList[j].partArray[k].posIndex;

        //The Default
        Transform parentObject = map.roomList[i].tileList[j].tileObject.transform;

        Vector3 pos = Vector3.zero;
        Vector3 rot = Vector3.zero;

        /*This switch takes the type which is stored in the borderTypeArray and tells the game what to do with it
         * -1 / -2: Places Nothing
         * 0: Piece
         * 1: Target
         * 2: Moving Soul Box
         * 3: Hide and Seek
        */

        //Checks if there is any furniture in the space, if so changes where the part will be placed
        int furnNum = map.roomList[i].tileList[j].furnArray[k].type;

        /*Assigns a random index based on furniture if:
         *  The type is one which would need a part index a.k.a. not -1 or -2
         *  There is furniture in the spot
        */
        if (partType >= 0 && furnNum > -1)
        {
            if (index == -1)
            {
                //Checks if the index is not present, but a part is
                map.roomList[i].tileList[j].partArray[k].posIndex = map.roomList[i].tileList[j].furnObjectArray[k].GetComponent<FurnitureController>().GetRandomPosition(partType);
            }

            index = map.roomList[i].tileList[j].partArray[k].posIndex;
            parentObject = map.roomList[i].tileList[j].furnObjectArray[k].GetComponent<FurnitureController>().transform;
        }
        else
        {
            //dictates direction for object to be placed in
            switch (k)
            {
                //Figure out why this is different from wall placement later
                case 0:
                    pos = new Vector3(0, 0, 0.5f);
                    rot = new Vector3(0, 180, 0);
                    break;
                case 1:
                    pos = new Vector3(0, 0, -0.5f);
                    break;
                case 2:
                    pos = new Vector3(0.5f, 0, 0);
                    rot = new Vector3(0, -90, 0);
                    break;
                case 3:
                    pos = new Vector3(-0.5f, 0, 0);
                    rot = new Vector3(0, 90, 0);
                    break;
            }
        }

        //Decides which object to place
        switch (partType)
        {
            case 0:
                //Basic Part
                obj.transform.parent = parentObject;
                obj.name = "Easter Egg Part " + map.roomList[i].roomNumber + " (" + map.roomList[i].tileList[j].position.x + "," + map.roomList[i].tileList[j].position.y + ")";
                obj.GetComponent<EEPieceController>().Initialize(new Vector3(i, j, k));
                break;
            case 1:
                //Target
                obj.transform.parent = parentObject;
                obj.name = "Easter Egg Target " + map.roomList[i].roomNumber + " (" + map.roomList[i].tileList[j].position.x + "," + map.roomList[i].tileList[j].position.y + ")";
                obj.GetComponent<EETargetController>().Initialize(new Vector3(i, j, k));
                break;
            case 2:
                //Moving Soul Box
                obj.transform.parent = parentObject;
                obj.name = "Easter Egg Soul Box Move " + map.roomList[i].roomNumber + " (" + map.roomList[i].tileList[j].position.x + "," + map.roomList[i].tileList[j].position.y + ")";
                obj.GetComponent<EESoulBoxController>().Initialize(new Vector3(i, j, k));
                break;
            case 3:
                //Hide and Seek
                obj.transform.parent = parentObject;
                obj.name = "Easter Egg Hide and Seek " + map.roomList[i].roomNumber + " (" + map.roomList[i].tileList[j].position.x + "," + map.roomList[i].tileList[j].position.y + ")";
                obj.GetComponent<EEHSController>().Initialize(new Vector3(i, j, k));
                break;
        }

        if (partType >= 0)
        {
            if (index != -1)
            {
                GameObject locObj = map.roomList[i].tileList[j].furnObjectArray[k].GetComponent<FurnitureController>().GetObjectPosition(partType, index);

                obj.transform.position = locObj.transform.position;
                obj.transform.rotation = locObj.transform.rotation;
            }
            else
            {
                obj.transform.localScale = new Vector3(1 / parentObject.localScale.x, 1 / parentObject.localScale.y, 1 / parentObject.localScale.z);
                obj.transform.localPosition = pos;
                obj.transform.localRotation = Quaternion.Euler(rot);
            }
        }
        #endregion
    }
}
