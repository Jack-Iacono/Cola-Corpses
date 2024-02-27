using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneStationController : InteractableStationController
{
    protected override void InRangeAction()
    {
        InteractPopUpController.StartPopup(gameObject, ValueStoreController.keyData.keyInteract.ToString(), Vector3.up * 3.25f);
    }
    protected override void OutRangeAction()
    {
        GameController.PauseGame(false);
        CloseMenu();

        InteractPopUpController.EndPopup(gameObject);
    }
}
