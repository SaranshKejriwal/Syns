using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectionManager : MonoBehaviour
{
    private static MainMenuUIManager instance;
    public static MainMenuUIManager Instance
    {
        /*Enemy Boss will be a singleton. It also controls EnemySpawn Handler.*/
        get { return instance; }
        private set { instance = value; }
    }

    [SerializeField] private Transform LevelHidingCeiling;
    //this is used to hide the maze underneath the Main Menu

    [SerializeField] private Button PrideLevelButton;


    private void Awake()
    {
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
        Debug.Log("Loading Progress for Level Type: "+ levelType);

        uint highestAccessibleLevelIndex = GameProgressManager.Instance.GetHighestAccessibleLevelForType(levelType);

    }

    public void ShowLevelSelectionMenu()
    {
        LevelHidingCeiling.localScale = Vector3.one;
    }

    public void HideLevelSelectionMenu()
    {
        LevelHidingCeiling.localScale = Vector3.zero;
    }

}
