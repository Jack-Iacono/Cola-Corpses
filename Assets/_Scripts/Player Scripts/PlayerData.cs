using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class PlayerData
{
    [Serialize]
    public int maxHealth;
    [Serialize]
    public float moveSpeed = 10;
    [Serialize]
    public float runSpeed = 15;
    [Serialize]
    public float jumpHeight = 10;
    [Serialize]
    public float gravity = 9.8f;
    [Serialize]
    public float accel = 5;
    [Serialize]
    public float decel = 5;
    [Serialize]
    public bool autoJump = true;
    [Serialize]
    public int currentHealth;
    [Serialize]
    public float moveMod = 1;
    [Serialize]
    public bool allowBHop = false;
    public Vector3 position { get; set; }
    public Vector3 rotation { get; set; }
}
