using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class uses Depth-First-Search or Recursive BackTracking to Construct a maze
//Difficulty can vary based on width of this maze


//Each node corresponds to a bit-square in a maze
[Flags]public enum cellWallState
{
    //0000 = No walls on this node
    //1111 = All walls on this node are untouched - Up,Dn,Rt,Lf,
    Left = 1, Right = 2, Bottom = 4, Top = 8,
    //0001, 0010, 0100,1000

    Visited = 128 //tracks which nodes have been touched
}
/*Flags attribute allows us to have multiple true states for this Enum, not just one, using | operator
//eg. newWallState = nodeWallState.Left | nodeWallState.Right; equiv to 0011

Add a state:- newWallState |= nodeWallState.Up; 1011
Remove a state:- newWallState &= ~nodeWallState.Right; 0101
Check state:- HasFlags(nodeWallState.Right)

*/

public struct PositionInMaze
{
    public int x;//track our current position in the recursive backtrack, using this structure
    public int z;
}

public struct MazeCellNeighbour
{
    public PositionInMaze neighbourPosition; //we traverse from our pos to neighbour pos, and break the common wall
    public cellWallState wallSharedWithNeighbour;
}

public class MazeBuilder : MonoBehaviour
{

    private static MazeBuilder instance;
    public static MazeBuilder Instance
    //this instance "Property" will be tracked by ALL enemies, while keeping actual PlayerTwo object private
    {
        get { return instance; }//not very different from getters and setters
        private set { instance = value; }//we do not want any other object to modify PlayerTwo entirely.
    }
    
    private uint numCellsOnSide = 10;
    private const uint totalMazeSideLength = 50;
    [SerializeField] private Transform mazeWallLogicPrefab = null;//needs wall prefab to create instances of the walls

    private float singleCellSideLength = 5f;
    private cellWallState[,] gameMaze;

    private void Awake()
    {
        //define Gamefloor area, node size based on difficulty level
        if (instance == null)
        {
            instance = this;//start by setting the Singleton instance 
        }
        else
        {
            Debug.Log("Fatal Error: Cannot have a predefined instance of MazeBuilder");
        }

        singleCellSideLength = (float)totalMazeSideLength / numCellsOnSide;
        gameMaze = CreateStartingGrid(numCellsOnSide);
        Debug.Log("Initating MazeBuilder with " + numCellsOnSide * numCellsOnSide + " cells. Cell Length = " + singleCellSideLength);
        gameMaze = MazeTraverser.ApplyRecursiveBacktracker(gameMaze,numCellsOnSide);
        DrawMazeOnGame();
    }
    // Start is called before the first frame update
    void Start()
    {


    }



    private void DrawMazeOnGame()
    {
        if (gameMaze == null)
        {
            Debug.Log("Error: Maze not created. Nothing to draw.");
            return;
        }
        float totalOffset = (float)totalMazeSideLength / 2; //center should be (0,0,0), hence shift the whole thing to (-offset, -offset)
        for (int i = 0; i < numCellsOnSide; i++)
        {
            for (int j = 0; j < numCellsOnSide; j++)
            {
                cellWallState cell = gameMaze[i, j];
                //center of the maze should be (0,0,0), hence starting in -x, -z plane
                Vector3 cellPosition = new Vector3(-totalOffset + i * singleCellSideLength, 0, -totalOffset + j * singleCellSideLength);
                //Multiplying by cellSideLength ensure that position accounts for the length offset.
                //Note -> numCellsOnSide is uint, so numCellsOnSide/2 = 0 is numCellsOnSide=1

                DrawTopWall(cell, cellPosition);
                DrawLeftWall(cell, cellPosition);
                DrawRightWall(cell, cellPosition);//redundant, because top and left should suffice, but the if-statements below weren't working
                DrawBottomWall(cell, cellPosition);//redundant, because top and left should suffice, but the if-statements below weren't working

                //Note - This iteration starts from bottom-left to top right, with i controlling x

                //need not draw bottom and right walls for all cells - compensated by tops and lefts of other cells
                if (i == 0)
                {
                    //draw bottom walls for bottom row only
                    //Debug.Log("drawing bottom border for " + cellPosition);
                    //DrawBottomWall(cell, cellPosition);
                }

                if (j == numCellsOnSide - 1)
                {
                    //draw right walls for rightmost row only.
                    //DrawRightWall(cell, cellPosition);
                    //Debug.Log("drawing right border for " + cellPosition);
                }

            }
        }
        
    }
    //Draws only top walls
    private void DrawTopWall(cellWallState cell, Vector3 cellPosition)
    {
        if(cell.HasFlag(cellWallState.Top))
        {

            Transform topWall = Instantiate(mazeWallLogicPrefab, transform);
            topWall.localPosition = cellPosition + new Vector3(0,0,singleCellSideLength/2);
            //top wall is higher than center of cell, by half the cell side

        }
    }

    //Draws only bottom walls
    private void DrawBottomWall(cellWallState cell, Vector3 cellPosition)
    {
        if (cell.HasFlag(cellWallState.Bottom))
        { 
            //no need to check enum because this is drawn for maze edge only
            Transform bottomWall = Instantiate(mazeWallLogicPrefab, transform);
            bottomWall.localPosition = cellPosition + new Vector3(0, 0, -singleCellSideLength / 2);
            //bottom wall is lower than center of cell, by half the cell side
        }
    }

    //Draws only left walls
    private void DrawLeftWall(cellWallState cell, Vector3 cellPosition)
    {
        if (cell.HasFlag(cellWallState.Left))
        {
            Transform leftWall = Instantiate(mazeWallLogicPrefab, transform);
            leftWall.localPosition = cellPosition + new Vector3(-singleCellSideLength / 2, 0, 0);
            //left wall is to the left of center of cell, by half the cell side

            //rotate the leftWall by 90 degrees
            leftWall.eulerAngles = new Vector3(0,90,0);//rotate 90 degrees on y axis only

        }
    }

    //Draws only right walls
    private void DrawRightWall(cellWallState cell, Vector3 cellPosition)
    {
        if (cell.HasFlag(cellWallState.Right))
        {
            Transform rightWall = Instantiate(mazeWallLogicPrefab, transform);
            rightWall.localPosition = cellPosition + new Vector3(singleCellSideLength / 2, 0, 0);
            //right wall is to the right of center of cell, by half the cell side

            //rotate the leftWall by 90 degrees
            rightWall.eulerAngles = new Vector3(0, 90, 0);//rotate 90 degrees on y axis only

        }
    }







    //return a 2D array which corresponds to the maze
    public static cellWallState[,] CreateStartingGrid(uint numCells)
    {
        //numCells indicates the number of cells on width and height; lesser numCells means bigger cells.
        cellWallState[,] gameMaze = new cellWallState[numCells, numCells];
        cellWallState initialCellState = cellWallState.Top | cellWallState.Bottom | cellWallState.Right | cellWallState.Left;//1111
        for (int i = 0; i < numCells;i++)
        {
            for(int j = 0; j < numCells; j++)
            {
                //construct all walls of the cell, to have a solid matrix at first.
                gameMaze[i, j] = initialCellState;
            }
        }

        return gameMaze;
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
