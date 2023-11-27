using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectionManager : MonoBehaviour
{
    private static LevelSelectionManager instance;
    public static LevelSelectionManager Instance
    {
        /*Enemy Boss will be a singleton. It also controls EnemySpawn Handler.*/
        get { return instance; }
        private set { instance = value; }
    }

    private const uint MAX_LEVEL_BUTTON_COUNT = 7;

    [SerializeField] private Transform LevelHidingCeiling;
    //this is used to hide the maze underneath the Main Menu

    [SerializeField] private Canvas LevelTypeSelectionCanvas;//used to show the Level Types

    [SerializeField] private Canvas LevelNumberSelectionCanvas;//used to show the accessible levels from 1-7 (3 for Base Type)

    [SerializeField] private Button[] LevelNumberButtons = new Button[MAX_LEVEL_BUTTON_COUNT];
    [SerializeField] private Button LevelNumberSelectionCanvasCloseButton;

    [SerializeField] private Button PrideLevelButton;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;//start by setting the Singleton instance 
        }
        else
        {
            Debug.LogError("Fatal Error: Cannot have a predefined instance of Level Selection Manager");
        }

        LevelNumberSelectionCanvas.enabled = false;//to be hidden initially.

        LevelNumberSelectionCanvasCloseButton.onClick.AddListener(CloseLevelNumberSelectionCanvas);

        PrideLevelButton.onClick.AddListener(delegate { LoadLevelNumProgressBarForLevelType(LevelType.Pride); });
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LoadLevelNumProgressBarForLevelType(LevelType levelType)
    {
        //Enable LevelNumber Selection Canvas
        LevelNumberSelectionCanvas.enabled = true;

        Debug.Log("Loading Progress for Level Type: "+ levelType);

        uint highestAccessibleLevelIndex = GameProgressManager.Instance.GetHighestAccessibleLevelForType(levelType);

        //Strategy - User can only choose 2 levels - 1st level, and max reachable level. This is because L1 buffs impact subsequent levels.

        
        //Only Level 1 and Max Level Reached button will have listeners added.
        //No other button will have listeners added. Tweak their color based on how far the player has reached.
        for (int i = 0;i<MAX_LEVEL_BUTTON_COUNT;i++)
        {
            if(i > GameProgressManager.Instance.GetPathObjectByLevelType(levelType).GetHighestLevelIndex())
            {
                LevelNumberButtons[i].enabled = false;//this will be used for Base Path only, to disable buttons 4-7
            }

            if(i > highestAccessibleLevelIndex)
            {
                //These buttons should be coloured red.
                LevelNumberButtons[highestAccessibleLevelIndex].GetComponent<Image>().color = new Color(100, 0, 0);

            }else if(i < highestAccessibleLevelIndex)
            {
                //These buttons should be coloured Dark Green.
                LevelNumberButtons[highestAccessibleLevelIndex].GetComponent<Image>().color = new Color(0,100,0);
            }

        }

        //Level 1 button will always be enabled and ready to load the base level
        LevelNumberButtons[0].enabled = true;
        LevelNumberButtons[0].GetComponent<Image>().color = Color.green;
        LevelNumberButtons[0].onClick.AddListener(delegate { GameProgressManager.Instance.LoadBaseLevelOfType(levelType); });

        //if Highest index is 0, then only colour will be overwritten, not the listener.
        LevelNumberButtons[highestAccessibleLevelIndex].enabled = true;
        LevelNumberButtons[highestAccessibleLevelIndex].GetComponent<Image>().color = Color.yellow;

        //Do not add a separate listener if User has not started the path yet
        if(highestAccessibleLevelIndex > 0)
        {
            LevelNumberButtons[highestAccessibleLevelIndex].onClick.AddListener(delegate { GameProgressManager.Instance.LoadHighestSavedLevelOfType(levelType); });

        }


    }

    public void ShowLevelSelectionMenu()
    {
        LevelHidingCeiling.localScale = Vector3.one;
        LevelTypeSelectionCanvas.enabled = true;
    }

    public void HideLevelSelectionMenu()
    {
        LevelHidingCeiling.localScale = Vector3.zero; //hide ceiling to reveal Level Build
        LevelNumberSelectionCanvas.enabled = false;//hide LevelNumberSelectionCanvas, if enabled
        LevelTypeSelectionCanvas.enabled = false;//hide all Level Selection buttons.
    }

    private void CloseLevelNumberSelectionCanvas()
    {
        LevelNumberSelectionCanvas.enabled = false;
    }

}
