using System.Collections;
using System.Collections.Generic;
using TMPro;
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


    [SerializeField] private Button LevelNumberSelectionCanvasCloseButton;

    [SerializeField] private Button[] LevelNumberButtons = new Button[MAX_LEVEL_BUTTON_COUNT];
    [SerializeField] private Button BaseLevelButton;
    [SerializeField] private Button GreedLevelButton;
    [SerializeField] private Button SlothLevelButton;
    [SerializeField] private Button EnvyLevelButton;
    [SerializeField] private Button GluttonyLevelButton;
    [SerializeField] private Button LustLevelButton;
    [SerializeField] private Button PrideLevelButton;
    [SerializeField] private Button WrathLevelButton;

    //First Level Intro Canvas buttons - Contains text to give some context about level to user
    [SerializeField] private Canvas FirstLevelIntroCanvas;
    [SerializeField] private TextMeshProUGUI FirstLevelIntroText;
    [SerializeField] private Button FirstLevelIntroContinueButton;


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
        FirstLevelIntroCanvas.enabled = false;//to be enabled only when Level 1 is clicked

        LevelNumberSelectionCanvasCloseButton.onClick.AddListener(CloseLevelNumberSelectionCanvas);

        //Add Listeners to All Level Type Buttons
        BaseLevelButton.onClick.AddListener(delegate { LoadLevelNumProgressBarForLevelType(LevelType.Base); });
        GreedLevelButton.onClick.AddListener(delegate { LoadLevelNumProgressBarForLevelType(LevelType.Greed); });
        SlothLevelButton.onClick.AddListener(delegate { LoadLevelNumProgressBarForLevelType(LevelType.Sloth); });
        EnvyLevelButton.onClick.AddListener(delegate { LoadLevelNumProgressBarForLevelType(LevelType.Envy); });
        GluttonyLevelButton.onClick.AddListener(delegate { LoadLevelNumProgressBarForLevelType(LevelType.Gluttony); });
        LustLevelButton.onClick.AddListener(delegate { LoadLevelNumProgressBarForLevelType(LevelType.Lust); });
        PrideLevelButton.onClick.AddListener(delegate { LoadLevelNumProgressBarForLevelType(LevelType.Pride); });
        WrathLevelButton.onClick.AddListener(delegate { LoadLevelNumProgressBarForLevelType(LevelType.Wrath); });

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

        Debug.Log("Loading Progress for Level Type: " + levelType);

        uint highestAccessibleLevelIndex = GameProgressManager.Instance.GetHighestAccessibleLevelForType(levelType);

        //Strategy - User can only choose 2 levels - 1st level, and max reachable level. This is because L1 buffs impact subsequent levels.


        //Only Level 1 and Max Level Reached button will have listeners added.
        //No other button will have listeners added. Tweak their color based on how far the player has reached.
        for (int i = 0; i < MAX_LEVEL_BUTTON_COUNT; i++)
        {
            if (i > GameProgressManager.Instance.GetPathObjectByLevelType(levelType).GetHighestLevelIndex())
            {
                LevelNumberButtons[i].enabled = false;//this will be used for Base Path only, to disable buttons 4-7
            }

            if (i > highestAccessibleLevelIndex)
            {
                //These buttons should be coloured red.
                LevelNumberButtons[highestAccessibleLevelIndex].GetComponent<Image>().color = new Color(100, 0, 0);

            } else if (i < highestAccessibleLevelIndex)
            {
                //These buttons should be coloured Dark Green.
                LevelNumberButtons[highestAccessibleLevelIndex].GetComponent<Image>().color = new Color(0, 100, 0);
            }

        }

        //Level 1 button will always be enabled and ready to load the base level
        LevelNumberButtons[0].enabled = true;
        LevelNumberButtons[0].GetComponent<Image>().color = Color.green;
        LevelNumberButtons[0].onClick.AddListener(delegate { ShowLevelIntroPanel(levelType); });

        //When First Level is clicked, user will get an Intro. Add delegate to start level on Continue button.
        FirstLevelIntroContinueButton.onClick.AddListener(delegate {GameProgressManager.Instance.LoadFirstLevelOfType(levelType); });

        //if Highest index is 0, then only colour will be overwritten, not the listener.
        LevelNumberButtons[highestAccessibleLevelIndex].enabled = true;
        LevelNumberButtons[highestAccessibleLevelIndex].GetComponent<Image>().color = Color.yellow;

        //Do not add a separate listener if User has not started the path yet
        if(highestAccessibleLevelIndex > 0)
        {
            LevelNumberButtons[highestAccessibleLevelIndex].onClick.AddListener(delegate { GameProgressManager.Instance.LoadHighestSavedLevelOfType(levelType); });

        }


    }

    public void ShowLevelIntroPanel(LevelType levelType)
    {
        //This method will be used when user opens a Base level - to give an intro about the Syn in 1 verse
        FirstLevelIntroCanvas.enabled = true;

        FirstLevelIntroText.text = GetFirstLevelIntroTextForLevelType(levelType);

        //Added for now until a prompt is introduced
        //GameProgressManager.Instance.LoadFirstLevelOfType(levelType);
    }


    public void ShowLevelSelectionMenu()
    {
        LevelHidingCeiling.localScale = Vector3.one;
        LevelTypeSelectionCanvas.enabled = true;
    }

    public void HideLevelSelectionMenus()
    {
        LevelHidingCeiling.localScale = Vector3.zero; //hide ceiling to reveal Level Build
        LevelNumberSelectionCanvas.enabled = false;//hide LevelNumberSelectionCanvas, if enabled
        LevelTypeSelectionCanvas.enabled = false;//hide all Level Selection buttons.
        FirstLevelIntroCanvas.enabled = false;
    }

    private void CloseLevelNumberSelectionCanvas()
    {
        LevelNumberSelectionCanvas.enabled = false;
    }


    private string GetFirstLevelIntroTextForLevelType(LevelType levelType)
    {
        switch (levelType)
        {
            case LevelType.Base:
                return "You are the Protector.\n You must Protect...";
            case LevelType.Greed:
                return "";
            case LevelType.Sloth:
                return "";
            case LevelType.Envy:
                return "";
            case LevelType.Gluttony:
                return "";
            case LevelType.Lust:
                return "";
            case LevelType.Pride:
                return "";
            case LevelType.Wrath:
                return "";
            default:
                return "";

        }
    }
}
