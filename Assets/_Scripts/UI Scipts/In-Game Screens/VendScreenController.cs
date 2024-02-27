using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VendScreenController : InterfaceScreenController
{
    public List<GameObject> vendSlots = new List<GameObject>();
    public TMP_Text vendName;
    public TMP_Text vendDMG;
    public TMP_Text vendTRT;
    public TMP_Text vendTSP;
    public TMP_Text vendXRD;
    public TMP_Text vendT1;
    public TMP_Text vendT2;

    public TMP_Text vendPrice;
    public TMP_Text vendTokens;

    public GameObject displayPlate;

    public GameObject confirmButton;

    public override void InitializeScreen()
    {
        base.InitializeScreen();
    }

    public override void ShowScreen()
    {
        base.ShowScreen();

        NewVend();
        DisplayVendPart(-1);
    }
    public override void HideScreen()
    {
        base.HideScreen();
    }

    public void NewVend()
    {
        if (VendingMachineController.VendInstance != null)
        {
            vendTokens.text = InventoryController.invCont.tokens.ToString();

            for (int i = 0; i < VendingMachineController.VendInstance.stockList.Length; i++)
            {
                if (VendingMachineController.VendInstance.stockList[i] != null)
                {
                    vendSlots[i].GetComponent<Image>().color = SodaPart.rarityColors[(int)VendingMachineController.VendInstance.stockList[i].rarity];
                    vendSlots[i].GetComponent<MenuButtonController>().enabled = true;
                }
                else
                {
                    vendSlots[i].GetComponent<Image>().color = MyColor.darkGrey;
                    vendSlots[i].GetComponent<MenuButtonController>().enabled = false;
                }
            }

            DisplayVendPart(VendingMachineController.VendInstance.currentPart);
        }
    }
    public void DisplayVendPart(int index)
    {
        VendingMachineController.VendInstance.SetCurrentPart(index);

        if (index == -1)
        {
            //Clear the display
            displayPlate.SetActive(false);

            vendName.text = "";
            vendDMG.text = "";
            vendTRT.text = "";
            vendTSP.text = "";
            vendXRD.text = "";
            vendT1.text = "";
            vendT2.text = "";
            vendPrice.text = "";
        }
        else
        {
            //Display the part
            displayPlate.SetActive(true);
            displayPlate.GetComponent<Image>().color = SodaPart.rarityColors[(int)VendingMachineController.VendInstance.stockList[index].rarity];

            vendName.text = VendingMachineController.VendInstance.stockList[index].sodaName;
            vendDMG.text = VendingMachineController.VendInstance.stockList[index].damage.ToString();
            vendTRT.text = VendingMachineController.VendInstance.stockList[index].throwRate.ToString();
            vendTSP.text = VendingMachineController.VendInstance.stockList[index].throwSpeed.ToString();
            vendXRD.text = VendingMachineController.VendInstance.stockList[index].explodeRadius.ToString();

            vendPrice.text = VendingMachineController.VendInstance.priceList[index].ToString();

            if (VendingMachineController.VendInstance.priceList[index] > InventoryController.invCont.tokens)
                vendPrice.color = Color.red;
            else
                vendPrice.color = Color.green;

            if (VendingMachineController.VendInstance.stockList[index].trait1 != SodaPart.Trait.None)
                vendT1.text = VendingMachineController.VendInstance.stockList[index].trait1.ToString();
            else
                vendT1.text = "";

            if (VendingMachineController.VendInstance.stockList[index].trait2 != SodaPart.Trait.None)
                vendT2.text = VendingMachineController.VendInstance.stockList[index].trait2.ToString();
            else
                vendT2.text = "";
        }

        if (VendingMachineController.VendInstance.currentPart == -1)
        {
            confirmButton.GetComponent<MenuButtonController>().enabled = false;
        }
        else
        {
            confirmButton.GetComponent<MenuButtonController>().enabled = true;
        }
    }
    public void VendPart()
    {
        VendingMachineController.VendInstance.GetComponent<VendingMachineController>().VendPart();
    }
}
