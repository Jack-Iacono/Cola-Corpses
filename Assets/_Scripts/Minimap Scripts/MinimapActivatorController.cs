using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapActivatorController : MonoBehaviour
{
    public Tile tile = null;
    public bool active = false;

    private void OnTriggerEnter(Collider other)
    {
        if (active)
        {
            if (other.tag == "Player")
            {
                tile.SetVisible(true);
                DestroyActivator();
            }
        }
    }
    public void DestroyActivator()
    {
        //Destroys this script and the collider on the object as it is no longer needed
        Destroy(GetComponent<Collider>());
        Destroy(this);
    }
    public void Activate(Tile newTile)
    {
        tile = newTile;
        active = true;
    }

}
