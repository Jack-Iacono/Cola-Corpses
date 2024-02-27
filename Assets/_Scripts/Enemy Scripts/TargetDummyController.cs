using System.Buffers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static SodaPart;

public class TargetDummyController : Enemy
{
    private void Start()
    {
        Initialize();
    }

    public override void Initialize()
    {
        // Sets Health
        currentHealth = maxHealth;

        //Gets the navMesh obstacle and agent components
        player = PlayerController.playerCont;

        // Used to get updates when the game is paused
        GameController.RegisterObserver(this);

        //Gets the animator
        anim = GetComponent<Animator>();

        // Makes a list long enough to house all traits
        for (int i = 0; i < Enum.GetNames(typeof(Trait)).Length; i++)
        {
            debuffTimers.Add(Enum.GetValues(typeof(Trait)).GetValue(i), new Timer(0));
        }

        // Add the end methods to the appropriate timers
        debuffTimers[Trait.Sticky].endMethod = EndSticky;
        debuffTimers[Trait.Iced].endMethod = EndIced;
        debuffTimers[Trait.Fizzy].endMethod = EndFizzy;
        debuffTimers[Trait.Sour].endMethod = EndSour;
        debuffTimers[Trait.Healthy].endMethod = EndHealthy;
        debuffTimers[Trait.Flat].endMethod = EndFlat;
    }
    protected override void RunBehaviorTree()
    {
        
    }
}
