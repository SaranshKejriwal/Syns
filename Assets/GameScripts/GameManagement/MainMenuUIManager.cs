using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUIManager : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;

    private const int LEVEL_SELECT_SCREEN_INDEX = 1;
    private const int LEVEL_PLAY_SCREEN_INDEX = 2;

    private void Awake()
    {
        //add listeners to button clicks.

        playButton.onClick.AddListener(onPlayButtonClick);//internal functions added as listeners
        quitButton.onClick.AddListener(onQuitButtonClick);
    }


    private void onPlayButtonClick()
    {
        SceneManager.LoadScene(LEVEL_SELECT_SCREEN_INDEX);//Scene at index 1
    }

    private void onQuitButtonClick()
    {
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
