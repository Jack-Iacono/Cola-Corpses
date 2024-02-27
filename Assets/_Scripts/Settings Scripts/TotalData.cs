using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TotalData
{
    
    public int totalKills = 0;
    public int totalTokensSpent = 0;
    public int totalTokensEarned = 0;

    public TotalData()
    {
        totalKills = 0;
        totalTokensSpent = 0;
        totalTokensEarned = 0;
    }
    public void AddData(TotalData data)
    {
        if (!CommandConsoleController.commandCont.allowCmd)
        {
            totalKills += data.totalKills;
            totalTokensSpent += data.totalTokensSpent;
            totalTokensEarned += data.totalTokensEarned;
        }
    }

}
