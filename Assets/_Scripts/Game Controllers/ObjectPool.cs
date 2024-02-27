using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool objPool;

    private PlayerController player;

    public List<GameObject> objectsToPool;
    public List<int> poolAmount;

    //This is a list of lists of gameobjects which have been pooled
    public List<List<GameObject>> pooledObjects;

    #region Initialization
    public void SetStaticInstance()
    {
        //Creates a singelton
        if (objPool != null && objPool != this)
        {
            Destroy(this);
        }
        else
        {
            objPool = this;
        }
    }
    public void Initialize()
    {
        //Gets the mapGeneration Controller
        player = FindObjectOfType<PlayerController>();

        //Clears the enemy list before loading
        Enemy.enemyList = new List<Enemy>();

        pooledObjects = new List<List<GameObject>>();

        for (int i = 0; i < objectsToPool.Count; i++)
        {
            pooledObjects.Add(new List<GameObject>());
        }

        pooledObjects[0] = new List<GameObject>();

        GameObject temp;

        //Runs through all items in the pooledObjects 
        for (int i = 0; i < objectsToPool.Count; i++)
        {
            GameObject parent = new GameObject(objectsToPool[i].name.Split('(')[0] + " List");
            parent.transform.parent = transform;

            if (objectsToPool[i].name == "Basic Zombie" && ValueStoreController.loadData)
            {

                switch (objectsToPool[i].name)
                {

                    case "Basic Zombie":
                        List<EnemyData> enemyDataList = ValueStoreController.loadedGameData.enemyData;

                        for (int j = 0; j < poolAmount[i]; j++)
                        {
                            temp = Instantiate(objectsToPool[i], player.transform.position, Quaternion.Euler(Vector3.zero), parent.transform);
                            temp.SendMessage("Initialize", SendMessageOptions.DontRequireReceiver);

                            if (j < enemyDataList.Count)
                            {
                                temp.GetComponent<Enemy>().SetEnemyData(enemyDataList[j]);
                            }
                            else
                            {
                                temp.SetActive(false);
                            }

                            pooledObjects[i].Add(temp);
                        }
                        break;
                }
                
            }
            else
            {
                for (int j = 0; j < poolAmount[i]; j++)
                {
                    temp = Instantiate(objectsToPool[i], player.transform.position, Quaternion.Euler(Vector3.zero), parent.transform);
                    temp.SendMessage("Initialize", SendMessageOptions.DontRequireReceiver);
                    temp.SetActive(false);
                    pooledObjects[i].Add(temp);
                }
            }
        }
    }
    #endregion
    public GameObject GetPooledObject(GameObject obj)
    {
        int objIndex = GetObjectIndex(obj);

        for(int i = 0; i < poolAmount[objIndex]; i++)
        {
            //Checks whether that object is being used
            if (!pooledObjects[objIndex][i].activeInHierarchy)
            {
                return pooledObjects[objIndex][i];
            }
        }
        return null;
    }
    public GameObject GetPooledObject(string name)
    {
        GameObject obj = null;

        for(int i = 0; i < objectsToPool.Count; i++)
        {
            if (objectsToPool[i].name == name)
            {
                obj = objectsToPool[i];
            }
        }

        int objIndex = GetObjectIndex(obj);

        for (int i = 0; i < poolAmount[objIndex]; i++)
        {
            //Checks whether that object is being used
            if (!pooledObjects[objIndex][i].activeInHierarchy)
            {
                return pooledObjects[objIndex][i];
            }
        }
        return null;
    }
    public int GetObjectIndex(GameObject testObj)
    {
        for(int n = 0; n < objectsToPool.Count; n++)
        {
            if (objectsToPool[n] == testObj)
            {
                return n;
            }
        }

        return -1;
    }
}
