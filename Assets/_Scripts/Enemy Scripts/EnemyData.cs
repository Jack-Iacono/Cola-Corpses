using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

[Serializable]
public class EnemyData
{
    public int maxHealth;
    public int currentHealth;
    public float moveSpeed;
    public float damage;
    public float hitDistance;
    public float hitSpeed;
    public float lootDropChance;
    
    public LootPoolController.LootPool lootPoolID;
    
    public int averageSoda;

    public string bufferedAction;

    public Vector3 position;
    public Vector3 rotation;
    public Vector3 spawnerPosition;
}
