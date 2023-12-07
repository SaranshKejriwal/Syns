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

    [SerializeField] private Canvas LevelCompletedCanvas;
    [SerializeField] private Canvas LevelFailedCanvas;


    //buttons to move to next level on Level Completion.
    [SerializeField] private Button ContinueToNextLevelButton;

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
        LevelFailedCanvas.enabled = false;  
    }

    public void ShowLevelComplete()
    {

        GameMaster.Instance.PauseGame();//put all objects on hold during transition

        //3 buttons to select next ENEMY buff.
        //2 stats - Boss defeated (Y/N) - if Boss alive, increase spawn rate.
        //Gold collected.

        //enable the canvas
        LevelCompletedCanvas.enabled = true;
    } 

    public void ShowLevelFailure()
    {

        GameMaster.Instance.PauseGame();//put all objects on hold during transition


        //Retry button - reload the level

        //Home button - Go to Level selector
        LevelFailedCanvas.enabled = true;
    }







}
