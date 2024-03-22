using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class SodaPart
{
    [Serialize]
    public string sodaName;

    //Stats are again going to be out of 100
    [Serialize]
    public int damage;
    [Serialize]
    public int explodeRadius;
    [Serialize]
    public int throwRate;
    [Serialize]
    public int throwSpeed;

    [Serialize]
    public Rarity rarity;
    
    [Serialize]
    public int dropWeight;

    //Need to be stored in a primative manner
    [Serialize]
    public Trait trait1 = Trait.None;
    [Serialize]
    public Trait trait2 = Trait.None;
    [Serialize]
    public Flavor flavor = Flavor.Cherry;

    /*Soda List
    Sticky (Slow), Iced (Ice Physics), Fizzy (Burn), Health (Life Drain), Sour (Poison), Flat (Weaken), 
    */
    public enum Trait { Sticky,Iced,Fizzy,Healthy,Sour, Flat, None };

    /*  Flavor List
     *  Cherry: Damage boost, but slower speed
     *  Orange: Improve fizz effect, but decrease direct damage
     *  Blue Raz: Heal more health, but over time
     *  Cola: Guarantee proc on next hit
     *  Root Beer: Cause explosion, long drink time
     *  Grapefruit: Improve sour effect, decrease direct damage
     *  Banana: Increase move speed, decrease damage
    */
    public enum Flavor { Cherry,Orange,Blueraz,Cola,Rootbeer,Grapefruit,Banana };
    public enum Rarity { COMMON, UNCOMMON, RARE, EPIC, LEGENDARY, UNIQUE, UNKNOWN };

    public static Color[] rarityColors { get; } = new Color[] { MyColor.lightGrey, Color.green, Color.blue, new Color(0.337f, 0.035f, 0.466f), Color.yellow, Color.cyan, Color.cyan };

    //Maybe add accuracy stat later?

    public SodaPart(string newName, int newDamage, int newExplodeRadius, int newThrowSpeed, int newThrowRate, Trait newTrait1, Trait newTrait2, Flavor newFlavor, Rarity rarity, int newDropWeight)
    {
        sodaName = newName;
        damage = newDamage;
        explodeRadius = newExplodeRadius;
        throwSpeed = newThrowSpeed;
        throwRate = newThrowRate;

        trait1 = newTrait1;
        trait2 = newTrait2;

        this.rarity = rarity;
        dropWeight = newDropWeight;

        flavor = newFlavor;
    }
    public SodaPart(int newDamage, int newExplodeRadius, int newThrowSpeed, int newThrowRate, Trait newTrait1, Trait newTrait2, Flavor newFlavor)
    {
        sodaName = "Spawned Soda";
        damage = newDamage;
        explodeRadius = newExplodeRadius;
        throwSpeed = newThrowSpeed;
        throwRate = newThrowRate;

        trait1 = newTrait1;
        trait2 = newTrait2;

        flavor = newFlavor;

        rarity = Rarity.UNKNOWN;
        dropWeight = 5;
    }
    public SodaPart(int newDamage, int newExplodeRadius, int newThrowSpeed, int newThrowRate, string newTrait1, string newTrait2, string newFlavor)
    {
        damage = newDamage;
        explodeRadius = newExplodeRadius;
        throwSpeed = newThrowSpeed;
        throwRate = newThrowRate;

        newTrait1 = newTrait1.ToLower().FirstCharacterToUpper();
        newTrait2 = newTrait2.ToLower().FirstCharacterToUpper();
        newFlavor = newFlavor.ToLower().FirstCharacterToUpper();

        trait1 = (Trait)Enum.Parse(typeof(Trait), newTrait1);
        trait2 = (Trait)Enum.Parse(typeof(Trait), newTrait2);

        flavor = (Flavor)Enum.Parse(typeof(Flavor), newFlavor);

        rarity = Rarity.UNKNOWN;
        dropWeight = 5;

        SetName();
    }

    public SodaPart(Rarity rarity)
    {
        this.rarity = rarity;
        GenerateSodaPart();
    }
    public SodaPart(Rarity rarity, Flavor[] excludeFlavor)
    {
        this.rarity = rarity;
        GenerateSodaPart();

        //Gets the new flavor that is not in the exclude list
        List<Flavor> flavList = new List<Flavor>();

        for(int i = 0; i < Enum.GetNames(typeof(Flavor)).Length; i++)
        {
            if (!excludeFlavor.Contains((Flavor)i))
            {
                flavList.Add((Flavor)i);
            }
        }

        if(flavList.Count > 0)
            flavor = flavList[UnityEngine.Random.Range(0,flavList.Count)];
    }

    #region Generation Methods
    public void GenerateSodaPart()
    {
        #region Randomizer
        int partPoints = 0;

        //Determines the amount of stat points to be distributed and part of the name for later
        switch (rarity)
        {

            case Rarity.COMMON:
                partPoints = 20;
                break;
            case Rarity.UNCOMMON:
                partPoints = 40;
                break;
            case Rarity.RARE:
                partPoints = 60;
                break;
            case Rarity.EPIC:
                partPoints = 80;
                break;
            case Rarity.LEGENDARY:
                partPoints = 100;
                break;
            case Rarity.UNKNOWN:
                partPoints = 400;
                break;
        }

        //Assigns the base stats for each part
        SetBaseStats(partPoints);

        switch (rarity)
        {
            case Rarity.UNCOMMON:
            case Rarity.RARE:
            case Rarity.EPIC:
                trait1 = GetRandomTrait();
                break;
            case Rarity.LEGENDARY:
                trait1 = GetRandomTrait();
                trait2 = GetRandomTrait(new Trait[1] { trait1 });
                break;
        }

        flavor = (Flavor)UnityEngine.Random.Range(0, Enum.GetNames(typeof(Flavor)).Length);

        #endregion

        SetDropWeight();
        SetName();
    }
    private void SetBaseStats(int partPoints)
    {
        List<int> distributionList = new List<int>();

        for(int i = 0; i < 3; i++)
        {
            int rand = Mathf.RoundToInt(UnityEngine.Random.Range(0f, partPoints));
            distributionList.Add(rand);
            partPoints -= rand;
        }

        distributionList.Add(partPoints);

        int index = -1;

        index = Mathf.RoundToInt(UnityEngine.Random.Range(0f, 3f));
        damage = distributionList[index];
        distributionList.RemoveAt(index);

        index = Mathf.RoundToInt(UnityEngine.Random.Range(0f, 2f));
        throwRate = distributionList[index];
        distributionList.RemoveAt(index);

        index = Mathf.RoundToInt(UnityEngine.Random.Range(0f, 1f));
        throwSpeed = distributionList[index];
        distributionList.RemoveAt(index);

        explodeRadius = distributionList[0];
    }
    private void SetName()
    {
        string name1 = trait1.ToString();
        string name2 = trait2.ToString();

        if(name1 != "None" && name2 != "None")
        {
            sodaName = name1 + " " + name2 + " " + flavor.ToString() + " " + " Soda";
        }
        else if (name1 != "None")
        {
            sodaName = name1 + " " + flavor.ToString() + " " + " Soda";
        }
        else
        {
            sodaName = flavor.ToString() + " Soda";
        }
        
    }
    private void SetDropWeight()
    {
        dropWeight = 5;
    }
    private Trait GetRandomTrait()
    {
        //Gets a random trait from the range of 1, to the length of the trait list
        // Excludes the last trait which SHOULD be the null trait
        return (Trait)UnityEngine.Random.Range(0, Enum.GetNames(typeof(Trait)).Length - 1);
    }
    private Trait GetRandomTrait(Trait[] exclude)
    {
        List<Trait> excludeList = new List<Trait>();

        for(int i = 1; i < Enum.GetNames(typeof(Trait)).Length; i++)
        {
            Trait t = (Trait)Enum.Parse(typeof(Trait), Enum.GetValues(typeof(Trait)).GetValue(i).ToString());

            if (!exclude.Contains(t) && t != Trait.None)
            {
                excludeList.Add((Trait)Enum.Parse(typeof(Trait), Enum.GetValues(typeof(Trait)).GetValue(i).ToString()));
            }
        }

        return excludeList[UnityEngine.Random.Range(0, excludeList.Count)];
    }
    #endregion

    #region Debug Methods

    public void Print()
    {
        Debug.Log(sodaName + ": " + damage + ", " + throwRate + ", " + throwSpeed + ", " + explodeRadius + ", " + trait1 + ", " + trait2);
    }

    #endregion

    #region Soda Trait Stats

    #region Random Check Methods
    //May change from static later, but I'm not sure

    public static bool SodaTraitCheck(int index, int level)
    {
        //This method takes the index for the soda and returns the result of the random check for that type
        // of that level
        switch (index)
        {
            case 0:
                return IcedProcCheck(level);
            case 1:
                return StickyProcCheck(level);
            case 2:
                return FizzyProcCheck(level);
            case 3:
                return HealthyProcCheck(level);
            case 4:
                return SourProcCheck(level);
            case 5:
                return FlatProcCheck(level);
            default:
                return false;
        }
    }

    private static bool IcedProcCheck(int level)
    {
        switch (level)
        {
            case 1:
            case 2:
                return RandomCheck(20);
            case 3:
            case 4:
                return RandomCheck(60);
            default:
                return RandomCheck(1);
        }
    }
    private static bool StickyProcCheck(int level)
    {
        switch (level)
        {
            case 1:
            case 2:
                return RandomCheck(25);
            case 3:
            case 4:
                return RandomCheck(40);
            default:
                return RandomCheck(1);
        }
    }
    private static bool FizzyProcCheck(int level)
    {
        switch (level)
        {
            case 1:
            case 2:
                return RandomCheck(15);
            case 3:
            case 4:
                return RandomCheck(30);
            default:
                return RandomCheck(1);
        }
    }
    private static bool HealthyProcCheck(int level)
    {
        switch (level)
        {
            case 1:
            case 2:
                return RandomCheck(5);
            case 3:
            case 4:
                return RandomCheck(15);
            default:
                return RandomCheck(1);
        }
    }
    private static bool SourProcCheck(int level)
    {
        switch (level)
        {
            case 1:
            case 2:
                return RandomCheck(15);
            case 3:
            case 4:
                return RandomCheck(30);
            default:
                return RandomCheck(1);
        }
    }
    private static bool FlatProcCheck(int level)
    {
        switch (level)
        {
            case 1:
            case 2:
                return RandomCheck(30);
            case 3:
            case 4:
                return RandomCheck(60);
            default:
                return RandomCheck(1);
        }
    }

    public static bool RandomCheck(float percent)
    {
        if (UnityEngine.Random.Range(0f, 100f) < percent)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    #endregion

    #region Trait Potency Methods

    public static ProcData SodaPotencyCheck( Trait trait, int level )
    {
        //This method takes the index for the soda and returns the result of the random check for that type
        // of that level
        ProcData procData = new ProcData();
        procData.trait = trait;

        switch (trait)
        {
            case Trait.Iced:
                return IcedPotencyCheck(level, procData);
            case Trait.Sticky:
                return StickyPotencyCheck(level, procData);
            case Trait.Fizzy:
                return FizzyPotencyCheck(level, procData);
            case Trait.Healthy:
                return HealthyPotencyCheck(level, procData);
            case Trait.Sour:
                return SourPotencyCheck(level, procData);
            case Trait.Flat:
                return FlatPotencyCheck(level, procData);
            default:
                return null;
        }
    }

    private static ProcData IcedPotencyCheck(int level, ProcData procData)
    {
        switch (level)
        {
            case 1:
                procData.time = 3;
                procData.potency = 0.75f;
                break;
            case 2:
            case 3:
                procData.time = 5;
                procData.potency = 0.6f;
                break;
            case 4:
                procData.time = 5;
                procData.potency = 0.3f;
                break;
        }

        return procData;
    }
    private static ProcData StickyPotencyCheck(int level, ProcData procData)
    {
        switch (level)
        {
            case 1:
                procData.time = 2;
                procData.potency = 0.6f;
                break;
            case 2:
            case 3:
                procData.time = 3;
                procData.potency = 0.4f;
                break;
            case 4:
                procData.time = 5;
                procData.potency = 0.1f;
                break;
        }

        return procData;
    }
    private static ProcData FizzyPotencyCheck(int level, ProcData procData)
    {
        switch (level)
        {
            case 1:
                procData.time = 0.5f;
                procData.ticks = 10;
                procData.potency = 5;
                break;
            case 2:
            case 3:
                procData.time = 0.4f;
                procData.ticks = 15;
                procData.potency = 10;
                break;
            case 4:
                procData.time = 0.1f;
                procData.ticks = 15;
                procData.potency = 20;
                break;
        }

        return procData;
    }
    private static ProcData HealthyPotencyCheck(int level, ProcData procData)
    {
        switch (level)
        {
            case 1:
                procData.time = 1f;
                procData.ticks = 1;
                procData.potency = 5;
                break;
            case 2:
            case 3:
                procData.time = 0.6f;
                procData.ticks = 2;
                procData.potency = 10;
                break;
            case 4:
                procData.time = 0.5f;
                procData.ticks = 3;
                procData.potency = 20;
                break;
        }

        return procData;
    }
    private static ProcData SourPotencyCheck(int level, ProcData procData)
    {
        switch (level)
        {
            case 1:
                procData.time = 1f;
                procData.ticks = 3;
                procData.potency = 0.05f;
                break;
            case 2:
            case 3:
                procData.time = 0.8f;
                procData.ticks = 4;
                procData.potency = 0.05f;
                break;
            case 4:
                procData.time = 0.5f;
                procData.ticks = 5;
                procData.potency = 0.075f;
                break;
        }

        return procData;
    }
    private static ProcData FlatPotencyCheck(int level, ProcData procData)
    {
        switch (level)
        {
            case 1:
                procData.time = 4f;
                procData.potency = 0.9f;
                break;
            case 2:
            case 3:
                procData.time = 6f;
                procData.potency = 0.75f;
                break;
            case 4:
                procData.time = 10f;
                procData.potency = 0.5f;
                break;
        }

        return procData;
    }

    #endregion

    public static string GetTraitColor(int index)
    {
        Trait flav = (Trait)index;

        switch (flav)
        {
            case Trait.Sticky:
                //Chartreuse
                return "#7FFF00";
            case Trait.Fizzy:
                //Orange Red
                return "#FF4500";
            case Trait.Iced:
                //PaleTurquoise
                return "#AFEEEE";
            case Trait.Sour:
                //PaleGreen
                return "#98FB98";
            case Trait.Healthy:
                //SeaGreen
                return "#2E8B57";
            case Trait.Flat:
                //SeaShell
                return "#FFF5EE";
            default:
                //White
                return "white";
        }
    }
    public static string GetTraitColor(Trait flav)
    {

        switch (flav)
        {
            case Trait.Sticky:
                //Chartreuse
                return "#7FFF00";
            case Trait.Fizzy:
                //Orange Red
                return "#FF4500";
            case Trait.Iced:
                //PaleTurquoise
                return "#AFEEEE";
            case Trait.Sour:
                //PaleGreen
                return "#98FB98";
            case Trait.Healthy:
                //SeaGreen
                return "#2E8B57";
            case Trait.Flat:
                //SeaShell
                return "#FFF5EE";
            default:
                //White
                return "white";
        }
    }

    #endregion

    #region Soda Flavor Methods

    public static FlavorData GetFlavorData(Flavor f, int level)
    {
        FlavorData fData = new FlavorData();

        switch (f)
        {
            case Flavor.Cherry:
                fData = GetCherryData(level);
                break;
            case Flavor.Orange:
                fData = GetOrangeData(level);
                break;
            case Flavor.Blueraz:
                fData = GetBlueRazData(level);
                break;
            case Flavor.Cola:
                fData = GetColaData(level);
                break;
            case Flavor.Rootbeer:
                fData = GetRootbeerData(level);
                break;
            case Flavor.Grapefruit:
                fData = GetGrapefruitData(level);
                break;
            case Flavor.Banana:
                fData = GetBananaData(level);
                break;
            default:
                fData = new FlavorData();
                break;
        }

        fData.level = level;

        return fData;
    }

    public static FlavorData GetCherryData(int level)
    {
        FlavorData fData = new FlavorData();

        switch (level)
        {
            case 1:
                fData.drinkTime = 3;
                fData.time = 2;
                fData.ticks = 1;

                fData.health = 2;

                fData.potency = 0.45f;
                fData.antiPotency = 0.4f;
                break;
            case 2:
                fData.drinkTime = 3;
                fData.time = 2;
                fData.ticks = 1;

                fData.health = 7;

                fData.potency = 0.65f;
                fData.antiPotency = 0.2f;
                break;
            case 3:
                fData.drinkTime = 2.5f;
                fData.time = 3.5f;
                fData.ticks = 4;

                fData.health = 16;

                fData.potency = 1f;
                fData.antiPotency = 0.2f;
                break;
            case 4:
                fData.drinkTime = 2.5f;
                fData.time = 5;
                fData.ticks = 1;

                fData.health = 20;
                
                fData.potency = 1.5f;
                fData.antiPotency = 0.1f;
                break;
        }

        return fData;
    }
    public static FlavorData GetOrangeData(int level)
    {
        FlavorData fData = new FlavorData();

        switch (level)
        {
            case 1:
                fData.drinkTime = 3;
                fData.time = 2;
                fData.ticks = 1;

                fData.health = 3;

                fData.potency = 1.1f;
                fData.antiPotency = 0.4f;
                break;
            case 2:
                fData.drinkTime = 3;
                fData.time = 2;
                fData.ticks = 1;

                fData.health = 7;

                fData.potency = 1.15f;
                fData.antiPotency = 0.2f;
                break;
            case 3:
                fData.drinkTime = 2.5f;
                fData.time = 3.5f;
                fData.ticks = 1;

                fData.health = 19;

                fData.potency = 1.3f;
                fData.antiPotency = 0.2f;
                break;
            case 4:
                fData.drinkTime = 2.5f;
                fData.time = 5;
                fData.ticks = 1;

                fData.health = 25;

                fData.potency = 1.5f;
                fData.antiPotency = 0.1f;
                break;
        }

        return fData;
    }
    public static FlavorData GetBlueRazData(int level)
    {
        FlavorData fData = new FlavorData();

        switch (level)
        {
            case 1:
                fData.drinkTime = 3;
                fData.time = 2;
                fData.ticks = 9;

                fData.health = 2;

                fData.potency = 2;
                fData.antiPotency = 0.4f;
                break;
            case 2:
                fData.drinkTime = 3;
                fData.time = 1.5f;
                fData.ticks = 8;

                fData.health = 7;

                fData.potency = 3;
                fData.antiPotency = 0.2f;
                break;
            case 3:
                fData.drinkTime = 2.5f;
                fData.time = 1;
                fData.ticks = 7;

                fData.health = 16;

                fData.potency = 4;
                fData.antiPotency = 0.2f;
                break;
            case 4:
                fData.drinkTime = 2.5f;
                fData.time = 1;
                fData.ticks = 5;

                fData.health = 19;

                fData.potency = 7;
                fData.antiPotency = 0.1f;
                break;
        }

        return fData;
    }
    public static FlavorData GetColaData(int level)
    {
        FlavorData fData = new FlavorData();

        switch (level)
        {
            case 1:
                fData.drinkTime = 3;
                fData.time = 15;
                fData.ticks = 1;

                fData.health = 1;

                fData.potency = 1;
                fData.antiPotency = 5;
                break;
            case 2:
                fData.drinkTime = 3;
                fData.time = 20;
                fData.ticks = 1;

                fData.health = 6;

                fData.potency = 1;
                fData.antiPotency = 7;
                break;
            case 3:
                fData.drinkTime = 2.5f;
                fData.time = 30;
                fData.ticks = 1;

                fData.health = 15;

                fData.potency = 1;
                fData.antiPotency = 10;
                break;
            case 4:
                fData.drinkTime = 2.5f;
                fData.time = 45;
                fData.ticks = 2;

                fData.health = 17;

                fData.potency = 1;
                fData.antiPotency = 12;
                break;
        }

        return fData;
    }
    public static FlavorData GetRootbeerData(int level)
    {
        FlavorData fData = new FlavorData();

        switch (level)
        {
            case 1:
                fData.drinkTime = 7;
                fData.time = 0.5f;
                fData.ticks = 1;

                fData.health = 2;

                fData.potency = 25;
                fData.antiPotency = 0;
                break;
            case 2:
                fData.drinkTime = 6f;
                fData.time = 0.5f;
                fData.ticks = 1;

                fData.health = 7;

                fData.potency = 30;
                fData.antiPotency = 0;
                break;
            case 3:
                fData.drinkTime = 5;
                fData.time = 0.5f;
                fData.ticks = 1;

                fData.health = 16;

                fData.potency = 40;
                fData.antiPotency = 0;
                break;
            case 4:
                fData.drinkTime = 4;
                fData.time = 0.5f;
                fData.ticks = 2;

                fData.health = 19;

                fData.potency = 60;
                fData.antiPotency = 0;
                break;
        }

        return fData;
    }
    public static FlavorData GetGrapefruitData(int level)
    {
        FlavorData fData = new FlavorData();

        switch (level)
        {
            case 1:
                fData.drinkTime = 3;
                fData.time = 2;
                fData.ticks = 1;

                fData.health = 3;

                fData.potency = 1.1f;
                fData.antiPotency = 0.4f;
                break;
            case 2:
                fData.drinkTime = 3;
                fData.time = 2;
                fData.ticks = 1;

                fData.health = 8;

                fData.potency = 1.15f;
                fData.antiPotency = 0.2f;
                break;
            case 3:
                fData.drinkTime = 2.5f;
                fData.time = 3.5f;
                fData.ticks = 4;

                fData.health = 19;

                fData.potency = 1.3f;
                fData.antiPotency = 0.2f;
                break;
            case 4:
                fData.drinkTime = 2.5f;
                fData.time = 5;
                fData.ticks = 1;

                fData.health = 25;

                fData.potency = 1.5f;
                fData.antiPotency = 0.1f;
                break;
        }

        return fData;
    }
    public static FlavorData GetBananaData(int level)
    {
        FlavorData fData = new FlavorData();

        switch (level)
        {
            case 1:
                fData.drinkTime = 3;
                fData.time = 10;
                fData.ticks = 1;

                fData.health = 2;

                fData.potency = 0.4f;
                fData.antiPotency = 0.4f;
                break;
            case 2:
                fData.drinkTime = 3;
                fData.time = 12;
                fData.ticks = 1;

                fData.health = 7;

                fData.potency = 0.5f;
                fData.antiPotency = 0.2f;
                break;
            case 3:
                fData.drinkTime = 2.5f;
                fData.time = 16;
                fData.ticks = 1;

                fData.health = 18;

                fData.potency = 0.7f;
                fData.antiPotency = 0.2f;
                break;
            case 4:
                fData.drinkTime = 2.5f;
                fData.time = 20;
                fData.ticks = 1;

                fData.health = 21;

                fData.potency = 0.8f;
                fData.antiPotency = 0.1f;
                break;
        }

        return fData;
    }

    public static string GetFlavorColor(int index)
    {

        Flavor flav = (Flavor)index;

        switch (flav)
        {
            case Flavor.Cherry:
                //Dark Red
                return "#8B0000";
            case Flavor.Orange:
                //Orange Red
                return "#FF4500";
            case Flavor.Blueraz:
                //Middle Blue
                return "#0000CD";
            case Flavor.Cola:
                //Maroon
                return "#800000";
            case Flavor.Rootbeer:
                //Sienna
                return "#A0522D";
            case Flavor.Grapefruit:
                //Light Pink
                return "#FFB6C1";
            case Flavor.Banana:
                //Yellow
                return "#FFFF00";
            default:
                //White
                return "white";
        }
    }

    #endregion
}
