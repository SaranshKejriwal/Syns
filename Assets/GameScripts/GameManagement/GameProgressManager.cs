using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


//this class will be used to save the state of the Game for Continue
//It will have 8 Path Progress objects - 1 for each sin, plus 1 Base Level.
// Player stats to be stored separately

//this class does not need to inherit mmonobehaviour since it is only storing float and bool values which can be referred everywhere.
[Serializable]
public class GameProgressManager
{
    private const int GAME_PATH_COUNT = 8;//Game will always have 8 paths. No need to serialize this

    private const uint BASE_PATH_HIGHEST_LEVEL_INDEX = 2;//3 levels only
    private const uint SYNS_PATH_HIGHEST_LEVEL_INDEX = 6;//7 levels

    //this will be a singleton to load from a JSON file
    private static GameProgressManager instance = new GameProgressManager();
    public static GameProgressManager Instance
    {
        get { return instance; }
        private set { instance = value; }
    }

    private LevelType currentlyPlayedLevelType = LevelType.None;
    //This will track the level path that is to be updated. 

    //Destination file
    public String SavePointLocation = Application.persistentDataPath + "/SavePoint.json";
    public string SaveTimestamp = "";



    //Create Global Game Level Properties
    public float TotalCoinsCollectedSoFar = 0f;



    //PlayerOne Properties
    public GenericPlayerController.GenericPlayerControllerProperties playerOneProperties = new GenericPlayerController.GenericPlayerControllerProperties();

    //PlayerTwo Properties
    public GenericPlayerController.GenericPlayerControllerProperties playerTwoProperties = new GenericPlayerController.GenericPlayerControllerProperties();

    //Create Progress Object for each of the 8 paths, including enemy stats.
    //we are creating array rather than a Dictionary, for easier JSON serialization. Enum can be used for array index
    public PathProgressObject[] GamePathProgressArray = new PathProgressObject[GAME_PATH_COUNT];//Array will always have 8 members only


    private void UpdatePlayerPropertiesInMemory()
    {
        //this will be called at each level completion to update the save file
        instance.playerOneProperties = PlayerOneController.Instance.GetPlayerControllerProperties();
        instance.playerTwoProperties = PlayerTwoController.Instance.GetPlayerControllerProperties();

        //Update SaveFile
        instance.WriteProgressToJSON();
    }

    public void UpdateBossPropertiesInMemoryForCurrentPath(GenericEnemyController.GenericEnemyControllerProperties bossProperties)
    {
        instance.GamePathProgressArray[(int)currentlyPlayedLevelType].enemyBossProperties.CopyPropertiesOf(bossProperties);

        //Update SaveFile
        instance.WriteProgressToJSON();
    }

    public void UpdateGruntPropertiesInMemoryForCurrentPath(GenericEnemyController.GenericEnemyControllerProperties gruntProperties)
    {
        instance.GamePathProgressArray[(int)currentlyPlayedLevelType].enemyGruntProperties.CopyPropertiesOf(gruntProperties);

        //Update SaveFile
        instance.WriteProgressToJSON();
    }


    //this method will save progress for overall game across all 8 paths.
    public void WriteProgressToJSON()
    {
        //we assume that any latest property value update should be done outside this class.

        //Write class instance to JSON file.
        //IMPORTANT - ALL properties need to be public to be written to JSON.
        instance.SaveTimestamp = DateTime.Now.ToString();

        string jsonSave = JsonUtility.ToJson(instance,true);//true corresponds to Pretty-Printed json


        //Write some text to the test.txt file
        Debug.Log(jsonSave);

        StreamWriter writer = new StreamWriter(SavePointLocation, false);//overwrite existing file
        writer.Write(jsonSave);

        writer.Close();
    }



    public void LoadProgressFromJSON()
    {
        //This will be called on 'Continue' click to load JSON data.
        StreamReader reader = new StreamReader(SavePointLocation);
        string jsonFromSaveFile = reader.ReadToEnd();
        reader.Close();

        //load json into class.
        instance = JsonUtility.FromJson<GameProgressManager>(jsonFromSaveFile);
        Debug.Log("Save Point Loaded successfully. Total Coins Loaded: " + instance.TotalCoinsCollectedSoFar);

    }

    public PathProgressObject GetPathObjectByLevelType(LevelType type)
    {
        return GamePathProgressArray[(int)type];
        //this should ideally never be called outside this class since it's not a Call by Reference
    }

    public void InitializeGamePathArray()
    {

        for (int i = 0; i < GAME_PATH_COUNT; i++)
        {
            //Set the LevelType against each array index.
            GamePathProgressArray[i] = new PathProgressObject((LevelType)i);//convert Index to corresponding Enum

            //Set base stats of Grunts and Bosses from FirstLevel stats.
            GamePathProgressArray[i].enemyBossProperties = GenericEnemyController.GetFirstLevelBossPropertiesForLevelType((LevelType)i);
            GamePathProgressArray[i].enemyGruntProperties = GenericEnemyController.GetFirstLevelEnemyGruntPropertiesForLevelType((LevelType)i);

        }

        //Base path should have 3 levels only, and should always be unlocked.
        GamePathProgressArray[(int)LevelType.Base].SetHighestLevelIndex(BASE_PATH_HIGHEST_LEVEL_INDEX);
        GamePathProgressArray[(int)LevelType.Base].isPathUnlocked = true;
        Debug.Log("GamePath Array initialized.");

    }

    private void UnlockAllSynPaths()
    {
        //unlock 1 through 6
        for (int i = 1; i < GAME_PATH_COUNT; i++)
        {
            GamePathProgressArray[i].isPathUnlocked = true;
        }

        WriteProgressToJSON();
    }

    public void ResetGameProgress()
    {
        //reset Game Path Array
        InitializeGamePathArray();
        //This will not be loaded from Save File at the start of the app.
        instance.playerOneProperties = PlayerOneController.Instance.GetPlayerControllerProperties();
        instance.playerTwoProperties = PlayerTwoController.Instance.GetPlayerControllerProperties();

        WriteProgressToJSON();
    }

    public bool IsExistingSaveFileAvailable()
    {
        if (System.IO.File.Exists(SavePointLocation))
        {
            Debug.Log("Previous Save File found...");
            return true;
        }
        else
        {
            Debug.Log("No Previous Save File Located...");
            return false;
        }
    }

    public uint GetHighestAccessibleLevelForType(LevelType levelType)
    {
        if (GamePathProgressArray[(int)levelType].IsPathCompleted())
        {
            return GamePathProgressArray[(int)levelType].GetHighestLevelIndex();//return
        }

        return GamePathProgressArray[(int)(levelType)].levelReachedIndex;
    }

    public void IncreaseAccessibleLevelOfCurrentPath()
    {
        //if path is completed, it will be captured here.
        instance.GamePathProgressArray[(int)currentlyPlayedLevelType].IncrementLevelReachedIndex();

        //in case the base path is completed, then unlock all the other levels.
        if(currentlyPlayedLevelType == LevelType.Base && instance.GamePathProgressArray[(int)currentlyPlayedLevelType].IsPathCompleted())
        {
            //manage level unlock implicitly.
            UnlockAllSynPaths();
        }

        //save game after incrementing array
        instance.WriteProgressToJSON();
    }

    public bool IsCurrentPathCompleted(LevelType levelType)
    {
        return instance.GamePathProgressArray[(int)levelType].IsPathCompleted();
    }

    public void LoadFirstLevelOfType(LevelType levelType)
    {
        currentlyPlayedLevelType = levelType;//this is the path that will be updated after level completion

        //Update Latest PlayerOne and PlayerTwo properties from Save, even when restarting a level
        PlayerOneController.Instance.SetPlayerPropertiesFromSave(instance.playerOneProperties);
        PlayerTwoController.Instance.SetPlayerPropertiesFromSave(instance.playerTwoProperties);

        //Load the default Grunt Boss stats, not from Save File 
        EnemySpawnHandler.Instance.SetCurrentGruntProperties(GenericEnemyController.GetFirstLevelEnemyGruntPropertiesForLevelType(levelType));
        EnemyBossController.Instance.SetEnemyPropertiesFromSave(GenericEnemyController.GetFirstLevelBossPropertiesForLevelType(levelType));

        //Build the Level and Spawn objects on the GameFloor
        LevelBuilder.Instance.ConstructLevel(levelType);

        //Hide Menu after LevelBuilder is done
        LevelSelectionManager.Instance.HideLevelSelectionMenus();

        //Hide Level Transition Menus, if open
        LevelTransitionManager.Instance.HideAllTransitionCanvases();

        //Change state of Game to Play.
        GameMaster.Instance.StartGamePlay();
    }

    public void LoadHighestSavedLevelOfType(LevelType levelType)
    {
        currentlyPlayedLevelType = levelType;//this is the path that will be updated after level completion



        //Update Latest PlayerOne and PlayerTwo properties from Save
        PlayerOneController.Instance.SetPlayerPropertiesFromSave(instance.playerOneProperties);
        PlayerTwoController.Instance.SetPlayerPropertiesFromSave(instance.playerTwoProperties);


        //Load the Grunt and Boss stats of this level type from the SaveFile.
        EnemyBossController.Instance.SetEnemyPropertiesFromSave(instance.GetPathObjectByLevelType(levelType).GetEnemyBossProperties());
        EnemySpawnHandler.Instance.SetCurrentGruntProperties(instance.GetPathObjectByLevelType(levelType).GetEnemyGruntProperties());
        //EnemySpawnHandler is used because it is responsible for creating new grunt objects

        //Build the Level and Spawn objects on the GameFloor
        LevelBuilder.Instance.ConstructLevel(levelType);

        //Hide Menu after LevelBuilder is done
        LevelSelectionManager.Instance.HideLevelSelectionMenus();

        //Hide Level Transition Menus, if open
        LevelTransitionManager.Instance.HideAllTransitionCanvases();

        //Change state of Game to Play.
        GameMaster.Instance.StartGamePlay();
    }

    public void LoadNextLevelOnCurrentPath()
    {
        //Write current properties to JSON first
        WriteProgressToJSON();


        //load the recently saved stats and build level.
        instance.LoadHighestSavedLevelOfType(currentlyPlayedLevelType);
    }

    public void IncreaseNextLevelSpawnRateForAliveBoss(LevelType levelType)
    {
        float SpawnDelayReductionMultiplier = 0.7f;

        //this will be called if previous Level boss is still alive after Level completion.
        //Increase spawn rate in Save File itself.
        EnemySpawnHandler.Instance.ReduceSpawnDelayForAliveBoss();

        //update Progress object to update JSON
        GamePathProgressArray[(int)levelType].enemyGruntProperties.gruntSpawnDelay = GamePathProgressArray[(int)levelType].enemyGruntProperties.gruntSpawnDelay * SpawnDelayReductionMultiplier;

        //Save Progress
        instance.WriteProgressToJSON();
    }
}
