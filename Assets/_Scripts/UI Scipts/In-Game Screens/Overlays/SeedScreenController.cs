using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SeedScreenController : InterfaceScreenController
{
    public TMP_InputField inputArea;
    public TMP_Text placeholderText;

    private const string placeholderMessage = "Name";

    public override void ShowScreen()
    {
        base.ShowScreen();

        inputArea.textComponent.text = "";
        inputArea.text = "";
        placeholderText.text = placeholderMessage;
    }

    public void SaveSeed(string inputText)
    {
        if (inputText != string.Empty)
        {
            List<string> nameList = SaveDataStore.GetSeedNames();

            if (!nameList.Contains(inputText))
            {
                ValueStoreController.SaveGameData(2, inputText);
                HideScreen();

                inputArea.textComponent.text = "";
                inputArea.text = "";
                placeholderText.text = placeholderMessage;
            }
            else
            {
                inputArea.textComponent.text = "";
                inputArea.text = "";
                placeholderText.text = "In Use";
            }
        }
        else
        {
            inputArea.textComponent.text = "";
            inputArea.text = "";
            placeholderText.text = "Invalid";
        }
    }
}
