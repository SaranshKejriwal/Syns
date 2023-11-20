using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUIManager : MonoBehaviour
{
    [SerializeField] private Canvas MainMenuCanvas;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button quitButton;

    [SerializeField] private Canvas ResetWarningPromptCanvas;
    [SerializeField] private Button AcceptResetButton;
    [SerializeField] private Button LoadSaveButton;

    //private const int LEVEL_SELECT_SCREEN_INDEX = 1;
    //private const int LEVEL_PLAY_SCREEN_INDEX = 2;

    private void Awake()
    {
        ResetWarningPromptCanvas.enabled = false;//Not to be shown until NewGame is clicked

        //add listeners to button clicks.
        continueButton.onClick.AddListener(LoadSavedGame);
        newGameButton.onClick.AddListener(checkExistingGameAndPromptUser);//internal functions added as listeners
        quitButton.onClick.AddListener(onQuitButtonClick);

        LoadSaveButton.onClick.AddListener(LoadSavedGame);
        AcceptResetButton.onClick.AddListener(StartNewGameAndOverwriteSave);

    }


    private void LoadSavedGame()
    {
        GameProgressManager.Instance.LoadProgressFromJSON();
        //we're now trying to fit everything in 1 Scene. All components exist in the same context.

        //Hide Main Menu Canvas and Warning prompt to reveal Level Selection Canvas.
        ResetWarningPromptCanvas.enabled = false;
        MainMenuCanvas.enabled = false;

    }

    private void checkExistingGameAndPromptUser() 
    {
        if (!continueButton.enabled)
        {
            Debug.Log("Starting New Game...");
            //continue button to be disabled because no earlier Save is available.
            StartNewGameAndOverwriteSave();
        }
        else
        {
            Debug.Log("Save Exists. Warning User...");
            ResetWarningPromptCanvas.enabled = true;
        }
        
    }

    private void StartNewGameAndOverwriteSave()
    {
        //load a popup here to double check that Player wants to Clear progress, IF ANY
        //SceneManager.LoadScene(LEVEL_SELECT_SCREEN_INDEX);//Scene at index 1
        GameProgressManager.Instance.ResetGameProgress();


        //Hide Main Menu Canvas and Warning prompt to reveal Level Selection Canvas.
        ResetWarningPromptCanvas.enabled = false;
        MainMenuCanvas.enabled = false;
    }

    private void onQuitButtonClick()
    {
        GameProgressManager.Instance.WriteProgressToJSON();

        Application.Quit();//exit the entire application; Will not work in the editor
    }

    // Start is called before the first frame update
    void Start()
    {
        if(!GameProgressManager.Instance.IsExistingSaveFileAvailable())
        {
            //No point in showing Continue button if no Save file is available.
            continueButton.enabled = false;
        }
    }

}
