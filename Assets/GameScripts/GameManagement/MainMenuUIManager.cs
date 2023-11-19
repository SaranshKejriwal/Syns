using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUIManager : MonoBehaviour
{
    [SerializeField] private Button continueButton;
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button quitButton;

    //private const int LEVEL_SELECT_SCREEN_INDEX = 1;
    //private const int LEVEL_PLAY_SCREEN_INDEX = 2;

    private void Awake()
    {
        //add listeners to button clicks.
        continueButton.onClick.AddListener(LoadSavedGame);
        newGameButton.onClick.AddListener(checkExistingGameAndPromptUser);//internal functions added as listeners
        quitButton.onClick.AddListener(onQuitButtonClick);
    }


    private void LoadSavedGame()
    {
        GameProgressManager.Instance.LoadProgressFromJSON();
        //Load the Level-Selector screen, assuming that Game Progress Manager
        //SceneManager.LoadScene(LEVEL_SELECT_SCREEN_INDEX);//Scene at index 1

        //we're now trying to fit everything in 1 Scene
    }

    private void checkExistingGameAndPromptUser() 
    {
        RemoveSaveAndStartNewGame();
    }

    private void RemoveSaveAndStartNewGame()
    {
        //load a popup here to double check that Player wants to Clear progress, IF ANY
        //SceneManager.LoadScene(LEVEL_SELECT_SCREEN_INDEX);//Scene at index 1
    }

    private void onQuitButtonClick()
    {
        GameProgressManager.Instance.SaveProgressToJSON();

        Application.Quit();//exit the entire application; Will not work in the editor
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
