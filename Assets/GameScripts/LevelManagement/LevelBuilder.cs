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
            Debug.Log("Fatal Error: Cannot have a predefined instance of Level Builder");
        }

        singleCellSideLength = (float)totalMazeSideLength / numCellsOnSide;
        Debug.Log("Initating MazeBuilder with " + numCellsOnSide * numCellsOnSide + " cells. Cell Length = " + singleCellSideLength);
        gameMaze = MazeLogicManager.ApplyRecursiveBacktrackerToMakeMaze(totalMazeSideLength, numCellsOnSide, singleCellSideLength);
        MazeRenderer.Instance.DrawMazeOnGame(gameMaze,totalMazeSideLength,numCellsOnSide,singleCellSideLength);
    }
    // Start is called before the first frame update
    void Start()
    {


    }














    public uint GetMazeTotalSideLength()
    {
        return totalMazeSideLength;//uint because these cannot be negative.
    }

    public uint GetMazeNumCells()
    {
        return numCellsOnSide;
    }

    public float GetCellSideLength()
    {
        return singleCellSideLength;
    }

}
