using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class CommandConsoleController : MonoBehaviour
{
    //This script will allow me to bug test more easily
    public static CommandConsoleController commandCont;

    private const KeyCode keyCommand = KeyCode.Slash;

    public bool allowCmd = false;
    public GameObject commandScreen;

    public GameObject scrollView;
    public GameObject contentArea;

    public TMP_InputField inputField;
    public TMP_Text inputFieldPlaceholder;

    public static bool commandLineActive = false;
    private List<string> commandHistory = new List<string>();

    private char commandChar = '>';

    #region Initialization

    public void SetStaticInstance()
    {
        if(commandCont != null && commandCont != this)
        {
            Destroy(this);
        }
        else
        {
            commandCont = this;
        }
    }
    public void Initialize()
    {
        inputField.textComponent.text = "";
        inputField.text = "";
        inputFieldPlaceholder.text = "Enter Command";

        commandScreen.SetActive(commandLineActive);
        UpdateOutputText();
    }
    private void OnDestroy()
    {
        commandCont = null;
    }

    #endregion

    private void Update()
    {
        if (allowCmd && Input.GetKeyDown(keyCommand) && Input.GetKey(KeyCode.LeftShift))
        {
            ToggleCommandBar();
        }
    }

    public void InputCommand(string command)
    {
        if(command != "")
        {
            if (command[0] == commandChar)
            {
                command = command.Remove(0, 1);
                string[] commandSplit = command.Split(" ");

                //Runs the command that is input
                switch (commandSplit[0])
                {

                    case "sethealth":
                        try
                        {
                            SetHealth(int.Parse(commandSplit[1]));
                            commandHistory.Add("Health set to " + commandSplit[1]);
                        }
                        catch (System.Exception)
                        {
                            commandHistory.Add("Incorrect Syntax for SetHealth command. Syntax is: SetHealth <int: health>");
                        }
                        break;
                    case "invincible":
                        try
                        {
                            SetInvincible(bool.Parse(commandSplit[1]));
                            commandHistory.Add("Set Invincible: " + commandSplit[1]);
                        }
                        catch (System.Exception)
                        {
                            commandHistory.Add("Incorrect Syntax for Invincible command. Syntax is: Invincible <bool>");
                        }
                        break;
                    case "settokens":
                        try
                        {
                            SetTokens(int.Parse(commandSplit[1]));
                            commandHistory.Add("Tokens set to " + commandSplit[1]);
                        }
                        catch (System.Exception)
                        {
                            commandHistory.Add("Incorrect Syntax for SetTokens command. Syntax is: SetTokens <int: tokens>");
                        }
                        break;
                    case "addpart":
                        try
                        {
                            AddPart
                            (
                            int.Parse(commandSplit[1]),
                            int.Parse(commandSplit[2]),
                            int.Parse(commandSplit[3]),
                            int.Parse(commandSplit[4]),
                            commandSplit[5],
                            commandSplit[6],
                            commandSplit[7]
                            );
                            commandHistory.Add("Part Added");
                        }
                        catch (System.Exception)
                        {
                            commandHistory.Add("Incorrect Syntax for addpart command. Syntax is: addpart <int: damage> <int: throwRate> <int: throwSpeed> <int: explodeRadius> <Trait: trait1> <Trait: trait2>");
                            throw;
                        }
                        break;
                    case "completeobjective":
                        try
                        {
                            CompleteObjectives(int.Parse(commandSplit[1]));
                        }
                        catch (System.Exception)
                        {
                            commandHistory.Add("Incorrect Syntax for completeobjective command. Syntax is: completeobjective <int: index>");
                        }
                        break;
                    case "despawnenemies":
                        try
                        {
                            DespawnEnemies();
                            commandHistory.Add("Enemies Despawned");
                        }
                        catch (System.Exception)
                        {
                            commandHistory.Add("Incorrect Syntax for despawnenemies command. Syntax is: despawnenemies");
                            throw;
                        }
                        break;
                    case "startwave":
                        try
                        {
                            StartWave(int.Parse(commandSplit[1]));
                            commandHistory.Add("Wave " + commandSplit[1] + " started");
                        }
                        catch (System.Exception)
                        {
                            commandHistory.Add("Incorrect Syntax for startwave command. Syntax is: startwave <int: wave>");
                            throw;
                        }
                        break;
                    case "setwave":
                        try
                        {
                            SetWave(int.Parse(commandSplit[1]));
                            commandHistory.Add("Set Wave " + commandSplit[1]);
                        }
                        catch (System.Exception)
                        {
                            commandHistory.Add("Incorrect Syntax for setwave command. Syntax is: setwave <int: wave>");
                            throw;
                        }
                        break;
                    case "vendwipe":
                        try
                        {
                            WipeVend();
                            commandHistory.Add("Vending Machines Wiped");
                        }
                        catch (System.Exception)
                        {
                            commandHistory.Add("Incorrect Syntax for wipevend command. Syntax is: wipevend");
                            throw;
                        }
                        break;
                    case "vendrestock":
                        try
                        {
                            RestockVend();
                            commandHistory.Add("Vending Machines Restocked");
                        }
                        catch (System.Exception)
                        {
                            commandHistory.Add("Incorrect Syntax for vendrestock command. Syntax is: vendrestock");
                            throw;
                        }
                        break;
                    case "revealmap":
                        try
                        {
                            RevealMinimap();
                            commandHistory.Add("Minimap Revealed");
                        }
                        catch (System.Exception)
                        {
                            commandHistory.Add("Incorrect Syntax for revealmap command. Syntax is: revealmap");
                            throw;
                        }
                        break;
                    case "setweapon":
                        try
                        {
                            SetParts(int.Parse(commandSplit[1]));
                            commandHistory.Add("Set Weapon " + commandSplit[1]);
                        }
                        catch (System.Exception)
                        {
                            commandHistory.Add("Incorrect Syntax for setweapon command. Syntax is: setweapon <int: rarity>");
                            throw;
                        }
                        break;
                    default:
                        commandHistory.Add("Unrecognized Command: " + command);
                        break;
                }
            }
            else
            {
                commandHistory.Add(command);
            }

            inputField.textComponent.text = "";
            inputField.text = "";
            inputFieldPlaceholder.text = "Enter Command";

            UpdateOutputText();
        }
    }

    #region Command Bar Toggles
    public void ToggleCommandBar()
    {
        //Toggles the command line active or inactive
        commandLineActive = !commandLineActive;
        ToggleCommandBar(commandLineActive);
    }
    public void ToggleCommandBar(bool b)
    {
        //Toggles the command line active or inactive
        commandLineActive = b;

        commandScreen.SetActive(commandLineActive);

        if (commandLineActive)
        {
            inputField.Select();
            inputField.ActivateInputField();
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            GameController.PauseGame(true);
        }
    }
    #endregion

    public void UpdateOutputText()
    {
        string fullString = "";
        int lineCount = 0;

        foreach (string str in commandHistory)
        {
            fullString += str + "\n";
            if (str.Length > 53)
                lineCount += 2;
            else
                lineCount++;
        }

        scrollView.GetComponentInChildren<TMP_Text>().text = fullString;
        contentArea.GetComponent<RectTransform>().sizeDelta = new Vector2(1, lineCount * 25);

        scrollView.GetComponent<ScrollRect>().verticalNormalizedPosition = 0;
    }

    #region Player Methods
    public void SetHealth(int i)
    {
        PlayerController.playerCont.SetHealth(i);
    }
    public void SetInvincible(bool b)
    {
        PlayerController.playerCont.invincible = b;
    }
    #endregion

    #region Inventory Methods

    public void SetTokens(int i)
    {
        //Changes the value then refreshes the displays
        InventoryController.invCont.tokens = i;
        InventoryController.invCont.AddTokens(0);
    }
    public void AddPart(int damage, int throwRate, int throwSpeed, int explodeRadius, string trait1, string trait2, string flavor)
    {
        InventoryController.invCont.AddInvPart(new SodaPart(damage,explodeRadius,throwSpeed,throwRate,trait1,trait2,flavor));
    }
    public void SetParts(int i)
    {
        SodaPart[] parts =
            {
                new SodaPart((SodaPart.Rarity)i),
                new SodaPart((SodaPart.Rarity)i),
                new SodaPart((SodaPart.Rarity)i),
                new SodaPart((SodaPart.Rarity)i)
            };

        WeaponController.weaponCont.SetParts(parts);
    }

    #endregion

    #region Gameplay Methods

    public void CompleteObjectives(int i)
    {
        if(i >= 0)
        {
            GameController.gameCont.CompleteObjectives(i);
            commandHistory.Add("Objective Complete: " + GameController.eeNames[i]);
        }
        else
        {
            GameController.gameCont.CompleteObjectives();
            commandHistory.Add("All Objectives Complete");
        }
    }
    public void SetWave(int i)
    {
        GameController.gameCont.SetWave(i);
    }
    public void StartWave(int i)
    {
        DespawnEnemies();
        GameController.gameCont.StartWave(i);
    }

    #endregion

    #region Vending Machine Methods

    public void WipeVend()
    {
        GameController.gameCont.WipeVendingMachines();
    }
    public void RestockVend()
    {
        GameController.gameCont.ForceVendingMachineRestock();
    }

    #endregion

    #region Enemy Methods

    public void DespawnEnemies()
    {
        Enemy.DespawnEnemies();
    }

    #endregion

    #region Map Methods

    public void RevealMinimap()
    {
        MapGenerationController.mapGenCont.map.RevealMinimap();
    }

    #endregion

}
