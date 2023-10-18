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
    [SerializeField] private TextMeshProUGUI EnemyCounterHUD;
    [SerializeField] private Image EnemySpawnTimerProgressBar;

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


    }

    // Start is called before the first frame update
    void Start()
    {
        EnemyCounterHUD.text = "1";//you will have 1 enemy at the start
    }

    // Update is called once per frame
    void Update()
    {
        //Should we use Update() script or should we call new functions in here whenever something changes
        //New function for spawn
        //New function for Player healthbar.

        //Update probably needed for spawn timer, but can be managed with new function.
    }

    public void SetEnemySpawnTimerProgressBarDisplay(float currentTimer, float maxTimer)
    {
        if(currentTimer > maxTimer)
        {
            Debug.LogError("HUD Error - Spawn timer cannot exceed Max Timer");
        }
        float lengthRatio = currentTimer / maxTimer;

        //increase the fillAmount of the progress bar, as per the scale of the length Ratio.
        EnemySpawnTimerProgressBar.fillAmount = lengthRatio;

        Color barColor = new Color(lengthRatio,1-lengthRatio,0); //(r,g,b)
        EnemySpawnTimerProgressBar.color = barColor;

    }

    public void SetEnemyCounterOnHUD(int enemyCount)
    {
        EnemyCounterHUD.text = enemyCount.ToString();
    }

}
