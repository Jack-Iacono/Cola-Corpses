using System;
using UnityEngine;
using static SodaPart;

public class LootData
{

    public int[] rarityDropWeight = new int[Enum.GetNames(typeof(Rarity)).Length - 1];
    public SodaPart[] uniqueDrops;

    private int rarityWeightTotal = 0;
    private int uniqueWeightTotal = 0;
    
    public LootData(int[] rarityDropWeight, SodaPart[] uniqueDrops)
    {
        this.rarityDropWeight = rarityDropWeight;
        this.uniqueDrops = uniqueDrops;

        CalculateTotalWeight();
    }
    public LootData(int[] rarityDropWeight)
    {
        this.rarityDropWeight = rarityDropWeight;
        this.uniqueDrops = null;

        CalculateTotalWeight();
    }

    public void CalculateTotalWeight()
    {
        rarityWeightTotal = 0;
        for (int i = 0; i < rarityDropWeight.Length; i++)
            rarityWeightTotal += rarityDropWeight[i];

        uniqueWeightTotal = 0;
        if(uniqueDrops != null)
            for (int i = 0; i < uniqueDrops.Length; i++)
                uniqueWeightTotal += uniqueDrops[i].dropWeight;
    }
    public SodaPart GetRandomPart()
    {
        int rand = UnityEngine.Random.Range(0, rarityWeightTotal + 1);

        int currentWeightCheck = 0;

        for(int i = 0; i < rarityDropWeight.Length; i++)
        {
            currentWeightCheck += rarityDropWeight[i];

            if (rand <= currentWeightCheck)
            {
                if ((Rarity)i == Rarity.UNIQUE)
                    return GetUniqueItem();
                else
                    return new SodaPart((Rarity)i);
            }
        }

        return null;
    }
    public SodaPart GetUniqueItem()
    {
        if(uniqueDrops.Length > 0)
        {
            int rand = UnityEngine.Random.Range(0, uniqueWeightTotal + 1);

            int currentWeightCheck = 0;

            for (int i = 0; i < uniqueDrops.Length; i++)
            {
                currentWeightCheck += uniqueDrops[i].dropWeight;

                if (rand <= currentWeightCheck)
                {
                    return uniqueDrops[i];
                }
            }
        }

        return new SodaPart(Rarity.RARE);
    }
}
