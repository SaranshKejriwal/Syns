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

    private const uint MAX_SYN_PATH_LEVEL_INDEX = 6; //can be ignored in the JSON serialization

    public bool runeCollected = false; //this will be true only if all 7 levels are completed.
    public bool isPathUnlocked = false; //to be set to true if Base Path is completed. To be managed in GameProgressManager.

    public GenericEnemyController.GenericEnemyControllerProperties enemyGruntProperties = new GenericEnemyController.GenericEnemyControllerProperties();
    public GenericEnemyController.GenericEnemyControllerProperties enemyBossProperties = new GenericEnemyController.GenericEnemyControllerProperties();


    //When Save is done, only the latest achieved level is saved, not a the max.


    public PathProgressObject(LevelType levelType, bool isUnlocked = false)
    {
        this.levelType = levelType;
        this.isPathUnlocked = isUnlocked;
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
        if(idx > MAX_SYN_PATH_LEVEL_INDEX)
        {
            Debug.LogError("Path Index cannot Exceed the 7th level. Capping at 6");//logging purpose only
        }
        if(idx >= MAX_SYN_PATH_LEVEL_INDEX)
        {
            this.levelReachedIndex = MAX_SYN_PATH_LEVEL_INDEX;
            this.runeCollected = true; // true only if idx == 6, ie 7 Levels are completed. 
            return;
        }

        this.levelReachedIndex = idx;

    }

    public uint GetLevelReachedIndex()
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
}
