using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    public static InventoryController invCont;

    //Stores the parts which are in your inventory
    public SodaPart[] invParts { get; } = new SodaPart[12];

    //Contains the indexes of the parts in invParts
    public int[] selectedParts { get; } = new int[4] { -1, -1, -1, -1};

    public int[] sodaStatTotals { get; set; } = new int[4];
    public int[] sodaTraitTotals { get; set; } = new int[Enum.GetNames(typeof(SodaPart.Trait)).Length];
    public int[] sodaFlavorTotals { get; set; } = new int[Enum.GetNames(typeof(SodaPart.Flavor)).Length];

    public int selectIndex { get; set; } = -1;

    #region Score Variables

    public int tokens { get; set; } = 0;

    #endregion

    #region Initialization
    //Used for setting static reference
    public void SetStaticInstance()
    {
        //Creating a singleton
        if (invCont != null && invCont != this)
        {
            Destroy(this);
        }
        else
        {
            invCont = this;
        }
    }
    public void Initialize()
    {
        if (ValueStoreController.loadData)
        {
            InventoryData loadParts = ValueStoreController.loadedGameData.invData;

            for(int i = 0; i < loadParts.invParts.Length; i++)
            {
                invParts[i] = loadParts.invParts[i];
            }

            tokens = loadParts.tokens;
        }
        else
        {
            tokens = 0;
        }

        for (int i = 0; i < selectedParts.Length; i++)
        {
            selectedParts[i] = -1;
        }
    }
    #endregion

    #region Inventory Methods

    public bool CheckInvOpen()
    {
        CondenseInvParts();
        for (int i = 0; i < invParts.Length; i++)
        {
            if (invParts[i] == null)
                return true;
        }
        return false;
    }

    public void AddInvPart(SodaPart part)
    {
        CondenseInvParts();

        for (int i = 0; i < invParts.Length; i++)
        {
            if (invParts[i] == null)
            {
                invParts[i] = part;
                i = invParts.Length;
            }
        }

        InterfaceController.intCont.screenPair[InterfaceController.Screen.MIX].GetComponent<MixScreenController>().NewMix();
    }
    private void DeleteInvPart(int index)
    {
        invParts[index] = null;
        InterfaceController.intCont.screenPair[InterfaceController.Screen.MIX].GetComponent<MixScreenController>().ClearDisplayMixInv();
    }
    private void CondenseInvParts()
    {
        SodaPart[] tempArray = new SodaPart[invParts.Length];
        int count = 0;

        //Takes the elements in the partList and moves them forward in the array
        for (int i = 0; i < invParts.Length; i++)
        {
            if (invParts[i] != null)
            {
                tempArray[count] = invParts[i];
                count++;
            }
            invParts[i] = null;
        }

        //Copying the data back to the part list
        for (int i = 0; i < invParts.Length; i++)
        {
            if (tempArray[i] != null)
            {
                invParts[i] = tempArray[i];
            }
        }
    }

    public void DisplayPart(int index)
    {
        selectIndex = index;
    }
    #endregion

    #region Item Combination Methods

    public void SelectPart(int index)
    {
        //Checks if the index is already in the array, if so it removes it (like a toggle)
        if (selectedParts.Contains(index))
        {

            //Removes the index from the selected indexes
            for (int i = 0; i < selectedParts.Length; i++)
            {
                if (selectedParts[i] == index)
                {
                    SubtractTotal(invParts[selectedParts[i]]);

                    selectedParts[i] = -1;
                    CondenseSelectedParts();
                }
            }
        }
        else
        {

            int openIndex = -1;

            for (int i = 0; i < selectedParts.Length; i++)
            {
                if (selectedParts[i] == -1)
                {
                    openIndex = i;
                }
            }

            if (openIndex != -1)
            {
                selectedParts[openIndex] = index;
                AddTotal(invParts[selectedParts[openIndex]]);
                CondenseSelectedParts();
            }
        }

        string test = "";
        for(int i = 0; i < selectedParts.Length; i++)
        {
            test += ", " + selectedParts[i];
        }

        
    }
    public void CondenseSelectedParts()
    {
        int[] tempArray = new int[selectedParts.Length];
        int count = 0;

        for (int i = 0; i < tempArray.Length; i++)
            tempArray[i] = -1;

        //Takes the elements in the partList and moves them forward in the array
        for (int i = 0; i < selectedParts.Length; i++)
        {
            if (selectedParts[i] != -1)
            {
                tempArray[count] = selectedParts[i];
                count++;
            }
            selectedParts[i] = -1;
        }

        //Copying the data back to the part list
        for (int i = 0; i < selectedParts.Length; i++)
        {
            if (tempArray[i] != -1)
            {
                selectedParts[i] = tempArray[i];
            }
        }
    }
    public void ClearSelectedPart()
    {
        for(int i = 0; i < selectedParts.Length; i++)
        {
            selectedParts[i] = -1;
            sodaStatTotals[i] = 0;
        }

        for(int i = 0; i < sodaTraitTotals.Length; i++)
        {
            sodaTraitTotals[i] = 0;
        }
        for (int i = 0; i < sodaFlavorTotals.Length; i++)
        {
            sodaFlavorTotals[i] = 0;
        }
    }

    private void AddTotal(SodaPart part)
    {
        sodaStatTotals[0] = Mathf.Clamp(sodaStatTotals[0] + part.damage, 0, 100);
        sodaStatTotals[1] = Mathf.Clamp(sodaStatTotals[1] + part.throwRate, 0, 100);
        sodaStatTotals[2] = Mathf.Clamp(sodaStatTotals[2] + part.throwSpeed, 0, 100);
        sodaStatTotals[3] = Mathf.Clamp(sodaStatTotals[3] + part.explodeRadius, 0, 100);

        if (part.trait2 != SodaPart.Trait.None)
        {
            sodaTraitTotals[(int)part.trait2]++;
            sodaTraitTotals[(int)part.trait1]++;
        }
        else if (part.trait1 != SodaPart.Trait.None)
        {
            sodaTraitTotals[(int)part.trait1]++;
        }

        sodaFlavorTotals[(int)part.flavor]++;
    }
    private void SubtractTotal(SodaPart part)
    {
        sodaStatTotals[0] = Mathf.Clamp(sodaStatTotals[0] - part.damage, 0, 100);
        sodaStatTotals[1] = Mathf.Clamp(sodaStatTotals[1] - part.throwRate, 0, 100);
        sodaStatTotals[2] = Mathf.Clamp(sodaStatTotals[2] - part.throwSpeed, 0, 100);
        sodaStatTotals[3] = Mathf.Clamp(sodaStatTotals[3] - part.explodeRadius, 0, 100);

        if (part.trait2 != SodaPart.Trait.None)
        {
            sodaTraitTotals[(int)part.trait2]--;
            sodaTraitTotals[(int)part.trait1]--;
        }
        else if (part.trait1 != SodaPart.Trait.None)
        {
            sodaTraitTotals[(int)part.trait1]--;
        }

        sodaFlavorTotals[(int)part.flavor]--;
    }
    public static string GetTotalNames(int[] traitTotals)
    {
        string finalString = "";

        for(int i = 0; i < traitTotals.Length; i++)
        {
            if (traitTotals[i] != 0)
            {
                if(finalString == "")
                    finalString += (SodaPart.Trait)i + " " + traitTotals[i];
                else
                    finalString += ", " + (SodaPart.Trait)i + " " + traitTotals[i];
            }
        }

        return finalString;
    }

    public string GetFlavorNames()
    {
        string final = "";

        for (int i = 0; i < sodaFlavorTotals.Length; i++)
        {
            if (sodaFlavorTotals[i] != 0)
            {
                if (final != "")
                    final += " ";

                final += "<color=" + SodaPart.GetFlavorColor(i) + ">";

                switch (sodaFlavorTotals[i])
                {
                    case 1:
                        final += ((SodaPart.Flavor)i).ToString();
                        break;
                    case 2:
                        final += "Double " + ((SodaPart.Flavor)i).ToString();
                        break;
                    case 3:
                        final += "Triple " + ((SodaPart.Flavor)i).ToString();
                        break;
                    case 4:
                        final += "Quad " + ((SodaPart.Flavor)i).ToString();
                        break;
                    default:
                        final += "Weird " + ((SodaPart.Flavor)i).ToString();
                        break;

                }

                final += "</color>";
            }
        }

        return final;
    }

    public void MixSoda()
    {
        SodaPart[] parts = new SodaPart[selectedParts.Length];

        for(int i = 0; i < selectedParts.Length; i++)
        {
            if (selectedParts[i] != -1)
            {
                parts[i] = invParts[selectedParts[i]];
                DeleteInvPart(selectedParts[i]);
            }
        }

        WeaponController.weaponCont.SetParts(parts);

        SoundManager.PlaySound(SoundManager.success);

        ClearSelectedPart();
        CondenseInvParts();

        InterfaceController.intCont.pauseScreenController.NewStats(WeaponController.weaponCont);

        selectIndex = -1;
    }
    #endregion

    #region Item Selling Methods

    public void ClearSellPart()
    {
        selectIndex = -1;
    }
    public void SelectPartSell(int index)
    {
        selectIndex = index;

        InterfaceController.intCont.screenPair[InterfaceController.Screen.SELL].GetComponent<SellScreenController>().NewPartSell();
    }
    public void SellPart()
    {
        //Gets the value of the part
        int value = GetPartValue(invParts[selectIndex].rarity);

        //Adds the tokens for the sell
        tokens += value;
        GameController.tokenCollected += value;

        //Removes the part from the player's inventory
        DeleteInvPart(selectIndex);
        CondenseInvParts();

        //Changes so that no part is selected
        selectIndex = -1;

        InterfaceController.intCont.screenPair[InterfaceController.Screen.SELL].GetComponent<SellScreenController>().NewPartSell();
        InterfaceController.intCont.screenPair[InterfaceController.Screen.HUD].GetComponent<HUDScreenController>().NewToken(tokens);
    }

    public static int GetPartValue(SodaPart.Rarity rarity)
    {
        switch (rarity)
        {
            case SodaPart.Rarity.COMMON:
                return 2;
            case SodaPart.Rarity.UNCOMMON:
                return 4;
            case SodaPart.Rarity.RARE:
                return 10;
            case SodaPart.Rarity.EPIC:
                return 20;
            case SodaPart.Rarity.LEGENDARY:
                return 30;
            default:
                return 0;
        }
    }
    public void ClearSelectIndex()
    {
        selectIndex = -1;
    }

    #endregion

    #region Token Methods

    public void AddTokens(int i)
    {
        if(i > 0)
        {
            GameController.tokenCollected += i;

            if(!CommandConsoleController.commandCont.allowCmd)
                ValueStoreController.totalData.totalTokensEarned += i;
        }
        else
        {
            GameController.tokenSpent += i;

            if (!CommandConsoleController.commandCont.allowCmd)
                ValueStoreController.totalData.totalTokensSpent += i;
        }

        tokens = Mathf.Clamp(tokens + i, 0, 10000);

        InterfaceController.intCont.screenPair[InterfaceController.Screen.HUD].GetComponent<HUDScreenController>().NewToken(tokens);
    }

    #endregion

    #region Saving Methods

    public InventoryData GetSaveData()
    {

        InventoryData invData = new InventoryData();

        invData.tokens = tokens;
        invData.invParts = invParts;

        return invData;
    }

    #endregion
}
