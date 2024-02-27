using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshBuildController : MonoBehaviour
{
    #region GameObjects

    private MapGenerationController mapGenCont;
    public static NavMeshBuildController navMeshBuildCont;
    public NavMeshAgent zombieAgent;

    #endregion

    #region Source Controls

    //General Variables
    Bounds bounds;

    //For Ground NavMesh
    LayerMask groundLayerMask;
    NavMeshCollectGeometry groundGeometry = NavMeshCollectGeometry.RenderMeshes;
    int groundDefaultArea = 0;
    List<NavMeshBuildMarkup> groundMarkup = new List<NavMeshBuildMarkup>();
    List<NavMeshBuildSource> groundSource = new List<NavMeshBuildSource>();

    //For Wall NavMesh
    LayerMask wallLayerMask;
    NavMeshCollectGeometry wallGeometry = NavMeshCollectGeometry.RenderMeshes;
    int wallDefaultArea = 1;
    List<NavMeshBuildMarkup> wallMarkup = new List<NavMeshBuildMarkup>();
    List<NavMeshBuildSource> wallSource = new List<NavMeshBuildSource>();

    //For Spawners
    LayerMask avoidLayerMask;
    NavMeshCollectGeometry avoidGeometry = NavMeshCollectGeometry.RenderMeshes;
    int avoidDefaultArea = 2;
    List<NavMeshBuildMarkup> avoidMarkup = new List<NavMeshBuildMarkup>();
    List<NavMeshBuildSource> avoidSource = new List<NavMeshBuildSource>();

    #endregion

    #region Build Settings

    NavMeshBuildSettings zombieSettings;
    float zombieClimb = 1.25f;
    float zombieHeight = 2f;
    float zombieRadius = 0.6f;
    float zombieSlope = 50;
    uint maxJobWorkers = 0;
    float minRegionArea = 1f;
    int tileSize = 0;
    float voxelSize = 0.16667f;

    #endregion

    #region Initialization
    public void SetStaticInstance()
    {
        //Creates a singelton
        if (navMeshBuildCont != null && navMeshBuildCont != this)
        {
            Destroy(this);
        }
        else
        {
            navMeshBuildCont = this;
        }
    }
    public void Initialize()
    {
        //Get the map controller
        mapGenCont = GetComponent<MapGenerationController>();
        Vector2 mapSize = new Vector2(mapGenCont.map.tileSize * mapGenCont.map.mapSize, mapGenCont.map.tileSize * mapGenCont.map.mapSize);
        bounds = new Bounds(new Vector3(mapSize.x / 2 - mapGenCont.map.tileSize, -5, mapSize.y / 2 - mapGenCont.map.tileSize), new Vector3(mapSize.x, 100, mapSize.y));

        //Finish Initializing Variables
        zombieSettings = NavMesh.CreateSettings();
        zombieSettings.agentClimb = zombieClimb;
        zombieSettings.agentHeight = zombieHeight;
        zombieSettings.agentRadius = zombieRadius;
        zombieSettings.agentSlope = zombieSlope;
        zombieSettings.agentTypeID = zombieAgent.agentTypeID;
        zombieSettings.maxJobWorkers = maxJobWorkers;
        zombieSettings.minRegionArea = minRegionArea;
        zombieSettings.tileSize = tileSize;
        zombieSettings.voxelSize = voxelSize;

        //Gets theareas by name
        groundDefaultArea = NavMesh.GetAreaFromName("Walkable");
        wallDefaultArea = NavMesh.GetAreaFromName("Not Walkable");
        avoidDefaultArea = NavMesh.GetAreaFromName("Slow");

        //Initializing Layer Masks
        groundLayerMask = LayerMask.GetMask("Walkable");
        wallLayerMask = LayerMask.GetMask("Not Walkable");
        avoidLayerMask = LayerMask.GetMask("Avoid");

        //Gets the Sources for the Ground
        NavMeshBuilder.CollectSources(bounds, groundLayerMask, groundGeometry, groundDefaultArea, groundMarkup, groundSource);
        NavMeshBuilder.CollectSources(bounds, wallLayerMask, wallGeometry, wallDefaultArea, wallMarkup, wallSource);
        NavMeshBuilder.CollectSources(bounds, avoidLayerMask, avoidGeometry, avoidDefaultArea, avoidMarkup, avoidSource);

        //Creates the list in which the sources will be combined
        List<NavMeshBuildSource> fullSource = new List<NavMeshBuildSource>();

        //Combines the sources into one list
        foreach(NavMeshBuildSource n in groundSource)
        {
            fullSource.Add(n);
        }

        foreach (NavMeshBuildSource n in wallSource)
        {
            fullSource.Add(n);
        }

        foreach(NavMeshBuildSource n in avoidSource)
        {
            fullSource.Add(n);
        }

        //Gets the data for the mesh
        NavMeshData fullData = NavMeshBuilder.BuildNavMeshData(zombieSettings, fullSource, bounds, Vector3.zero, Quaternion.Euler(Vector3.zero));

        //Wipes the previous NavMesh
        NavMesh.RemoveAllNavMeshData();

        //Actually Adds the NavMesh to the Game
        NavMesh.AddNavMeshData(fullData);

        mapGenCont.DeRenderRooms();
    }
    #endregion

    public int GetAgentID(int i)
    {
        switch (i)
        {

            case 0:
                return zombieSettings.agentTypeID;

        }

        return -1;
    }
    
}
