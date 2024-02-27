using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LootPoolController
{
    public enum LootPool { BASIC_1, BASIC_2, BASIC_3, BASIC_ZOMBIE };
    private static Dictionary<LootPool, LootData> lootPoolDict = new Dictionary<LootPool, LootData>()
    {
        { LootPool.BASIC_1, new LootData(new int[] { 50, 25, 13, 6, 3, 1 }) },
        { LootPool.BASIC_2, new LootData(new int[] { 40, 35, 20, 10, 4, 2 }) },
        { LootPool.BASIC_3, new LootData(new int[] { 50, 30, 10, 15, 3, 3 }) },
        { LootPool.BASIC_ZOMBIE, new LootData(new int[] { 50, 25, 13, 6, 3, 1 }, new SodaPart[] { UniqueItem.supremeFizz }) },
    };

    #region Item Dropping Methods

    public static void CreateItemDrop(Vector3 pos, LootPool pool)
    {
        SodaPart part = lootPoolDict[pool].GetRandomPart();
        
        //Gets the object for the loot crate
        GameObject obj = ObjectPool.objPool.GetPooledObject("Gun Drop");

        if (obj)
        {
            obj.SetActive(true);
            obj.transform.position = pos + (Vector3.up * 0.5f);

            //Get a random soda part
            obj.GetComponent<LootDropController>().ActivatePickup(new Vector3(Random.Range(-2, 2), 4, Random.Range(-2, 2)), part);
        }
    }

    #endregion
}
