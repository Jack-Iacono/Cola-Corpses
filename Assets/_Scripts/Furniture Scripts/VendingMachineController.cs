using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VendingMachineController : InteractableStationController
{
    public static VendingMachineController VendInstance { get; private set; }

    public int vendIndex { get; private set; } = -1;
    private int roomDistance = -1;

    public bool generateStatic = false;

    public int currentPart { get; private set; } = -1;

    public SodaPart[] stockList { get; private set; } = new SodaPart[12];
    public int[] priceList { get; private set; } = new int[12];

    public override void Initialize()
    {
        base.Initialize();

        if (generateStatic)
        {
            for (int i = 0; i < priceList.Length; i++)
            {
                priceList[i] = -1;
            }

            GenerateStockStatic();
        }
    }
    protected override void Interact()
    {
        VendInstance = this;

        base.Interact();
    }

    // Start is called before the first frame update
    public void Activate(int rDist, int vID)
    {
        roomDistance = rDist;
        vendIndex = vID;

        if (GameController.gameCont.vendList[vendIndex] != null)   
        {
            VendingMachineData vendData = GameController.gameCont.vendList[vendIndex];

            stockList = vendData.stockList;
            priceList = vendData.priceList;
        }
        else
        {
            for (int i = 0; i < priceList.Length; i++)
            {
                priceList[i] = -1;
            }

            GenerateStock();
            GameController.gameCont.vendList[vendIndex] = GetVendData();
        }

        //Lets the game controller access the machine for restocking
        GameController.gameCont.vendObjList.Add(this);
    }

    private void GenerateStock()
    {
        //Generates the stock present at the beginning of the game
        int stockSize = Random.Range(1, 5);

        //Change to generate random rarities later
        for(int i = 0; i < stockSize; i++)
        {
            int rand = Mathf.FloorToInt(Random.Range(0, 2));
            stockList[i] = new SodaPart((SodaPart.Rarity)rand);
            Debug.Log(rand);
            switch (stockList[i].rarity)
            {
                case SodaPart.Rarity.COMMON:
                    priceList[i] = Mathf.RoundToInt(Random.Range(5,20));
                    break;
                case SodaPart.Rarity.UNCOMMON:
                    priceList[i] = Mathf.RoundToInt(Random.Range(10, 25));
                    break;
                case SodaPart.Rarity.RARE:
                    priceList[i] = Mathf.RoundToInt(Random.Range(20, 40));
                    break;
                case SodaPart.Rarity.EPIC:
                    priceList[i] = Mathf.RoundToInt(Random.Range(50, 100));
                    break;
                case SodaPart.Rarity.LEGENDARY:
                    priceList[i] = Mathf.RoundToInt(Random.Range(100, 200));
                    break;
            }
        }
    }
    private void GenerateStockStatic()
    {
        stockList[0] = new SodaPart(SodaPart.Rarity.COMMON);
        stockList[1] = new SodaPart(SodaPart.Rarity.UNCOMMON);
        stockList[2] = new SodaPart(SodaPart.Rarity.RARE);
        stockList[3] = new SodaPart(SodaPart.Rarity.EPIC);
        stockList[4] = new SodaPart(SodaPart.Rarity.LEGENDARY);

        priceList[0] = 1;
        priceList[1] = 1;
        priceList[2] = 1;
        priceList[3] = 1;
        priceList[4] = 1;
    }
    public void RefreshStock(int wave)
    {
        //Adds stock at the beginning of every round

        List<int> openSpaces = new List<int>();
        for(int i = 0; i < stockList.Length; i++)
        {
            if (stockList[i] == null)
                openSpaces.Add(i);
        }

        if (openSpaces.Count > 0)
        {
            //Sets the amount of new sodas that will be added
            int stockSize = Mathf.Clamp(Random.Range(1, 4), 1, openSpaces.Count);

            //Change to generate random rarities later
            for (int i = 0; i < stockSize; i++)
            {
                int rand = Mathf.FloorToInt(Random.Range(Mathf.Clamp(0 + Mathf.FloorToInt(wave / 5), 0, 3), 5));
                if(rand == 5)
                {
                    rand = 6;
                }

                stockList[openSpaces[i]] = new SodaPart((SodaPart.Rarity)rand);

                switch (stockList[openSpaces[i]].rarity)
                {
                    case SodaPart.Rarity.COMMON:
                        priceList[openSpaces[i]] = Mathf.RoundToInt(Random.Range(5, 20));
                        break;
                    case SodaPart.Rarity.UNCOMMON:
                        priceList[openSpaces[i]] = Mathf.RoundToInt(Random.Range(10, 25));
                        break;
                    case SodaPart.Rarity.RARE:
                        priceList[openSpaces[i]] = Mathf.RoundToInt(Random.Range(20, 40));
                        break;
                    case SodaPart.Rarity.EPIC:
                        priceList[openSpaces[i]] = Mathf.RoundToInt(Random.Range(50, 100));
                        break;
                    case SodaPart.Rarity.LEGENDARY:
                        priceList[openSpaces[i]] = Mathf.RoundToInt(Random.Range(100, 200));
                        break;
                }
            }
        }

    }
    public void WipeStock()
    {
        for(int i = 0; i < stockList.Length; i++)
        {
            stockList[i] = null;
        }
    }

    #region Menu Methods

    public void SetCurrentPart(int index)
    {
        currentPart = index;
    }
    public void VendPart()
    {
        if (InventoryController.invCont.CheckInvOpen() && InventoryController.invCont.tokens >= priceList[currentPart] && currentPart != -1)
        {
            InventoryController.invCont.AddInvPart(stockList[currentPart]);
            InventoryController.invCont.AddTokens(-priceList[currentPart]);

            stockList[currentPart] = null;

            currentPart = -1;

            InterfaceController.intCont.screenPair[InterfaceController.Screen.VEND].GetComponent<VendScreenController>().NewVend();
            InterfaceController.intCont.screenPair[InterfaceController.Screen.VEND].GetComponent<VendScreenController>().DisplayVendPart(-1);

            SoundManager.PlaySoundSource(transform.position, SoundManager.success);
        }
        else
        {
            SoundManager.PlaySoundSource(transform.position, SoundManager.error);
        }

        if(!ValueStoreController.isTutorial)
            GameController.gameCont.vendList[vendIndex] = GetVendData();
    }

    #endregion

    #region Saving
    public VendingMachineData GetVendData()
    {
        VendingMachineData vendData = new VendingMachineData();

        vendData.vendIndex = vendIndex;
        vendData.roomDistance = roomDistance;
        vendData.stockList = stockList;
        vendData.priceList = priceList;

        return vendData;
    }
    #endregion
}
