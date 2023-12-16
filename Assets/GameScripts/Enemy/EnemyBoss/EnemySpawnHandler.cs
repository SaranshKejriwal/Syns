using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySpawnHandler : MonoBehaviour
{
    //This class is responsible for running a timer and spawning a new instance of an enemy.
    //It is attached to the Enemy boss object because Death of Enemy boss will stop this counter for the level

    private static EnemySpawnHandler instance;
    public static EnemySpawnHandler Instance
    {
        /*Enemy Boss will be a singleton. It also controls EnemySpawn Handler.*/
        get { return instance; }
        private set { instance = value; }
    }

    private bool isEnemySpawnTimerActive = false;
    private int aliveEnemyCount = 0;
    private float currentTimerElapsedSeconds = 0f;
    //private float maxTimerSeconds = 30f;//Spawn time will now be managed by EnemyProperties

    [SerializeField] private Transform enemyPrefab;//make sure to link the prefab in the inspector, and not an instance.

    //Current values are too small - need to expand for the full map
    private float minSpawnDistanceFromOrigin = 5f;//to get min distance from (0,0), to not spawn on top of players

    //These Properties will be maintained by the Enemy Spawn handler and applied to the spawned grunt
    private GenericEnemyController.GenericEnemyControllerProperties currentGruntProperties;
 
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;//start by setting the Singleton instance 
        }
        else
        {
            Debug.LogError("Fatal Error: Cannot have a predefined instance of Enemy Spawn Handler");
        }
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isEnemySpawnTimerActive)
        {
            UpdateEnemySpawnTimer();
        }
        
    }
    //update timer and spawn enemy when it reaches limit.
    private void UpdateEnemySpawnTimer()
    {
        if (!isEnemySpawnTimerActive || EnemyBossController.Instance.IsEnemyDead() || LevelBuilder.Instance.IsLevelCompleted())
        {
            return;//Do not spawn if timer is inactive, or Boss is dead, or Level is completed.
        }
        currentTimerElapsedSeconds += Time.deltaTime;
        if (currentTimerElapsedSeconds >= currentGruntProperties.gruntSpawnDelay)
        {
            ResetEnemySpawnTimer();
            SpawnNewEnemy();
        }
        LevelHUDStatsManager.Instance.SetEnemySpawnTimerProgressBarDisplay(currentTimerElapsedSeconds, currentGruntProperties.gruntSpawnDelay);
    }

    private void ResetEnemySpawnTimer()
    {
        currentTimerElapsedSeconds = 0f;
    }

    public void SpawnNewEnemy()
    {
        if (!isEnemySpawnTimerActive)
        {
            return; //extra check to not spawn anything if counter is inactive
        }

        Transform newEnemy = Instantiate(enemyPrefab);

       //Apply current Buff properties of the level to new Enemy.
        newEnemy.transform.GetComponent<GenericEnemyController>().SetEnemyPropertiesFromSave(currentGruntProperties);

        //one-fourth maze length from either side of origin should define the inner limit of enemy spawn
        minSpawnDistanceFromOrigin = LevelBuilder.Instance.GetMazeTotalSideLength() / 4;

        newEnemy.localPosition = MathFunctions.GetRandomSpawnPointOnFarSideMap(minSpawnDistanceFromOrigin);//always modify localPosition with respect to parent.
        aliveEnemyCount++;
        Debug.Log("Spawning New Enemy at "+ newEnemy.localPosition);
        LevelHUDStatsManager.Instance.SetEnemyCounterOnHUD(aliveEnemyCount);
    }

    public void StartEnemySpawn()
    {
        //activate timer to spawn new grunts
        instance.isEnemySpawnTimerActive = true;

        SpawnNewEnemy();//this will be called when new level is setup. Start with 1 new enemy on the board.
    }

    public void StopEnemySpawnTimer()
    {
        //this will be called when boss dies.
        instance.isEnemySpawnTimerActive = false;
        ResetEnemySpawnTimer();
    }

    //this is the listener to the OnBossDeath event which will stop the spawn timer
    public void StopEnemySpawnOnBossDeathEvent(object boss, EventArgs e)
    {
        Debug.Log("Listening to Boss Death Event. Stopping Enemy Spawn");
        //this will be called when boss dies.
        isEnemySpawnTimerActive = false;
        ResetEnemySpawnTimer();
    }

    public int GetAliveEnemyCount()
    {
        return aliveEnemyCount;
    }

    //this will be called by each enemy grunt when it dies.
    public void ReduceAliveEnemyCountOnEnemyDeath()
    {
        aliveEnemyCount--;
        LevelHUDStatsManager.Instance.SetEnemyCounterOnHUD(aliveEnemyCount);
    }

    public void ResetAllPreviousBuffs()
    {
        //this may be called when a player starts another track...
        //but what about resume? We will need to save the states for each of the level paths, right?
    }

    public void SetCurrentGruntProperties(GenericEnemyController.GenericEnemyControllerProperties newGruntProperties)
    {
        if (newGruntProperties == null)
        {
            Debug.LogError("newGruntProperties is null; Switching to Default Level Properties for Grunts");
        }
        instance.currentGruntProperties = newGruntProperties;
    }

    public void SetNextGruntPropertiesByBuffObject(EnemyBuffObject buffObj)
    {
        instance.currentGruntProperties.BuffEnemyPropertiesByBuffObject(buffObj);
    }

    public GenericEnemyController.GenericEnemyControllerProperties GetCurrentGruntProperties()
    {
        return instance.currentGruntProperties;
    }

    public void ReduceSpawnDelayForAliveBoss()
    {
        ReduceSpawnRateByMultiplier(0.7f);//reduce Spawn rate by 30% because Boss was left alive.
    }

    private void ReduceSpawnRateByMultiplier(float factor)
    {
        if(factor <= 0.5)
        {
            Debug.LogError("Cannot reduce Spawn Rate by factor of " + factor);
            return;
        }
        instance.currentGruntProperties.gruntSpawnDelay = instance.currentGruntProperties.gruntSpawnDelay*factor;  
    }

}
