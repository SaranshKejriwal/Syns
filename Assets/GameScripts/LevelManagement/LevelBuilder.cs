using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;





public class LevelBuilder : MonoBehaviour
{

    private static LevelBuilder instance;
    public static LevelBuilder Instance
    //this instance "Property" will be tracked by ALL enemies, while keeping actual PlayerTwo object private
    {
        get { return instance; }//not very different from getters and setters
        private set { instance = value; }//we do not want any other object to modify PlayerTwo entirely.
    }
    
    [SerializeField] private uint numCellsOnSide = 10;
    [SerializeField] private uint totalMazeSideLength = 50;
    
    private float singleCellSideLength = 5f;
    private MazeCell[,] gameMaze;

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
        Debug.Log("Initating MazeBuilder with " + numCellsOnSide * numCellsOnSide + " cells. Cell Length = " + singleCellSideLength);
        gameMaze = MazeBuildLogicManager.ApplyRecursiveBacktrackerToMakeMaze(totalMazeSideLength, numCellsOnSide, singleCellSideLength);
        
    }
    // Start is called before the first frame update
    void Start()
    {
        //Always put external GameObject references in Start(), not Awake
        MazeRenderer.Instance.DrawMazeOnGame(gameMaze, totalMazeSideLength, numCellsOnSide, singleCellSideLength);
        RecursiveMazeTraverser.Instance.SetLevelMazeReference(gameMaze);
    }

    //this will be used to spawn collectible items away from, or at walls.
    public Vector3 GetPositionOfMazeCellAtIndex(int x, int z)
    {
        if (x >= numCellsOnSide)
        {
            Debug.Log("Specified x index is out of Maze bounds. Capping at topMost row");
            x = (int)numCellsOnSide - 1;
        }else if(x < 0)
        {
            Debug.Log("Specified x index cannot be negative. Capping at bottomMost row");
            x = 0;
        }
        if (z >= numCellsOnSide)
        {
            Debug.Log("Specified z index is out of Maze bounds. Capping at rightMost column");
            z = (int)numCellsOnSide - 1;
        }else if (z < 0)
        {
            Debug.Log("Specified z index cannot be negative. Capping at leftMost row");
            z = 0;
        }

        return gameMaze[x, z].cellPositionOnMap;
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

}
