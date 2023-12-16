using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;





public class LevelBuilder : MonoBehaviour
{

    private static LevelBuilder instance;
    public static LevelBuilder Instance
    {
        get { return instance; }//not very different from getters and setters
        private set { instance = value; }//we do not want any other object to modify PlayerTwo entirely.
    }
    
    [SerializeField] private uint numCellsOnSide = 10;
    [SerializeField] private uint totalMazeSideLength = 50;
    
    private float singleCellSideLength = 5f;
    private MazeCell[,] gameMaze;

    private MazeCell startingCell;//playerOne and playerTwo will be spawned here.

    private bool isLevelCompleted = false;

    private LevelType currentLevelType;//determines the levelType being played so that GameProgress can be updated on the right track.

    private void Awake()
    {
        //define Gamefloor area, node size based on difficulty level
        if (instance == null)
        {
            instance = this;//start by setting the Singleton instance 
        }
        else
        {
            Debug.LogError("Fatal Error: Cannot have a predefined instance of Level Builder");
        }

        singleCellSideLength = (float)totalMazeSideLength / numCellsOnSide;
        /*Debug.Log("Initating MazeBuilder with " + numCellsOnSide * numCellsOnSide + " cells. Cell Length = " + singleCellSideLength);
        gameMaze = MazeBuildLogicManager.ApplyRecursiveBacktrackerToMakeMaze(totalMazeSideLength, numCellsOnSide, singleCellSideLength);
        SetupStartingCell();//needed before Player Start is called.*/

    }
    // Start is called before the first frame update
    void Start()
    {
        /*
        RecursiveMazeTraverser.Instance.SetLevelMazeReference(gameMaze);
        //Always put external GameObject references in Start(), not Awake
        MazeRenderer.Instance.DrawMazeOnGame(gameMaze, totalMazeSideLength, numCellsOnSide, singleCellSideLength);
        */

    }

    public void ConstructLevel(LevelType levelType)
    {
        instance.currentLevelType = levelType;

        //Make Maze array before drawing
        Debug.Log("Initating MazeBuilder with " + numCellsOnSide * numCellsOnSide + " cells. Cell Length = " + singleCellSideLength);
        gameMaze = MazeBuildLogicManager.ApplyRecursiveBacktrackerToMakeMaze(totalMazeSideLength, numCellsOnSide, singleCellSideLength);
        SetupStartingCell();//needed before Player Start is called.

        //Setup Level Maze reference for PlayerTwo Traverser.
        RecursiveMazeTraverser.Instance.SetLevelMazeReference(gameMaze);

        //Draw constructed maze
        MazeRenderer.Instance.DrawMazeOnGame(levelType, gameMaze, totalMazeSideLength, numCellsOnSide, singleCellSideLength);

        //Note - LevelType will eventually play a role in the Material selection for visuals.

        //Spawn Collectibles
        CollectiblesSpawnHandler.Instance.SpawnLevelCollectibles();
                
        //Place Players
        PlayerTwoController.Instance.PlacePlayerTwoOnLevelStart();
        PlayerOneController.Instance.PlacePlayerOneOnLevelStart();

        //Place Level Boss
        EnemyBossController.Instance.PlaceLevelBossOnFarMap();

        //Start Enemy spawn on map
        EnemySpawnHandler.Instance.StartEnemySpawn();

        //Show Level HUD
        LevelHUDStatsManager.Instance.ShowHUD();
    }

    private void SetupStartingCell()
    {
        uint midIndex = numCellsOnSide / 2;
        startingCell = gameMaze[midIndex,midIndex];

    }

    public MazeCell GetGameStartingCell()
    {
        return startingCell;
    }

    public void RecordLevelVictory()
    {
        Debug.Log("Level Won!");
        isLevelCompleted = true;



        //If Boss is alive at the end, increase the spawn rate of the next level via ProgressManager
        if (!EnemyBossController.Instance.IsEnemyDead())
        {
            Debug.Log("Boss Left alive. Increasing Spawn Rate...");
            GameProgressManager.Instance.IncreaseNextLevelSpawnRateForAliveBoss(currentLevelType);
        }

        //increment Max level on Game Progress manager
        GameProgressManager.Instance.IncreaseAccessibleLevelOfCurrentPath();


        //if Path is not complete, show next buffs to select, else show the rune that is won.
        if (GameProgressManager.Instance.IsCurrentPathCompleted(currentLevelType))
        {
            LevelTransitionManager.Instance.ShowPathCompletionCanvas();
        }
        else
        {
            //allocate 3 buffs
            EnemyBuffManager.Instance.AllocateThreeRandomEnemyBuffs();
            //Show Level Transition canvas after buffs are allocated
            LevelTransitionManager.Instance.ShowLevelCompletionCanvas();
        }






    }

    public void LevelDefeat()
    {
        Debug.Log("Level Lost! :(");
        isLevelCompleted = false;

        LevelTransitionManager.Instance.ShowLevelFailure();
    }

    //this will be used to spawn collectible items away from, or at walls.
    public MazeCell GetMazeCellAtIndex(int x, int z)
    {
        if (x >= numCellsOnSide)
        {
            Debug.Log("Specified x index is out of Maze bounds. Capping at rightMost row");
            x = (int)numCellsOnSide - 1;
        }else if(x < 0)
        {
            Debug.Log("Specified x index cannot be negative. Capping at leftMost row");
            x = 0;
        }
        if (z >= numCellsOnSide)
        {
            Debug.Log("Specified z index is out of Maze bounds. Capping at topMost column");
            z = (int)numCellsOnSide - 1;
        }else if (z < 0)
        {
            Debug.Log("Specified z index cannot be negative. Capping at bottomMost row");
            z = 0;
        }

        return gameMaze[x, z];
    }

    public MazeCell GetDeadEndCellForBossSpawn()
    {
        MazeCell deadEndMazeCellForBoss = gameMaze[numCellsOnSide - 1, numCellsOnSide - 1];//initialize at top right corner
        //for(int i=0; i<numCellsOnSide;i++)
        for(int i = 0; i < numCellsOnSide; i++)
        {
            for(int j = 0; j < numCellsOnSide; j++)
            {
                //these booleans ensure that Boss remains on Map edge and not on top of the head of the player
                bool xOnEdge = (i==0 || i ==numCellsOnSide - 1);
                bool zOnEdge = (j == 0 || j == numCellsOnSide - 1);

                if(!xOnEdge && !zOnEdge)
                {
                    //this means that neither index is on edge and a middle cell will be selected.
                    continue;
                }
                                
                MazeCell cell = gameMaze[i, j];

                if (!cell.cellWallState.HasFlag(cellWallState.Top))
                {
                    continue;//Top wall should not be the only wall open
                }
                if(GetNumWallsInCell(cell) == 3)
                {
                    
                    deadEndMazeCellForBoss = cell;
                    Debug.Log("Boss Spawn point Found at " + deadEndMazeCellForBoss.cellPositionOnMap);
                    return deadEndMazeCellForBoss;
                }

            }
        }
        return deadEndMazeCellForBoss;
    }

    private int GetNumWallsInCell(MazeCell cell)
    {
        int numWalls = 4;//rather than incrementing, we decrement from 4
        if (!cell.cellWallState.HasFlag(cellWallState.Top))
        {
            numWalls--;
        }
        if (!cell.cellWallState.HasFlag(cellWallState.Bottom))
        {
            numWalls--;
        }
        if (!cell.cellWallState.HasFlag(cellWallState.Left))
        {
            numWalls--;
        }
        if (!cell.cellWallState.HasFlag(cellWallState.Right))
        {
            numWalls--;
        }

        return numWalls;
    }


    public uint GetMazeTotalSideLength()
    {
        return totalMazeSideLength;//uint because these cannot be negative.
    }

    public uint GetMazeNumCellsOnSide()
    {
        return numCellsOnSide;
    }

    public float GetCellSideLength()
    {
        return singleCellSideLength;
    }

    public MazeCell[,] GetLevelMaze()
    {
        return gameMaze;
    }

    public bool IsLevelCompleted()
    {
        return isLevelCompleted;
    }

}
