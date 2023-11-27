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
    private int maxTimerSeconds = 30;//30 seconds between enemy spawns.
                                   //Can be changed for increasing/decreasing spawn rate

    [SerializeField] private Transform enemyPrefab;//make sure to link the prefab in the inspector, and not an instance.

    //Current values are too small - need to expand for the full map
    private float minSpawnDistanceFromOrigin = 5f;//to get min distance from (0,0), to not spawn on top of players

    //These Properties will be maintained by the Enemy Spawn handler.
    //When spawning a new grunt, it will fire an event which all grunts will listen, to ensure that their properties are updated.
    //This is being done because Spawn handler is creating a 'Transform' object.
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
        if (currentTimerElapsedSeconds >= maxTimerSeconds)
        {
            ResetEnemySpawnTimer();
            SpawnNewEnemy();
        }
        LevelHUDStatsManager.Instance.SetEnemySpawnTimerProgressBarDisplay(currentTimerElapsedSeconds, maxTimerSeconds);
    }

    private void ResetEnemySpawnTimer()
    {
        currentTimerElapsedSeconds = 0f;
    }

    public void SpawnNewEnemy()
    {
        if (!isEnemySpawnTimerActive)
        {
            Debug.Log("Spawn Timer inactive. Enemy spawning stopped.");
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
        SpawnNewEnemy();//this will be called when new level is setup. Start with 1 new enemy on the board.

        //activate timer to spawn new grunts
        instance.isEnemySpawnTimerActive = true;

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
        //this will be called when a player starts another track...
        //but what about resume? We will need to save the states for each of the level paths, right?
    }

    public void SetCurrentGruntProperties(GenericEnemyController.GenericEnemyControllerProperties newGruntProperties)
    {
        instance.currentGruntProperties = newGruntProperties;
        instance.maxTimerSeconds = newGruntProperties.gruntSpawnDelay; //set spawn time based on Level loaded by GameProgressManager
    }

    public void SetEnemyPropertiesForLevelType(LevelType levelType)
    {
        //Implementation needed here for different syn buffs.


    }
}
