using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{

    #region Gameobjects

    public static GameController gameCont;

    private static List<MonoBehaviour> observers = new List<MonoBehaviour>();

    #endregion

    public static string mainMenu = "Main Menu";
    public static string startingRoom = "Generator Level";

    public static int totalKills { get; private set; } = 0;
    public static int tokenSpent { get; set; } = 0;
    public static int tokenCollected { get; set; } = 0;

    public int roundKills { get; set; } = 0;
    public int roundSpawned { get; set; } = 0;

    public static int waveNum { get; set; } = 0;

    //These are the amount of kills required for the next wave to start
    //These numbers represent the static wave amounts throughout the game, but the variable below is for the rounds surpassing those listed
    private int[] waveThresholds = new int[] { 10, 25, 35, 45, 60, 75, 90, 120, 150, 200, 250, 300, 350 };

    public float totalTime { get; private set; } = 0.00f;

    #region Furniture Variables
    /*  This list assigns the indexes to furniture around to map
     * 
     * 0: Vending Machine
     * 
    */
    private List<int> furnIndexList = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0 };
    public List<VendingMachineData> vendList { get; set; } = new List<VendingMachineData>();
    public List<VendingMachineController> vendObjList = new List<VendingMachineController>();

    #endregion

    #region Easter Egg Related Variables

    /*Controls which easter egg elements are in the game currently
     * 
     * 0: Find Parts
     * 1: Soul Boxes
     * 2: Wall Codes
     * 3: Targets
     * 4: Moving Soul Box
     * 5: Hide and Seek
     * 6: Total Check
     * 7: Alternate Ammo Proc
     * 
    */
    public bool[] activeEE { get; private set; } =  { false, false, false, false, false, false };
    public static string[] eeNames = 
        { 
            "Collect Presents",
            "Fill Coolers",
            "Check Posters",
            "Hit Targets",
            "Fill-Up Cups",
            "Find Some Fruit"
        };

    public List<List<int>> checkList { get; set; } = new List<List<int>>();

    //This list stores the remaining moves for the moving easter egg objects
    /*
     * 0: Moving Soul Box
     * 1: Hide and Seek
    */

    private List<int> indexList = new List<int>{ 0, 0, 0, 0, 0, 0};

    //Controls when the easter egg steps are complete
    private List<bool> completionList = new List<bool>();
    #endregion

    //Controls if the game is paused
    public static bool gamePaused { get; set; } = false;
    public bool gameEnd { get; private set; } = false;

    #region Initialization
    //Setting the static reference
    public void SetStaticInstance()
    {
        //Creates a singelton
        if (gameCont != null && gameCont != this)
        {
            Destroy(this);
        }
        else
        {
            gameCont = this;
        }
    }
    public void Initialize()
    {
        if (ValueStoreController.loadData)
        {
            GameData data = ValueStoreController.loadedGameData.gameData;

            totalKills = data.totalKills;
            tokenSpent = data.tokenSpent;
            tokenCollected = data.tokenCollected;
            totalTime = data.totalTime;
            waveNum = data.waveNum;
            roundKills = data.roundKills;
            roundSpawned = data.roundSpawned;

            activeEE = data.activeEE;
            checkList = data.checkList;

            vendList = data.vendList;

            //Need this so game toggles to true
            GameController.gamePaused = false;
            InterfaceController.intCont.TogglePauseMenu(true);

            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            //Initializes the array used to check what steps are complete
            for(int i = 0; i < activeEE.Length; i++)
            {
                checkList.Add(new List<int>());
            }

            //Generates a random set of easter egg steps
            for (int i = 0; i < 3; i++)
            {
                List<int> openSteps = new List<int>();

                for (int j = 0; j < activeEE.Length; j++)
                {
                    if (!activeEE[j])
                        openSteps.Add(j);
                }

                if(openSteps.Count > 0)
                {
                    //Sets the selected steps to be true on the array
                    activeEE[openSteps[Random.Range(0, openSteps.Count)]] = true;
                }
                
            }

            InterfaceController.intCont.TogglePauseMenu(false);

            gamePaused = false;
            Cursor.lockState = CursorLockMode.None;
        }

        //Initializes the completion list
        for(int i = 0; i < checkList.Count; i++)
        {
            completionList.Add(false);
        }

        
    }
    #endregion

    private void OnDisable()
    {
        Debug.Log("Cleared");
        observers.Clear();
    }

    // Update is called once per frame
    void Update()
    {
        if (!gamePaused)
        {
            totalTime += Time.deltaTime;
        }

        //Pauses the game
        if (Input.GetKeyDown(ValueStoreController.keyData.keyPause) && !gameEnd && !CommandConsoleController.commandLineActive)
        {
            gamePaused = !gamePaused;
            PauseGame(gamePaused);
        }
    }

    #region Enemy Methods
    public void AddKill()
    {
        if (!CommandConsoleController.commandCont.allowCmd)
            ValueStoreController.totalData.totalKills++;

        totalKills++;

        roundKills++;

        //If controls waves before max wave number and else controls everything after
        if (waveNum < waveThresholds.Length - 1)
        {
            if (roundKills == waveThresholds[waveNum])
            {
                waveNum++;

                roundKills = 0;
                roundSpawned = 0;

                InterfaceController.intCont.screenPair[InterfaceController.Screen.HUD].GetComponent<HUDScreenController>().NewWave((waveNum + 1).ToString());
                
                foreach (VendingMachineController v in vendObjList)
                {
                    v.RefreshStock(waveNum + 1);
                }
            }
        }
        else
        {
            if (roundKills == waveThresholds[waveThresholds.Length - 1])
            {
                waveNum++;

                roundKills = 0;
                roundSpawned = 0;

                InterfaceController.intCont.screenPair[InterfaceController.Screen.HUD].GetComponent<HUDScreenController>().NewWave((waveNum + 1).ToString());
            }
        }
    }
    public void RemoveEnemy()
    {
        roundSpawned--;
    }
    public bool EnemySpawnAvailable()
    {
        //Allows a spawn if there are less enemies killed and spawned than are in the wave
        if(waveNum < waveThresholds.Length - 1 && roundKills + roundSpawned < waveThresholds[waveNum])
        {
            return true;
        }
        else if(waveNum >= waveThresholds.Length - 1 && roundKills + roundSpawned < waveThresholds[waveThresholds.Length-1])
        {
            return true;
        }
            
        return false;
    }
    #endregion

    #region Easter Egg Methods

    #region Indexing Methods
    public int GetIndex(int type)
    {
        int i = indexList[type];
        indexList[type]++;

        return i;
    }
    public int GetFurnIndex(int type)
    {
        int i = furnIndexList[type];
        furnIndexList[type]++;

        return i;
    }
    #endregion

    [Tooltip("part\nsoulBox\nwallCode\ntarget\nmovingSoulBox\nhideAndSeek")]
    public void UpdateChecklist(string type, int index, int value)
    {
        int typeInt = GetEEIndex(type);

        checkList[typeInt][index] = value;

        CheckCompletionList(typeInt);
    }

    public void CheckCompletionList(int index)
    {
        bool check = true;

        for(int i = 0; i < checkList[index].Count; i++)
        {
            if (checkList[index][i] >= 0)
            {
                check = false;
            }
        }

        if (check)
            Debug.Log(index + " complete");

        completionList[index] = check;
    }
    public static int GetEEIndex(string name)
    {
        int typeInt = -1;

        switch (name)
        {
            case "part":
                typeInt = 0;
                break;
            case "soulBox":
                typeInt = 1;
                break;
            case "wallCode":
                typeInt = 2;
                break;
            case "target":
                typeInt = 3;
                break;
            case "movingSoulBox":
                typeInt = 4;
                break;
            case "hideAndSeek":
                typeInt = 5;
                break;
        }

        return typeInt;
    }

    public void ResetList(string name)
    {
        int typeIndex = GetEEIndex(name);

        for(int i = 0; i < checkList[typeIndex].Count; i++)
        {
            checkList[typeIndex][i] = 1;
        }
    }

    #endregion

    #region Game Flow Methods

    public void EndGame(bool win)
    {
        gamePaused = true;
        gameEnd = true;

        CommandConsoleController.commandCont.ToggleCommandBar(false);

        //Shows Death/Win Screen depending on the variable Screen
        if (win)
        {
            InterfaceController.intCont.SetUpScreen(InterfaceController.Screen.WIN);
        }
        else
        {
            InterfaceController.intCont.SetUpScreen(InterfaceController.Screen.LOSE);
        }

        //Deletes the saved data due to game ending
        SaveDataStore.DeleteData(0);

        Cursor.lockState = CursorLockMode.None;
    }
    public bool CheckEndCriteria()
    {
        List<bool> completionList = new List<bool>();

        //Checks every relevant number in the list and checks whether everything is complete
        for(int i = 0; i < activeEE.Length; i++)
        {
            if (activeEE[i] == true)
            {
                bool check = true;

                foreach(int num in checkList[i])
                {
                    if (num >= 0)
                        check = false;
                }

                completionList.Add(check); 
            }
        }

        if (!completionList.Contains(false))
            return true;
        else
            return false;
    }

    public void CompleteObjectives()
    {
        for (int i = 0; i < activeEE.Length; i++)
        {
            if (activeEE[i] == true)
            {
                for(int j = 0; j < checkList[i].Count; j++)
                {
                    checkList[i][j] = -1;
                }
            }
        }
    }
    public void CompleteObjectives(int index)
    {
        for (int i = 0; i < activeEE.Length; i++)
        {
            if (activeEE[i] == true && i == index)
            {
                for (int j = 0; j < checkList[i].Count; j++)
                {
                    checkList[i][j] = -1;
                }
            }
        }
    }

    #endregion

    #region Wave Methods

    public void StartWave(int i)
    {
        waveNum = i;

        roundKills = 0;
        roundSpawned = 0;

        InterfaceController.intCont.screenPair[InterfaceController.Screen.HUD].GetComponent<HUDScreenController>().NewWave((waveNum + 1).ToString());

        foreach (VendingMachineController v in vendObjList)
        {
            v.RefreshStock(waveNum + 1);
        }
    }
    public void SetWave(int i)
    {
        waveNum = i;
        InterfaceController.intCont.screenPair[InterfaceController.Screen.HUD].GetComponent<HUDScreenController>().NewWave((waveNum + 1).ToString());
    }

    #endregion

    #region Vending Machine Methods

    public void WipeVendingMachines()
    {
        foreach (VendingMachineController v in vendObjList)
        {
            v.WipeStock();
        }
    }
    public void ForceVendingMachineRestock()
    {
        foreach (VendingMachineController v in vendObjList)
        {
            v.RefreshStock(waveNum + 1);
        }
    }

    #endregion

    #region Observer Methods

    public static void RegisterObserver(MonoBehaviour o)
    {
        if(!observers.Contains(o))
            observers.Add(o);
    }
    public static void UnRegisterObserver(MonoBehaviour o)
    {
        if(observers.Contains(o))
            observers.Remove(o);
    }

    private static void NotifyObservers(string eventName, object o)
    {
        foreach(MonoBehaviour m in observers)
        {
            m.SendMessage(eventName, o, SendMessageOptions.DontRequireReceiver);
        }
    }
    private static void NotifyObservers(string eventName)
    {
        foreach (MonoBehaviour m in observers)
        {
            m.SendMessage(eventName, SendMessageOptions.DontRequireReceiver);
        }
    }

    #endregion

    public static void PauseGame(bool b)
    {
        gamePaused = b;
        NotifyObservers("GamePause", b);
    }

    public GameData SaveData()
    {
        GameData data = new GameData();

        data.totalKills = totalKills;
        data.tokenSpent = tokenSpent;
        data.tokenCollected = tokenCollected;
        data.totalTime = totalTime;
        data.waveNum = waveNum;
        data.roundKills = roundKills;
        data.roundSpawned = roundSpawned;

        data.activeEE = activeEE;
        data.checkList = checkList;

        data.vendList = vendList;

        return data;
    }
}
