using System;
using System.Collections;
using System.Collections.Generic;
using TMPro; // For TextMeshProUGUI
using UnityEngine;
using UnityEngine.UI;

public class GameHUDStatsManager : MonoBehaviour
{
    private static GameHUDStatsManager instance;
    public static GameHUDStatsManager Instance
    //this instance "Property" will be tracked by ALL enemies, while keeping actual PlayerTwo object private
    {
        get { return instance; }//not very different from getters and setters
        private set { instance = value; }//we do not want any other object to modify PlayerTwo entirely.
    }

    //TextMeshProUGUI needs to be used since we're using TextMeshPro lib from Unity
    //Do not use TextMeshPro class because this is UI Text
    [SerializeField] private TextMeshProUGUI EnemyCounter;
    [SerializeField] private TextMeshProUGUI TotalGoldCounter;

    //useful for showing Player One and Player Two Levels.
    [SerializeField] private TextMeshProUGUI PlayerOneLevel;
    [SerializeField] private TextMeshProUGUI PlayerTwoLevel;

    [SerializeField] private Image EnemySpawnTimerProgressBar;
    [SerializeField] private Image PlayerOneHealthBar;
    [SerializeField] private Image PlayerTwoHealthBar;
    [SerializeField] private Image PlayerTwoSpeedBar;
    [SerializeField] private Image PlayerOneCurrentXPBar;
    [SerializeField] private Image PlayerTwoCurrentXPBar;

    [SerializeField] private RawImage enemyBossIcon; //will be greyed when Boss dies.

    private float totalGoldCollected = 0f;//this is to show the total gold in one object, rather than adding P1 and P2
     
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;//start by setting the Singleton instance 
        }
        else
        {
            Debug.LogError("Fatal Error: Cannot have a predefined instance of HUD Stats Manager");
        }

        //Both players have no XP at start
        PlayerOneCurrentXPBar.fillAmount = 0f;
        PlayerTwoCurrentXPBar.fillAmount = 0f;

    }

    // Start is called before the first frame update
    void Start()
    {
        EnemyCounter.text = EnemySpawnHandler.Instance.GetAliveEnemyCount().ToString();//you will have 1 enemy at the start
    }

    public void SetEnemySpawnTimerProgressBarDisplay(float currentTimer, float maxTimer)
    {
        float lengthRatio = currentTimer / maxTimer;
        if (currentTimer > maxTimer)
        {
            Debug.LogError("HUD Error - Spawn timer cannot exceed Max Timer");
            lengthRatio = 1f;
        }

        //increase the fillAmount of the progress bar, as per the scale of the length Ratio.
        EnemySpawnTimerProgressBar.fillAmount = lengthRatio;

        Color barColor = new Color(lengthRatio,1-lengthRatio,0); //(r,g,b)
        EnemySpawnTimerProgressBar.color = barColor;

    }

    //this will be called when boss dies and spawn stops. This should ideally be listening to a boss death event
    public void UpdateHUDBossIconOnBossDeathEvent(object boss, EventArgs e)
    {
        //Debug.Log("Listening to Boss Death event. Updating HUD.");
        EnemySpawnTimerProgressBar.fillAmount = 1f;//set it to max fill and then show inactive color.
        EnemySpawnTimerProgressBar.color = Color.black;

        enemyBossIcon.color = Color.grey;
        
    }

    public void UpdateHUDPlayerCurrentXPBar(GenericPlayerController player, float currentPlayerXP, float maxPlayerXPForLevelUp)
    {
        float lengthRatio = currentPlayerXP / maxPlayerXPForLevelUp;
        if (lengthRatio < 0f)
        {
            Debug.LogError("HUD Cannot display negative XP. Flooring to 0");
            lengthRatio = 0f;
        }
        else if (lengthRatio > 1f)
        {
            Debug.LogError("HUD Cannot have current Player XP Exceeding Max XP. Flooring to 1");
            lengthRatio = 1f;
        }

        if (player == PlayerOneController.Instance)
        {
            PlayerOneCurrentXPBar.fillAmount = lengthRatio;//no coloring required here
        }
        else if (player == PlayerTwoController.Instance)
        {
            PlayerTwoCurrentXPBar.fillAmount = lengthRatio;//no coloring required here
        }
    }

    public void SetPlayerLevelOnHUD(GenericPlayerController player, uint playerLevel)
    {
        if (player == PlayerOneController.Instance)
        {
            PlayerOneLevel.text = playerLevel.ToString();
        }
        else if (player == PlayerTwoController.Instance)
        {
            PlayerTwoLevel.text = playerLevel.ToString();
        }
    }

    public void UpdateHUDPlayerHealthBar(GenericPlayerController player, float currentPlayerHealth, float maxPlayerHealth)
    {
        float lengthRatio = currentPlayerHealth / maxPlayerHealth;
        if(lengthRatio < 0f)
        {
            Debug.LogError("HUD Cannot display negative health. Flooring to 0");
            lengthRatio = 0f;
        }else if(lengthRatio > 1f)
        {
            Debug.LogError("HUD Cannot have current Player health Exceeding Max health. Flooring to 1");
            lengthRatio = 1f;
        }

        Color barColor = new Color(1-lengthRatio, lengthRatio, 0); //(r,g,b)

        if(player == PlayerOneController.Instance)
        {
            PlayerOneHealthBar.color = barColor;
            PlayerOneHealthBar.fillAmount = lengthRatio;
        }else if(player == PlayerTwoController.Instance)
        {
            PlayerTwoHealthBar.color = barColor;
            PlayerTwoHealthBar.fillAmount= lengthRatio;
        }

    }

    public void UpdateHUDPlayerTwoSpeedbar(float currentSpeed, float maxSpeed)
    {
        float lengthRatio = currentSpeed / maxSpeed;
        //No need to add checks here because speed is purely controlled within Min/Max speeds of PlayerTwo
        PlayerTwoSpeedBar.fillAmount = lengthRatio;
    }


    public void UpdateHUDOnPlayerHealthChangeEvent(object player, EventArgs e)
    {

    }

    public void SetEnemyCounterOnHUD(int enemyCount)
    {
        EnemyCounter.text = enemyCount.ToString();
    }

    public void AddToGoldCounterOnHUD(float goldCollected)
    {
        //Note - A separate Gold Counter makes sense for HUD because we don't want to run Update() method to read the count from GameMaster
        totalGoldCollected += goldCollected;
        TotalGoldCounter.text = totalGoldCollected.ToString();
    }

}
