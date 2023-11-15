using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this is the master script that will trigger levelbuilding and main menus.
public class GameMaster : MonoBehaviour
{
    //this enum will define the various states that the overall game will be in.
    public enum GameStates
    {
        onWaitingToStart,
        onMainMenu,
        onLevelStartCountdown,
        onLevelPlay,
        onGamePause,
        onLevelOver
    }

    //this enum will be used to capture the type of level - 7 for each sin and 1 base
    public enum LevelType
    {
        Base,
        Greed,
        Sloth,
        Envy,
        Gluttony,
        Lust,
        Pride,
        Wrath
    }

    private static GameMaster instance;
    public static GameMaster Instance
    //this instance "Property" will be tracked by ALL enemies, while keeping actual PlayerTwo object private
    {
        get { return instance; }//not very different from getters and setters
        private set { instance = value; }//we do not want any other object to modify PlayerTwo entirely.
    }

    private GameStates gameState = GameStates.onWaitingToStart;

    private float levelStartCountdownTimer = 3f;

    private float totalUnspentGold = 0f;//This will increase as gold is collected.
    //This is a Game level property, not a single level property.

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;//start by setting the Singleton instance 
        }
        else
        {
            Debug.LogError("Fatal Error: Cannot have a predefined instance of Game Master");
        }


    }

    // Start is called before the first frame update
    void Start()
    {
        //instance.gameState = GameStates.onLevelStartCountdown;
        instance.gameState = GameStates.onLevelPlay;//this one is for testing only.
    }

    // Update is called once per frame
    void Update()
    {
        switch(instance.gameState)
        {
            case GameStates.onLevelStartCountdown:
                levelStartCountdownTimer -= Time.deltaTime; break;
        }

        //Debug.Log((int)levelStartCountdownTimer);
        if(levelStartCountdownTimer <= 0)
        {
            instance.gameState = GameStates.onLevelPlay;
            return;
        }

    }

    public bool IsLevelPlaying()
    {
        return instance.gameState == GameStates.onLevelPlay;
    }


    public void PauseGame()
    {
        instance.gameState = GameStates.onGamePause;
    }

    public void AddTotalCollectedGold(float goldValue)
    {
        //Game Master will maintain a tally of total gold across all levels
        totalUnspentGold += goldValue;
    }

}
