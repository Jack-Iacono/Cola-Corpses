using System.Buffers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;
using System.Linq;

public class WeaponController : MonoBehaviour
{

    public static WeaponController weaponCont;

    #region Gameobject

    public GameObject canObject;
    private ObjectPool objPool;
    private GameController gameCont;
    private SoundManager soundCont;
    private AudioSource audioSrc;
    public PlayerController player { get; set; }
    public CameraController cam;
    public GameObject hand;
    private InterfaceController intCont;

    #endregion

    #region Properties

    public int damage { get; set; } = 10;
    public float explodeRadius = 2;
    public float throwRate = 0.05f;
    public float throwSpeed = 10f;
    public int canPerThrow = 1;
    public bool autoThrow = true;

    //Need these for easy stat displaying
    public int rawDamage;
    public int rawExplodeRadius;
    public int rawThrowSpeed;
    public int rawThrowRate;

    private float damageMod = 1;

    private SodaPart[] sodaParts = new SodaPart[4];

    public float drinkSpeed = 1;

    public int[] sodaTraits { get; private set; } = new int[Enum.GetNames(typeof(SodaPart.Trait)).Length];
    public FlavorData[] sodaFlavors { get; private set; } = new FlavorData[Enum.GetNames(typeof(SodaPart.Flavor)).Length];

    /*
     * Buff Timers
     * 0: Cherry
     * 1: Orange
     * 2: Blue Raz
     * 3: Cola
     * 4: Rootbeer
     * 5: Grapefruit
     * 6: Banana
    */
    public BuffData buffData { get; private set; } = new BuffData(Enum.GetNames(typeof(SodaPart.Flavor)).Length); 

    public Animator weaponAnim;

    #endregion

    #region Behavior Tree Stuff

    Tree behaviorTree = new Tree("ThrowTree");

    Sequence throwActions;
    Sequence drinkActions;
    Sequence noAction;

    Node.Status status = Node.Status.RUNNING;

    private float throwRateTimer = -1f;
    private float drinkSpeedTimer = -1f;

    #endregion

    private void Start()
    {
        //Creating a singleton
        if (weaponCont != null && weaponCont != this)
        {
            Destroy(this);
        }
        else
        {
            weaponCont = this;
        }
    }
    public void Initialize()
    {
        player = FindObjectOfType<PlayerController>();
        objPool = FindObjectOfType<ObjectPool>();
        soundCont = FindObjectOfType<SoundManager>();
        gameCont = GameController.gameCont;
        intCont = FindObjectOfType<InterfaceController>();
        audioSrc = GetComponent<AudioSource>();

        //Change to initialize later
        #region Behavior Tree

        throwActions = new Sequence("ThrowCan");
        throwActions.AddChild(new Action("Throw", Throw));
        throwActions.AddChild(new Action("WaitThrow", WaitThrow));

        drinkActions = new Sequence("DrinkCan");
        drinkActions.AddChild(new Action("WaitDrink", WaitDrink));
        drinkActions.AddChild(new Action("Drink", Drink));

        noAction = new Sequence("NoAction");
        noAction.AddChild(new Action("WaitNull", WaitNull));

        behaviorTree = new Tree("ThrowTree");
        behaviorTree.AddChild(throwActions);
        behaviorTree.AddChild(drinkActions);
        behaviorTree.AddChild(noAction);

        behaviorTree.StartSequence("NoAction");

        #endregion

        if (ValueStoreController.loadData)
        {
            WeaponData data = ValueStoreController.loadedGameData.weaponData;

            if(data != null)
            {
                for(int i = 0; i < data.sodaParts.Length; i++)
                {
                    if (data.sodaParts[i] != null)
                    {
                        sodaParts[i] = data.sodaParts[i];
                    }
                }
            }
            else
            {
                sodaParts[0] = new SodaPart(SodaPart.Rarity.COMMON, new SodaPart.Flavor[] { SodaPart.Flavor.Orange, SodaPart.Flavor.Grapefruit, SodaPart.Flavor.Cola });
                sodaParts[1] = new SodaPart(SodaPart.Rarity.COMMON, new SodaPart.Flavor[] { SodaPart.Flavor.Orange, SodaPart.Flavor.Grapefruit, SodaPart.Flavor.Cola });
                sodaParts[2] = new SodaPart(SodaPart.Rarity.COMMON, new SodaPart.Flavor[] { SodaPart.Flavor.Orange, SodaPart.Flavor.Grapefruit, SodaPart.Flavor.Cola });
                sodaParts[3] = null;
            }
        }
        else
        {
            sodaParts[0] = new SodaPart(SodaPart.Rarity.COMMON, new SodaPart.Flavor[] { SodaPart.Flavor.Orange, SodaPart.Flavor.Grapefruit, SodaPart.Flavor.Cola });
            sodaParts[1] = new SodaPart(SodaPart.Rarity.COMMON, new SodaPart.Flavor[] { SodaPart.Flavor.Orange, SodaPart.Flavor.Grapefruit, SodaPart.Flavor.Cola });
            sodaParts[2] = new SodaPart(SodaPart.Rarity.COMMON, new SodaPart.Flavor[] { SodaPart.Flavor.Orange, SodaPart.Flavor.Grapefruit, SodaPart.Flavor.Cola });
            sodaParts[3] = null;
        }

        StatCalc();

        intCont = FindObjectOfType<InterfaceController>();

        //Stops buffs from proccing on load
        for(int i = 0; i < buffData.buffTimers.Length; i++)
        {
            buffData.buffTimers[i] = -1;
        }
        for (int i = 0; i < buffData.activeBuff.Length; i++)
        {
            buffData.activeBuff[i] = -1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameController.gamePaused)
        {
            if ((Input.GetKey(ValueStoreController.keyData.keyFire) && autoThrow || Input.GetKeyDown(ValueStoreController.keyData.keyFire) && !autoThrow) && behaviorTree.GetCurrentSequenceName() != "ThrowCan")
            {
                behaviorTree.StartSequence("ThrowCan");
            }

            if (Input.GetKeyDown(ValueStoreController.keyData.keyDrink) && behaviorTree.GetCurrentSequenceName() != "ThrowCan")
            {
                behaviorTree.StartSequence("DrinkCan");
            }

            if (status == Node.Status.RUNNING)
            {
                status = behaviorTree.Check();
            }
            else if (status == Node.Status.SUCCESS)
            {

                switch (behaviorTree.GetCurrentSequenceName())
                {

                    default:
                        //Starts the chase sequence if any action finishes, default in case no other action is specified beforehand
                        behaviorTree.StartSequence("NoAction");
                        break;

                }

                status = behaviorTree.Check();
            }

            BuffTimerManager();

            weaponAnim.SetFloat("MoveSpeed", player.currentSpeed);
        }
        else
        {
            weaponAnim.SetFloat("MoveSpeed", 0);
        }
    }

    #region Stat Methods

    private void StatCalc()
    {
        //Clearing the array for claculations
        sodaTraits = new int[Enum.GetNames(typeof(SodaPart.Trait)).Length];
        sodaFlavors = new FlavorData[Enum.GetNames(typeof(SodaPart.Flavor)).Length];

        rawDamage = 0;
        rawExplodeRadius = 0;
        rawThrowRate = 0;
        rawThrowSpeed = 0;

        //Temporarily used to total the flavors for assignment later
        int[] sodaFlav = new int[Enum.GetNames(typeof(SodaPart.Flavor)).Length];

        //Sets the soda trait and flavor arrays
        for (int i = 0; i < sodaParts.Length; i++)
        {
            if (sodaParts[i] != null)
            {
                rawDamage += sodaParts[i].damage;
                rawExplodeRadius += sodaParts[i].explodeRadius;
                rawThrowRate += sodaParts[i].throwRate;
                rawThrowSpeed += sodaParts[i].throwSpeed;

                if (sodaParts[i].trait1 != SodaPart.Trait.None)
                {
                    sodaTraits[(int)sodaParts[i].trait1]++;
                }
                if (sodaParts[i].trait2 != SodaPart.Trait.None)
                {
                    sodaTraits[(int)sodaParts[i].trait2]++;
                }

                sodaFlav[(int)sodaParts[i].flavor]++;
            }
        }

        // Ensures that the player can always do at least 1 damage or have some way to kill enemies
        if (rawDamage <= 0 && sodaFlav[(int)SodaPart.Flavor.Rootbeer] == 0 && sodaTraits[(int)SodaPart.Trait.Fizzy] == 0)
        {
            rawDamage = 1;
        }

        //Assigns FlavorData values to the weapon from the total flavors accumulated
        for(int i = 0; i < sodaFlav.Length; i++)
        {
            if (sodaFlav[i] > 0)
                sodaFlavors[i] = SodaPart.GetFlavorData((SodaPart.Flavor)i, sodaFlav[i]);
            else
                sodaFlavors[i] = null;
        }

        float totalTime = 0;
        //Gets the drink time for the soda
        for(int i = 0; i < sodaFlavors.Length; i++)
        {
            if (sodaFlavors[i] != null)
            {
                totalTime += sodaFlavors[i].drinkTime;
            }
        }
        //Gets the average drink speed
        drinkSpeed = totalTime / 4;

        rawDamage = Mathf.Clamp(rawDamage, 0, 100);
        rawExplodeRadius = Mathf.Clamp(rawExplodeRadius, 0, 100);
        rawThrowRate = Mathf.Clamp(rawThrowRate, 0, 100);
        rawThrowSpeed = Mathf.Clamp(rawThrowSpeed, 0, 100);

        //y = 14x/100 + 1
        //100: 1 -> 15
        damage = rawDamage;

        //-0.0099x + 1
        //100: 1 -> 0.01
        throwRate = -0.0099f * rawThrowRate + 1;

        //0.27x + 3
        //100: 3 -> 30
        throwSpeed = 0.27f * rawThrowSpeed + 3;

        //0.09x + 1
        //100: 1 -> 10
        explodeRadius = 0.09f * rawExplodeRadius + 1;

        //Resets all timers for buffs
        for(int i = 0; i < buffData.activeBuff.Length; i++)
        {
            buffData.activeBuff[i] = -1;
        }
        for(int i = 0; i < buffData.buffTimers.Length; i++)
        {
            buffData.buffTimers[i] = -1;
        }
        CheckBuff();

        intCont.screenPair[InterfaceController.Screen.PAUSE].GetComponent<PauseScreenController>().NewStats(this);

        weaponAnim.SetFloat("ThrowSpeed", Mathf.Clamp(0.01f / throwRate * 120, 0, 10));
    }
    public void SetParts(SodaPart[] parts)
    {
        for(int i = 0; i < parts.Length; i++)
        {
            if (parts[i] != null)
            {
                sodaParts[i] = parts[i];
            }
            else
            {
                sodaParts[i] = null;
            }
        }
        StatCalc();
    }

    #endregion

    #region Flavor Methods

    public string GetFlavorNames()
    {
        string final = "";

        for(int i = 0; i < sodaFlavors.Length; i++)
        {
            if (sodaFlavors[i] != null)
            {
                if (final != "")
                    final += " ";

                final += "<color=" + SodaPart.GetFlavorColor(i) + ">";

                switch (sodaFlavors[i].level)
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
    public void EnableBuff(int index)
    {
        //Sets the buff to active
        buffData.activeBuff[index] = sodaFlavors[index].ticks - 1;

        //Sets the buff timer to the value for the specific buff
        buffData.buffTimers[index] = sodaFlavors[index].time;
        buffData.buffMaxTime[index] = sodaFlavors[index].time;

        //Heals the player by the appropriate amount
        player.ChangeHealth(sodaFlavors[index].health);

        float moveMod = 1;
        damageMod = 1;

        for(int i = 0; i < sodaFlavors.Length; i++)
        {
            if (buffData.activeBuff[i] > -1)
            {
                switch (i)
                {
                    case 0:
                        damageMod += sodaFlavors[i].potency;
                        moveMod -= sodaFlavors[i].antiPotency;
                        break;
                    case 3:
                        player.ChangeHealth((int)-sodaFlavors[i].antiPotency);
                        break;
                    case 1:
                    case 5:
                        damageMod -= sodaFlavors[i].antiPotency;
                        break;
                    case 6:
                        moveMod += sodaFlavors[i].potency;
                        damageMod -= sodaFlavors[i].antiPotency;
                        break;
                }
            }
        }

        damageMod = Mathf.Clamp(damageMod, 0, 100);
        moveMod = Mathf.Clamp(moveMod, 0, 100);

        player.moveMod = moveMod;
    }
    public void DisableBuff(int index)
    {
        float moveMod = 1;
        damageMod = 1;

        //ReChecks the buffs
        for (int i = 0; i < sodaFlavors.Length; i++)
        {
            if (buffData.activeBuff[i] > -1)
            {
                switch (i)
                {
                    case 0:
                        damageMod += sodaFlavors[i].potency;
                        moveMod -= sodaFlavors[i].antiPotency;
                        break;
                    case 1:
                    case 5:
                        damageMod -= sodaFlavors[i].antiPotency;
                        break;
                    case 6:
                        moveMod += sodaFlavors[i].potency;
                        damageMod -= sodaFlavors[i].antiPotency;
                        break;
                }
            }
        }

        damageMod = Mathf.Clamp(damageMod, 0, 100);
        moveMod = Mathf.Clamp(moveMod, 0, 100);

        player.moveMod = moveMod;

        buffData.activeBuff[index] = -1;
        buffData.buffTimers[index] = -1;
    }
    public void CheckBuff()
    {
        float moveMod = 1;
        damageMod = 1;

        for (int i = 0; i < sodaFlavors.Length; i++)
        {
            if (buffData.activeBuff[i] > -1)
            {
                switch (i)
                {
                    case 0:
                        damageMod += sodaFlavors[i].potency;
                        moveMod -= sodaFlavors[i].antiPotency;
                        break;
                    case 1:
                    case 5:
                        damageMod -= sodaFlavors[i].antiPotency;
                        break;
                    case 6:
                        moveMod += sodaFlavors[i].potency;
                        damageMod -= sodaFlavors[i].antiPotency;
                        break;
                }
            }
        }

        damageMod = Mathf.Clamp(damageMod, 0, 100);
        moveMod = Mathf.Clamp(moveMod, 0, 100);

        player.moveMod = moveMod;
    }

    #endregion

    #region Nodes
    public Node.Status Throw()
    {
        weaponAnim.SetTrigger("Throw");

        //This will be changed to use object pooling later
        GameObject can = objPool.GetPooledObject("CanThrown");
        can.SetActive(true);

        intCont.screenPair[InterfaceController.Screen.HUD].GetComponent<HUDScreenController>().NewReticle(0);
        drinkSpeedTimer = -1;

        can.GetComponent<CanController>().Activate(Mathf.FloorToInt(damage * damageMod), explodeRadius, sodaTraits, this);

        //Teleports the can to the hand's position
        can.transform.rotation = Quaternion.Euler(new Vector3(cam.transform.rotation.x, transform.rotation.y, 0));
        can.transform.position = hand.transform.position;

        //Sets the can to move in the specified direction with the force being scaled with the throwSpeed variable
        can.GetComponent<Rigidbody>().AddForce(
            throwSpeed * 100 * new Vector3(
                Mathf.Cos(cam.transform.rotation.eulerAngles.x * Mathf.Deg2Rad) * Mathf.Sin(transform.rotation.eulerAngles.y * Mathf.Deg2Rad),
                -Mathf.Sin(cam.transform.rotation.eulerAngles.x * Mathf.Deg2Rad),
                Mathf.Cos(cam.transform.rotation.eulerAngles.x * Mathf.Deg2Rad) * Mathf.Cos(transform.rotation.eulerAngles.y * Mathf.Deg2Rad)
                )
            );
        can.GetComponent<Rigidbody>().AddTorque(new Vector3(UnityEngine.Random.Range(-900, 900), 0, UnityEngine.Random.Range(-900, 900)));

        SoundManager.PlaySound(SoundManager.canThrow);

        return Node.Status.SUCCESS;
    }
    public Node.Status WaitThrow()
    {
        if(throwRateTimer == -1)
        {
            throwRateTimer = throwRate;
        }

        if (throwRateTimer > 0)
        {
            throwRateTimer = Mathf.Clamp(throwRateTimer - Time.deltaTime, 0, 100000);
        }
        else if (throwRateTimer == 0)
        {
            throwRateTimer = -1;
            return Node.Status.SUCCESS;
        }

        return Node.Status.RUNNING;
    }

    public Node.Status Drink()
    {
        //Data for when soda is drank
        for(int i = 0; i < sodaFlavors.Length; i++)
        {
            if (sodaFlavors[i] != null)
            {
                EnableBuff(i);
            }
        }

        return Node.Status.SUCCESS;
    }
    public Node.Status WaitDrink()
    {
        if (drinkSpeedTimer == -1)
        {
            drinkSpeedTimer = drinkSpeed;
            intCont.screenPair[InterfaceController.Screen.HUD].GetComponent<HUDScreenController>().NewReticle(0, "Drinking");

            weaponAnim.SetTrigger("Up");
        }

        if (drinkSpeedTimer > 0)
        {
            drinkSpeedTimer = Mathf.Clamp(drinkSpeedTimer - Time.deltaTime, 0, 100000);
            intCont.screenPair[InterfaceController.Screen.HUD].GetComponent<HUDScreenController>().NewReticle((drinkSpeed - drinkSpeedTimer) / drinkSpeed);
        }
        else if (drinkSpeedTimer == 0)
        {
            drinkSpeedTimer = -1;
            intCont.screenPair[InterfaceController.Screen.HUD].GetComponent<HUDScreenController>().NewReticle(0, "Drinking");

            weaponAnim.SetTrigger("Down");
            return Node.Status.SUCCESS;
        }

        return Node.Status.RUNNING;
    }

    public Node.Status WaitNull()
    {
        return Node.Status.RUNNING;
    }
    #endregion

    #region Saving Methods

    public WeaponData GetSaveData()
    {
        WeaponData weapon = new WeaponData();

        for (int i = 0; i < sodaParts.Length; i++)
        {
            if (sodaParts[i] != null)
            {
                weapon.sodaParts[i] = sodaParts[i];
            }
            else
            {
                weapon.sodaParts[i] = null;
            }
        }

        return weapon;
    }

    #endregion

    #region Timer Methods

    /*  Flavor List
     *  0 Cherry: Damage boost, but slower speed
     *  1 Orange: Improve fizz effect, but decrease direct damage
     *  2 Blue Raz: Heal more health, but over time
     *  3 Cola: Guarantee proc on next hit, long drink time
     *  4 Root Beer: Cause explosion, long drink time
     *  5 Grapefruit: Improve sour effect, decrease direct damage
     *  6 Banana: Increase move speed, decrease damage
    */
    private void BuffTimerManager()
    {
        for (int i = 0; i < buffData.buffTimers.Length; i++)
        {
            if (buffData.buffTimers[i] > 0)
            {
                //Decrements the time on an active timer
                buffData.buffTimers[i] -= Time.deltaTime;

                if (buffData.buffTimers[i] < 0)
                    buffData.buffTimers[i] = 0;
            }
            //-1 is the number indicating inactive timer
            else if (buffData.buffTimers[i] != -1)
            {
                if (buffData.activeBuff[i] > 0)
                {
                    buffData.activeBuff[i]--;
                    buffData.buffTimers[i] = sodaFlavors[i].time;
                }
                else
                {
                    buffData.activeBuff[i] = -1;
                    buffData.buffTimers[i] = -1;
                    DisableBuff(i);
                }

                //What to do when activated every time(only certain buffs)
                switch (i)
                {
                    case 2:
                        //Heal over time
                        player.ChangeHealth((int)sodaFlavors[2].potency);
                        break;
                    case 4:
                        //Cause explosion
                        Collider[] col = Physics.OverlapSphere(player.transform.position, 10);
                        foreach(Collider c in col)
                        {
                            c.SendMessage("ExplodeHit", sodaFlavors[4].potency, SendMessageOptions.DontRequireReceiver);
                        }
                        break;
                }

            }
        }

        intCont.screenPair[InterfaceController.Screen.HUD].GetComponent<HUDScreenController>().UpdateBuffs(buffData);
    }

    #endregion
}
