using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapController : MonoBehaviour
{
    public static MinimapController minimapCont;
    public List<MinimapItem> minimap = new List<MinimapItem>();

    public static string minimapLayerName = "MinimapItem";

    #region Initialization
    public void SetStaticInstance()
    {
        if(minimapCont != null && minimapCont != this)
        {
            Destroy(this);
        }
        else
        {
            minimapCont = this;
        }
    }
    public void Initialize()
    {
        if (ValueStoreController.loadData)
        {
            //Change to load later
            minimap = new List<MinimapItem>();
        }
        else
        {
            minimap = new List<MinimapItem>();
        }
    }
    #endregion

    public void OpenMinimap(int x, int y)
    {
    }
    public void SetMinimapItem(Vector2 pos, GameObject obj)
    {
       minimap.Add(new MinimapItem(pos, obj));
    }
}
