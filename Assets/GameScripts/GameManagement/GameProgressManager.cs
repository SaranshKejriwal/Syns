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
    //this will be a singleton to load from a JSON file
    private static GameProgressManager instance = new GameProgressManager();
    public static GameProgressManager Instance
    {
        get { return instance; }
        private set { instance = value; }
    }

    //Destination file
    public String SavePointLocation = Application.persistentDataPath + "/SavePoint.json";

    //Create Progress Object for each of the 8 paths, including enemy stats.

    //Create Global Game Level Properties
    public float TotalCoinsCollectedSoFar = 0f;

    public string SaveTimestamp = "";


    //PlayerOne Properties
    public GenericPlayerController.GenericPlayerControllerProperties playerOneProperties = new GenericPlayerController.GenericPlayerControllerProperties();

    //PlayerTwo Properties
    public GenericPlayerController.GenericPlayerControllerProperties playerTwoProperties = new GenericPlayerController.GenericPlayerControllerProperties();

    private void UpdateProgressInMemory()
    {
        //this will be called at each level completion to update the save file
        instance.playerOneProperties = PlayerOneController.Instance.GetPlayerControllerProperties();
        instance.playerTwoProperties = PlayerTwoController.Instance.GetPlayerControllerProperties();
        //Note - Player Monobehaviours do not exist outside their Scene.
    }


    //this method will save progress for overall game across all 8 paths.
    public void SaveProgressToJSON()
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
        Debug.Log(jsonFromSaveFile);
        reader.Close();

        //load json into class.
        instance = JsonUtility.FromJson<GameProgressManager>(jsonFromSaveFile);
        Debug.Log("Save Point Loaded successfully.");

        Debug.Log(instance.TotalCoinsCollectedSoFar);

    }
}
