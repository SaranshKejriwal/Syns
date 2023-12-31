using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this class will spawn one exit key, one exit door, and anywhere between [30,40) gold coins all over the map.
//need to ensure that exit door is close to a top wall and objects are not spawned inside walls
public class CollectiblesSpawnHandler : MonoBehaviour
{
    private static CollectiblesSpawnHandler instance;
    public static CollectiblesSpawnHandler Instance
    {
        get { return instance; }//not very different from getters and setters
        private set { instance = value; }//we do not want any other object to modify PlayerTwo entirely.
    }

    //Exit Key spawn
    [SerializeField] private Transform ExitKeyPrefab;//make sure to link the prefab in the inspector
    private bool isExitKeySpawnedAlready = false;//this will check that ExitKey is not spawned again.

    //Exit Door spawn
    [SerializeField] private Transform ExitDoorPrefab;//make sure to link the prefab in the inspector
    private bool isExitDoorSpawnedAlready = false;//this will check that ExitKey is not spawned again.

    //Coin spawn
    [SerializeField] private Transform GoldCoinPrefab;//make sure to link the prefab in the inspector
    //this needs to spawn multiple times
    private int numCoinsOnLevel = 50;//at least 50 coins on the level 

    //Healing spawn
    [SerializeField] private Transform HealPrefab;//make sure to link the prefab in the inspector
    //this needs to spawn multiple times
    private int numHealsOnLevel = 10;//at least 5 MedKits on the level 

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;//start by setting the Singleton instance 
        }
        else
        {
            Debug.LogError("Fatal Error: Cannot have a predefined instance of Collectibles Spawn Handler");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        /*
        numCoinsOnLevel = MathFunctions.GetRandomIntInRange(40, 50);
        numHealsOnLevel = MathFunctions.GetRandomIntInRange(5, 8);
        // will spawn once on Start Only. No Update() required
        Debug.Log("Spawning Collectibles...");


        //spawn Exit Door.Ensure that ExitDoor is spawned close to a TopWall
        MazeCell exitDoorContainerCell = MathFunctions.GetRandomMazeCellWithTopWall();
        Vector3 randomExitDoorPosition = exitDoorContainerCell.cellPositionOnMap + new Vector3(0,0, LevelBuilder.Instance.GetCellSideLength() / 1.9f);
        SpawnSingleObject(ExitDoorPrefab, this.isExitDoorSpawnedAlready, randomExitDoorPosition);
        ExitDoorController.Instance.SetExitDoorContainerCell(exitDoorContainerCell);

        //spawn Exit Key. Ensure that ExitKey is spawned on a cell at the edges only
        Vector3 randomExitKeyPosition = MathFunctions.GetRandomMazeEdgeCellCenterSpawnWithOffset(0,LevelBuilder.Instance.GetCellSideLength() / 4f);
        SpawnSingleObject(ExitKeyPrefab,this.isExitKeySpawnedAlready, randomExitKeyPosition);

        //update checkers
        isExitDoorSpawnedAlready =true;
        isExitKeySpawnedAlready=true;

        //spawn Coins and Heals
        SpawnMultipleObjectsByCount(GoldCoinPrefab, numCoinsOnLevel);
        SpawnMultipleObjectsByCount(HealPrefab, numHealsOnLevel);
        */
    }    

    public void SpawnLevelCollectibles()
    {
        numCoinsOnLevel = MathFunctions.GetRandomIntInRange(40, 50);
        numHealsOnLevel = MathFunctions.GetRandomIntInRange(5, 8);
        // will spawn once on Start Only. No Update() required
        //Debug.Log("Spawning Collectibles...");

        //spawn Exit Door.Ensure that ExitDoor is spawned close to a TopWall
        MazeCell exitDoorContainerCell = MathFunctions.GetRandomMazeCellWithTopWall();
        Vector3 randomExitDoorPosition = exitDoorContainerCell.cellPositionOnMap + new Vector3(0, 0, LevelBuilder.Instance.GetCellSideLength() / 1.9f);
        ExitDoorController.Instance.SetExitDoorContainerCell(exitDoorContainerCell);
        ExitDoorController.Instance.SetExitDoorPosition(randomExitDoorPosition);

        //spawn Exit Key. Ensure that ExitKey is spawned on a cell at the edges only
        Vector3 randomExitKeyPosition = MathFunctions.GetRandomMazeEdgeCellCenterSpawnWithOffset(0, LevelBuilder.Instance.GetCellSideLength() / 4f);
        ExitKeyController.Instance.SetExitKeyPosition(randomExitKeyPosition);

        //spawn Coins and Heals
        SpawnMultipleObjectsByCount(GoldCoinPrefab, numCoinsOnLevel);
        SpawnMultipleObjectsByCount(HealPrefab, numHealsOnLevel);
    }

    /*
    private void SpawnSingleObject(Transform objectPrefab, bool isAlreadySpawned, Vector3 preferredPosition)
    {
        if (isAlreadySpawned)
        {
            //how to move the position of the singleton object from here??
            return;//don't want 2 instances of singular object.
        }

        Transform singleObject = Instantiate(objectPrefab);
        singleObject.localPosition = preferredPosition;
        //Ensure that ExitDoor is spawned close to a TopWall
    }*/

    private void SpawnMultipleObjectsByCount(Transform objectPrefab, int objectCount)
    {
        //spawn a random number of coins across the map.
        for (int i = 0;i<objectCount;i++)
        {
            Transform coin = Instantiate(objectPrefab);
            coin.localPosition = MathFunctions.GetRandomSpawnPointAllOverMap();
            //spawn coins anywhere
        }
    }

}
