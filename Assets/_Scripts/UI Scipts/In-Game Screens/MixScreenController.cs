using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MixScreenController : InterfaceScreenController
{
    [Header("Inventory Screen Variables")]
    public List<GameObject> mixInvSlots = new List<GameObject>();

    public MenuButtonController mixButton;
    public GameObject selectedSodaArea;

    [Header("Selected Item Texts")]
    public TMP_Text selectName;
    public TMP_Text selectDamage;
    public TMP_Text selectRate;
    public TMP_Text selectSpeed;
    public TMP_Text selectRadius;
    public TMP_Text selectTrait1;
    public TMP_Text selectTrait2;

    [Header("Mixed Soda Texts")]
    public TMP_Text mixName;
    public TMP_Text mixDamage;
    public TMP_Text mixRate;
    public TMP_Text mixSpeed;
    public TMP_Text mixRadius;
    public TMP_Text mixTraits;

    public override void InitializeScreen()
    {
        base.InitializeScreen();

        ClearDisplayMixInv();
    }

    public override void ShowScreen()
    {
        base.ShowScreen();

        ClearDisplayMixInv();
        InventoryController.invCont.ClearSelectedPart();
        NewMix();
    }
    public override void HideScreen()
    {
        base.HideScreen();
    }

    public void NewMix()
    {
        for (int i = 0; i < InventoryController.invCont.invParts.Length; i++)
        {
            if (InventoryController.invCont.invParts[i] != null)
            {
                mixInvSlots[i].GetComponent<MenuButtonController>().enabled = true;
                mixInvSlots[i].GetComponent<MenuButtonController>().SetSpriteSetIndex(0);
                mixInvSlots[i].GetComponent<Image>().color = SodaPart.rarityColors[(int)InventoryController.invCont.invParts[i].rarity];
            }
            else
            {
                mixInvSlots[i].GetComponent<MenuButtonController>().enabled = false;
                mixInvSlots[i].GetComponent<MenuButtonController>().SetSpriteSetIndex(1);
                mixInvSlots[i].GetComponent<Image>().color = MyColor.darkGrey;
            }

            mixInvSlots[i].GetComponent<MenuButtonController>().SetSpriteSetIndex(0);
            mixInvSlots[i].GetComponentInChildren<TMP_Text>().text = "";
        }

        for (int i = 0; i < InventoryController.invCont.selectedParts.Length; i++)
        {
            if (InventoryController.invCont.selectedParts[i] != -1)
            {
                mixInvSlots[InventoryController.invCont.selectedParts[i]].GetComponent<MenuButtonController>().SetSpriteSetIndex(1);
                mixInvSlots[InventoryController.invCont.selectedParts[i]].GetComponentInChildren<TMP_Text>().text = (i + 1).ToString();
            }
        }

        mixName.text = InventoryController.invCont.GetFlavorNames();

        if (InventoryController.invCont.selectedParts[0] != -1)
        {
            mixButton.SetSpriteSetIndex(0);
            mixButton.enabled = true;
        }
        else
        {
            mixButton.SetSpriteSetIndex(1);
            mixButton.enabled = false;
        }

        mixDamage.text = InventoryController.invCont.sodaStatTotals[0].ToString();
        mixDamage.color = GetChangeColor(InventoryController.invCont.sodaStatTotals[0], WeaponController.weaponCont.damage);
        mixRate.text = InventoryController.invCont.sodaStatTotals[1].ToString();
        mixRate.color = GetChangeColor(InventoryController.invCont.sodaStatTotals[1], (int)WeaponController.weaponCont.rawThrowRate);
        mixSpeed.text = InventoryController.invCont.sodaStatTotals[2].ToString();
        mixSpeed.color = GetChangeColor(InventoryController.invCont.sodaStatTotals[2], (int)WeaponController.weaponCont.rawThrowSpeed);
        mixRadius.text = InventoryController.invCont.sodaStatTotals[3].ToString();
        mixRadius.color = GetChangeColor(InventoryController.invCont.sodaStatTotals[3], (int)WeaponController.weaponCont.rawExplodeRadius);

        mixTraits.text = InventoryController.GetTotalNames(InventoryController.invCont.sodaTraitTotals);
    }

    public void DisplayMix(int index)
    {
        SodaPart part;

        part = InventoryController.invCont.invParts[index];

        selectedSodaArea.GetComponent<Image>().color = SodaPart.rarityColors[(int)part.rarity];

        selectName.text = part.sodaName;
        selectDamage.text = part.damage.ToString();
        selectRate.text = part.throwRate.ToString();
        selectSpeed.text = part.throwSpeed.ToString();
        selectRadius.text = part.explodeRadius.ToString();

        if (part.trait1 != SodaPart.Trait.None)
            selectTrait1.text = part.trait1.ToString();
        else
            selectTrait1.text = "";

        if (part.trait2 != SodaPart.Trait.None)
            selectTrait2.text = part.trait2.ToString();
        else
            selectTrait2.text = "";

        selectedSodaArea.GetComponent<CanvasGroup>().alpha = 1;
    }
    public void ClearDisplayMixInv()
    {
        selectedSodaArea.GetComponent<Image>().color = MyColor.lightGrey;

        selectName.text = "";
        selectDamage.text = "";
        selectRate.text = "";
        selectSpeed.text = "";
        selectRadius.text = "";
        selectTrait1.text = "";
        selectTrait2.text = "";

        selectedSodaArea.GetComponent<CanvasGroup>().alpha = 0;
    }

    public static Color GetChangeColor(int n, int m)
    {
        if (n > m)
        {
            return Color.green;
        }
        else if (n < m)
        {
            return Color.red;
        }

        return Color.black;
    }
    public static string GetChangeColorHex(int n, int m)
    {
        if (n > m)
        {
            return "#00ff00";
        }
        else if (n < m)
        {
            return "#ff0000";
        }

        return "#ffffff";
    }

    #region Button Methods

    public void Mix()
    {
        InventoryController.invCont.MixSoda();

        NewMix();
    }
    public void SelectPart(int i)
    {
        InventoryController.invCont.SelectPart(i);

        NewMix();
    }
    public void DisplayPart(int i)
    {
        InventoryController.invCont.DisplayPart(i);

        DisplayMix(i);
    }

    #endregion

}
