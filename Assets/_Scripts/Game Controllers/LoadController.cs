using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadController : MonoBehaviour
{
    public List<GameObject> objectLoadOrder;
    public bool isTutorial = false;

    // Start is called before the first frame update
    private void Start()
    {
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Tutorial Level"))
            ValueStoreController.isTutorial = true;
        else
            ValueStoreController.isTutorial = false;

        //Loads the scenes
        TestSetup();
    }

    #region Load Order Methods

    /*This section describes the order in which things will load within this file
     * 
     * Weapon Part Pool
     * Loot Pool
     * Inventory Controller
     * Map Builder
     * NavMesh Builder
     * Object Pool
     * Pause Controller
     * Interface Controller
     * 
    */

    public void TestSetup()
    {
        foreach(GameObject g in objectLoadOrder)
        {
            g.BroadcastMessage("SetStaticInstance", SendMessageOptions.DontRequireReceiver);
        }

        foreach(GameObject g in objectLoadOrder)
        {
            g.BroadcastMessage("Initialize", SendMessageOptions.DontRequireReceiver);
        }
        FinishLoad();
    }

    public void RunSceneSetup()
    {
        GetComponent<GameController>().Initialize();
        GetComponent<MapGenerationController>().Initialize();
        GetComponent<NavMeshBuildController>().Initialize();
        GetComponent<ObjectPool>().Initialize();
        GetComponent<InterfaceController>().Initialize();
        GetComponent<InventoryController>().Initialize();

        FindObjectOfType<PlayerController>().Initialize();
        FindObjectOfType<CameraController>().Initialize();
        FindObjectOfType<WeaponController>().Initialize();
        FinishLoad();
    }
    private void FinishLoad()
    {
        if (!ValueStoreController.loadData && !isTutorial)
        {
            ValueStoreController.SaveGameData(1);
        }
    }
    #endregion
}
