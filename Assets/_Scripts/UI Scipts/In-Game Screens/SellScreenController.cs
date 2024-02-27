using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SellScreenController : InterfaceScreenController
{
    public List<GameObject> sellSlots = new List<GameObject>();
    public TMP_Text sellName;
    public TMP_Text sellDMG;
    public TMP_Text sellTRT;
    public TMP_Text sellTSP;
    public TMP_Text sellXRD;
    public TMP_Text sellT1;
    public TMP_Text sellT2;

    public TMP_Text sellValue;
    public GameObject sellButton;

    public GameObject displayPlateSell;

    public override void InitializeScreen()
    {
        base.InitializeScreen();

        NewPartSell();
    }

    public override void ShowScreen()
    {
        base.ShowScreen();

        InventoryController.invCont.ClearSellPart();
        NewPartSell();
    }
    public override void HideScreen()
    {
        base.HideScreen();
    }

    public void NewPartSell()
    {
        for (int i = 0; i < InventoryController.invCont.invParts.Length; i++)
        {
            if (InventoryController.invCont.invParts[i] != null)
            {
                sellSlots[i].GetComponent<Image>().color = SodaPart.rarityColors[(int)InventoryController.invCont.invParts[i].rarity];
                sellSlots[i].GetComponent<MenuButtonController>().enabled = true;
            }
            else
            {
                sellSlots[i].GetComponent<Image>().color = MyColor.darkGrey;
                sellSlots[i].GetComponent<MenuButtonController>().enabled = false;
            }
        }

        DisplaySellPart(InventoryController.invCont.selectIndex);
    }
    public void DisplaySellPart(int index)
    {
        if (index == -1)
        {
            //Clear the display
            displayPlateSell.SetActive(false);

            sellName.text = "";
            sellDMG.text = "";
            sellTRT.text = "";
            sellTSP.text = "";
            sellXRD.text = "";
            sellT1.text = "";
            sellT2.text = "";
            sellValue.text = "";
        }
        else
        {
            SodaPart part = InventoryController.invCont.invParts[index];

            //Display the part
            displayPlateSell.SetActive(true);
            displayPlateSell.GetComponent<Image>().color = SodaPart.rarityColors[(int)part.rarity];

            sellName.text = part.sodaName;
            sellDMG.text = part.damage.ToString();
            sellTRT.text = part.throwRate.ToString();
            sellTSP.text = part.throwSpeed.ToString();
            sellXRD.text = part.explodeRadius.ToString();

            sellValue.text = InventoryController.GetPartValue(part.rarity).ToString();

            if (part.trait1 != SodaPart.Trait.None)
                sellT1.text = part.trait1.ToString();
            else
                sellT1.text = "";

            if (part.trait2 != SodaPart.Trait.None)
                sellT2.text = part.trait2.ToString();
            else
                sellT2.text = "";
        }

        if (InventoryController.invCont.selectIndex == -1)
        {
            sellButton.GetComponent<MenuButtonController>().enabled = false;
            sellButton.SetActive(false);
        }
        else
        {
            sellButton.GetComponent<MenuButtonController>().enabled = true;
            sellButton.SetActive(true);
        }
    }

    #region Button Methods

    public void SelectPart(int i)
    {
        InventoryController.invCont.SelectPartSell(i);
    }
    public void SellPart()
    {
        SoundManager.PlaySound(SoundManager.success);
        InventoryController.invCont.SellPart();
    }

    #endregion
}
