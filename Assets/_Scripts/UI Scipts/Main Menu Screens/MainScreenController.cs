using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class MainScreenController : InterfaceScreenController
{
    #region Slider Objects

    public TMP_Text difficultyText;
    private int difficulty = 0;
    
    #endregion

    public override void InitializeScreen()
    {
        base.InitializeScreen();

        difficultyText.text = GetDifficultyName(difficulty);
    }

    public void ChangeDifficulty()
    {
        if (difficulty < MapGenerationController.mapTileCounts.Length - 1)
            difficulty++;
        else
            difficulty = 0;

        difficultyText.text = GetDifficultyName(difficulty);
        ValueStoreController.mapDiffIndex = difficulty;
    }
    private string GetDifficultyName(int i)
    {
        switch (i)
        {
            case 0:
                return "Easy";
            case 1:
                return "Medium";
            case 2:
                return "Hard";
        }

        return "Ya Mom";
    }
}
