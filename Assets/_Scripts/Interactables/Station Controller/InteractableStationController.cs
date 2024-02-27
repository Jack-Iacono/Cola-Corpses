using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableStationController : InteractableController
{
    public bool hasMenu = false;
    public InterfaceController.Screen openMenu;
    
    public static InteractableStationController currentOpenMenu { get; private set; } = null;

    protected override void ExtraUpdate()
    {
        if (Instance != this)
            CloseMenu();
    }
    protected override void Interact()
    {
        if (hasMenu)
        {
            GameController.PauseGame(true);
            OpenMenu();
        }   
    }

    protected override void InRangeAction()
    {
        InteractPopUpController.StartPopup(gameObject, ValueStoreController.keyData.keyInteract.ToString(), Vector3.up * 2 + transform.forward);
    }
    protected override void OutRangeAction()
    {
        GameController.PauseGame(false);
        CloseMenu();

        InteractPopUpController.EndPopup(gameObject);
    }

    public void OpenMenu()
    {
        if(currentOpenMenu == null)
        {
            InterfaceController.intCont.SetUpScreen(openMenu);
            currentOpenMenu = this;
        }
    }
    public void CloseMenu()
    {
        if (currentOpenMenu == this)
            currentOpenMenu = null;
    }

}
