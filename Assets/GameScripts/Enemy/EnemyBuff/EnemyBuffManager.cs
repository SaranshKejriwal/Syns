using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

//this class provides all the buffs that the player can select after a level completion.

/*Enemy buff types - MaxHealth Grunt/Boss, Damage Grunt/Boss, Speed Grunt only. Detection Radius Grunt only, Spawn timer*/
public class EnemyBuffManager : MonoBehaviour
{
    private static EnemyBuffManager instance;
    public static EnemyBuffManager Instance
    //this instance "Property" will be tracked by ALL enemies, while keeping actual PlayerTwo object private
    {
        get { return instance; }//not very different from getters and setters
        private set { instance = value; }//we do not want any other object to modify PlayerTwo entirely.
    }

    private const uint BOSS_HEALTH_BUFF_INDEX = 0;
    private const uint BOSS_DAMAGE_BUFF_INDEX = 1;
    private const uint GRUNT_HEALTH_BUFF_INDEX = 2; //increasing grunt health
    private const uint GRUNT_DAMAGE_BUFF_INDEX = 3; //increase grunt damage
    private const uint GRUNT_SPEED_BUFF_INDEX = 4; //increase grunt speed
    private const uint GRUNT_DETECT_BUFF_INDEX = 5; //increase detection radius
    private const uint GRUNT_SPAWN_BUFF_INDEX = 6; //reduce spawn time'

    private const uint TOTAL_BUFF_TYPES = 7;//increase this if a new buff comes along

    //Buff will specify how much to increase a value in 1 instance of level transition.
    private const float Buff = 1.2f;// 20% buff compounded 6 times is 2.98, which is not too little, not too much
    private const float noBuff = 1.0f;//When no buff is applied on a property

    private EnemyBuffObject[] BuffArray = new EnemyBuffObject[TOTAL_BUFF_TYPES];

    //specify the buffs under each Buff object
    private void InitializeBuffObjectsArray()
    {
        BuffArray[BOSS_HEALTH_BUFF_INDEX] = new EnemyBuffObject(EnemyType.Boss, "Monolith", "Next Boss has More HP",noBuff,Buff,noBuff,noBuff,noBuff);
        BuffArray[BOSS_DAMAGE_BUFF_INDEX] = new EnemyBuffObject(EnemyType.Boss, "GigaBite", "Next Boss does Extra Damage", Buff, noBuff, noBuff, noBuff, noBuff);
        BuffArray[GRUNT_HEALTH_BUFF_INDEX] = new EnemyBuffObject(EnemyType.Grunt, "Brawler", "Enemy Spawn have More HP", noBuff, Buff, noBuff, noBuff, noBuff);
        BuffArray[GRUNT_DAMAGE_BUFF_INDEX] = new EnemyBuffObject(EnemyType.Grunt, "Berserk", "Enemy Spawn does Extra Damage", Buff, noBuff, noBuff, noBuff, noBuff);
        BuffArray[GRUNT_SPEED_BUFF_INDEX] = new EnemyBuffObject(EnemyType.Grunt, "Lunger", "Enemy Spawn Moves Faster", noBuff, noBuff, Buff, noBuff, noBuff);
        BuffArray[GRUNT_DETECT_BUFF_INDEX] = new EnemyBuffObject(EnemyType.Grunt, "Hunter", "Enemy Spawn Sees Further", noBuff, noBuff, noBuff, Buff, noBuff);
        BuffArray[GRUNT_SPAWN_BUFF_INDEX] = new EnemyBuffObject(EnemyType.Grunt, "Sleuth", "Enemies Spawn Faster", noBuff, noBuff, noBuff, noBuff, Buff);

    }

    //UI Visuals_________________________

    //3 buttons for 3 buff selections.
    [SerializeField] private Button LeftSideBuffButton;
    [SerializeField] private Button RightSideBuffButton;
    [SerializeField] private Button MiddleBuffButton;

    //3 indices corresponding to the 3 buffs and their respective icons to load on UI Buttons
    private int leftSideBuffButtonIndex = -1;
    private int rightSideBuffButtonIndex = -1;
    private int middleBuffButtonIndex = -1;
    //initialized at invalid index.

    //3 Button Titles and Descriptions
    [SerializeField] private TextMeshProUGUI LeftBuffButtonTitle;
    [SerializeField] private TextMeshProUGUI RightBuffButtonTitle;
    [SerializeField] private TextMeshProUGUI MiddleBuffButtonTitle;

    [SerializeField] private TextMeshProUGUI LeftBuffButtonDescription;
    [SerializeField] private TextMeshProUGUI RightBuffButtonDescription;
    [SerializeField] private TextMeshProUGUI MiddleBuffButtonDescription;

    //Icons for All 8 buffs
    private Image[] BuffIconsArray = new Image[TOTAL_BUFF_TYPES];


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;//start by setting the Singleton instance 
        }
        else
        {
            Debug.LogError("Fatal Error: Cannot have a predefined instance of Enemy Buff Manager");
        }

        //specify the buffs under each Buff object on awake itself
        InitializeBuffObjectsArray();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AllocateThreeRandomEnemyBuffs()
    {

        //create iterator count and MaxIteration count for the while loops below
        int iteratorCount = 0;
        int maxIterationsCount = 100;

        //pick 3 random, DISTINCT numbers from 0 to array length.
        leftSideBuffButtonIndex = MathFunctions.GetRandomIntInRange(0, BuffArray.Length);
        rightSideBuffButtonIndex = MathFunctions.GetRandomIntInRange(0, BuffArray.Length);
        middleBuffButtonIndex = MathFunctions.GetRandomIntInRange(0, BuffArray.Length);

        //If any two index values are identical, run these while loops to ensure that the 3 values are distinct.
        while ( rightSideBuffButtonIndex == leftSideBuffButtonIndex && iteratorCount < maxIterationsCount)
        {
            rightSideBuffButtonIndex = MathFunctions.GetRandomIntInRange(0, BuffArray.Length);
        }
        //get middleBuff index that is different from the other 2
        while ((middleBuffButtonIndex == leftSideBuffButtonIndex || middleBuffButtonIndex == rightSideBuffButtonIndex) && iteratorCount < maxIterationsCount)
        {
            middleBuffButtonIndex = MathFunctions.GetRandomIntInRange(0, BuffArray.Length);
        }

        //Update buff titles and descriptions
        LeftBuffButtonTitle.text = GetBuffObjectAtIndex(leftSideBuffButtonIndex).GetBuffTitle();
        MiddleBuffButtonTitle.text = GetBuffObjectAtIndex(middleBuffButtonIndex).GetBuffTitle();
        RightBuffButtonTitle.text = GetBuffObjectAtIndex(rightSideBuffButtonIndex).GetBuffTitle();

        LeftBuffButtonDescription.text = GetBuffObjectAtIndex(leftSideBuffButtonIndex).GetBuffDescription();
        MiddleBuffButtonDescription.text = GetBuffObjectAtIndex(middleBuffButtonIndex).GetBuffDescription();
        RightBuffButtonDescription.text = GetBuffObjectAtIndex(rightSideBuffButtonIndex).GetBuffDescription();

        //Remove all previously applied listeners. Add buff listeners to 3 buttons to apply the buff.
        LeftSideBuffButton.onClick.RemoveAllListeners();
        MiddleBuffButton.onClick.RemoveAllListeners();
        RightSideBuffButton.onClick.RemoveAllListeners();

        LeftSideBuffButton.onClick.AddListener(delegate { ApplySelectedBuff(GetBuffObjectAtIndex(leftSideBuffButtonIndex)); });
        MiddleBuffButton.onClick.AddListener(delegate { ApplySelectedBuff(GetBuffObjectAtIndex(middleBuffButtonIndex)); });
        RightSideBuffButton.onClick.AddListener(delegate { ApplySelectedBuff(GetBuffObjectAtIndex(rightSideBuffButtonIndex)); });
    }



    private EnemyBuffObject GetBuffObjectAtIndex(int index)
    {
        //check if index is assigned correctly
        if (index < 0 || index >= BuffArray.Length)
        {
            Debug.LogError("Received invalid index for applying buff from array: " + index);
            return null;
        }

        return BuffArray[index];
    }

    public void ApplySelectedBuff(EnemyBuffObject buffObj)
    {

        if (buffObj == null)
        {
            Debug.LogError("Error while applying Enemy Buff. Buff Object is null");
            return;
        }

        if(buffObj.GetBuffedEnemyType() == EnemyType.Boss)
        {
            EnemyBossController.Instance.SetEnemyBossPropertiesByBuffObject(buffObj);

            //save new properties into memory
            GameProgressManager.Instance.UpdateBossPropertiesInMemoryForCurrentPath(EnemyBossController.Instance.GetCurrentEnemyControllerProperties());

        }else if(buffObj.GetBuffedEnemyType() == EnemyType.Grunt)
        {
            EnemySpawnHandler.Instance.SetNextGruntPropertiesByBuffObject(buffObj);

            //save new properties into memory
            GameProgressManager.Instance.UpdateGruntPropertiesInMemoryForCurrentPath(EnemySpawnHandler.Instance.GetCurrentGruntProperties());
        }
        else
        {
            Debug.LogError("Error while applying Enemy Buff. Buff Object type is invalid.");
            return;
        }


        //if buff is applied successfully, disable the Buff buttons.
        LeftSideBuffButton.enabled = false;
        MiddleBuffButton.enabled = false;
        RightSideBuffButton.enabled = false;

        //Continue to Next level only when a buff is selected.
        GameProgressManager.Instance.LoadNextLevelOnCurrentPath();

    }
}
