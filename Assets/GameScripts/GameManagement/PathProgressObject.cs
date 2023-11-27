using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this class will illustrate the properties on each of the 8 paths - 7 syns and 1 base
[Serializable]
public class PathProgressObject 
{//this class needs to be serializable, and all members need to be public for JSONUtility

    public LevelType levelType;//associate the Syn to the path.

    //which level has been reached
    public uint levelReachedIndex = 0;//will vary from 0-6, for one path

    private uint highestLevelIndex = 6; //can be ignored in the JSON serialization
    //this value will be 2 for the Base level - 3 levels only.
    //Total Level count = highestLevelIndex + 1, since index starts from 0

    public bool isPathCompletedOrRuneCollected = false; //this will be true only if all 7 levels are completed.
    public bool isPathUnlocked = false; //to be set to true if Base Path is completed. To be managed in GameProgressManager.

    public GenericEnemyController.GenericEnemyControllerProperties enemyGruntProperties = new GenericEnemyController.GenericEnemyControllerProperties();
    public GenericEnemyController.GenericEnemyControllerProperties enemyBossProperties = new GenericEnemyController.GenericEnemyControllerProperties();


    //When Save is done, only the latest achieved level is saved, not a the max.


    public PathProgressObject(LevelType levelType, bool isUnlocked = false)
    {
        this.levelType = levelType;
        this.isPathUnlocked = isUnlocked;

        //Syn type will be the same between Path Progress Object and corresponding Enemies.
        enemyBossProperties.enemySynType = levelType;
        enemyGruntProperties.enemySynType = levelType;
    }

    public void SetEnemyGruntProperties(GenericEnemyController.GenericEnemyControllerProperties newGruntProperties)
    {
        this.enemyGruntProperties = newGruntProperties;
    }

    public void SetEnemyBossProperties(GenericEnemyController.GenericEnemyControllerProperties newBossProperties)
    {
        this.enemyBossProperties = newBossProperties;
    }

    public GenericEnemyController.GenericEnemyControllerProperties GetEnemyGruntProperties()
    {
        return this.enemyGruntProperties;
    }

    public GenericEnemyController.GenericEnemyControllerProperties GetEnemyBossProperties()
    {
        return this.enemyBossProperties;
    }

    public void SetLevelReachedIndex(uint idx)
    {
        if(idx > highestLevelIndex)
        {
            Debug.LogError("Path Index cannot Exceed the 7th level. Capping at 6");//logging purpose only
        }
        if(idx >= highestLevelIndex)
        {
            this.levelReachedIndex = highestLevelIndex;
            this.isPathCompletedOrRuneCollected = true; // true only if idx == 6, ie 7 Levels are completed. 
            return;
        }

        this.levelReachedIndex = idx;

    }
    public bool IsPathCompleted()
    {
        return isPathCompletedOrRuneCollected;
    }

    public uint GetHighestAccessibleLevelIndex()
    {
        return this.levelReachedIndex;
    }

    public LevelType GetLevelType()
    {
        return this.levelType;
    }

    public void SetLevelType(LevelType newLevelType)
    {
        this.levelType = newLevelType;
    }

    public void SetHighestLevelIndex(uint highestLevelIndex)
    {
        //this will be called for Base only. Highest index will be 6 by default for the others.
        this.highestLevelIndex = highestLevelIndex;
    }

    public uint GetHighestLevelIndex()
    {
        return this.highestLevelIndex;
    }
}
