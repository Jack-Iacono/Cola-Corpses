using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Map
{
    public int mapSize { get; set; }
    public float tileSize { get; set; }
    public float wallHeight { get; set; }
    public Vector3 spawnLocation { get; set; }

    //For loading and rendering
    public int currentRoom { get; set; } = 0;

    public int roomTileCount { get; set; }
    public int mapTileCount { get; set; }
    public List<Room> roomList { get; set; }
    public Tile[,] mapLayout { get; set; }
    public GameObject mapObject { get; set; }

    #region Constructors
    public Map(int newMapSize, float newTileSize, float newWallHeight)
    {
        mapSize = newMapSize;
        tileSize = newTileSize;
        wallHeight = newWallHeight;
        roomList = new List<Room>();

        //Creates a 2D array with all values being null
        mapLayout = new Tile[mapSize, mapSize];
        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                mapLayout[i, j] = null;
            }
        }
    }
    public Map(int newMapSize, float newTileSize, float newWallHeight, int newMapTileCount, int newRoomTileCount)
    {
        mapSize = newMapSize;
        tileSize = newTileSize;
        wallHeight = newWallHeight;
        roomList = new List<Room>();

        mapTileCount = newMapTileCount;
        roomTileCount = newRoomTileCount;

        //Creates a 2D array with all values being null
        mapLayout = new Tile[mapSize, mapSize];
        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                mapLayout[i, j] = null;
            }
        }
    }
    #endregion

    public Tile GetTile(Vector2 pos)
    {
        //Gets the room containing the tile with pos
        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].GetTilePostions().Contains(pos))
            {
                return roomList[i].GetTile(pos);
            }
        }

        return null;
    }
    public Room GetRoom(Vector2 pos)
    {
        //Gets the room containing the tile with pos
        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].GetTilePostions().Contains(pos))
            {
                return roomList[i];
            }
        }

        return null;
    }
    public Room GetRoom(int roomNumber)
    {
        for(int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].roomNumber == roomNumber)
                return roomList[i];
        }
        return null;
    }

    public DoorController GetDoor(Vector2 pos, int directionIndex)
    {
        for(int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].GetDoor(pos, directionIndex))
            {
                return roomList[i].GetDoor(pos, directionIndex);
            }
        }

        return null;
    }
    public List<Room> GetConnectingRooms(Room room)
    {
        List<Room> rooms = new List<Room>();

        //Takes all the tiles in the current room
        foreach(Tile tile in room.tileList)
        {
            //Gets all the doors associated with that tile
            List<int> doors = tile.GetAllDoors();

            //Checks if there are any doors on that tile
            if(doors != null)
            {
                //Runs through all the doors on the tile
                for(int i = 0; i < doors.Count; i++)
                {
                    //Checks the map for other rooms depending on the index of the door
                    switch (doors[i])
                    {

                        case 0:
                            rooms.Add(GetRoom(tile.position + new Vector2(0, 1)));
                            break;
                        case 1:
                            rooms.Add(GetRoom(tile.position + new Vector2(0, -1)));
                            break;
                        case 2:
                            rooms.Add(GetRoom(tile.position + new Vector2(1, 0)));
                            break;
                        case 3:
                            rooms.Add(GetRoom(tile.position + new Vector2(-1, 0)));
                            break;

                    }
                }

            }
        }

        return rooms;

    }

    public void RevealMinimap()
    {
        for(int i = 0; i < roomList.Count; i++)
        {
            roomList[i].RevealMinimap();
        }
    }

    public void DeRenderRooms(int rNumber)
    {
        //Gets the room with room number rNumber
        Room room = GetRoom(rNumber);

        //Gets every room that connects to the current room
        List<Room> rooms = GetConnectingRooms(room);

        //Gets a list of tiles which sould also be rendered due to them having a connection to the other rooms
        List<List<Tile>> tiles = new List<List<Tile>>();
        //Initalizing the tile list
        for(int i = 0; i < roomList.Count; i++)
        {
            tiles.Add(new List<Tile>());
        }

        //Cycles through each connecting room to find walls/tiles that should be excluded from derendering
        foreach(Room r in rooms)
        {
            //Cycles through each tile in the room
            foreach(Tile t in r.tileList)
            {
                //Cycles through each border object in the tiles
                for(int i = 0; i < t.borderTypeArray.Length; i++)
                {
                    //If the borderarray dictates that there is a connecting wall, add the tile to the exception array
                    if (t.borderTypeArray[i] == -3)
                    {
                        switch (i)
                        {
                            case 0:
                                tiles[GetRoom(new Vector2((int)t.position.x, (int)t.position.y + 1)).roomNumber].Add(mapLayout[(int)t.position.x, (int)t.position.y + 1]);
                                break;
                            case 1:
                                tiles[GetRoom(new Vector2((int)t.position.x, (int)t.position.y - 1)).roomNumber].Add(mapLayout[(int)t.position.x, (int)t.position.y - 1]);
                                break;
                            case 2:
                                tiles[GetRoom(new Vector2((int)t.position.x + 1, (int)t.position.y)).roomNumber].Add(mapLayout[(int)t.position.x + 1, (int)t.position.y]);
                                break;
                            case 3:
                                tiles[GetRoom(new Vector2((int)t.position.x - 1, (int)t.position.y)).roomNumber].Add(mapLayout[(int)t.position.x - 1, (int)t.position.y]);
                                break;
                        }
                    }
                }
            }
        }

        //Adds the original room to the list of rooms that will be rendered
        rooms.Add(room);

        for (int i = 0; i < roomList.Count; i++)
        {
            //If the rooms is not in the list, check for tiles that should stay rendered, else render the room
            if (!rooms.Contains(roomList[i]))
            {
                //If the tile array at the index of the room number has a tile in it
                if (tiles[roomList[i].roomNumber].Count != 0)
                {
                    roomList[i].RenderRoom(tiles[roomList[i].roomNumber]);
                }
                else
                {
                    roomList[i].RenderRoom(false);
                }
            }
            else
                roomList[i].RenderRoom(true);
        }

    }
}
