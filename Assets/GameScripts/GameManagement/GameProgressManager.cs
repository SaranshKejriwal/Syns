using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this class will be used to save the state of the Game for Continue
//It will have 8 Path Progress objects - 1 for each sin, plus 1 Base Level.
// Player stats to be stored separately
public class GameProgressManager : ScriptableObject
{
    //this will be a singleton to load from a JSON file
    private static GameProgressManager instance;
    public static GameProgressManager Instance
    {
        get { return instance; }
        private set { instance = value; }
    }

    //Create Progress Object for each of the 8 paths

    //

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
        
    }

    //this method will save progress for overall game across all 8 paths.
    public void SaveProgressToJSON()
    {
        //
    }

    public void UpdateProgress()
    {
        //this will be called at each level completion to update the save file
    }

    public void LoadProgressFromJSON()
    {
        //This will be called on 'Continue' click to load JSON data.
    }
}
