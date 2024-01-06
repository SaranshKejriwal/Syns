using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelTransitionManager : MonoBehaviour
{
    private static LevelTransitionManager instance;
    public static LevelTransitionManager Instance
    //this instance "Property" will be tracked by ALL enemies, while keeping actual PlayerTwo object private
    {
        get { return instance; }//not very different from getters and setters
        private set { instance = value; }//we do not want any other object to modify PlayerTwo entirely.
    }

    //this canvas contains the 3 buff selection buttons
    [SerializeField] private Canvas LevelCompletedCanvas;


    //this canvas will serve 3 purposes
    //1. Show level Fail when P2 dies. 2. Show Pause Menu. 3. Show Path completion
    //3 common buttons will be shown/hidden based on requirement, and background color will be changed.
    [SerializeField] private Canvas LevelPathNavigationCanvas;
    [SerializeField] private Image LevelPathNavigCanvasBackground;

    [SerializeField] private Button ContinueLevelButton;
    [SerializeField] private Button MainMenuButton;
    [SerializeField] private Button RestartLevelButton;

    //Level Completion Canvas will remain separate because it has a different Button Set.

    //We need this separate public event which will be subscribed by all enemy grunts. 
    //When restart is clicked, all enemy grunts from previous attempts will be destroyed, dead or alive.
    //We cannot have Enemy Grunts Die on P2 death because their animation events would still be firing and the object would be destroyed.
    //public event EventHandler OnLevelRestartButtonClick;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;//start by setting the Singleton instance 
        }
        else
        {
            Debug.LogError("Fatal Error: Cannot have a predefined instance of LevelTransition Manager");
        }

    }

    // Start is called before the first frame update
    private void Start()
    {
        //hide both canvases at the start
        LevelCompletedCanvas.enabled = false;
        LevelPathNavigationCanvas.enabled = false;

        //Add functions for buttons
        ContinueLevelButton.onClick.AddListener(ResumeGame);
        RestartLevelButton.onClick.AddListener(RestartLevel);

        MainMenuButton.onClick.AddListener(GoToMainMenu);
    }


    public void ShowLevelCompletionCanvas()
    {

        GameMaster.Instance.PauseGame();//put all objects on hold during transition

        //3 buttons to select next ENEMY buff.
        //2 stats - Boss defeated (Y/N) - if Boss alive, increase spawn rate.
        //Gold collected.

        //enable the canvas
        LevelCompletedCanvas.enabled = true;
        LevelPathNavigationCanvas.enabled = false;

    }

    public void ShowLevelPauseCanvas(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        GameMaster.Instance.PauseGame();

        //2 buttons - Continue Level and Main Menu
        LevelPathNavigationCanvas.enabled = true;
        LevelPathNavigCanvasBackground.color = new Color(150, 0, 150);

        //Show Continue Button, if previously hidden.
        ContinueLevelButton.transform.localScale = Vector3.one;
    }

    public void ShowPathCompletionCanvas()
    {
        //Hide Continue and Restart Button.
        ContinueLevelButton.transform.localScale = Vector3.zero;
        RestartLevelButton.transform.localScale += Vector3.zero;

        GameMaster.Instance.PauseGame();//put all objects on hold during transition

        //Show The details of the Rune of this Syn type.

        //enable the canvas
        LevelPathNavigationCanvas.enabled = true;
        LevelPathNavigCanvasBackground.color = new Color(0, 150, 50);

    }

    public void ShowLevelFailure()
    {

        GameMaster.Instance.PauseGame();//put all objects on hold during transition

        //Hide Continue Button, Not applicable here.
        ContinueLevelButton.transform.localScale = Vector3.zero;

        LevelPathNavigationCanvas.enabled = true;
        LevelPathNavigCanvasBackground.color = new Color(150, 0, 50);
    }

    public void HideAllTransitionCanvases()
    {
        LevelCompletedCanvas.enabled = false;
        LevelPathNavigationCanvas.enabled=false;
    }

    private void ResumeGame()
    {
        HideAllTransitionCanvases();
        GameMaster.Instance.StartGamePlay();
    }

    private void RestartLevel()
    {
        HideAllTransitionCanvases();

        GameProgressManager.Instance.LoadHighestLevelOnCurrentPath();
    }

    private void GoToMainMenu()
    {
        HideAllTransitionCanvases();

        LevelSelectionManager.Instance.ShowLevelSelectionMenu();

    }

}
