using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySpawnHandler : MonoBehaviour
{
    //This class is responsible for running a timer and spawning a new instance of an enemy.
    //It is attached to the Enemy boss object because Death of Enemy boss will stop this counter for the level

    private bool isEnemySpawnTimerActive = true;
    private int enemySpawnCount = 0;
    private float currentTimerCount = 0f;
    private int maxTimerCount = 30;//30 seconds between enemy spawns. Can be changed for increasing/decreasing spawn rate

    [SerializeField] private Transform enemyPrefab;//make sure to link the prefab in the inspector, and not an instance.

    //Current values are too small - need to expand for the full map
    private float minSpawnDistanceFromOrigin = 5f;//to get min distance from (0,0), to not spawn on top of players
    private float maxSpawnDistanceFromOrigin = 10f;//to get max distance from (0,0), to not spawn outside map

    private void Awake()
    {
        
    }
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Starting Enemy Spawn Handler...");  
        
        //one-fourth maze length from either side of origin should define the inner limit of enemy spawn
        minSpawnDistanceFromOrigin = LevelBuilder.Instance.GetMazeTotalSideLength()/4;
        //less than one-half maze length from either side of origin should define the outer limit of enemy spawn
        maxSpawnDistanceFromOrigin = LevelBuilder.Instance.GetMazeTotalSideLength()/2.5f;

        SpawnNewEnemy();//Start the game with One Enemy.

    }

    // Update is called once per frame
    void Update()
    {
        UpdateEnemySpawnTimer();
    }
    //update timer and spawn enemy when it reaches limit.
    private void UpdateEnemySpawnTimer()
    {
        currentTimerCount += Time.deltaTime;
        if (currentTimerCount > maxTimerCount)
        {
            ResetEnemySpawnTimer();
            SpawnNewEnemy();
        }
    }

    private void ResetEnemySpawnTimer()
    {
        currentTimerCount = 0f;
    }

    private void SpawnNewEnemy()
    {
        if (!isEnemySpawnTimerActive)
        {
            Debug.Log("Spawn Timer inactive. Enemy spawning stopped.");
            return; //extra check to not spawn anything if counter is inactive
        }

        Transform newEnemy = Instantiate(enemyPrefab);
        newEnemy.localPosition = GetRandomFarAwaySpawnPoint();//always modify localPosition with respect to parent.
        enemySpawnCount++;
        Debug.Log("Spawning New Enemy at "+ newEnemy.localPosition);
    }

    private Vector3 GetRandomFarAwaySpawnPoint()
    {
        //Get random spawning point between x=[-20,20] and z=[-20,20], excluding the [-10,10] square in the middle
        Vector3 randomSpawnPoint = new Vector3(Random.Range(minSpawnDistanceFromOrigin,maxSpawnDistanceFromOrigin)*GetRandomSpawnSide(),0, Random.Range(minSpawnDistanceFromOrigin, maxSpawnDistanceFromOrigin) * GetRandomSpawnSide());

        return randomSpawnPoint;
    }

    private int GetRandomSpawnSide()
    {
        //CoinToss to Return 1 or -1 randomly - there is equal probability of getting a number >1 or <1
        int CoinToss = (int)Random.Range(0, 2);
        return CoinToss == 1?1:-1; //shorthand if-else
    }

  

    public void StopEnemySpawnTimer()
    {
        //this will be called when boss dies.
        isEnemySpawnTimerActive = false;
    }



}
