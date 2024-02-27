using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatScreenController : InterfaceScreenController
{

    public TMP_Text statText;
    public GameObject statArea;

    public override void InitializeScreen()
    {
        UpdateStats();
    }

    public override void ShowScreen()
    {
        base.ShowScreen();

        UpdateStats();
    }
    public override void HideScreen()
    {
        base.HideScreen();
    }

    public void UpdateStats()
    {
        TotalData total = ProfileSaveController.LoadTotalData(ValueStoreController.fileOwner);
        statText.text = "<color=blue>Kills:</color>\nTotal: " + total.totalKills + "\n<color=blue>Tokens:</color>\nEarned: " + total.totalTokensEarned + "\nSpent: " + total.totalTokensSpent;
    }
    public void ResetData()
    {
        ValueStoreController.totalData = new TotalData();
        ValueStoreController.SaveProfileData();
        UpdateStats();
    }
}
