using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapItem
{
    public bool isVisible;
    public Vector2 pos;
    [NonSerialized]
    public GameObject parentObject;

    public MinimapItem()
    {
        isVisible = false;
        parentObject = null;
    }
    public MinimapItem(Vector2 newPos, GameObject obj)
    {
        pos = newPos;
        parentObject = obj;
    }
    public MinimapItem(bool b, GameObject obj)
    {
        isVisible = b;
        parentObject = obj;

        SetConnectedRender();
    }

    public void SetConnectedRender()
    {
        if (parentObject)
        {
            //Enables or disables the renderers for the object
            foreach (Renderer rend in parentObject.GetComponentsInChildren<Renderer>())
            {
                //Checks if the renderer is on the correct layer
                if (rend.gameObject.layer == LayerMask.NameToLayer(MinimapController.minimapLayerName))
                {
                    rend.enabled = isVisible;
                }
            }
        }
    }
    public void SetParentObject(GameObject obj)
    {
        parentObject = obj;
        SetConnectedRender();
    }
    public void Activate()
    {
        isVisible = true;

        SetConnectedRender();
    }
}
