using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using UnityEngine;

//this class holds the level of each rune and the effect applied by each rune, if a player has it.

//this Enum will be used to hold/save the combination of Runes that each player has, including Store and Sack
//we don't need to create a separate object for Runes. Just a class needed for RuneLevel Management, when upgraded by gold.
public enum Runes
{
    None = 0, //for initialization
    GreedRune_Glimmer = 1, //increase gold value by 15% - coded
    SlothRune_Haste = 2, //10% probability of ignoring enemy damage - coded
    EnvyRune_Direction = 4, //creates a permanent compass to show direction of Player
    GluttonyRune_Resolve = 8, //0.5% heal damage every second - increase based on Rune Level - coded
    LustRune_Apathy = 16, //invisible for 2 seconds when detected. 5 second cooldown
    PrideRune_Humility = 32,//10% chance of enemy getting stunned when hunting
    WrathRune_Vengeance = 64,//reflect 10% of damage taken back at enemy - coded

}

public class RuneEffectManager : MonoBehaviour
{//MonoBehaviour because this script will be used in the Shop object while upgrading Runes.
    private static RuneEffectManager instance;
    public static RuneEffectManager Instance
    {
        /*Enemy Boss will be a singleton. It also controls EnemySpawn Handler.*/
        get { return instance; }
        private set { instance = value; }
    }

    

    [Serializable]
    public class RuneEffectProperties
    {

        public uint[] RuneLevelBySynType = new uint[8];//it is assumed that base path will never be accessed.


    }

    private RuneEffectProperties runeEffectProperties = new RuneEffectProperties();

    //Gluttony Rune needs a timer to apply health heal every 1 second, on the player that has it.
    private float gluttonyTimerElapsedSeconds = 0f;
    private bool isGluttonyTimerUp = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;//start by setting the Singleton instance 
        }
        else
        {
            Debug.LogError("Fatal Error: Cannot have a predefined instance of Rune Effect Manager");
        }
    }

    public float GetGreedRuneEffect(bool playerHasGreedRune)
    {
        if (!playerHasGreedRune)
        {
            return 1;//coin value multiplier is 1 without Greed rune
        }
        else
        {
            //each level up increases the Gold value on collect by 15%
            return (1 + runeEffectProperties.RuneLevelBySynType[(int)LevelType.Greed] * 0.15f);
        }
    }

    public float GetWrathRuneEffect(bool playerHasWrathRune)
    {
        if (!playerHasWrathRune)
        {
            return 0;//no damage to enemy if player does not have the Wrath rune
        }
        else
        {
            //each level up increases the Gold value on collect by 15%
            return (1 + runeEffectProperties.RuneLevelBySynType[(int)LevelType.Wrath] * 0.1f);
        }
    }

    public bool GetSlothRuneEffect(bool playerHasSlothRune)
    {
        if (!playerHasSlothRune)
        {
            return false;//no rune, therefore, no effect of rune
        }

        //the higher the level, the greater the probabililty of ignoring enemy damage
        return (MathFunctions.GetTrueWithProbability(0.1f * runeEffectProperties.RuneLevelBySynType[(int)LevelType.Sloth]));
    }

    public float GetGluttonyRuneEffect(bool playerHasGluttonyRune, float playerMaxHealth)
    {
        if (!playerHasGluttonyRune || !isGluttonyTimerUp)
        {
            return 0;//no auto heal on player if it doesn't have Gluttony Rune, and Rune has a 1 second cooldown
        }
        else
        {
            //each level up increases the health added by 0.5%
            return (runeEffectProperties.RuneLevelBySynType[(int)LevelType.Gluttony] * 0.005f * playerMaxHealth);//return 0.5% of Player's max health additively 
        }
    }

    private void UpdateGluttonyTimer()
    {
        gluttonyTimerElapsedSeconds += Time.deltaTime;
        if(gluttonyTimerElapsedSeconds >= 1f)
        {
            ResetGluttonyTimer();
        }
        else
        {
            isGluttonyTimerUp = false;//reset the boolean for the rest of the duration
        }

    }
    private void ResetGluttonyTimer()
    {
        gluttonyTimerElapsedSeconds = 0f;
        isGluttonyTimerUp = true;//for player Health to go up.
    } 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateGluttonyTimer();
    }

    public void UpdateRuneLevels(RuneEffectProperties newProperties)
    {
        this.runeEffectProperties.RuneLevelBySynType = newProperties.RuneLevelBySynType;
    }
}
