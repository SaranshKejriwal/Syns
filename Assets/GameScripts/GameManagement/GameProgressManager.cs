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

    //this will be a singleton to load from a JSON file
    private static GameProgressManager instance = new GameProgressManager();
    public static GameProgressManager Instance
    {
        get { return instance; }
        private set { instance = value; }
    }



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
    public PathProgressObject[] GamePathArray = new PathProgressObject[GAME_PATH_COUNT];//Array will always have 8 members only


    private void UpdateProgressInMemory()
    {
        //this will be called at each level completion to update the save file
        instance.playerOneProperties = PlayerOneController.Instance.GetPlayerControllerProperties();
        instance.playerTwoProperties = PlayerTwoController.Instance.GetPlayerControllerProperties();
        //Note - Player Monobehaviours do not exist outside their Scene.
    }


    //this method will save progress for overall game across all 8 paths.
    public void WriteProgressToJSON()
    {
        //Get latest version of the Properties to Save.
        UpdateProgressInMemory();

        //Write class instance to JSON file.
        //IMPORTANT - ALL properties need to be public to be written to JSON.

        instance.SaveTimestamp = DateTime.Now.ToString();

        string jsonSave = JsonUtility.ToJson(instance);


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

    public PathProgressObject GetPathObjectByLevelType (LevelType type)
    {
        return GamePathArray[(int)type];
        //this should ideally never be called outside this class since it's not a Call by Reference
    }

    public void InitializeGamePathArray()
    {

        for (int i = 0;i < GAME_PATH_COUNT;i++)
        {
            //Set the LevelType against each array index.
            GamePathArray[i] = new PathProgressObject((LevelType)i);//convert Index to corresponding Enum
        }

        Debug.Log("GamePath Array initialized.");
    }

    public void UnlockAllSynPaths()
    {
        //unlock 1 through 6
        for (int i = 1; i < GAME_PATH_COUNT; i++)
        {
            GamePathArray[i].isPathUnlocked = true;
        }
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
}
