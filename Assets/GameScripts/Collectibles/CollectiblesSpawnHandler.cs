using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this class will spawn one exit key, one exit door, and anywhere between [30,40) gold coins all over the map.
//need to ensure that exit door is close to a top wall and objects are not spawned inside walls
public class CollectiblesSpawnHandler : MonoBehaviour
{

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

    // Start is called before the first frame update
    void Start()
    {
        numCoinsOnLevel = MathFunctions.GetRandomIntInRange(40, 50);
        numHealsOnLevel = MathFunctions.GetRandomIntInRange(5, 8);
        // will spawn once on Start Only. No Update() required
        Debug.Log("Spawning Collectibles...");

        //spawn Exit Door.Ensure that ExitDoor is spawned close to a TopWall
        Vector3 randomExitDoorPosition = MathFunctions.GetRandomMazeCellCenterSpawnWithOffset(0, LevelBuilder.Instance.GetCellSideLength() / 2.1f);
        SpawnSingleObject(ExitDoorPrefab, this.isExitDoorSpawnedAlready, randomExitDoorPosition);

        //spawn Exit Key. Ensure that ExitKey is not spawned too close, else it's too easy
        Vector3 randomExitKeyPosition = MathFunctions.GetRandomSpawnPointOnFarSideMap(LevelBuilder.Instance.GetMazeTotalSideLength() / 4.1f);
        SpawnSingleObject(ExitKeyPrefab,this.isExitKeySpawnedAlready, randomExitKeyPosition);

        //update checkers
        isExitDoorSpawnedAlready =true;
        isExitKeySpawnedAlready=true;

        //spawn Coins and Heals
        SpawnMultipleObjectsByCount(GoldCoinPrefab, numCoinsOnLevel);
        SpawnMultipleObjectsByCount(HealPrefab, numHealsOnLevel);

    }    

    private void SpawnSingleObject(Transform objectPrefab, bool isAlreadySpawned, Vector3 preferredPosition)
    {
        if (isAlreadySpawned)
        {
            return;//don't want 2 instances of singular object.
        }

        Transform singleObject = Instantiate(objectPrefab);
        singleObject.localPosition = preferredPosition;
        //Ensure that ExitDoor is spawned close to a TopWall
    }

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
