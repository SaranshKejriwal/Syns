using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

//this static class is used for generic math functions used in the game
public static class MathFunctions 
{
 
    public static int GetCoinToss(int option1, int option2)
    {
        //CoinToss to Return >1 or <1 randomly - there is equal probability of getting either >1 or <1
        int CoinToss = (int)Random.Range(0, 2);
        return CoinToss == 1 ? option1 : option2; //shorthand if-else
    }

    public static int GetCoinToss(uint option1, uint option2)
    {
        //CoinToss to Return >1 or <1 randomly - there is equal probability of getting either >1 or <1
        int CoinToss = (int)Random.Range(0, 2);
        return CoinToss == 1 ? (int)option1 : (int)option2; //shorthand if-else
    }


    public static int GetRandomIntInRange(int minValueInclusive, int maxValueExclusive)
    {
        return (int)Random.Range(minValueInclusive, maxValueExclusive);
    }

    public static int GetRandomIntInRange(uint minValueInclusive, uint maxValueExclusive)
    {
        return (int)Random.Range(minValueInclusive, maxValueExclusive);
    }

    public static float GetRandomFloatInRange(float minValueInclusive, float maxValueExclusive)
    {
        return Random.Range(minValueInclusive, maxValueExclusive);
    }


    //Spawn handlers___________________________________________
    //return a spawn point that is away from the origin.
    public static Vector3 GetRandomSpawnPointOnFarSideMap(float minSpawnDistanceFromOrigin)
    {
        float maxSpawnDistanceFromOrigin = LevelBuilder.Instance.GetMazeTotalSideLength() /2.5f; //the extra 0.5 is taken as buffer to not have anything spawn outside the wall
        //Get random spawning point within +-maxSpawnDistanceFromOrigin, excluding the minSpawnDistanceFromOrigin square in the middle
        Vector3 randomSpawnPoint = new Vector3(MathFunctions.GetRandomFloatInRange(minSpawnDistanceFromOrigin, maxSpawnDistanceFromOrigin) * GetCoinToss(1,-1), 0, Random.Range(minSpawnDistanceFromOrigin, maxSpawnDistanceFromOrigin) * GetCoinToss(1, -1));
        //Coin toss ensures that spawn happens on -x and -z sides as well.

        return randomSpawnPoint;
    }

    public static Vector3 GetRandomSpawnPointAllOverMap()
    {
        return GetRandomSpawnPointOnFarSideMap(0);//minimum distance from origin is set to 0; Same as all over Map      
    }

    //Spawn away from walls - use Maze center points as reference.
    public static Vector3 GetRandomMazeCellCenterSpawnWithOffset(float xOffsetFromCellCenter, float zOffsetFromCellCenter)
    {
        int xIndex = GetRandomIntInRange(0, LevelBuilder.Instance.GetMazeNumCellsOnSide());//note Maze numCells is Excluded in Random function
        int zIndex = GetRandomIntInRange(0, LevelBuilder.Instance.GetMazeNumCellsOnSide());

        //get MazeCell Center point at this random index
        Vector3 MazeCellCenter = LevelBuilder.Instance.GetPositionOfMazeCellAtIndex(xIndex, zIndex);
        //return final vector after adding offset
        return MazeCellCenter + new Vector3(xOffsetFromCellCenter, 0, zOffsetFromCellCenter);
    }

    public static Vector3 GetRandomMazeEdgeCellCenterSpawnWithOffset(float xOffsetFromCellCenter, float zOffsetFromCellCenter)
    {
        //This cointoss will ensure that you always get a cell on the edge of the map only...any edge.
        int xIndex = GetCoinToss(0, LevelBuilder.Instance.GetMazeNumCellsOnSide());//note Maze numCells is Excluded in Random function
        int zIndex = GetCoinToss(0, LevelBuilder.Instance.GetMazeNumCellsOnSide());

        //get MazeCell Center point at this random index
        Vector3 MazeCellCenter = LevelBuilder.Instance.GetPositionOfMazeCellAtIndex(xIndex, zIndex);
        //return final vector after adding offset
        return MazeCellCenter + new Vector3(xOffsetFromCellCenter, 0, zOffsetFromCellCenter);
    }

}
