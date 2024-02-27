using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class InterfaceScreenController : MonoBehaviour
{
    // This is the base class for all interface screens to derive from

    public virtual void InitializeScreen() { }
    public virtual void InitializeScreenMenu() { }

    // What to do when showing or hiding the screen
    public virtual void ShowScreen() { gameObject.SetActive(true); }
    public virtual void HideScreen() { gameObject.SetActive(false); }
}
