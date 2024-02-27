using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcData
{

    public SodaPart.Trait trait = SodaPart.Trait.None;
    public float potency = 0;
    public float time = 0;
    public int ticks = 0;

    public ProcData(float pot, float tm, int tk, SodaPart.Trait trait)
    {
        potency = pot;
        time = tm;
        ticks = tk;
        this.trait = trait;
    }
    public ProcData()
    {
        potency = 0;
        time = 0;
        ticks = 0;
        trait = SodaPart.Trait.None;
    }

}
