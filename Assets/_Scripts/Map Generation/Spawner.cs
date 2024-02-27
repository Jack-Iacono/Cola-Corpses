using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Spawner
{
    [NonSerialized]
    public GameObject spawnerObject;
    public Vector3 spawnerLocation;

    [NonSerialized]
    private static Vector3 offset = new Vector3(0,0.6f,0);

    public Spawner(Vector3 pos)
    {
        spawnerLocation = pos + offset;
    }
    public Spawner()
    {
        spawnerLocation = offset;
    }
}
