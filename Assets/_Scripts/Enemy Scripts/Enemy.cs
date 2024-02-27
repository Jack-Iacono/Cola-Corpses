using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static SodaPart;

public class Enemy : MonoBehaviour
{
    //Stores the data for all active enemies
    public static List<Enemy> enemyList = new List<Enemy>();

    [Header("Enemy Variables")]
    public float baseMoveSpeed = 10;
    protected float moveSpeedMod = 1;

    public int maxHealth = 100;
    protected int currentHealth;

    public float baseDamage;
    protected float damageMod = 1;

    public float baseAcceleration;
    protected float accelMod = 1;

    public float baseAngulerSpeed;
    protected float angMod = 1;

    public float baseHitDistance;
    protected float hitDistanceMod = 1;

    public float baseHitSpeed;
    protected float hitSpeedMod = 1;

    [Header("Loot Stuff")]
    public float lootDropChance;
    public int avgTokenDrop;

    public LootPoolController.LootPool lootPoolID;

    private float playerDespawnDist = 100;

    private float waveMod = 1;

    [Header("Behavior Tree Variables")]
    

    protected float currentTime = 0;
    protected string currentTimerOwner = null;

    [Header("GameObjects")]
    public GameObject damageText;
    public GameObject minimapObj;
    protected PlayerController player;
    protected Animator anim;

    protected NavMeshAgent navAgent;
    protected NavMeshObstacle navObstacle;

    // The location of the spawner that this enemy spawned from
    protected Vector3 spawnerPosition { get; set; }

    // The position that the enemy spawned at
    protected Vector3 spawnLocation;
    // The Object that the enemy is currently targetting
    protected GameObject targetObject;

    // Holds the sequence that should be returned to, may not need
    protected string bufferedSequence;

    // Ensures that the navAgent will only resume movement if it was movin before a pause
    private bool wasPaused = false;
    //private float animPause = 0;

    #region Soda Trait Variables

    private int fizzTicks = 0;
    private int fizzDamage = 0;

    private int sourTicks = 0;
    private float sourPercent = 0;

    #endregion

    #region Private Variables

    protected Dictionary<object, Timer> debuffTimers = new Dictionary<object, Timer>();

    //Load Realted Stuff
    public bool loadOverride { get; set; } = false;

    //Soul Box Variables
    protected EESoulBoxController currentSoulBox { get; set; }
    #endregion

    #region Nodes

    protected Node.Status status = Node.Status.RUNNING;

    protected Tree behaviorTree;

    protected Sequence dormantActions;

    #endregion

    #region Initialization
    public virtual void Initialize()
    {
        //Gets the navMesh obstacle and agent components
        navObstacle = GetComponent<NavMeshObstacle>();
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.speed = baseMoveSpeed;
        player = PlayerController.playerCont;

        // Used to get updates when the game is paused
        GameController.RegisterObserver(this);

        //Gets the animator
        anim = GetComponent<Animator>();
        anim.speed = 1;
        Debug.Log("Initialize");

        // Makes a list long enough to house all traits
        for (int i = 0; i < Enum.GetNames(typeof(Trait)).Length; i++)
        {
            debuffTimers.Add(Enum.GetValues(typeof(Trait)).GetValue(i), new Timer(0));
        }

        // Add the end methods to the appropriate timers
        debuffTimers[Trait.Sticky].endMethod = EndSticky;
        debuffTimers[Trait.Iced].endMethod = EndIced;
        debuffTimers[Trait.Fizzy].endMethod = EndFizzy;
        debuffTimers[Trait.Sour].endMethod = EndSour;
        debuffTimers[Trait.Healthy].endMethod = EndHealthy;
        debuffTimers[Trait.Flat].endMethod = EndFlat;

        #region Behavior Tree
        //Initialize the behavior tree
        behaviorTree = new Tree("EnemyBehavior");

        dormantActions = new Sequence("Dormant");
        dormantActions.AddChild(new Action("BeginDormant", Dormant));
        dormantActions.AddChild(new Action("Dormant", Dormant));

        behaviorTree.AddChild(dormantActions);

        #endregion

        behaviorTree.StartSequence("Dormant");
    }
    public virtual void SpawnSetup(Vector3 spawnerLocation)
    {
        SetWaveMod(GameController.waveNum);
        SetFullHealth();

        ResetTimers();

        spawnerPosition = spawnerLocation;

        enemyList.Add(this);

        anim.speed = 1;
        Debug.Log("Spawn");
    }
    protected virtual void SetWaveMod(int wave)
    {
        switch (wave)
        {
            case 0:
                waveMod = 1;
                break;
            case 1:
                waveMod = 1.10f;
                break;
            case 2:
                waveMod = 1.25f;
                break;
            case 3:
                waveMod = 1.45f;
                break;
            case 4:
                waveMod = 1.60f;
                break;
            case 5:
                waveMod = 1.85f;
                break;
            case 6:
                waveMod = 2;
                break;
            default:
                waveMod = 2 + (wave - 6);
                break;
        }
    }

    #endregion

    void Update()
    {
        if (!GameController.gamePaused)
        {
            RunBehaviorTree();
            ExtraUpdate();
            TimerManager();
        }
    }
    
    protected virtual void RunBehaviorTree()
    {
        // This code handles enemy despawning from distance, this is applicable to all enemies
        float distance = MyFunctions.Distance(player.transform.position, transform.position);

        if (distance > playerDespawnDist)
        {
            DespawnEnemy();
            StartBehavior("Dormant");
        }
    }
    protected virtual void ExtraUpdate()
    {

    }

    public void GamePause(bool b)
    {
        if (b)
        {
            if (navAgent && navAgent.enabled)
            {
                wasPaused = navAgent.isStopped;
                navAgent.isStopped = true;
            }

            anim.speed = 0;
        }
        else
        {
            if (navAgent && navAgent.enabled)
            {
                navAgent.isStopped = wasPaused;
            }

            // Check for later when animation speed may change
            anim.speed = 1;
        }
    }
    public void SetSoulBox(EESoulBoxController soulBox, bool b)
    {
        //Sets which soul box this zombie is tied to
        if (currentSoulBox == soulBox && !b)
        {
            currentSoulBox = null;
        }
        else if (b)
        {
            currentSoulBox = soulBox;
        }

    }


    #region Behavior Tree Nodes

    protected void StartBehavior(string str)
    {
        behaviorTree.StartSequence(str);
        ResetDelay();
    }

    #region Dormant Nodes

    protected Node.Status Dormant()
    {
        return Node.Status.RUNNING;
    }
    protected Node.Status BeginDormant()
    {
        return Node.Status.SUCCESS;
    }

    #endregion

    #region Misc Nodes

    protected Node.Status MoveTo(Vector3 location)
    {
        if (MyFunctions.Distance(navAgent.destination, location) > hitDistanceMod * baseHitDistance && navAgent.enabled)
            navAgent.destination = location;

        if (MyFunctions.Distance(transform.position, location) < hitDistanceMod * baseHitDistance)
        {
            return Node.Status.SUCCESS;
        }

        return Node.Status.RUNNING;
    }
    
    protected Node.Status Delay(float f)
    {
        if (currentTimerOwner != behaviorTree.GetCurrentSequenceName())
        {
            currentTime = 0;
            currentTimerOwner = behaviorTree.GetCurrentSequenceName();
        }

        if (currentTime < f)
        {
            currentTime += Time.deltaTime;
            return Node.Status.RUNNING;
        }

        return Node.Status.SUCCESS;
    }
    protected void ResetDelay()
    {
        // Resets the delay manually
        currentTimerOwner = "";
        currentTime = -1;
    }

    #endregion

    #endregion

    #region Soda Trait Methods

    public void SodaTraitStart(ProcData procData)
    {
        // Does something different depending on the trait passed through
        switch(procData.trait)
        {
            case Trait.Sticky:
                moveSpeedMod = procData.potency;
                Popup("<color=" + GetTraitColor(Trait.Sticky) + ">Sticky</color>");
                UpdateMoveSpeed();
                break;
            case Trait.Iced:
                accelMod = procData.potency;
                angMod = procData.potency;
                Popup("<color=" + GetTraitColor(Trait.Iced) + ">Iced</color>");
                UpdateMoveSpeed();
                break;
            case Trait.Fizzy:
                fizzDamage = (int)procData.potency;
                fizzTicks = procData.ticks;
                break;
            case Trait.Sour:
                sourPercent = procData.potency;
                sourTicks = procData.ticks;
                break;
            case Trait.Healthy:
                player.ChangeHealth((int)procData.potency);
                break;
            case Trait.Flat:
                damageMod = procData.potency;
                Popup("<color=" + GetTraitColor(Trait.Flat) + ">Flat</color>");
                break;
            default:
                Debug.Log("Invalid Trait Name");
                break;
        }

        // Starts the trait timer for the given trait
        debuffTimers[procData.trait].Start(procData.time);
    }

    protected virtual void EndSticky()
    {
        moveSpeedMod = 1;
        UpdateMoveSpeed();
    }
    protected virtual void EndIced()
    {
        accelMod = 1;
        angMod = 1;
        UpdateMoveSpeed();
    }
    protected virtual void EndFizzy()
    {
        //Checks to see if the orange buff is currently active and if so increases the damage done by the fizz
        if (WeaponController.weaponCont.buffData.activeBuff[1] > -1)
            ApplyMiscDamage(Mathf.FloorToInt(fizzDamage * WeaponController.weaponCont.sodaFlavors[1].potency * fizzDamage), SodaPart.GetTraitColor(SodaPart.Trait.Fizzy));
        else
            ApplyMiscDamage(fizzDamage, SodaPart.GetTraitColor(SodaPart.Trait.Fizzy));

        if (fizzTicks != 0)
        {
            fizzTicks--;
            debuffTimers[SodaPart.Trait.Fizzy].Restart();
        }
    }
    protected virtual void EndSour()
    {
        //Checks to see if the Grapefruit buff is currently active and if so increases the damage done by the sour
        if (WeaponController.weaponCont.buffData.activeBuff[5] > -1)
            ApplyMiscDamage(Mathf.FloorToInt(currentHealth * sourPercent * WeaponController.weaponCont.sodaFlavors[5].potency), SodaPart.GetTraitColor(SodaPart.Trait.Sour));
        else
            ApplyMiscDamage(Mathf.FloorToInt(currentHealth * sourPercent), SodaPart.GetTraitColor(SodaPart.Trait.Sour));

        if (sourTicks != 0)
        {
            sourTicks--;
            debuffTimers[SodaPart.Trait.Sour].Restart();
        }
    }
    protected virtual void EndHealthy() 
    {
        // Do Nothing
    }
    protected virtual void EndFlat()
    {
        damageMod = 1;
    }

    #endregion


    #region Enemy Group Methods

    public static void DespawnEnemies()
    {
        List<Enemy> enemies = new List<Enemy>();

        for (int i = 0; i < enemyList.Count; i++)
        {
            enemies.Add(enemyList[i]);
        }

        foreach (Enemy cont in enemies)
        {
            cont.DespawnEnemy();
        }
    }

    #endregion

    #region Despawn Methods

    protected void DespawnEnemy()
    {
        gameObject.SetActive(false);
        SetFullHealth();

        ResetTimers();

        //Takes the enemy off of the list of spawned enemies in game controller
        GameController.gameCont.RemoveEnemy();

        enemyList.Remove(this);
    }

    #endregion

    #region Saving Methods

    public static List<EnemyData> SaveData()
    {
        List<EnemyData> enemies = new List<EnemyData>();

        foreach (Enemy e in enemyList)
        {
            enemies.Add(e.GetEnemyData());
        }

        return enemies;
    }
    public EnemyData GetEnemyData()
    {
        EnemyData enemyData = new EnemyData();
        enemyData.maxHealth = maxHealth;
        enemyData.damage = baseDamage;
        enemyData.currentHealth = currentHealth;
        enemyData.moveSpeed = baseMoveSpeed;
        enemyData.hitDistance = baseHitDistance;
        enemyData.hitSpeed = baseHitSpeed;
        enemyData.lootDropChance = lootDropChance;
        enemyData.lootPoolID = lootPoolID;
        enemyData.position = transform.position;
        enemyData.rotation = transform.rotation.eulerAngles;
        enemyData.bufferedAction = behaviorTree.GetCurrentSequenceName();
        enemyData.spawnerPosition = spawnerPosition;

        return enemyData;
    }
    public void SetEnemyData(EnemyData enemyData)
    {
        maxHealth = enemyData.maxHealth;
        currentHealth = enemyData.currentHealth;
        baseDamage = enemyData.damage;
        baseMoveSpeed = enemyData.moveSpeed;
        baseHitDistance = enemyData.hitDistance;
        baseHitSpeed = enemyData.hitSpeed;
        lootDropChance = enemyData.lootDropChance;
        lootPoolID = enemyData.lootPoolID;

        navAgent.Warp(enemyData.position);
        transform.rotation = Quaternion.Euler(enemyData.rotation);

        behaviorTree.StartSequence(enemyData.bufferedAction);

        if (enemyData.bufferedAction != "Spawn")
        {
            navAgent.enabled = true;
        }

        spawnerPosition = enemyData.spawnerPosition;

        //Adds the enemy to the list of enemies in the scene
        enemyList.Add(this);
    }

    #endregion


    #region Health Related Methods
    public void ChangeHealth(int i)
    {
        currentHealth += i;

        if (currentHealth <= 0)
        {
            Die();
        }
    }
    public float GetHealth()
    {
        return currentHealth;
    }
    public void SetHealth(int i)
    {
        currentHealth = i;
    }
    public void SetFullHealth()
    {
        //Sets the health to full and removes any status ailments
        currentHealth = Mathf.FloorToInt(maxHealth * waveMod);

        sourTicks = 0;
        fizzTicks = 0;

        moveSpeedMod = 1;
        accelMod = 1;
        angMod = 1;
        UpdateMoveSpeed();

        RestartTimers();
    }

    protected virtual void Die()
    {
        DespawnEnemy();
    }

    #endregion

    #region Movement Methods

    private void UpdateMoveSpeed()
    {
        if(navAgent)
        {
            navAgent.speed = baseMoveSpeed * moveSpeedMod;
            navAgent.acceleration = baseAcceleration * accelMod;
            navAgent.angularSpeed = baseAngulerSpeed * angMod;
        }
    }
    private void FreezeMovement(bool b)
    {
        if (b)
        {
            navAgent.speed = 0;
            navAgent.acceleration = 1000;
        }
        else
        {
            UpdateMoveSpeed();
        }
    }

    protected void NavObstacleEnabled(bool b)
    {
        // Swaps Between the nav mesh agent and nav mesh obstacle
        if (b)
        {
            navAgent.enabled = false;
            navObstacle.enabled = true;
        }
        else
        {
            StartCoroutine(EnableNavMeshAgent());
        }
    }
    private IEnumerator EnableNavMeshAgent()
    {
        navObstacle.enabled = false;
        yield return new WaitForSeconds(0.1f);
        navAgent.enabled = true;
    }

    #endregion

    #region Damage Methods

    public void ApplyMiscDamage(int damage)
    {
        ChangeHealth(-damage);

        Popup(damage.ToString());
    }
    public void ApplyMiscDamage(int damage, string colorTag)
    {
        ChangeHealth(-damage);

        Popup("<color=" + colorTag + ">" + damage + "</color>");
    }

    //These two methods take damage from the can
    public virtual void CanHit(int damage)
    {
        ChangeHealth(-damage);

        SoundManager.PlaySoundSource(transform.position, SoundManager.canHit);
        Popup(damage.ToString());

        anim.SetInteger("RandHurt", Mathf.FloorToInt(UnityEngine.Random.Range(1, 3.99f)));
        anim.SetTrigger("TrHurt");
    }
    public virtual void ExplodeHit(int damage)
    {
        ChangeHealth(-damage);

        Popup(damage.ToString());

        anim.SetInteger("RandHurt", Mathf.FloorToInt(UnityEngine.Random.Range(1, 3.99f)));
        anim.SetTrigger("TrHurt");
    }

    #endregion


    #region PopUp Methods

    private void Popup(string str)
    {
        //Creates popup damage numbers
        GameObject popUp = ObjectPool.objPool.GetPooledObject("PopUp Text");

        if (popUp)
        {
            popUp.SetActive(true);
            popUp.GetComponent<PopController>().StartMove(gameObject);
            popUp.GetComponent<PopController>().SetText(str);
        }
    }

    #endregion

    #region Timer Methods

    protected void TimerManager()
    {
        float dt = Time.deltaTime;

        // Run through all timers in debuff timers and increment them
        foreach(Timer t in debuffTimers.Values)
        {
            t.CheckTime(dt);
        }
    }
    protected void ResetTimers()
    {
        // Run through all timers in debuff timers and increment them
        foreach (Timer t in debuffTimers.Values)
        {
            t.Reset();
        }
    }
    protected void RestartTimers()
    {
        foreach (Timer t in debuffTimers.Values)
        {
            t.Restart();
        }
    }

    #endregion
}
