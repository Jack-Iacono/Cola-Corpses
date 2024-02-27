using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadMenuButtonController : MonoBehaviour
{
    public string seedName;
    public static LoadScreenController loadCont;
    
    public void Initialize(string seedName)
    {
        this.seedName = seedName;
    }
    public void SelectSeedName()
    {
        loadCont.SetSeedName(seedName);
    }
}
