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
        instance.currentEnemyState = EnemyStates.isStanding;//Boss should not be moving on spawn
        instance.defaultEnemyState = EnemyStates.isStanding;
        //instance.enemyWalkingMovementSpeed = 0;
        //instance.enemyHuntingMovementSpeed = 0;

        //this is hard coded based on the GameObject Sizes, and cannot be configured anyway.
        instance.attackRadius = 5f;

        instance.currentEnemyMovementDirection = Vector3.zero; //because Boss doesn't move
        
        //Get enemies properties from static method instead of hard-coding each property
        instance.SetEnemyPropertiesFromSave(GenericEnemyController.GetFirstLevelBossPropertiesForLevelType(LevelType.Base));

        //start at Max Health
        instance.currentEnemyHealth = instance.EnemyProperties.maxEnemyHealth;

    }

    // Start is called before the first frame update
    void Start()
    {
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

    public void ResetBossForNewLevel()
    {
        //If enemy Boss is being placed via this function, then it has to be alive, even if previously killed.
        instance.currentEnemyState = instance.defaultEnemyState;//if previously dead, change its state

        //restore Boss to current Max Health, after buff is applied.
        instance.currentEnemyHealth = instance.EnemyProperties.maxEnemyHealth;
        PlaceLevelBossOnFarMap();//move boss to far end of the map
    }

    private void PlaceLevelBossOnFarMap()
    {
        //To Spawn - Find a mazeCell that is in a dead-end for spawning
        //We don't want to see the butt of the Boss, to the cell should have 1 opening except the top one.
        //Dead-end cell should be at the very edge of the level
        MazeCell containerCell = LevelBuilder.Instance.GetDeadEndCellForBossSpawn();
        transform.position = containerCell.cellPositionOnMap;

        //Face the opening of the cell only
        transform.eulerAngles = new Vector3(0, GetRotationAngleInCell(containerCell), 0);
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

    public void SetEnemyBossPropertiesByBuffObject(EnemyBuffObject buffObj)
    {
        instance.EnemyProperties.BuffEnemyPropertiesByBuffObject(buffObj);
    }

}
