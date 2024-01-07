using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this abstract class will represent the types of buffs that can be applied to Enemies on Level Completion
public class EnemyBuffObject
{
    private string buffTitle;//Catchy title on the UI;

    private string buffDescription;//Description shown on the UI

    private EnemyType buffedEnemyType;//this will be used to differentiate between Grunt Buff and Boss Buff

    //Define common multiplier objects that will increase Damage, MaxHealth
    private float damageMultiplier = 1.0f;
    private float enemyMaxHealthMultiplier = 1.0f;

    //applies to All grunts and Lust boss only
    private float enemyMovementSpeedMultiplier = 1.0f;
    private float enemyDetectionRadiusMultiplier = 1.0f;

    private float enemySpawnTimeReducer = 1.0f;//Spawn time will be reduced


    public EnemyBuffObject(EnemyType buffedEnemyType, string buffTitle, string buffDescription, float damageMultiplier, float healthMultiplier, float movementSpeedMultiplier, float detectionRadiusMultiplier, float enemySpawnTimeReducer)
    {
        this.buffedEnemyType = buffedEnemyType;
        this.buffTitle = buffTitle;
        this.buffDescription = buffDescription;
        this.damageMultiplier = damageMultiplier;
        this.enemyMaxHealthMultiplier = healthMultiplier;
        this.enemyMovementSpeedMultiplier = movementSpeedMultiplier;
        this.enemyDetectionRadiusMultiplier = detectionRadiusMultiplier;
        this.enemySpawnTimeReducer = enemySpawnTimeReducer;
    }

    public EnemyType GetBuffedEnemyType()
    {
        return buffedEnemyType;
    }


    public string GetBuffTitle()
    {
        return buffTitle;
    }

    public string GetBuffDescription()
    {
        return buffDescription;
    }

    public float GetDamageMultiplier()
    {
        return damageMultiplier;
    }

    public float GetEnemyMaxHealthMultiplier()
    {
        return enemyMaxHealthMultiplier;
    }

    public float GetEnemyMovementSpeedMultiplier()
    {
        return enemyMovementSpeedMultiplier;
    }

    public float GetEnemyDetectionRadiusMultiplier()
    {
        return enemyDetectionRadiusMultiplier;
    }

    public float GetEnemySpawnTimeReducer()
    {
        return enemySpawnTimeReducer;
    }
}
