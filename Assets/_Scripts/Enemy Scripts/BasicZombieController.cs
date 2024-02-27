using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicZombieController : Enemy
{
    // Timers for actions
    public float targetDelayTime = 5;
    public float spawnDelayTime = 5;
    public float attackDelayTime = 5;
    public float deathDelayTime = 0;

    protected Sequence targetActions;
    protected Sequence attackActions;
    protected Sequence spawnActions;
    protected Sequence deathActions;

    public override void Initialize()
    {
        base.Initialize();

        //Initializing the nodes
        targetActions = new Sequence("Target");
        targetActions.AddChild(new Action("BeginTargetMove", BeginTargetMove));
        targetActions.AddChild(new Action("TargetMove", TargetMove));
        targetActions.AddChild(new Action("TargetMoveDelay", TargetMoveDelay));
        targetActions.AddChild(new Action("EndTargetMove", EndTargetMove));

        attackActions = new Sequence("Attack");
        attackActions.AddChild(new Action("BeginAttack", BeginAttack));
        attackActions.AddChild(new Action("Attack", Attack));
        attackActions.AddChild(new Action("AttackDelay", AttackDelay));
        attackActions.AddChild(new Action("EndAttack", EndAttack));

        spawnActions = new Sequence("Spawn");
        spawnActions.AddChild(new Action("BeginSpawnMove", BeginSpawn));
        spawnActions.AddChild(new Action("SpawnMove", Spawn));
        spawnActions.AddChild(new Action("SpawnMoveDelay", SpawnDelay));
        spawnActions.AddChild(new Action("EndSpawnMove", EndSpawn));

        deathActions = new Sequence("Death");
        deathActions.AddChild(new Action("BeginDeath", BeginDeath));
        deathActions.AddChild(new Action("DeathDelay", DeathDelay));
        deathActions.AddChild(new Action("Death", Death));
        deathActions.AddChild(new Action("EndDeath", EndDeath));

        //Set the sequences inside the tree
        behaviorTree.AddChild(targetActions);
        behaviorTree.AddChild(attackActions);
        behaviorTree.AddChild(spawnActions);
        behaviorTree.AddChild(deathActions);
    }
    public override void SpawnSetup(Vector3 spawnerLocation)
    {
        base.SpawnSetup(spawnerLocation);

        navAgent.enabled = false;
        navObstacle.enabled = false;

        transform.position = spawnerLocation + (transform.up * -5);
        
        StartBehavior("Spawn");

        status = behaviorTree.Check();
    }

    protected override void RunBehaviorTree()
    {
        //Forces enemy to move faster if it is far behind
        if (behaviorTree.GetCurrentSequenceName() != "Spawn")
        {
            base.RunBehaviorTree();

            //Stops enemies from pushing eachother too close to the player
            if (behaviorTree.GetCurrentSequenceName() == "Attack" && navAgent.enabled)
            {
                NavObstacleEnabled(true);
            }
            else if (behaviorTree.GetCurrentSequenceName() != "Attack" && navObstacle.enabled)
            {
                //Makes sure the agent waits until the mesh is back
                NavObstacleEnabled(false);
            }
        }

        //If the status of this action is RUNNING it will continue to check the actions sequence
        if (status == Node.Status.RUNNING)
        {
            status = behaviorTree.Check();
        }
        else if (status == Node.Status.SUCCESS)
        {
            switch (behaviorTree.GetCurrentSequenceName())
            {
                case "Spawn":
                    StartBehavior("Target");
                    break;
                case "Target":
                    StartBehavior("Attack");
                    break;
                case "Attack":
                    StartBehavior("Attack");
                    break;
                case "Death":
                    StartBehavior("Dormant");
                    break;
                default:
                    //Starts the chase sequence if any action finishes, default in case no other action is specified beforehand
                    if (navAgent.isOnNavMesh)
                        behaviorTree.StartSequence("Chase");
                    break;
            }

            status = behaviorTree.Check();
        }
    }

    #region Behavior Tree Nodes

    #region Move to Target

    protected virtual Node.Status BeginTargetMove()
    {
        // Modify to change the action performed when entering this state
        targetObject = player.gameObject;
        return Node.Status.SUCCESS;
    }
    protected virtual Node.Status TargetMove()
    {
        // Modify to change the actions when moving to the target location
        return MoveTo(targetObject.transform.position);
    }
    protected virtual Node.Status TargetMoveDelay()
    {
        // Modify this to change the time of delay for this action
        return Delay(targetDelayTime);
    }
    protected virtual Node.Status EndTargetMove()
    {
        // Modify to change the action performed when exiting this state
        return Node.Status.SUCCESS;
    }

    #endregion

    #region Attack Target

    protected virtual Node.Status BeginAttack()
    {
        // Modify to change action when beginning to move to bed
        return Node.Status.SUCCESS;
    }
    protected virtual Node.Status Attack()
    {
        // Modify to change the actions when moving to the target location
        targetObject.SendMessage("EnemyHit", (int)(baseDamage * damageMod), SendMessageOptions.DontRequireReceiver);

        return Node.Status.SUCCESS;
    }
    protected virtual Node.Status AttackDelay()
    {
        // Modify this to change the time of delay for this action
        if (MyFunctions.Distance(targetObject.transform.position, transform.position) > baseHitDistance * hitDistanceMod)
            StartBehavior("Target");

        return Delay(attackDelayTime);
    }
    protected virtual Node.Status EndAttack()
    {
        // Modify to change the action performed when exiting this state
        return Node.Status.SUCCESS;
    }

    #endregion

    #region Spawn Nodes

    protected virtual Node.Status BeginSpawn()
    {
        // Modify to change action when beginning to move to bed
        return Node.Status.SUCCESS;
    }
    protected virtual Node.Status Spawn()
    {
        // Modify to change the actions when moving to the target location
        if (Mathf.Abs(transform.position.y - (spawnerPosition.y + 1)) < 0.4f)
        {
            minimapObj.SetActive(true);
            navAgent.enabled = true;
            return Node.Status.SUCCESS;
        }

        minimapObj.SetActive(false);

        transform.Rotate(new Vector3(0, 1, 0));
        transform.position += Vector3.up * 0.01f;

        return Node.Status.RUNNING;
    }
    protected virtual Node.Status SpawnDelay()
    {
        // Modify this to change the time of delay for this action
        return Delay(spawnDelayTime);
    }
    protected virtual Node.Status EndSpawn()
    {
        // Modify to change the action performed when exiting this state
        return Node.Status.SUCCESS;
    }

    #endregion

    #region Death Nodes

    protected virtual Node.Status BeginDeath()
    {
        return Node.Status.SUCCESS;
    }
    protected virtual Node.Status Death()
    {
        if (Random.Range(0, 100) < lootDropChance)
        {
            //Calls to lootPool to drop the item, but needs to pass current position and lootPoolID
            LootPoolController.CreateItemDrop(transform.position + Vector3.up, lootPoolID);
        }

        GameObject token = ObjectPool.objPool.GetPooledObject("Token");
        if (token)
        {
            int value = Random.Range(Mathf.Clamp(avgTokenDrop * (GameController.waveNum / 3) - 5, 0, 1000000), avgTokenDrop * (GameController.waveNum / 3) + 5);

            if (value > 0)
            {
                token.SetActive(true);
                token.GetComponent<TokenController>().Spawn(value, transform.position);
            }
        }

        //Checks for all soul boxes in range
        if (currentSoulBox != null)
        {
            currentSoulBox.EnemyKill(gameObject);
        }

        DespawnEnemy();

        GameController.gameCont.AddKill();

        return Node.Status.SUCCESS;
    }
    protected virtual Node.Status DeathDelay()
    {
        return Delay(deathDelayTime);
    }
    protected virtual Node.Status EndDeath()
    {
        return Node.Status.SUCCESS;
    }

    #endregion

    #endregion

    protected override void Die()
    {
        behaviorTree.StartSequence("Death");
        status = behaviorTree.Check();
    }

}
