using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBossController : GenericEnemyController
{
    private static EnemyBossController instance;
    public static EnemyBossController Instance
    {
        /*Enemy Boss will be a singleton. It also controls EnemySpawn Handler.*/
        get { return instance; }
        private set { instance = value; }
    }

    public event EventHandler OnBossDeath;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;//start by setting the Singleton instance 
        }
        else
        {
            Debug.LogError("Fatal Error: Cannot have a predefined instance of Enemy Boss");
        }
        instance.enemyType = EnemyType.Boss;
        instance.currentEnemyState = enemyStates.isStanding;//Boss should not be moving
        instance.defaultEnemyState = enemyStates.isStanding;
        instance.enemyWalkingMovementSpeed = 0;
        instance.enemyHuntingMovementSpeed = 0;
        instance.currentEnemyHealth = 75;
        instance.EnemyProperties.maxEnemyHealth = 75;
        instance.attackRadius = 5f;
        instance.EnemyProperties.damageMultiplier = 2.5f;
        instance.IncreaseAttackDamageByMultiplier(instance.EnemyProperties.damageMultiplier);//2.5x damage for boss as a start.
        instance.currentEnemyMovementDirection = Vector3.zero; //because Boss doesn't move
        instance.enemyDetectionRadius = 9f;//Boss detection radius is hard-coded, not picked up from properties
    }

    // Start is called before the first frame update
    void Start()
    {
        //To Spawn - Find a mazeCell that is in a dead-end for spawning
        //We don't want to see the butt of the Boss, to the cell should have 1 opening except the top one.
        //Dead-end cell should be at the very edge of the level
        MazeCell containerCell = LevelBuilder.Instance.GetDeadEndCellForBossSpawn();
        transform.position = containerCell.cellPositionOnMap;

        //Face the opening of the cell only
        transform.eulerAngles = new Vector3(0, GetRotationAngleInCell(containerCell), 0);


        //Add subscriptions to OnBossDeath event on Start
        OnBossDeath += EnemySpawnHandler.Instance.StopEnemySpawnOnBossDeathEvent;
        OnBossDeath += LevelHUDStatsManager.Instance.UpdateHUDBossIconOnBossDeathEvent;


    }

    // Update is called once per frame
    void Update()
    {
        if (!GameMaster.Instance.IsLevelPlaying())
        {
            return;//do nothing if game is paused or level has ended.
        }

        //Get Nearest Player and react to it
        base.ReactToPlayer(base.GetNearestPlayer());
    }

    private int GetRotationAngleInCell(MazeCell cell)
    {
        if (!cell.cellWallState.HasFlag(cellWallState.Top))
        {
            Debug.LogError("Boss Container Cell should not open at the top");
            return 0;
        }
        if (!cell.cellWallState.HasFlag(cellWallState.Bottom))
        {
            return 180;
        }
        if (!cell.cellWallState.HasFlag(cellWallState.Left))
        {
            return 270;
        }
        if (!cell.cellWallState.HasFlag(cellWallState.Right))
        {
            return 90;
        }

        return 0;
    }

    public void FireOnBossDeathEvent()
    {
        if(OnBossDeath != null)
        {
            OnBossDeath(this, EventArgs.Empty);//fire event if not null
        }

    }

}
