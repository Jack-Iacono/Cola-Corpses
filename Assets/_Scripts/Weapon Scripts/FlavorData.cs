using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class FlavorData
{
    public int level;

    public int health;

    public float time;
    public float drinkTime;

    public float antiPotency;
    public float potency;

    public int ticks;
    public FlavorData()
    {
        health = 0;
        time = 0;
        drinkTime = 0;

        potency = 0;
        antiPotency = 0;

        ticks = 0;
    }
}
