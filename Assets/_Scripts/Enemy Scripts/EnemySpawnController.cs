using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions.Must;

public class EnemySpawnController : MonoBehaviour
{

    #region Game Objects

    private InterfaceController intCont;
    private GameController gameCont;

    #endregion

    #region Control Variables
    public float spawnRadius = 5;
    public float spawnDelay = 2;

    public LayerMask spawnableRegions;
    public bool active = true;

    private float playerRadius = 200;
    public string enemyName { get; set; } = "Basic Zombie";
    #endregion

    #region GameObjects
    private ObjectPool objPool;
    private PlayerController player;
    #endregion

    private List<float> timers = new List<float>();

    // Start is called before the first frame update
    void Start()
    {
        //Gets the Game Controller
        intCont = FindObjectOfType<InterfaceController>();
        gameCont = FindObjectOfType<GameController>();

        //Sets and starts the timer for spawning
        timers.Add(0f);
        timers[0] = spawnDelay;

        //Gets the object pooling script
        objPool = FindObjectOfType<ObjectPool>();
        player = FindObjectOfType<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        //Checks to see if the game is paused and if the spawner is active as well as in range
        if (active && CheckPlayerInRange() && !GameController.gamePaused)
        {
            TimerManager();
        }
    }

    #region Activation Methods

    public void Activate(bool t)
    {
        if (!active)
        {
            active = t;
        }
    }

    #endregion

    #region Spawning Methods

    private void SpawnEnemy()
    {
        GameObject enemy = objPool.GetPooledObject(enemyName);

        if (enemy)
        {
            enemy.SetActive(true);
            enemy.GetComponent<Enemy>().SpawnSetup(transform.position);

            gameCont.roundSpawned++;
        }

        timers[0] = Random.Range(spawnDelay + 0.5f, spawnDelay + 0.5f);
    }
    #endregion

    #region Player Detection Methods

    private bool CheckPlayerInRange()
    {
        float distance = Vector3.SqrMagnitude(player.transform.position - transform.position);

        return distance <= playerRadius * playerRadius;
    }

#endregion

    #region Timer Methods

    public void TimerManager()
    {
        if (timers.Count > 0)
        {
            for (int i = 0; i < timers.Count; i++)
            {
                if (timers[i] <= 0)
                {
                    timers[i] = 0;

                    switch (i)
                    {
                        case 0:
                            if (gameCont.EnemySpawnAvailable())
                                SpawnEnemy();
                            else
                                timers[0] = Random.Range(spawnDelay - 0.5f, spawnDelay + 0.5f);
                            break;
                    }

                }
                else
                {
                    timers[i] -= 1 * Time.deltaTime;
                }
            }
        }
    }

    #endregion
}
